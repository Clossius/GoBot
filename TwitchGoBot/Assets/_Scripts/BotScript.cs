using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;
using alias = System.Random;
using TMPro;

public class BotScript : MonoBehaviour {


	public GameObject AskForMoveMenu;
	public GameObject LoginButtonObject;
	public GameObject PollMenu;
	public TextMeshProUGUI chatBox1;
	public InputField opponentsMove;
	GameObject Scripts;
	Component Settings;

	bool TwitchMoveCondition = false;
	int BoardSize = 19;
	int chatCount = -1;
	string settingsFileName = "Settings";
	string oauth = "";
	string botName = "";
	string channelName = "#clossius";
	public string pointsName = "Points";
	List<string> ranks = new List<string>();
	bool useRanks = false;
	public Toggle usePoints;
	int rankUpCost = 1500;
	bool loggedIn = false;



	// Use this for initialization
	void Start () {
		ranks.Add("Owner");
		ranks.Add("Admin");
		ranks.Add("Viewer");
		Scripts = GameObject.Find("_Scripts");
		Settings = Scripts.GetComponent<Settings>();


		AskForMoveMenu.SetActive(false);

	}


	public void ActivateAskForMoveButton() {
		if(loggedIn){AskForMoveMenu.SetActive(true);}
	}


	//Login to the Robot channel, then joins the specified channel chat.
	public void Connect()
    {	
    	string botNameLower = "";
    	for (int i=0;i<botName.Length;i++) {
    		botNameLower = botNameLower + Char.ToLower(botName[i]);
    	}

		TwitchIrc.Instance.enabled = true;
		TwitchIrc.Instance.Username = botName;
		//TwitchIrc.Instance.OauthToken = OauthToken.textComponent.text;
		TwitchIrc.Instance.OauthToken = oauth;
		TwitchIrc.Instance.Channel = channelName;
		//TwitchIrc.Instance.OnServerMessage += OnServerMessage;
        TwitchIrc.Instance.Connect();

		Scripts.GetComponent<ChatManager>().Connecting();
		
		AskForMoveMenu.SetActive(true);
		LoginButtonObject.SetActive(false);
		loggedIn = true;

		//Subscribe for events
        //TwitchIrc.Instance.OnChannelMessage += OnChannelMessage;
        //TwitchIrc.Instance.OnUserLeft += OnUserLeft;
        //TwitchIrc.Instance.OnUserJoined += OnUserJoined;
        //TwitchIrc.Instance.OnServerMessage += OnServerMessage;
        //TwitchIrc.Instance.OnExceptionThrown += OnExceptionThrown;
		//StartCoroutine(GetURL("https://api.twitch.tv/kraken/channels/clossius/follows"));
    }

    IEnumerator GetURL (string url) {
		WWW www = new WWW(url);
		yield return new WaitForSeconds(5.0f);
		Debug.Log(www.text);
		StartCoroutine(GetURL("https://api.twitch.tv/kraken/channels/clossius/follows"));
    }


    public void LoginButton() {
		botName = Scripts.GetComponent<Settings>().GetBotUserName();
		oauth = Scripts.GetComponent<Settings>().GetOauthToken();
		channelName = Scripts.GetComponent<Settings>().GetChannelToJoinName();

		if( botName != "N/A" && oauth != "N/A" && channelName != " N/A" &&
			botName != "n/a" && oauth != "n/a" && channelName != " n/a" ) {
    		Connect();
    		Scripts.GetComponent<OnUserJoins>().Login();
    	}

    	else {chatBox1.text = "Please update your settings.";}
    }

    public void Logout()
    {
    	TwitchIrc.Instance.Disconnect();
		LoginButtonObject.SetActive(true);
		AskForMoveMenu.SetActive(false);

		chatBox1.text = "LOGGED OUT.";
		loggedIn = false;

		TwitchIrc.Instance.enabled = false;
    }

}