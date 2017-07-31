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

public class ChatManager : MonoBehaviour {

	public InputField InputText;
	public GameObject ChatInputFeild;
	GameObject Scripts;
	public GameObject SubmitButtonObject;
	int length = 6; //Max text line length.
	public Toggle usePoints; //The toggle for points.

	List<string> chatArray = new List<string>();
	public TextMeshProUGUI chatTextPro1;
	public TextMeshProUGUI chatTextPro2;


	// Use this for initialization
	void Start () {
		Scripts = GameObject.Find("_Scripts");

		InitializeChatArray();
		DisplayChat();
		SubmitButtonObject.SetActive(false); //Set to false so you cannot submit text until logged in.
		ChatInputFeild.SetActive(false);
		chatTextPro1.text = "Welcome GoBot user!\n\n" +
						"This program was developed by http://www.ShawnsGoGroup.com" +"\n\n"+
						"Before using this bot, you will need to input your information\n" +
						"in the settings. Make sure the oauth token is to the bots account.\nViewers are updated" +
						" every 20 seconds or so." +
						"\n\n";
		chatTextPro2.text	= "I hope you enjoy GoBot!\n\n" +
						"For feature request, or questions, message me on Patreon at www.patreon.com/Clossius";

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//When connected, load the settings.
	public void Connecting ()
	{
		SubmitButtonObject.SetActive(true);
		ChatInputFeild.SetActive(true);

		ClearChatText();

		TwitchIrc.Instance.OnServerMessage += OnServerMessage;
		TwitchIrc.Instance.OnChannelMessage += OnChannelMessage;
	}

	//Receive message from server
    void OnServerMessage(string message)
    {
        ReInitializeChatArray("<b>SERVER:</b> " + message + "\n");
    }

	//Receive username that has been left from channel 
	void OnChannelMessage(ChannelMessageEventArgs channelMessageArgs)
    {	
    	string tempText = "";
		tempText = "<b>" + channelMessageArgs.From + ":</b> " + channelMessageArgs.Message + "\n";

		tempText = CheckTextForEmoji( tempText );

		if( tempText == chatArray[chatArray.Count -1] ){ return; }
        ReInitializeChatArray(tempText);
        DisplayChat();
    }

	//Initialize the Chat array
	void InitializeChatArray() {

		chatArray = new List<string>();
	}

	void ReInitializeChatArray(string tempString) {
		if (chatArray.Count < length*2) {
			AddToChatArray(tempString);

		}

		else if (chatArray.Count >= length*2) {
			for (int i=0;i<chatArray.Count-1;i++) {
				chatArray[i] = chatArray[i+1];
			}
			chatArray[chatArray.Count-1] = tempString;
			Debug.Log("Finished ReInitializing Chat Array.");
		}
		DisplayChat();
	}

	//Adds string given to the chat array.
	void AddToChatArray(string tempString) {
		chatArray.Add(tempString);
	}

	//Updates the chat text display.
	void DisplayChat() {

		chatTextPro1.text = "";
		chatTextPro2.text = "";

		for( int i=0; i<chatArray.Count; i++ ){ chatArray[i] = CheckTextForEmoji( chatArray[i] ); }

		if(chatArray.Count > 0 && chatArray.Count >= length ){
			for (int i=0; i<length; i++) {
				if(i < chatArray.Count){chatTextPro2.text += chatArray[i] + "\n";}
			}
			for (int i=length; i<chatArray.Count; i++) {
				chatTextPro1.text += chatArray[i] + "\n";
			}
		}

		else if(chatArray.Count > 0 ){
			for (int i=0; i<=length; i++) {
				if(i < chatArray.Count){chatTextPro1.text += chatArray[i] + "\n";}
			}
		}
	}

	//Clears the chat text.
	public void ClearChatText ()
	{
		chatTextPro1.text = "";
		chatTextPro2.text = "";

		DisplayChat();
	}


	// Sends the message given to Twitch.
	// TODO: Fix the duplicate message bug
	// after logging out and back in.
	public void MessageSend(string MessageText)
    {
    	if ( MessageText == chatArray[chatArray.Count-1] )
    		return;

        if (String.IsNullOrEmpty(MessageText))
            return;

        TwitchIrc.Instance.Message(MessageText);

        String tempText = "<b>" + TwitchIrc.Instance.Username + "</b>: " + MessageText +"\n";
        string botName = Scripts.GetComponent<Settings>().GetBotUserName();

		GameObject.Find("_Scripts").GetComponent<CommandsScript>().CheckCommand(botName, MessageText, 0);

        ReInitializeChatArray(tempText);
        DisplayChat();

        MessageText = "";
    }

	public void MessageSend(string MessageText, string message2)
    {
		if ( MessageText == chatArray[chatArray.Count-1] )
    		return;

		if ( message2 == chatArray[chatArray.Count-1] )
    		return;

        if (String.IsNullOrEmpty(MessageText))
            return;

        TwitchIrc.Instance.Message(MessageText);

        String tempText = "<b>" + TwitchIrc.Instance.Username + "</b>: " + MessageText + "\n";
		ReInitializeChatArray("<b>" + TwitchIrc.Instance.Username + "</b>: " + message2 + "\n");
        DisplayChat();
        MessageText = "";
    }

	public void SendPrivateMessage(string username, string MessageText)
    {
		if ( MessageText == chatArray[chatArray.Count-1] )
    		return;

        if (String.IsNullOrEmpty(MessageText))
            return;

        TwitchIrc.Instance.Message(MessageText);
		String tempText = "PRIVMSG #jtv :/w " + username + " " + MessageText + "\n";
		ReInitializeChatArray(tempText);
        DisplayChat();
        MessageText = "";
    }


    public void SubmitButton() {
    	MessageSend(InputText.textComponent.text);
    	InputText.textComponent.text = "";
    }

	void OnGUI() {
		if(InputText.isFocused && InputText.text != "" && Input.GetKey(KeyCode.Return)) {
         	SubmitButton();
			InputText.text = "";
    	 }

		if( Input.GetKey(KeyCode.Return) && InputText.isFocused )
    	 {
    	 	Debug.Log( "Focusing on Input Field" );
			InputText.Select();
			InputText.ActivateInputField();
    	 }

 	}


 	// Replace text with emoji.
 	string CheckTextForEmoji( string text )
 	{
 		string[] words = text.Split( ' ' );

 		for( int i=0; i<words.Length; i++ )
 		{
			if( words[i].ToLower() == "kappa" || words[i].ToLower() == "kappa\n" )
			{ 
				words[i] = "<sprite=\"kappa\" name=\"kappa\">";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == "xd" || words[i].ToLower() == "xd\n" )
			{ 
				words[i] = "<sprite=6>";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == ":)" || words[i].ToLower() == ":)\n" )
			{ 
				words[i] = "<sprite=0>";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == ":(" || words[i].ToLower() == ":(\n" )
			{ 
				words[i] = "<sprite=15>";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == "^.~" || words[i].ToLower() == "^.~\n" )
			{ 
				words[i] = "<sprite=11>";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == ":p" || words[i].ToLower() == ":p\n" )
			{ 
				words[i] = "<sprite=1>";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == ":d" || words[i].ToLower() == ":d\n" )
			{ 
				words[i] = "<sprite=8>";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == "bloodtrail" || words[i].ToLower() == "bloodtrail\n" )
			{ 
				words[i] = "<sprite=\"bloodtrail\" name=\"bloodtrail\">";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == "dansgaming" || words[i].ToLower() == "dansgaming\n" )
			{ 
				words[i] = "<sprite=\"dansgaming\" name=\"dansgaming\">";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == "keepo" || words[i].ToLower() == "keepo\n" )
			{ 
				words[i] = "<sprite=\"keepo\" name=\"keepo\">";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == "mrdestructoid" || words[i].ToLower() == "mrdestructoid\n" )
			{ 
				words[i] = "<sprite=\"mrdestructoid\" name=\"mrdestructoid\">";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}
			else if( words[i].ToLower() == "pjsalt" || words[i].ToLower() == "pjsalt\n" )
			{ 
				words[i] = "<sprite=\"pjsalt\" name=\"pjsalt\">";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == "pogchamp" || words[i].ToLower() == "pogchamp\n" )
			{ 
				words[i] = "<sprite=\"pogchamp\" name=\"pogchamp\">";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}

			else if( words[i].ToLower() == "biblethump" || words[i].ToLower() == "biblethump\n" )
			{ 
				words[i] = "<sprite=\"popo_emoji\" index=3>";
				if( i == words.Length-1 ){ words[i] = words[i] + "\n"; }
			}
 		}

 		text = "";
 		for( int i=0; i<words.Length; i++ )
 		{ 
 			text = text + words[i]; 
 			if( i < words.Length-1 ){ text = text + " "; }
 		}

 		return text;
 	}
}
