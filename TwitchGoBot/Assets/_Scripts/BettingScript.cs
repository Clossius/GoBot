using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;
using alias = System.Random;

public class BettingScript : MonoBehaviour {

	bool betting = false;
	public GameObject Scripts;

	public class Bets {
		public string username;
		public int betAmount;
		public int betOn;

		public Bets (string user, int betA, int betO) {
			username = user;
			betAmount = betA;
			betOn = betO;
		}
	}


	List<string> options = new List<string>();
	List<Bets> bets = new List<Bets>();

	public void StartBetting(string command) {
		string[] entries = command.Split(':');
		string message = "";
		options = new List<string>();
		bets = new List<Bets>();
		for(int i=1;i<entries.Length;i++){
			if(entries[i] != ""){options.Add(entries[i].ToLower());}
		}

		message = "Betting is now open! Options are ";

		for(int i=0;i<options.Count;i++) {
			message = message + " : (" + i + ")" + options[i];
		}
		betting = true;
		Scripts.GetComponent<ChatManager>().MessageSend(message);
	}

	public void StopBetting() {
		betting = false;
		string message = "Betting has finished.";
		Scripts.GetComponent<ChatManager>().MessageSend(message);
	}

	public void ChooseWinner(int winner) {
		int position = -1;
		float payout = 1.0f;
		int winners = 0;
		StopBetting();
		for(int i=0;i<bets.Count;i++) {
			if(i == winner){position = i;}
		}
		if(position >= 0){
			if(bets.Count > 0){
				for(int i=0;i<bets.Count;i++) {
					if(bets[i].betOn == position) {
						winners++;
					}
				}

				payout = 1 - (winners/bets.Count);
				Debug.Log("Payout%: " + payout);

				for(int i=0;i<bets.Count;i++){
					if(bets[i].betOn == position) {
						int points = (int) (bets[i].betAmount + (bets[i].betAmount * payout));
						Scripts.GetComponent<Points>().AddPoints(bets[i].username, points);
					}
					else {
						Scripts.GetComponent<Points>().TakePoints(bets[i].username, bets[i].betAmount);
					}
				}
			}
		}
	}

	public void BetOn(string username, int option, int amountToBet) {
		int position = -1;
		string message = "";
		bool hasNotBet = true;
		for(int i=0;i<bets.Count;i++){
			if(bets[i].username == username){
				message = username + ", you have already made a bet."; hasNotBet = false;
			}
		}
		for(int i=0;i<options.Count;i++){if(option == i){position = i;}}

		if(position<0 && hasNotBet){message = option + " is not an option to bet on.";}
		else if (hasNotBet) {
			Bets bet = new Bets(username, amountToBet, position);
			bets.Add(bet);
			message = bet.username + " bet " + bet.betAmount + " on " + options[bet.betOn] + "!";
		}
		Scripts.GetComponent<ChatManager>().MessageSend(message);
	}



}
