using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Irc;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using alias = System.Random;
using TMPro;

public class Commands {
		public string commandName;
		public string response;

		public Commands (string command, string response) {
			commandName = command;
			response = response;
		}
	}

	public class PollClass {
		public string option;
		public int vote;

		public PollClass (string o, int v) {
			option = o;
			vote = v;
		}
	}

	public class RaffleClass {
		public string username;
		public int tickets;
		public int maxTickets;

		public RaffleClass (string u, int t, int m) {
			username = u;
			tickets = t;
			maxTickets = m;
		}
	}

public class CommandsScript : MonoBehaviour {

	public Toggle usePoints;
	bool poll = false;
	bool raffle = false;
	int maxTickets = 0;
	int ticketCost = 0;
	GameObject Scripts;
	Component pointsScript;
	public InputField PollOption01;
	public InputField PollOption02;
	public InputField PollOption03;
	public InputField PollTime;
	public TextMeshProUGUI PollText;
	public Toggle BuyExtraMoveVoteToggle;
	public Toggle useRanks;
	public Toggle useBetting;
	public Toggle use8Ball;
	public Toggle useQuotes;
	public Toggle useHeist;
	string notEnoughMessage;
	string pointsName;




	List<Commands> commandList = new List<Commands>();
	List<PollClass> pollList = new List<PollClass>();
	List<string> alreadyVoted = new List<string>();
	List<RaffleClass> raffleList = new List<RaffleClass>();

	// Use this for initialization
	void Start () {
		Scripts = GameObject.Find("_Scripts");
		pointsScript = Scripts.GetComponent<Points>();
		TwitchIrc.Instance.OnChannelMessage += OnChannelMessage;
		pointsName = Scripts.GetComponent<Settings>().GetPointsName();
		notEnoughMessage = " you do not have enough " + pointsName + ".";

		PollText.text = "Start a Poll.";


	}
	
	// Update is called once per frame
	void Update () {
		if(poll) {UpdatePollTextBoxes();}
	}



	//Receive username that has been left from channel 
	void OnChannelMessage(ChannelMessageEventArgs channelMessageArgs)
    {	
    	string tempText = "";
    	int rank = GameObject.Find("_Scripts").GetComponent<Points>().GetRankInt(channelMessageArgs.From);
		if(channelMessageArgs.Message[0] == '!') {CheckCommand(channelMessageArgs.From, channelMessageArgs.Message, rank);}
    }

    public void CheckCommand(string username, string message, int rank) {
    	string command = "!";
		string lowerCaseUsername = "";

		if(usePoints){GameObject.Find("_Scripts").GetComponent<Points>().CheckForPointsCommand(username, message);}

		for (int g=0;g<username.Length;g++) {lowerCaseUsername = lowerCaseUsername + Char.ToLower(username[g]);}
    	for (int i=1;i<message.Length;i++) {command = command + Char.ToLower(message[i]);}

    	string[] comment = command.Split(' ');
    	Debug.Log("Check Command: " + comment[0]);

    	if(comment[0] == "!points" && usePoints) {
			GameObject.Find("_Scripts").GetComponent<Points>().DisplayPoints(lowerCaseUsername);
    	}

    	else if(comment[0] == "!getpoints" && comment.Length > 1 && rank <= 1 && usePoints) {
    		username = comment[1];
			lowerCaseUsername = "";
			for (int g=0;g<username.Length;g++) {lowerCaseUsername = lowerCaseUsername + Char.ToLower(username[g]);}
			GameObject.Find("_Scripts").GetComponent<Points>().DisplayPoints(lowerCaseUsername);
    	}

		else if(comment[0] == "!addpoints" && comment.Length > 1 && rank <= 1 && usePoints) {
    		username = comment[1];
			lowerCaseUsername = "";
			for (int g=0;g<username.Length;g++) {lowerCaseUsername = lowerCaseUsername + Char.ToLower(username[g]);}
			int points = Int32.Parse(comment[2]);
			GameObject.Find("_Scripts").GetComponent<Points>().AddPoints(lowerCaseUsername, points);
			}

		else if(comment[0] == "!takepoints" && comment.Length > 1 && rank <= 1 && usePoints) {
    		username = comment[1];
			lowerCaseUsername = "";
			for (int g=0;g<username.Length;g++) {lowerCaseUsername = lowerCaseUsername + Char.ToLower(username[g]);}
			int points = Int32.Parse(comment[2]);
			GameObject.Find("_Scripts").GetComponent<Points>().TakePoints(lowerCaseUsername, points);
			}

		else if(comment[0] == "!rankup" && useRanks.isOn && usePoints) {
			Scripts.GetComponent<Ranks>().CheckRankUp(username);
		}

		else if(comment[0] == "!buyrank" && useRanks.isOn && usePoints) {
			Scripts.GetComponent<Ranks>().BuyRankUp(username);
		}

		else if(comment[0] == "!changerank" && rank == 0) {
			string newRank = "";
			for(int i=2;i<comment.Length;i++){
				if(i==2){newRank = comment[i];}
				else{newRank = newRank + " " + comment[i];}
			}
			Scripts.GetComponent<Points>().ChangeRank(comment[1], newRank);
		}

		else if(comment[0] == "!startpoll" && rank <= 1) {
			StartPoll(message);
		}

		else if(comment[0] == "!cancelpoll" && rank <= 1) {
			PollCancelButtonPressed();
		}

		else if(comment[0] == "!vote" && poll) {
			bool alreadyVotedBoolean = false;
			for(int i=0;i<alreadyVoted.Count;i++) {
				if(username == alreadyVoted[i]){alreadyVotedBoolean = true;}
			}
			if(!alreadyVotedBoolean){
				VoteUpPoll(comment[1]);
				alreadyVoted.Add(username);
			}
		}

		else if(comment[0] == "!buymove" && BuyExtraMoveVoteToggle.isOn) {
			int cost = Scripts.GetComponent<TwitchPlays>().CostForExtraMoveVote();
			int userPoints = Scripts.GetComponent<Points>().GetPoints(username);
			bool canAfford = Scripts.GetComponent<Points>().CanAfford(username, cost);
			if(!canAfford){
				string newMessage = username + notEnoughMessage;
				Scripts.GetComponent<ChatManager>().MessageSend(newMessage);
			}
			else if(canAfford){
				Scripts.GetComponent<TwitchPlays>().UpVoteMove(comment[1], username, cost);
			}
		}

		else if(comment[0] == "!store" && usePoints) {
			Scripts.GetComponent<Store>().StoreCommand();
		}

		else if(comment[0] == "!buy" && usePoints) {
			string item = "";
			for(int i=1;i<comment.Length;i++){item = item + comment[i]; if(i<(comment.Length-1)){item = item + " ";}}
			Scripts.GetComponent<Store>().BuyCommand(username, item);
		}

		else if(comment[0] == "!startraffle" && rank <= 1 && usePoints) {
			StartRaffle(message);
		}

		else if(comment[0] == "!cancelraffle" && usePoints && rank <= 1 && usePoints) {
			CancelRaffle();
		}

		else if(comment[0] == "!buyticket" && usePoints) {
			Debug.Log("Buy Ticket Command " + comment[1]);
			BuyTicket(username, Int32.Parse(comment[1]));
		}

		else if(comment[0] == "!tickets" && usePoints) {
			CheckTickets(username);
		}

		else if(comment[0] == "!startbet" && useBetting.isOn && rank <=1 && usePoints) {
			Scripts.GetComponent<BettingScript>().StartBetting(command);
		}

		else if(comment[0] == "!bet" && useBetting.isOn && usePoints) {
			if(comment.Length >= 3) {
				Scripts.GetComponent<BettingScript>().BetOn(username, Int32.Parse(comment[1]), Int32.Parse(comment[2]));
			}
		}

		else if(comment[0] == "!stopbet" && useBetting.isOn && rank <=1 && usePoints) {
			Scripts.GetComponent<BettingScript>().StopBetting();
		}

		else if(comment[0] == "!betwinner" && useBetting.isOn && rank <=1 && usePoints) {
			if(comment.Length >= 2) {
				Scripts.GetComponent<BettingScript>().ChooseWinner(Int32.Parse(comment[1]));
			}
		}

		else if(comment[0] == "!8ball" && use8Ball.isOn) {
			Scripts.GetComponent<EightBall>().AskEightBall();
		}

		else if(comment[0] == "!quote" && useQuotes.isOn) {
			Scripts.GetComponent<QuoteScript>().GetQuote();
		}

		else if(comment[0] == "!addquote" && useQuotes.isOn) {
			Scripts.GetComponent<QuoteScript>().AddQuote(comment);
		}

		else if(comment[0] == "!startheist" && useHeist.isOn && rank <=1 && usePoints) {
			Scripts.GetComponent<Heist>().StartHeist();
		}

		else if(comment[0] == "!cancelheist" && useHeist.isOn && rank <=1 && usePoints) {
			Scripts.GetComponent<Heist>().CancelHeist();
		}

		else if(comment[0] == "!heist" && useHeist.isOn && usePoints) {
			if(comment.Length >= 2) {
				Scripts.GetComponent<Heist>().JoinHeist(username, Int32.Parse(comment[1]));
			}
		}

		else if( comment[0] == "!addsub" && rank <= 1 ) {
			if(comment.Length == 3) {
				Scripts.GetComponent<Subscriptions>().AddSub( comment[1], Int32.Parse( comment[2] ) );
			}
			else 
			{
				string newMessage = 	"The Format for AddSub is '!addsub username donation' Ex. " +
									"'!addsub Clossius 1' Use whole numbers only.";
				Scripts.GetComponent<ChatManager>().MessageSend( newMessage );
			}
		}

		else if( comment[0] == "!removesub" && rank <= 1 ) {
			if(comment.Length >= 2) {
				Scripts.GetComponent<Subscriptions>().DeleteSub( comment[1] );
			}
			else 
			{
				string newMessage = 	"The Format for RemoveSub is '!removesub username' Ex. " +
									"'!removesub Clossius'";
				Scripts.GetComponent<ChatManager>().MessageSend( newMessage );
			}
		}

		else if( comment[0] == "!deletesub" && rank <= 1 ) {
			if(comment.Length >= 2) {
				Scripts.GetComponent<Subscriptions>().DeleteSub( comment[1] );
			}
			else 
			{
				string newMessage = 	"The Format for RemoveSub is '!removesub username' Ex. " +
									"'!removesub Clossius'";
				Scripts.GetComponent<ChatManager>().MessageSend( newMessage );
			}
		}

    }


    void StartPoll(string message) {
    	string[] options = message.Split(':');
    	poll = true;
		float seconds = float.Parse(options[1]);
    	pollList = new List<PollClass>();
    	alreadyVoted = new List<string>();
		for(int i=2;i<options.Length;i++) {
			string option = options[i].ToLower();
			PollClass pollClass = new PollClass(option, 0);
    		pollList.Add(pollClass);
    	}
    	message = "Starting Poll. To vote type !vote option. Options:";
		for(int i=0;i<pollList.Count;i++){int votePos = i+1; message = message + " (" + votePos.ToString() + ")" + pollList[i].option;}
		Scripts.GetComponent<ChatManager>().MessageSend(message);
    	StartCoroutine(waitPollFunction(seconds));
    }

	void StartPoll(string[] entries) {
    	poll = true;
    	string message = "";
		float seconds = float.Parse(entries[1]);
    	pollList = new List<PollClass>();
    	alreadyVoted = new List<string>();
		for(int i=2;i<entries.Length;i++) {
			string option = entries[i].ToLower();
			PollClass pollClass = new PollClass(option, 0);
    		pollList.Add(pollClass);
    	}
    	message = "Starting Poll. To vote type !vote option. Options:";
		for(int i=0;i<pollList.Count;i++){
			int votePos = i+1;
			message = message + " (" + votePos.ToString() + ")" + pollList[i].option;
		}
		Scripts.GetComponent<ChatManager>().MessageSend(message);
    	StartCoroutine(waitPollFunction(seconds));
    }

    IEnumerator waitPollFunction(float seconds){
		yield return new WaitForSeconds(seconds);
		string message = "";
		if(poll){
				message = "Voting time has ended!";
			Scripts.GetComponent<ChatManager>().MessageSend(message);
 				PostPollResult();
 			}
		poll = false;
    }

    void PostPollResult() {
		string message = "Results:";
		List<PollClass> tempPollList = new List<PollClass>();
    	for(int i=0;i<pollList.Count;i++){
    		if(i==0){tempPollList.Add (pollList[i]);}
    		else if (pollList[i].vote == tempPollList[0].vote) {tempPollList.Add(pollList[i]);}
    		else if (pollList[i].vote > tempPollList[0].vote){tempPollList = new List<PollClass>(); tempPollList.Add(pollList[i]);}
    		message = message + " " + pollList[i].option + ": " + pollList[i].vote;
    	}
		Scripts.GetComponent<ChatManager>().MessageSend(message);
		PollText.text = message;
		message = "Winner:";
		for(int i=0;i<tempPollList.Count;i++){message = message + " " + tempPollList[i].option;}
		Scripts.GetComponent<ChatManager>().MessageSend(message);
		PollText.text = PollText.text + "\n\n" + message;

    }

    void VoteUpPoll(string option) {
    	int position = Int32.Parse(option);
    	if(position >= 1 && position <= pollList.Count){
			pollList[position-1].vote = pollList[position-1].vote + 1;
    	}
    }

    public void PollStartButtonPressed() {
    	string menuPollOptions = "";
		menuPollOptions = "Time:" + PollTime.textComponent.text;
		if (PollOption01.textComponent.text != ""){menuPollOptions = menuPollOptions + ":" + PollOption01.textComponent.text;}
		if (PollOption02.textComponent.text != ""){menuPollOptions = menuPollOptions + ":" + PollOption02.textComponent.text;}
		if (PollOption03.textComponent.text != ""){menuPollOptions = menuPollOptions + ":" + PollOption03.textComponent.text;}
    	string[] entries = menuPollOptions.Split(':');
    	StartPoll(entries);
    }

    public void PollCancelButtonPressed() {
    	poll = false;
    	PollText.text = "Poll Canceled.";
		Scripts.GetComponent<ChatManager>().MessageSend("Poll Canceled.");

    }

    void UpdatePollTextBoxes() {
    	string pollText = "";
    	for(int i=0;i<pollList.Count;i++){
    		pollText = pollText + pollList[i].option + "\n" + pollList[i].vote + "\n";
    	}
    	PollText.text = pollText;
    }

    void StartRaffle(string message) {
    	string[] entries = message.Split(':');
		StartRaffle(float.Parse(entries[1]), Int32.Parse(entries[2]), Int32.Parse(entries[3]), entries[4]);
    }

    void StartRaffle(float time, int cost, int limit, string reward) {
    	string message = "";
    	raffle = true;
    	maxTickets = limit;
    	ticketCost = cost;
    	string pointsName = Scripts.GetComponent<Settings>().GetPointsName();
    	raffleList = new List<RaffleClass>();
    	message = 	"Raffle has started for '" + reward + "'. It will last " + time + " seconds." +
    				"Tickets cost " + cost + " " + pointsName + ". You can buy up to " + limit +
    				" tickets. Type '!buyticket #' to buy a ticket. Ex: !buyticket 1";
		Scripts.GetComponent<ChatManager>().MessageSend(message);
		StartCoroutine(waitForRaffle(time));
    }

    IEnumerator waitForRaffle(float seconds) {
    	Debug.Log("Raffle Waiting...");
		yield return new WaitForSeconds(seconds);
		maxTickets = 0;
		if(raffle){
			string message = "Raffle time has ended!";
			Scripts.GetComponent<ChatManager>().MessageSend(message);
			GetRaffleWinner();
		}
		raffle = false;

    }

    void BuyTicket(string username, int amount) {
		Debug.Log("Buying Ticket...");
    	bool canAfford = Scripts.GetComponent<Points>().CanAfford(username, ticketCost*amount);
    	string message = "";
    	if(!canAfford){message = username + ", you cannot afford that many tickets.";}
    	else if(amount > maxTickets){message = username + ", you cannot buy that many tickets.";}
    	else if(raffleList.Count == 0 && canAfford && amount <=maxTickets){
    		RaffleClass raffleClass = new RaffleClass(username, amount, maxTickets);
    		raffleList.Add(raffleClass);
			message = username + " bought " + amount + " tickets!";
    	}
		else if(canAfford && amount <=maxTickets){
			int position = -1;
			for(int i=0;i<raffleList.Count;i++){
				if(raffleList[i].username == username){position = i;}
			}
			if(position <0) {
				RaffleClass raffleClass = new RaffleClass(username, amount, maxTickets);
    			raffleList.Add(raffleClass);
    			message = username + " bought " + amount + " tickets!";
			}
			else if((raffleList[position].tickets + amount) > maxTickets) {
				message = username + ", you cannot buy that many tickets.";
			}
			else {
				raffleList[position].tickets = raffleList[position].tickets + amount;
				message = username + " bought " + amount + " tickets!";
			}
    	}
		Scripts.GetComponent<ChatManager>().MessageSend(message);
    }

    void CheckTickets(string username) {
    	string message = "";
    	int position = -1;
    	for(int i=0;i<raffleList.Count;i++) {
    		if(raffleList[i].username == username){position = i;}
    	}
    	if(position<0){message = username + ": 0 tickets.";}
    	else {message = username + ": " + raffleList[position].tickets + " tickets.";}
		Scripts.GetComponent<ChatManager>().MessageSend(message);
    }

    void GetRaffleWinner() {
    	string username = "";
    	string message = "";
		List<string> tickets = new List<string>();
		for(int i=0;i<raffleList.Count;i++){
			for(int g=0;g<raffleList[i].tickets;g++){tickets.Add(raffleList[i].username);}
		}
		alias rnd = new alias();
		int min = 0;
		int max = tickets.Count;
		if(max>0){username = tickets[rnd.Next(min, max)];}
		if(username != ""){
			message = "Winner: " + username + "!";
		}
		Scripts.GetComponent<ChatManager>().MessageSend(message);
    }

    void CancelRaffle() {
    	string message = "Raffle canceled.";
    	raffle = false;
		Scripts.GetComponent<ChatManager>().MessageSend(message);
    }
}
