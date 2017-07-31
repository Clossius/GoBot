using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Irc;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;

public class Store : MonoBehaviour {

	GameObject Scripts;
	string storeFileName = "Store";
	string transactionsFileName = "Transactions";
	public InputField itemNameInputField;
	public InputField costInputField;
	public Text storeItemsText;
	public Text recentTransactionsText;

	public class Item {
		public string item;
		public int cost;

		public Item (string i, int c) {
			item = i;
			cost = c;
		}
	}

	public class Transaction {
		public string username;
		public string item;
		public int cost;

		public Transaction (string user, string i, int c) {
			username = user;
			item = i;
			cost = c;
		}
	}

	List<Item> store = new List<Item>();
	List<Transaction> transactions = new List<Transaction>();

	// Use this for initialization
	void Start () {
		LoadSettings(storeFileName);
		LoadTransactions(transactionsFileName);
		Scripts = GameObject.Find("_Scripts");
		if(store.Count>0){DisplayItems();}
		if(transactions.Count>0){DisplayTransactions();}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Load settings
	void LoadSettings(string fileName) {
		Debug.Log("Loading Store Items...");
		StreamReader fileObject = this.GetComponent<FileManager>().LoadSettings( fileName );
		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				if (line != null && line[0] != '/') {
					string[] entries = line.Split(':');
					Item item = new Item(entries[0], Int32.Parse(entries[1]));
					store.Add(item);

				}
				}
				while (line != null);
			fileObject.Close();

		}
	}

	//Load transactions
	void LoadTransactions(string fileName) {
		Debug.Log("Loading Transactions...");
		StreamReader fileObject = this.GetComponent<FileManager>().LoadSettings( fileName );
		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				if (line != null && line[0] != '/') {
					string[] entries = line.Split(':');
					Transaction transaction = new Transaction(entries[0], entries[1], Int32.Parse(entries[2]));
					transactions.Add(transaction);

				}
				}
				while (line != null);
			fileObject.Close();

		}
	}

	void SaveTransactions() {
		string recentTransactions = "";
		for(int i=0;i<transactions.Count;i++) {
			recentTransactions = recentTransactions + transactions[i].username + ":" + transactions[i].item + ":" + transactions[i].cost + "\n";
		}
		this.GetComponent<FileManager>().SaveSettings( transactionsFileName, recentTransactions );
	}

	void SaveStoreItems() {
		string storeItems = "";
		for(int i=0;i<store.Count;i++) {
			storeItems = storeItems + store[i].item + ":" + store[i].cost + "\n";
		}
		this.GetComponent<FileManager>().SaveSettings( storeFileName, storeItems );
	}

	public void StoreCommand(){
		string pointsName = Scripts.GetComponent<Settings>().GetPointsName();
		string storeMessage = "Prices in " + pointsName + ". ";
		for(int i=0;i<store.Count;i++){
			storeMessage =  storeMessage + " ~ (" + store[i].item + ")" + store[i].cost;
		}
		Debug.Log("Store: " + storeMessage);
		Scripts.GetComponent<ChatManager>().MessageSend(storeMessage);
		storeMessage = "To buy, type !buy item";
		Scripts.GetComponent<ChatManager>().MessageSend(storeMessage);
	}

	public void BuyCommand(string username, string item) {
		int position = -1;
		string message = "";
		string message2 = "";
		for(int i=0;i<store.Count;i++){
			string storeItem = "";
			string itemChoice = "";
			for(int g=0;g<store[i].item.Length;g++){storeItem = storeItem + Char.ToLower(store[i].item[g]);}
			//for(int g=0;g<item.Length;g++){itemChoice = itemChoice + Char.ToLower(item[g]);}
			//storeItem = store[i].item.ToLower();
			itemChoice = item;
			Debug.Log(storeItem);
			Debug.Log(itemChoice);
			if(storeItem == itemChoice){position = i;}
		}
		Debug.Log("Position: " + position);
		bool hasFunds = false;
		if(position >= 0){hasFunds = CheckForFunds(username, store[position].cost);}
		if(position==-1){message = "I'm sorry, I did not find that item in the store.";}
		else if (hasFunds){
			message = username + " bought " + store[position].item + "!";
			Scripts.GetComponent<Sfx>().LoginButtonClicked();
			Scripts.GetComponent<Points>().TakePoints(username, store[position].cost);
			message2 = "<b><color=red>" + message + "</color></b>";
			AddTransaction(username, position);
			SaveTransactions();
			DisplayTransactions();
		}
		else{message = username + ", you do not have enough to buy " + store[position].item + ".";}
		if(message2 == ""){Scripts.GetComponent<ChatManager>().MessageSend(message);}
		else{
			Scripts.GetComponent<ChatManager>().MessageSend(message, message2);
			Scripts.GetComponent<Sfx>().LoginButtonClicked();
		}

	}

	bool CheckForFunds(string username, int cost) {
		bool hasEnoughPoints = false;
		int points = Scripts.GetComponent<Points>().CheckForPoints(username);
		if(points >= cost){hasEnoughPoints = true;}
		return hasEnoughPoints;
	}

	void DisplayItems() {
		string display = "";
		for(int i=0;i<store.Count;i++){
			display = display + store[i].item + "\n" + store[i].cost.ToString() + "\n";
		}
		storeItemsText.text = display;
	}

	void DisplayTransactions() {
		string display = "";
		if(transactions.Count>0){
			for(int i=0;i<transactions.Count;i++){
				display = display +"<b>" + transactions[i].username +"</b>\n" + transactions[i].item + "\n" + transactions[i].cost.ToString() + "\n";
			}
		}
		recentTransactionsText.text = display;
	}

	void AddTransaction(string username, int itemPosition) {
		if(transactions.Count<5){
			Transaction newTransaction = new Transaction(username, store[itemPosition].item, store[itemPosition].cost);
			transactions.Add(newTransaction);
		}
		else {
			List<Transaction> newTransactionList = new List<Transaction>();
			for(int i=1;i<transactions.Count;i++){
				newTransactionList.Add(transactions[i]);
			}
			Transaction newTransaction = new Transaction(username, store[itemPosition].item, store[itemPosition].cost);
			transactions  = newTransactionList;
			transactions.Add(newTransaction);
			DisplayTransactions();
		}
	}

	public void AddorEditItem () {
		int position = -1;
		bool notNull = false;
		string item = itemNameInputField.textComponent.text.ToLower();
		Debug.Log("Item to Add/Edit: " + item);
		if(itemNameInputField.textComponent.text != "" && costInputField.textComponent.text != ""){notNull = true;}
		for(int i=0;i<store.Count;i++){
			if(item == store[i].item){position = i;}
		}
		if(notNull && position >0) {
			store[position].cost = Int32.Parse(costInputField.textComponent.text);
		}
		else if(notNull){
			Item newItem = new Item(item, Int32.Parse(costInputField.textComponent.text));
			store.Add(newItem);
		}
		SaveStoreItems();
		DisplayItems();
	}

	public void RemoveItem() {
		bool notNull = false;
		int position = -1;
		string item = itemNameInputField.textComponent.text.ToLower();
		if(itemNameInputField.textComponent.text != ""){notNull = true;}
		for(int i=0;i<store.Count;i++){
			if(item == store[i].item){position = i;}
		}
		if(notNull && position >0) {
			store.RemoveAt(position);
		}
		SaveStoreItems();
		DisplayItems();
	}
}
