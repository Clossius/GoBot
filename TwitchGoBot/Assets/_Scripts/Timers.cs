using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;
using System.Timers;

public class TimersScript : MonoBehaviour {

	string fileName = "Timers";
	public Toggle useTimers;

	public class CommandTimers {
		public string name;
		public string message;
		public double timer;

		public CommandTimers (string n, string m, double t) {
			name = n;
			message = m;
			timer = t;
		}
	}

	List<CommandTimers> commandTimers = new List<CommandTimers>();
	List<Timer> timers = new List<Timer>();

	// Use this for initialization
	void Start () {
		LoadTimers();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Load ranks
	void LoadTimers() {
		Debug.Log("Loading Timers...");
		StreamReader fileObject = new StreamReader(Application.dataPath + "/Resources/" + fileName + ".txt");
		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				
				if (line != null && line[0] != '/') {string[] entries = line.Split(':');
					CommandTimers commandTimer = new CommandTimers(entries[0], entries[2], float.Parse(entries[1]));
					commandTimers.Add(commandTimer);
				}
			}
			while (line != null);
			fileObject.Close();
		}
	}

	void SaveTimers () {
		string timerText = "";

		for(int i=0;i<commandTimers.Count;i++){
			timerText = timerText + commandTimers[i].name + ":" + commandTimers[i].timer + ":" + commandTimers[i].message + "\n";
		}


		string filePath = Application.dataPath + "/Resources/" + fileName + ".txt";
		File.WriteAllText(filePath, timerText);
		Debug.Log("Saving Timers...");
	}

	void startTimers() {
		
	}


}
