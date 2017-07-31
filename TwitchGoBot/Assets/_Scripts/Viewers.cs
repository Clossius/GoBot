using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Irc;
using System.Linq;
using System.Collections.Generic;



public class Viewers : MonoBehaviour {

	GameObject Scripts;
	public Text viewerCountDisplay;
	public Text ChatBoxDisplay01;
	public Text ChatBoxDisplay02;
	public Text ChatBoxDisplay03;
	public Text ChatBoxDisplay04;
	int viewerCount = 0;
	int chatBoxListSize = 16;

	public class viewerClass {
		public string username;
		public string rank;
		public int points;

		public viewerClass (string u, string r, int p) {
			username = u;
			rank = r;
			points = p;
		}
	}

	List<viewerClass> viewerList = new List<viewerClass>();
	List<string> botList = new List<string>();
	
	// Use this for initialization
	void Start () {
		TwitchIrc.Instance.OnUserLeft += OnUserLeft;
        TwitchIrc.Instance.OnUserJoined += OnUserJoined;
		TwitchIrc.Instance.OnChannelMessage += OnChannelMessage;
		TwitchIrc.Instance.OnExceptionThrown += OnExceptionThrown;
        Scripts = GameObject.Find("_Scripts");
        botList.Add("moobot");
		botList.Add("nightbot");
		botList.Add(Scripts.GetComponent<Settings>().GetBotUserName().ToLower());
        DisplayViewers();
	}
	
	// Update is called once per frame
	void Update () {


	}

	void DisplayViewers() {
		viewerCount = viewerList.Count;
		for(int i=0;i<viewerList.Count;i++){
			for(int g=0;g<botList.Count;g++){if(botList[g] == viewerList[i].username){viewerCount = viewerCount - 1;}}
		}
		viewerCountDisplay.text = "Viewers: " + viewerCount.ToString();
		string chatBox01 = "";
		string chatBox02 = "";
		string chatBox03 = "";
		string chatBox04 = "";
		if (viewerCount <= chatBoxListSize){
			for(int i=0;i<viewerList.Count;i++)	{
					bool isBot = false;
					for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox01 = chatBox01 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
		}
		else if (viewerCount <= (chatBoxListSize*2)){
			for(int i=0;i<chatBoxListSize;i++)	{						
					bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox01 = chatBox01 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
			for(int i=chatBoxListSize;i<viewerList.Count;i++)	{	
					bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox02 = chatBox02 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
		}
		else if (viewerCount <= (chatBoxListSize*3)){
			for(int i=0;i<chatBoxListSize;i++)	{	
					bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox01 = chatBox01 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
			for(int i=(chatBoxListSize);i<(chatBoxListSize*2);i++)	{	
							bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox02 = chatBox02 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
			for(int i=(chatBoxListSize*2);i<viewerList.Count;i++)	{	
								bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox03 = chatBox03 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
		}
		else if(viewerCount <= (chatBoxListSize*4)) {
			for(int i=0;i<chatBoxListSize;i++)	{	
									bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox01 = chatBox01 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
			for(int i=(chatBoxListSize);i<(chatBoxListSize*2);i++)	{	
										bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox02 = chatBox02 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
			for(int i=(chatBoxListSize*2);i<(chatBoxListSize*3);i++)	{	
											bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox03 = chatBox03 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
			for(int i=(chatBoxListSize*3);i<(viewerList.Count);i++)	{	
												bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox04 = chatBox04 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
		}
		else {
			for(int i=0;i<chatBoxListSize;i++)	{	
													bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox01 = chatBox01 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
			for(int i=(chatBoxListSize);i<(chatBoxListSize*2);i++)	{	
														bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox02 = chatBox02 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
			for(int i=(chatBoxListSize*2);i<(chatBoxListSize*3);i++)	{	
															bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox03 = chatBox03 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
			for(int i=(chatBoxListSize*3);i<(chatBoxListSize*4);i++)	{	
																bool isBot = false;
				for(int g=0;g<botList.Count;g++){if(viewerList[i].username == botList[g]){isBot=true;}}
					if(!isBot){
					chatBox04 = chatBox04 + "<b>" + viewerList[i].username + "</b>\n" + 
					"(" + Scripts.GetComponent<Points>().GetRank(viewerList[i].username) + 
					") " + Scripts.GetComponent<Points>().GetPoints(viewerList[i].username).ToString() + "\n";}
												}
		}

		ChatBoxDisplay01.text = chatBox01;
		ChatBoxDisplay02.text = chatBox02;
		ChatBoxDisplay03.text = chatBox03;
		ChatBoxDisplay04.text = chatBox04;
	}

	//Receive exeption if something goes wrong
    private void OnExceptionThrown(Exception exeption)
    {
        Debug.Log("EXEPTION: " + exeption);
    }

	//Get the name of the user who joined to channel 
    void OnUserJoined(UserJoinedEventArgs userJoinedArgs)
    {
        //ChatText.text += "<b>" + "USER JOINED" + ":</b> " + userJoinedArgs.User + "\n";
        string username = userJoinedArgs.User;
		string rank = Scripts.GetComponent<Points>().GetRank(username);
		int points = Scripts.GetComponent<Points>().CheckForPoints(username);
		AddViewer(username, rank, points);
		DisplayViewers();
    }

    public void AddViewer(string username, string rank, int points) {
		viewerClass viewer = new viewerClass("", "", 0);
		bool viewerInList = false;
		for(int i=0;i<viewerList.Count;i++){if(username == viewerList[i].username){viewerInList = true;}}

		if(!viewerInList) {
			viewer.username = username;
			viewer.rank = rank;
			viewer.points = points;
			viewerList.Add(viewer);
		}
		DisplayViewers();
    }

	//Get the name of the user who left the channel.
    void OnUserLeft(UserLeftEventArgs userLeftArgs)
    {
        //ChatText.text += "<b>" + "USER LEFT" + ":</b> " + userLeftArgs.User + "\n";
        Debug.Log("User Left: " + userLeftArgs.User);
        int position = -1;
        for(int i=0;i<viewerList.Count;i++){if(viewerList[i].username == userLeftArgs.User){position = i;}}
        if(position>=0){viewerList.RemoveAt(position);}
		DisplayViewers();
    }

	//Receive username and message that sends message to channel
	void OnChannelMessage(ChannelMessageEventArgs channelMessageArgs)
    {	
		//tempText = "<b>" + channelMessageArgs.From + ":</b> " + channelMessageArgs.Message + "\n";
        //Debug.Log("MESSAGE: " + channelMessageArgs.From + ": " + channelMessageArgs.Message);
        bool IsOnline = false;
        for(int i=0;i<viewerList.Count;i++){if(channelMessageArgs.From == viewerList[i].username){IsOnline = true;}}
        if(!IsOnline){
			viewerClass viewer = new viewerClass("", "", 0);
			viewer.username = channelMessageArgs.From;
			viewer.rank = Scripts.GetComponent<Points>().GetRank(channelMessageArgs.From);
			viewer.points = Scripts.GetComponent<Points>().CheckForPoints(channelMessageArgs.From);
			viewerList.Add(viewer);
        }
		DisplayViewers();

    }

    public List<String> GetViewerList () {
    	List<string> viewerListToSend = new List<string>();
    	for(int i=0;i<viewerList.Count;i++){
    		string viewer = "";
			for(int g=0;g<viewerList[i].username.Length;g++){viewer = viewer + Char.ToLower(viewerList[i].username[g]);}
    		viewerListToSend.Add(viewer);
    	}
    	return viewerListToSend;
    }
}
