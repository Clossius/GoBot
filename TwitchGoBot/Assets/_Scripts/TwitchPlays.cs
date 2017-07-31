using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;
using alias = System.Random;

public class TwitchPlays : MonoBehaviour {

	int boardSize = 19;
	float secondsToWait = 35.0f;
	bool askingForMove = false;
	int extraMoveVoteCost = 100;
	GameObject scripts;
	public GameObject askForMoveButton;
	public GameObject cancelAskForMoveButton;
	public InputField opponentsMoveInputField;
	public Text winningMove;
	public Toggle autoStartAskForMove;
	public Text extraMoveCostText;
	public InputField votingRewardInputField;
	public Toggle rewardVotingToggle;
	public Text votingRewardAmountText;
	int votingRewardAmount = 0;
	IEnumerator waitRoutine;
	public Toggle buyMoveToggle;

	public class Move {
		public string upperCaseMove;
		public string lowerCaseMove;
		public int vote;

		public Move (string upperMov, string lowerMov, int vot) {
			upperCaseMove = upperMov;
			lowerCaseMove = lowerMov;
			vote = vot;
		}
	}


	List<Move> moves = new List<Move>();
	List<string> users = new List<string>();

	// Use this for initialization
	void Start () {
		scripts = GameObject.Find("_Scripts");
		InitializeMoves();
		InitializeUsers();
		autoStartAskForMove.isOn = false;
		TwitchIrc.Instance.OnChannelMessage += OnChannelMessage;
		winningMove.text = "Move:";
		waitRoutine = WaitForMove(secondsToWait);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		if(opponentsMoveInputField.isFocused && opponentsMoveInputField.text != "" && Input.GetKey(KeyCode.Return)) {
			if(autoStartAskForMove.isOn && !askingForMove){AskForMove();
				scripts.GetComponent<ChatManager>().MessageSend("Opponent's move: " + opponentsMoveInputField.textComponent.text);
			opponentsMoveInputField.text = "";
			}
			else {
				scripts.GetComponent<ChatManager>().MessageSend("Opponent's move: " + opponentsMoveInputField.textComponent.text);
				opponentsMoveInputField.text = "";
			}
    	 }
    }

	//Receive username and message that sends message to channel
	void OnChannelMessage(ChannelMessageEventArgs channelMessageArgs)
    {	
		//tempText = "<b>" + channelMessageArgs.From + ":</b> " + channelMessageArgs.Message + "\n";
		bool alreadyVoted = false;
		if(askingForMove) {
			for(int i=0;i<users.Count;i++){
				if(channelMessageArgs.From == users[i]){
					alreadyVoted = true;
					string message = channelMessageArgs.From + ", you already voted.";
				}
			}
		}

        if(askingForMove && !alreadyVoted){
        	UpVoteMove(channelMessageArgs.Message, channelMessageArgs.From);
        }

    }

    void UpVoteMove(string move, string username) {
		for(int i=0;i<moves.Count;i++){
			if(move == moves[i].upperCaseMove || move == moves[i].lowerCaseMove){
				moves[i].vote += 1;
				users.Add(username);
				Debug.Log(username + " UpVoted: " + moves[i].upperCaseMove);
				if(rewardVotingToggle.isOn){scripts.GetComponent<Points>().AddPointsSilently(username, votingRewardAmount);}
			}
		}
    }

    public void UpVoteMove(string move, string username, int cost) {
    	bool isAMove = false;
    	if(askingForMove) {
			for(int i=0;i<moves.Count;i++){
				if(move == moves[i].upperCaseMove || move == moves[i].lowerCaseMove){
					moves[i].vote += 1;
					isAMove = true;
					Debug.Log(username + " UpVoted: " + moves[i].upperCaseMove);
					string message = username + ", you bought " + moves[i].upperCaseMove + ".";
					scripts.GetComponent<ChatManager>().MessageSend(message);
					scripts.GetComponent<Points>().TakePoints(username, extraMoveVoteCost);
				}
			}
			if(!isAMove) {
				string message = username + " that is not a valid move.";
				scripts.GetComponent<ChatManager>().MessageSend(message);
			}
		}
		else {
			string message = username + ", I am not asking for a move right now.";
			scripts.GetComponent<ChatManager>().MessageSend(message);
		}
	}

    public int CostForExtraMoveVote () {
    	return extraMoveVoteCost;
    }

    void InitializeUsers() {
    	users = new List<string>();
    }

	void InitializeMoves () {
		for(int i=0;i<boardSize;i++){
			for(int g=0; g<boardSize;g++) {
				string lowerX = "";
				string upperX = "";
				int y = g+1;
				if(i == 0)			{upperX = "A"; lowerX = "a";}
				else if(i == 1)		{upperX = "B"; lowerX = "b";}
				else if(i == 2)		{upperX = "C"; lowerX = "c";}
				else if(i == 3)		{upperX = "D"; lowerX = "d";}
				else if(i == 4)		{upperX = "E"; lowerX = "e";}
				else if(i == 5)		{upperX = "F"; lowerX = "f";}
				else if(i == 6)		{upperX = "G"; lowerX = "g";}
				else if(i == 7)		{upperX = "H"; lowerX = "h";}
				else if(i == 8)		{upperX = "J"; lowerX = "j";}
				else if(i == 9)		{upperX = "K"; lowerX = "k";}
				else if(i == 10)	{upperX = "L"; lowerX = "l";}
				else if(i == 11)	{upperX = "M"; lowerX = "m";}
				else if(i == 12)	{upperX = "N"; lowerX = "n";}
				else if(i == 13)	{upperX = "O"; lowerX = "o";}
				else if(i == 14)	{upperX = "P"; lowerX = "p";}
				else if(i == 15)	{upperX = "Q"; lowerX = "q";}
				else if(i == 16)	{upperX = "R"; lowerX = "r";}
				else if(i == 17)	{upperX = "S"; lowerX = "s";}
				else if(i == 18)	{upperX = "T"; lowerX = "t";}
				Move move = new Move(upperX + y.ToString(), lowerX + y.ToString(), 0);
				moves.Add(move);
			}
		}

		Move resign = new Move ("Resign", "resign", 0);
		Move pass = new Move ("Pass", "pass", 0);
		moves.Add(resign);
		moves.Add(pass);

		//For Debugging moves

		/*for(int i=0;i<moves.Count;i++) {
			Debug.Log("Move Options: " + moves[i].upperCaseMove + " " + moves[i].lowerCaseMove + " " + moves[i].vote.ToString());
		}*/
	}

	public void AskForMove () {
		if(!askingForMove){
			askingForMove = true;
			askForMoveButton.SetActive(false);
			cancelAskForMoveButton.SetActive(true);
			InitializeUsers();
			winningMove.text = "Move:";
			string message = "Please vote on a move! :Ex: D4 :Ex: Resign :Ex: Pass";
			scripts.GetComponent<ChatManager>().MessageSend(message);
			string pointsName = scripts.GetComponent<Settings>().GetPointsName();
			if(buyMoveToggle){message = "To buy an extra vote for " + extraMoveVoteCost + " " + pointsName + ", type !buymove. ex: !buymove D4";}
			scripts.GetComponent<ChatManager>().MessageSend(message);
			scripts.GetComponent<Sfx>().CancelSound();
			StartCoroutine("WaitForMove", secondsToWait);
		}
	}

	void StopAskingForMove () {
		askingForMove = false;
		askForMoveButton.SetActive(true);
		cancelAskForMoveButton.SetActive(false);
	}

	void GetMove () {
		string resultMessage = "";
		List<Move> votedMove = new List<Move>();
		scripts.GetComponent<ChatManager>().MessageSend("Voting time has ended!");

		for(int i=0;i<moves.Count;i++) {
			if(i==0){votedMove.Add(moves[i]); Debug.Log("First move added.");}
			else {
				if(votedMove[0].vote == moves[i].vote){votedMove.Add(moves[i]);}
				else if (moves[i].vote > votedMove[0].vote) {
					votedMove = new List<Move>();
					votedMove.Add(moves[i]);
					Debug.Log("Move Added: " + moves[i]);
				}
			}
		}

		string move = "";

		alias rnd = new alias();
		int min = 0;
		int max = votedMove.Count;
		move = votedMove[rnd.Next(min, max)].upperCaseMove;

		resultMessage = "The winning move is: " + move;
		winningMove.text = "Move: <b>" + move + "</b>";
		string displayMessage = "<b>" + resultMessage + "</b>";
		scripts.GetComponent<ChatManager>().MessageSend(resultMessage, displayMessage);
		scripts.GetComponent<Sfx>().LoginButtonClicked();
	}

	void ReInitializeMoves() {
		moves = new List<Move>();
		InitializeMoves();
	}

	public void CancelMoveRequest() {
		StopCoroutine("WaitForMove");
		StopAskingForMove();
		ReInitializeMoves();
		string message = "Move request canceled.";
		scripts.GetComponent<ChatManager>().MessageSend(message);
		scripts.GetComponent<Sfx>().CancelSound();

	}

	IEnumerator WaitForMove(float seconds) {
		Debug.Log("Waiting for move...");
		yield return new WaitForSeconds(seconds);
		Debug.Log("Finished waiting for move.");
		StopAskingForMove();
		GetMove();
		ReInitializeMoves();
	}

	public void SetCostToBuyExtraMoveVote(int cost) {
		extraMoveVoteCost = cost;
		extraMoveCostText.text = "Cost to buy extra vote: \n" + extraMoveVoteCost;
	}

	public void ChangeVotingReward(int points){
		votingRewardAmount = points;
		votingRewardAmountText.text = votingRewardAmount + " /vote";
	}


	public int RewardVotingAmount() {
		return votingRewardAmount;
	}

}
