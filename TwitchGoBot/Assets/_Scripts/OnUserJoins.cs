using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Irc;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
 

public class OnUserJoins : MonoBehaviour {

	public BotScript botScript;
	public GameObject scripts;
	string fileA = "CustomIntroductions";
	public Toggle greetToggle;
	public Toggle customGreetToggle;

	public class CustomIntroductions {
		public string userName;
		public string customIntro;

		public CustomIntroductions (string name, string introduction) {
			userName = name;
			customIntro = introduction;
		}
	}

	List<CustomIntroductions> customUserIntroList = new List<CustomIntroductions>();

	// Use this for initialization
	void Start () 
	{
		// Load a text file.
		loadFile(fileA, customUserIntroList);
		scripts = GameObject.Find("_Scripts");
	}

	public void Login() 
	{
        TwitchIrc.Instance.OnUserJoined += OnUserJoined;
	}

	void loadFile(string fileName, List<CustomIntroductions> custom) 
	{
		StreamReader fileObject = this.GetComponent<FileManager>().LoadSettings( fileName );

		using (fileObject) 
		{
			string line;

			do 
			{
				line = fileObject.ReadLine();

				if (line != null) 
				{
					string[] entries = line.Split(':');
					if (entries.Length > 0) 
					{
						CustomIntroductions temp = new CustomIntroductions(entries[0], entries[1]);
						Debug.Log("Custom Intro Loaded: " + entries[0]);
						custom.Add(temp);
					}
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
        Debug.Log("USER JOINED: " + userJoinedArgs.User);

        bool condition = false;
        if (customUserIntroList.Count > 0) {
        	int length = customUserIntroList.Count;
			Debug.Log("UserIntroLength: " + length);
        	for (int i=0;i<length;i++) {
				if (userJoinedArgs.User == customUserIntroList[i].userName && customGreetToggle.isOn ) {
					scripts.GetComponent<ChatManager>().MessageSend( userJoinedArgs.User + " " + customUserIntroList[i].customIntro );
					condition = true;
				}
        	}
        }

		if ( !condition && greetToggle.isOn ) {scripts.GetComponent<ChatManager>().MessageSend("/w " + userJoinedArgs.User + " Welcome! " + userJoinedArgs.User);}

		GameObject.Find("_Scripts").GetComponent<Points>().UserJoined(userJoinedArgs.User);
    }

}
