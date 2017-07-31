using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;

public class Ranks : MonoBehaviour {

	string ranksFileName = "Ranks";
	GameObject Scripts;
	public Toggle useRanks;
	public InputField rankNameInputField;
	public InputField costInputField;
	public InputField lvlInputField;
	public Text textBox01;
	public Text textBox02;
	public Text textBox03;
	public Text textBox04;
	int textBoxLength = 5;


	public class Rank {
		public string name;
		public int permission;
		public int cost;
		public string canBuy;

		public Rank(string n, int p, int c, string cb) {
			name = n;
			permission = p;
			cost = c;
			canBuy = cb;
		}
	}

	List<Rank> ranks = new List<Rank>();

	// Use this for initialization
	void Start () {
		InitializeRanks();
		LoadRanks();
		Scripts = GameObject.Find("_Scripts");
		DisplayRanks();
	}

	void InitializeRanks() {
		Rank owner = new Rank("Owner", 0, 0, "false");
		Rank admin = new Rank("Admin", 1, 0, "false");
		Rank gm = new Rank("GrandMaster", 2, 10000, "true");
		Rank m = new Rank("Master", 3, 5000, "true");
		Rank r = new Rank("Regular", 4, 2500, "true");
		Rank v = new Rank("Viewer", 5, 1000, "true");
		Rank ng = new Rank("NewGuy", 6, 0, "false");
		ranks.Add(owner);
		ranks.Add(admin);
		ranks.Add(gm);
		ranks.Add(m);
		ranks.Add(r);
		ranks.Add(v);
		ranks.Add(ng);

	}

	//Load ranks
	void LoadRanks() {
		Debug.Log("Loading Ranks...");

		string filePath = Application.dataPath + "/Resources/" + ranksFileName + ".txt";
		bool exist = File.Exists( filePath  );

		if(!exist)
		{ 
			Debug.Log( "Creating Default Ranks..." );
			SaveRanks();
			DisplayRanks();
		}

		ranks.Clear();
		ranks = new List<Rank>();
		StreamReader fileObject = this.GetComponent<FileManager>().LoadSettings( ranksFileName );
		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				
				if (line != null && line[0] != '/') {string[] entries = line.Split(':');
					if(entries[0] == "0"){
						Rank rank = new Rank(entries[1], Int32.Parse(entries[0]), 0, "false");
						ranks.Add(rank);
					}
					else if(entries[0] == "1"){
						Rank rank = new Rank(entries[1], Int32.Parse(entries[0]), 0, entries[3]);
						ranks.Add(rank);
					}
					else if(entries[3] == "default") {
						Rank rank = new Rank(entries[1], Int32.Parse(entries[0]), 0, "false");
						ranks.Add(rank);
					}
					else {
						Rank rank = new Rank(entries[1], Int32.Parse(entries[0]), Int32.Parse(entries[2]), entries[3]);
						ranks.Add(rank);
					}
				}
			}
			while (line != null);
			fileObject.Close();

		}
	}

	public void SaveRanks () {
		string ranksText = "";

		for(int i=0;i<ranks.Count;i++){
			ranksText = ranksText + ranks[i].permission.ToString() + ":" + ranks[i].name + ":" + ranks[i].cost.ToString() + ":" + ranks[i].canBuy + "\n";
		}


		this.GetComponent<FileManager>().SaveSettings( ranksFileName, ranksText );
		DisplayRanks();

	}

	void AddRank(string name, int position, int cost, string canBuy) {
		Rank rank = new Rank(name, position, cost, canBuy);
		ranks.Insert(position, rank);
		SaveRanks();
		DisplayRanks();
	}

	void RemoveRankAt(int position) {
		ranks.RemoveAt(position);
		SaveRanks();
		DisplayRanks();
	}

	public string GetDefaultRank () {
		int position = -1;
		for(int i=0;i<ranks.Count;i++){
			if(ranks[i].canBuy == "dafault"){position = i;}
		}
		if(position<0){position = ranks.Count-1;}
		return ranks[position].name;
	}

	public int GetPermissionLevel(string rank) 
	{
		int position = -1;

		for(int i=0;i<ranks.Count;i++)
		{
			if(ranks[i].name == rank){position = i;}
		}

		if(position<0){position = ranks.Count-1;}

		Debug.Log("Permission lvl: " + position);

		return position;
	}

	public string CheckOrConvertRank(string rank) {
		bool rankExist = false;
		string newRank = "";
		Debug.Log("Rank to Convert: " + rank);
		if(rank == "Owner" || rank == "owner"){rank = ranks[0].name;}
		else if(rank == "Admin" || rank == "admin"){rank = ranks[1].name;}

		for(int i=0;i<ranks.Count;i++) {
			if(rank == ranks[i].name){rankExist = true;}
		}
		if(rankExist){newRank = rank;}
		else {newRank = ranks[ranks.Count-1].name;}
		return newRank;

	}

	public void CheckRankUp(string username) {
		string rank = Scripts.GetComponent<Points>().GetRank(username);
		Debug.Log("Rank Received: " + rank);
		int position = -1;
		string pointsName = Scripts.GetComponent<Settings>().GetPointsName();
		for(int i=0;i<ranks.Count;i++){if(ranks[i].name.ToLower() == rank.ToLower()){position = i;}}
		if(position == -1){position = ranks.Count-1;}
		string message = "";
		bool canRankUp = false;
		if(position > 0){
			if(ranks[position-1].canBuy == "true"){canRankUp = true;}
		}
		if(!canRankUp){message = username + ", you cannot rank up any further.";}
		else if(canRankUp) {
			message = 	username + ", the next rank is '" + ranks[position-1].name + "'. It cost " +
						ranks[position-1].cost + " " + pointsName + ". To buy type !buyrank";
		}
		Scripts.GetComponent<ChatManager>().MessageSend(message);
	}

	public void BuyRankUp(string username) {
		string rank = Scripts.GetComponent<Points>().GetRank(username);
		int position = -1;
		string message = "";
		Debug.Log(username + " is trying to buy a rank.");
		string pointsName = Scripts.GetComponent<Settings>().GetPointsName();

		for(int i=0;i<ranks.Count;i++){if(ranks[i].name.ToLower() == rank.ToLower()){position = i;}}
		if(position == -1){position = ranks.Count-1;}
		bool canAfford = false;
		if(position > 0){canAfford = Scripts.GetComponent<Points>().CanAfford(username, ranks[position-1].cost);}

		if(position>0) {
			if(ranks[position-1].canBuy == "false"){
				message = username + ", you cannot rank up any further.";
			}
			else if(!canAfford){
				message = username + ", you do not have enough " + pointsName + " for this rank.";
			}
			else if(canAfford && ranks[position-1].canBuy == "true") {
				Scripts.GetComponent<Points>().ChangeRank(username, ranks[position-1].name);
				Scripts.GetComponent<Points>().TakePoints(username, ranks[position-1].cost);
		}}

		else{message = username + ", you cannot rank up any further.";}

		GameObject.Find("_Scripts").GetComponent<ChatManager>().MessageSend(message);

	}

	public string GetAdminRank() {
		string rank = ranks[1].name;
		return rank;
	}

	public void AddRankButtonPressed() {
		bool rankExist = false;

		if(rankNameInputField.textComponent.text != "" && costInputField.textComponent.text != "") {
			for(int i=0;i<ranks.Count;i++) {
				if(ranks[i].name == rankNameInputField.textComponent.text){
					rankExist = true;
					ranks[i].cost = Int32.Parse(costInputField.textComponent.text);
					rankNameInputField.textComponent.text = "";
					costInputField.textComponent.text = "";
					DisplayRanks();
					SaveRanks();
				}
			}
		}

		if(rankNameInputField.textComponent.text != "" && costInputField.textComponent.text != "" && lvlInputField.textComponent.text != "" && !rankExist){
			int cost = 0;
			string canBuy = "true";
			if(costInputField.textComponent.text == "n/a"){
				canBuy = "false";
			}
			else {cost = Int32.Parse(costInputField.textComponent.text);}
			AddRank(rankNameInputField.textComponent.text, Int32.Parse(lvlInputField.textComponent.text), cost, canBuy);
			rankNameInputField.textComponent.text = "";
			costInputField.textComponent.text = "";
			lvlInputField.textComponent.text = "";
			DisplayRanks();
			SaveRanks();
		}
	}

	public void RemoveButtonPressed() {
		if(rankNameInputField.textComponent.text != "") {
			for(int i=0;i<ranks.Count;i++) {
				if(ranks[i].name == rankNameInputField.textComponent.text){
					RemoveRankAt(i);
					rankNameInputField.textComponent.text = "";
					SaveRanks();
					DisplayRanks();				
				}
			}
		}
	}

	public void DisplayRanks() {
		string text01 = "";
		string text02 = "";
		string text03 = "";
		string text04 = "";

		for(int i=0;i<ranks.Count;i++){
			if(i<=textBoxLength){
				string cost = "";
				if(ranks[i].cost == 0){cost = "n/a";}
				else if(ranks[i].cost > 0){cost = ranks[i].cost.ToString();}
				text01 = text01 + "<b>" + ranks[i].name + "</b>" + "\nCost: " + cost + "\n";
			}
			else if(i<=textBoxLength*2){
				string cost = "";
				if(ranks[i].cost == 0){cost = "n/a";}
				else if(ranks[i].cost > 0){cost = ranks[i].cost.ToString();}
				text02 = text02 + "<b>" + ranks[i].name + "</b>" + "\nCost: " + cost + "\n";
			}
			else if(i<=textBoxLength*3){
				string cost = "";
				if(ranks[i].cost == 0){cost = "n/a";}
				else if(ranks[i].cost > 0){cost = ranks[i].cost.ToString();}
				text03 = text03 + "<b>" + ranks[i].name + "</b>" + "\nCost: " + cost + "\n";
			}
			else if(i<=textBoxLength*4){
				string cost = "";
				if(ranks[i].cost == 0){cost = "n/a";}
				else if(ranks[i].cost > 0){cost = ranks[i].cost.ToString();}
				text04 = text04 + "<b>" + ranks[i].name + "</b>" + "\nCost: " + cost + "\n";
			}
		}

		textBox01.text = text01;
		textBox02.text = text02;
		textBox03.text = text03;
		textBox04.text = text04;

	}

	public int GetRankListSize ()
	{
		return ranks.Count;
	}
}
