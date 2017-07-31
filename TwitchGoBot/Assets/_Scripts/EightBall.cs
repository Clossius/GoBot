using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;
using alias = System.Random;

public class EightBall : MonoBehaviour {

	string fileName = "EightBall";
	public GameObject Scripts;

	List<string> eightBall = new List<string>();

	// Use this for initialization
	void Start () {
		LoadEightBall();
	}

	//Load eight ball
	void LoadEightBall() {
		Debug.Log("Loading Eight Ball...");
		StreamReader fileObject = this.GetComponent<FileManager>().LoadSettings( fileName );
		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				
				if (line != null && line[0] != '/') {
					eightBall.Add(line);
				}
			}
			while (line != null);
			fileObject.Close();

		}
	}

	public void AskEightBall() {
		string message = "";
		alias rnd = new alias();
		int min = 0;
		int max = eightBall.Count;
		message = eightBall[rnd.Next(min, max)];
		Scripts.GetComponent<ChatManager>().MessageSend(message);
	}

}
