using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;
using alias = System.Random;

public class QuoteScript : MonoBehaviour {

	string fileName = "Quotes";

	List<string> quotes = new List<string>();

	// Use this for initialization
	void Start () {
		LoadQuotes();
	}


	//Load quotes
	void LoadQuotes() {
		StreamReader fileObject = this.GetComponent<FileManager>().LoadSettings( fileName );
		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				
				if (line != null && line[0] != '/') {
					quotes.Add(line);
				}
			}
			while (line != null);
			fileObject.Close();

		}
	}

	void SaveQuotes () {
		string quotesText = "";

		for(int i=0;i<quotes.Count;i++){
			quotesText = quotesText + quotes[i] + "\n";
		}


		this.GetComponent<FileManager>().SaveSettings( fileName, quotesText );

	}

	public void AddQuote(string[] message) {
		string quote = "";
		for(int i=1;i<message.Length;i++) {
			quote = quote + message[i];
			if(i != message.Length-1){
				quote = quote + " ";
			}
		}
		quotes.Add(quote);
		SaveQuotes();
		GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend("Quote Saved.");
	}

	public void GetQuote () {
		alias rnd = new alias();
		int min = 0;
		int max = quotes.Count;
		string message = "Quote: '" + quotes[rnd.Next(min, max)] + "'";
		GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(message);
	}
}
