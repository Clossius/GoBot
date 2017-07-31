using UnityEngine;
using System.Collections;
using Irc;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;


public class Points : MonoBehaviour {

	string pointsName = "ClossiCoins";
	string pointsFileName = "Points";
	float pointTimeSchedule = 300.0f;
	int pointsToBeAdded = 5;
	public GameObject chatBox;
	public GameObject TextBoxesObject;
	public GameObject pointsMenu;
	public GameObject mainMenu;
	public GameObject pollMenu;
	public GameObject chatButton;
	public GameObject AskForMoveMenu;
	public GameObject ViewerChatBoxes;
	public GameObject SettingsMenu;
	public GameObject StoreMenu;
	public GameObject RanksMenu;
	public GameObject CommandsMenu;
	public Text userBox1;
	public Text userBox2;
	public Text userBox3;
	public Text userBox4;
	int userBoxChatLength = 14;
	public InputField pointsToChange;
	public InputField userNameToChange;
	public Toggle usePoints;
	public Toggle useBonus;
	public Toggle subsBonus;
	public Toggle useSubs;
	bool useRanks = false;
	int rankUpCost = 1500;



	public class Users {
		public string userName;
		public int points;
		public string rank;

		public Users (string UN, int p, string r) {
			userName = UN;
			points = p;
			rank = r;
		 } 

	}


	List<Users> userList = new List<Users>();

	// Use this for initialization
	void Start () {
		Debug.Log("Loading Points...");
		LoadPointsFile(pointsFileName);
		SavePoints();
		chatBox.SetActive(true);
		if(userList.Count == 0){Users user = new Users("Username", 0, "Rank"); userList.Add(user); SavePoints();}

		TwitchIrc.Instance.OnUserLeft += OnUserLeft;
		TwitchIrc.Instance.OnChannelMessage += OnChannelMessage;

		StartCoroutine(waitForTwitch(pointTimeSchedule));

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Get the name of the user who joined to channel 
    public void UserJoined(string user)
    {	
    	Debug.Log("Points.OnUserJoined");
    	Debug.Log("User: " + user);
        bool condition = true;
        string username = "";
		for(int i=0;i<user.Length;i++){username = username + Char.ToLower(user[i]); Debug.Log("USERNAME: " + username);}
        for (int i=0; i<userList.Count;i++) {
			if (username == userList[i].userName) {condition = false; Debug.Log("User Found in UserList");}

        }

        if (condition) {
			Debug.Log("User Not Found in UserList");
			AddUserToUserList(username);
        }
        int position = -1;
        for(int i=0;i<userList.Count;i++){
			if (username == userList[i].userName) {position = i;}
        }
        if(position == -1){Debug.Log("ERROR: Points.OnUserJoined");}
		else if (userList[position].rank == "null" || userList[position].rank == "Null" || userList[position].rank == "NULL" || userList[position].rank == null){
			string defaultRank = GameObject.Find("_Scripts").GetComponent<Ranks>().GetDefaultRank();
			userList[position].rank = defaultRank;
		}
		GameObject.Find("_Scripts").GetComponent<Viewers>().AddViewer(userList[position].userName, userList[position].rank, userList[position].points);

    }

    //Add user to UserList

    void AddUserToUserList(string userName) {
    		string rank = GameObject.Find("_Scripts").GetComponent<Ranks>().GetDefaultRank();
    		Debug.Log("New User: " + userName + " 0 " + rank);
			Users newUser = new Users(userName, 0, rank); 
			userList.Add(newUser);
        	
        	SavePoints();
    }


	//Get the name of the user who left the channel.
    void OnUserLeft(UserLeftEventArgs userLeftArgs)
    {
        //ChatText.text += "<b>" + "USER LEFT" + ":</b> " + userLeftArgs.User + "\n";
        int position = -1;

    }

	//Receive username that has been left from channel 
	void OnChannelMessage(ChannelMessageEventArgs channelMessageArgs)
    {	

		//tempText = "<b>" + channelMessageArgs.From + ":</b> " + channelMessageArgs.Message + "\n";
        Debug.Log("MESSAGE: " + channelMessageArgs.From + ": " + channelMessageArgs.Message);

        bool notInViewerList = true;
        /*for(int i=0;i<viewerList.Count;i++) {
        	if(viewerList[i].userName == channelMessageArgs.From){notInViewerList = false;}
        }
        if (notInViewerList) {
        	AddUserToViewerList(channelMessageArgs.From);
        }*/

		bool notInUserList = true;
        for(int i=0;i<userList.Count;i++) {
			if(userList[i].userName == channelMessageArgs.From){notInUserList = false;}
        }
		if (notInUserList) {
        	AddUserToUserList(channelMessageArgs.From);
        }

        if(usePoints.isOn){CheckForPointsCommand(channelMessageArgs.From, channelMessageArgs.Message);}

    }

    //Checks to see if the custom points command was used.
    public void CheckForPointsCommand(string username, string comment) {
		string[] message = comment.Split(' ');
		string messageText = "";
		pointsName = GameObject.Find("_Scripts").GetComponent<Settings>().GetPointsName();
    	string pointsCommand = "!" + pointsName.ToLower();

		if(message[0].ToLower() == pointsCommand.ToLower() && message.Length==1) {

				int points = 0;
				int position = 0;
				for (int i=0; i<userList.Count;i++) {
					string lowerCaseUsername = "";
					for (int g=0;g<username.Length;g++) {lowerCaseUsername = lowerCaseUsername + Char.ToLower(username[g]);}
					if (userList[i].userName == lowerCaseUsername) {points = userList[i].points; position=i;}
				}
				messageText = username + "(" + userList[position].rank + "): " + points.ToString() + " " + pointsName + ".";
				GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(messageText);
        	}

		else if(message[0].ToLower() == pointsCommand.ToLower()) {

				int points = 0;
				int position = 0;
				for (int i=0; i<userList.Count;i++) {
					string lowerCaseUsername = "";
					username = message[1];
					for (int g=0;g<username.Length;g++) {lowerCaseUsername = lowerCaseUsername + Char.ToLower(username[g]);}
					if (userList[i].userName == lowerCaseUsername) {points = userList[i].points; position=i;}
				}
				messageText = username + "(" + userList[position].rank + "): " + points.ToString() + " " + pointsName + ".";
			GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(messageText);
        }
    }

    public int GetPoints(string username) {
    	int points = 0;
    	for(int i=0;i<userList.Count;i++) {
			string lowerCaseUsername = "";
			for (int g=0;g<username.Length;g++) {lowerCaseUsername = lowerCaseUsername + Char.ToLower(username[g]);}
			if (userList[i].userName == lowerCaseUsername) {points = userList[i].points;}
    	}
    	return points;
    }

    public bool CanAfford(string username, int cost) {
    	bool canAfford = false;

		for(int i=0;i<userList.Count;i++) {
			string lowerCaseUsername = "";
			for (int g=0;g<username.Length;g++) {lowerCaseUsername = lowerCaseUsername + Char.ToLower(username[g]);}
			if (userList[i].userName == lowerCaseUsername) {
				if(userList[i].points >= cost){canAfford = true;}
			}
    	}
    	return canAfford;
    }

	//Load .txt file with username and points.

	void LoadPointsFile(string fileName) {
		StreamReader fileObject = GameObject.Find("_Scripts").GetComponent<FileManager>().LoadSettings( fileName );
		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				if (line != null) {
					string[] entries = line.Split(':');
					if (entries.Length > 0) {
						int points = Int32.Parse(entries[1]);
						Users tempUser = new Users(entries[0], points, entries[2]);
						Debug.Log("UserName: " + tempUser.userName +"\nPoints: " + tempUser.points + " Rank: " + tempUser.rank);
						userList.Add(tempUser);
					}
				}
				}
				while (line != null);
			fileObject.Close();

		}
	}

	//Save points to file

	void SavePoints () {
		string pointsText = "";
		Debug.Log(userList.Count);
		for (int i=0; i< userList.Count;i++) {
			if(i > 0) {pointsText = pointsText +"\n";}
			string botName = GameObject.Find("_Scripts").GetComponent<Settings>().GetBotUserName();
			string lowerCaseBotName = "";
			for(int g=0;g<botName.Length;g++){lowerCaseBotName = lowerCaseBotName + Char.ToLower(botName[g]);}
			if(userList[i].userName == lowerCaseBotName){
				string rank = GameObject.Find("_Scripts").GetComponent<Ranks>().GetAdminRank();
				pointsText = pointsText + userList[i].userName + ":" + userList[i].points.ToString() + ":" + rank;
			}
			else {
				pointsText = pointsText + userList[i].userName + ":" + userList[i].points.ToString() + ":" + userList[i].rank;
			}
		}

		GameObject.Find("_Scripts").GetComponent<FileManager>().SaveSettings( pointsFileName, pointsText );

	}

	//Add points on a time table

	IEnumerator waitForTwitch(float seconds) {
		yield return new WaitForSeconds(seconds);
		if(usePoints.isOn) {
			Debug.Log("Giving out points.");
			AddScheduledPoints();
		}

		StartCoroutine(waitForTwitch(pointTimeSchedule));

	}

	void AddScheduledPoints() {
		List<string> viewers = GameObject.Find("_Scripts").GetComponent<Viewers>().GetViewerList();
		for(int i=0;i<viewers.Count;i++) {
			for(int g=0;g<userList.Count;g++){
				if(userList[g].userName == viewers[i])
				{
					int bonus = 0;
					int subBonus = 0;

					if ( useBonus.isOn )
					{
						bonus = GetBonusPoints( g );
					}

					if ( useSubs.isOn && subsBonus.isOn )
					{
						subBonus = 	GameObject.Find( "_Scripts" ).
									GetComponent<Subscriptions>().
									GetSubDonation( userList[g].userName );
					}

					userList[g].points = userList[g].points + pointsToBeAdded + bonus + subBonus;
				}
			}
		}
		SavePoints();
		UpDateUserPoints();
	}

	int GetBonusPoints ( int userPosition ) 
	{
		int bonus =  0;
		int rank = GetRankInt ( userList[userPosition].userName );

		bonus = GameObject.Find( "_Scripts" ).GetComponent<Ranks>().GetRankListSize();
		bonus = bonus - rank - 1;

		if ( bonus < 0 )
		{
			Debug.Log ( "Error: --Points.GetBonusPoints--" );
			bonus = 0;
		}

		return bonus;
	}

	//Add points to a specific user

	public void AddPoints(string userName, int points) {
		int position = 0;
		for (int i=0;i<userList.Count;i++) {
			if(userList[i].userName == userName){position = i;}
		}
		userList[position].points = userList[position].points + points;
		string message = userName + " gained " + points +" "+ pointsName + "!";
		GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(message);
		SavePoints();
		UpDateUserPoints();
	}

	public void AddPointsSilently(string userName, int points) {
		int position = 0;
		for (int i=0;i<userList.Count;i++) {
			if(userList[i].userName == userName){position = i;}
		}
		userList[position].points = userList[position].points + points;
		SavePoints();
		UpDateUserPoints();
	}

	//Take points

	public void TakePoints(string userName, int points) {
		int position = 0;
		for (int i=0;i<userList.Count;i++) {
			if(userList[i].userName == userName){position = i;}
		}
		userList[position].points = userList[position].points - points;
		if (userList[position].points < 0) {userList[position].points = 0;}
		string message = userName + " lost " + points +" "+ pointsName + ".";
		GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(message);
		SavePoints();

	}

	public void TakePointsSilently ( string userName, int points ) 
	{
		int position = 0;
		for (int i=0;i<userList.Count;i++) {
			if(userList[i].userName == userName){position = i;}
		}
		userList[position].points = userList[position].points - points;
		if (userList[position].points < 0) {userList[position].points = 0;}
		SavePoints();

	}

	//Display points when points button is clicked

	public void DisplayPoints() {
		UpDateUserPoints();
		chatBox.SetActive(false);
		TextBoxesObject.SetActive(true);
		pointsMenu.SetActive(true);
		mainMenu.SetActive(false);
		pollMenu.SetActive(false);
		AskForMoveMenu.SetActive(false);
		ViewerChatBoxes.SetActive(false);
		SettingsMenu.SetActive(false);
		StoreMenu.SetActive(false);
		RanksMenu.SetActive(false);
		CommandsMenu.SetActive(false);
	}

	void ReSetRanks() {
		for(int i=0;i<userList.Count;i++){
			userList[i].rank = GameObject.Find("_Scripts").GetComponent<Ranks>().CheckOrConvertRank(userList[i].rank);
		}
	}

	void UpDateUserPoints() {
		string textToBeShown01 = "";
		string textToBeShown02 = "";
		string textToBeShown03 = "";
		string textToBeShown04 = "";

		List<Users> sortedUserList = userList.OrderByDescending(o=>o.points).ToList();

		for(int i=0; i < userList.Count;i++) {
			if(i <= userBoxChatLength)			{textToBeShown01 = textToBeShown01 + "<b>" + sortedUserList[i].userName + "</b>" + "\n " + sortedUserList[i].rank + " " + sortedUserList[i].points + "\n";}
			else if(i <= userBoxChatLength*2)	{textToBeShown02 = textToBeShown02 + "<b>" + sortedUserList[i].userName + "</b>" + "\n " + sortedUserList[i].rank + " " + sortedUserList[i].points + "\n";}
			else if(i <= userBoxChatLength*3)	{textToBeShown03 = textToBeShown03 + "<b>" + sortedUserList[i].userName + "</b>" + "\n " + sortedUserList[i].rank + " " + sortedUserList[i].points + "\n";}
			else if(i <= userBoxChatLength*4)	{textToBeShown04 = textToBeShown04 + "<b>" + sortedUserList[i].userName + "</b>" + "\n " + sortedUserList[i].rank + " " + sortedUserList[i].points + "\n";}

		}
		userBox1.text = textToBeShown01;
		userBox2.text = textToBeShown02;
		userBox3.text = textToBeShown03;
		userBox4.text = textToBeShown04;
	}

	public void DisplayPoints(string username) {
		string textToBeShown = "";
		int position = -1;
		for(int i=0;i<userList.Count;i++) {if(userList[i].userName == username){position = i;}}
		if(position < 0){Debug.Log("ERROR: Points.DisplatPoints");}
		if (useRanks){textToBeShown = textToBeShown + username + "(" + userList[position].rank + "): " + userList[position].points + " " + pointsName;}
		else {textToBeShown = textToBeShown + username + ": " + userList[position].points + " " + pointsName;}
		if(position >-1) {GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(textToBeShown);}
	}

	public void PollButtonPressed () {
		chatBox.SetActive(false);
		TextBoxesObject.SetActive(false);
		pointsMenu.SetActive(false);
		mainMenu.SetActive(false);
		pollMenu.SetActive(true);
		AskForMoveMenu.SetActive(false);
		ViewerChatBoxes.SetActive(false);
		SettingsMenu.SetActive(false);
		StoreMenu.SetActive(false);
		RanksMenu.SetActive(false);
		CommandsMenu.SetActive(false);
	}

	public void RanksButtonPressed () {
		chatBox.SetActive(false);
		TextBoxesObject.SetActive(false);
		pointsMenu.SetActive(false);
		mainMenu.SetActive(false);
		pollMenu.SetActive(false);
		AskForMoveMenu.SetActive(false);
		ViewerChatBoxes.SetActive(false);
		SettingsMenu.SetActive(false);
		StoreMenu.SetActive(false);
		RanksMenu.SetActive(true);
		CommandsMenu.SetActive(false);
	}

	public void StoreButtonPressed () {
		chatBox.SetActive(false);
		TextBoxesObject.SetActive(false);
		pointsMenu.SetActive(false);
		mainMenu.SetActive(false);
		pollMenu.SetActive(false);
		AskForMoveMenu.SetActive(false);
		ViewerChatBoxes.SetActive(false);
		SettingsMenu.SetActive(false);
		StoreMenu.SetActive(true);
		RanksMenu.SetActive(false);
		CommandsMenu.SetActive(false);
	}

	public void SettingsButtonPressed () {
		chatBox.SetActive(false);
		TextBoxesObject.SetActive(false);
		pointsMenu.SetActive(false);
		mainMenu.SetActive(false);
		pollMenu.SetActive(false);
		AskForMoveMenu.SetActive(false);
		ViewerChatBoxes.SetActive(false);
		SettingsMenu.SetActive(true);
		StoreMenu.SetActive(false);
		RanksMenu.SetActive(false);
		CommandsMenu.SetActive(false);
	}

	public void ChatButtonPressed () {
		chatBox.SetActive(true);
		TextBoxesObject.SetActive(false);
		pointsMenu.SetActive(false);
		mainMenu.SetActive(true);
		pollMenu.SetActive(false);
		ViewerChatBoxes.SetActive(false);
		GameObject.Find("_Scripts").GetComponent<BotScript>().ActivateAskForMoveButton();
		SettingsMenu.SetActive(false);
		StoreMenu.SetActive(false);
		RanksMenu.SetActive(false);
		CommandsMenu.SetActive(false);
	}

	public void ViewerButtonPressed () {
		chatBox.SetActive(false);
		TextBoxesObject.SetActive(false);
		pointsMenu.SetActive(false);
		mainMenu.SetActive(false);
		pollMenu.SetActive(false);
		ViewerChatBoxes.SetActive(true);
		AskForMoveMenu.SetActive(false);
		SettingsMenu.SetActive(false);
		StoreMenu.SetActive(false);
		RanksMenu.SetActive(false);
		CommandsMenu.SetActive(false);
	}

	public void CommandsButtonPressed () {
		chatBox.SetActive(false);
		TextBoxesObject.SetActive(false);
		pointsMenu.SetActive(false);
		mainMenu.SetActive(false);
		pollMenu.SetActive(false);
		ViewerChatBoxes.SetActive(false);
		AskForMoveMenu.SetActive(false);
		SettingsMenu.SetActive(false);
		StoreMenu.SetActive(false);
		RanksMenu.SetActive(false);
		CommandsMenu.SetActive(true);
	}

	//adjust points from menu buttons

	public void AddButtonPressed () {
		string userName = userNameToChange.textComponent.text.ToLower();
		int points = Int32.Parse(pointsToChange.textComponent.text);
		AddPoints(userName, points);
		SavePoints();
		DisplayPoints();
	}

	public void TakeButtonPressed () {
		string userName = userNameToChange.textComponent.text.ToLower();
		int points = Int32.Parse(pointsToChange.textComponent.text);
		TakePoints(userName, points);
		SavePoints();
		DisplayPoints();
	}

	public void AddToAllButtonPressed () {
		List<string> viewerList = GameObject.Find("_Scripts").GetComponent<Viewers>().GetViewerList();
		int points = Int32.Parse(pointsToChange.textComponent.text);
		for(int i=0;i<viewerList.Count;i++){
			for(int g=0;g<userList.Count;g++){
				if(viewerList[i] == userList[g].userName){AddPoints(userList[g].userName, points);}
			}
		}
		SavePoints();
		DisplayPoints();
	}

	//Get points name from BotScript
	public void GetPointsName (string pointsNameFromSettings) {
		pointsName = pointsNameFromSettings;
		Debug.Log("Points Name: " + pointsName);
	}

	//Check for points.
	public int CheckForPoints(string username) {
		int points = 0;
		for(int i=0; i<userList.Count;i++) {
			if(userList[i].userName == username.ToLower()) {points = userList[i].points;}
		}

		return points;
	}


	public void PointsToBeAdded(int number) {
		pointsToBeAdded = number;
	}



	public string GetRank(string username) {
		string rank = "";
		for(int i=0;i<userList.Count;i++){
			if(userList[i].userName.ToLower() == username.ToLower()){
				rank = userList[i].rank; 
				Debug.Log("Rank: " + rank);
				Debug.Log(userList[i].userName);
			}
		}
		return rank;
	}

	public void ChangeRank(string username, string newRank) {
		string message = "";
		for(int i=0;i<userList.Count;i++) {
			if(userList[i].userName == username){
				userList[i].rank = newRank;
				message = username + " changed rank to " + userList[i].rank + "!";
				}
			}
		GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(message);
		SavePoints();
	}

	public int GetRankInt(string username) 
	{
		string rank = "";
		int permission = 100;

		for(int i=0;i<userList.Count;i++)
		{
			if(userList[i].userName == username){rank = userList[i].rank;}
		}

		if (permission == 100)
		{
			Debug.Log ( "Rank not found. --Points.GetRankInt--" );
		}

		permission = GameObject.Find("_Scripts").GetComponent<Ranks>().GetPermissionLevel(rank);

		Debug.Log ( "Permission Level: " + permission );

		return permission;
	}


}
