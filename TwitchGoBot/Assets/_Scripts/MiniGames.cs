using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Irc;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using alias = System.Random;



public class MiniGames : MonoBehaviour {

	public BotScript botScript;
	string EightBallFile = "eightBall";

	List<string> eightBallList = new List<string>();

	// Use this for initialization
	void Start () {
		TwitchIrc.Instance.OnUserLeft += OnUserLeft;
        TwitchIrc.Instance.OnUserJoined += OnUserJoined;
		TwitchIrc.Instance.OnChannelMessage += OnChannelMessage;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void EightBall() {
		loadFile(EightBallFile);

		string response = "";
		alias rnd = new alias();
    	int max = eightBallList.Count-1;
		int arrayCoord = rnd.Next(0, max);
		response = eightBallList[arrayCoord];

		GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(response);
	}

	void loadFile(string fileName) {
		StreamReader fileObject = new StreamReader(Application.dataPath + "/Resources/" + fileName + ".txt");
		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				if (line != null) {
					eightBallList.Add(line);
				}
				}
				while (line != null);
			fileObject.Close();

		}
	}

	//Get the name of the user who joined to channel 
    void OnUserJoined(UserJoinedEventArgs userJoinedArgs)
    {
        //ChatText.text += "<b>" + "USER JOINED" + ":</b> " + userJoinedArgs.User + "\n";
        //Debug.Log("USER JOINED: " + userJoinedArgs.User);
    }

	//Get the name of the user who left the channel.
    void OnUserLeft(UserLeftEventArgs userLeftArgs)
    {
        //ChatText.text += "<b>" + "USER JOINED" + ":</b> " + userLeftArgs.User + "\n";
        //Debug.Log("USER JOINED: " + userLeftArgs.User);
    }

	//Receive username that has been left from channel and check for command.
	void OnChannelMessage(ChannelMessageEventArgs channelMessageArgs)
    {	
    	string tempText = "";
		//tempText = "<b>" + channelMessageArgs.From + ":</b> " + channelMessageArgs.Message + "\n";
        //Debug.Log("MESSAGE: " + channelMessageArgs.From + ": " + channelMessageArgs.Message);
        if (channelMessageArgs.Message[0] == '!') {
        	//List<string> tempStringList = SplitText(channelMessageArgs.Message);
        	//CheckForCommand(tempStringList, channelMessageArgs.From);
        }
    }

   /* List<string> SplitText(string text) {
    	List<string> tempStringList = text.Split(' ');

    	return tempStringList;
    }*/

    void CheckForCommand(List<string> tempStringList, string name) {
    	/*if (tempStringList[0] == "!8ball" || tempStringList == "!8ball") {
    		EightBall();
    	}*/
    }
}
