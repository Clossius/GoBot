using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;
using alias = System.Random;

public class Heist : MonoBehaviour {

	bool heistActive = false;
	public GameObject Scripts;
	float heistTimeLimit = 60.0f;

	public class Users {
		public string username;
		public int pointsBet;

		public Users (string u, int p) {
			username = u;
			pointsBet = p;
		}
	}

	List<Users> users = new List<Users>();

	public void StartHeist() {
		users = new List<Users>();
		heistActive = true;
		string message = "The banks are open! Join to help before the cops arrive! Ex: !heist 100";
		Scripts.GetComponent<ChatManager>().MessageSend(message);
		StartCoroutine("waitForHeist", heistTimeLimit);
	}

	public void JoinHeist(string username, int points) {
		int position = -1;
		string pointsName = Scripts.GetComponent<Settings>().GetPointsName();;
		bool hasEnough = Scripts.GetComponent<Points>().CanAfford(username, points);
		string message = "";
		for(int i=0;i<users.Count;i++) {
			if(users[i].username.ToLower() == username.ToLower()){position = i;}
		}
		if(position>=0) {message = username + ", you are already in the heist.";}
		else if(!hasEnough){message = username + " you do not have enough " + pointsName + ".";}
		else if(position<0 && hasEnough) {
			Users user = new Users(username, points);
			users.Add(user);
			message = username + " joined the heist.";

		}

		Scripts.GetComponent<ChatManager>().MessageSend(message);
	}

	IEnumerator waitForHeist(float seconds) {
		Debug.Log("Waiting for heist...");
		yield return new WaitForSeconds(seconds);
		if(heistActive){RewardHeist();}
		EndHeist();
	}

	void EndHeist() {
		heistActive = false;
	}

	public void CancelHeist() {
		EndHeist();
		string message = "Heist canceled.";
		StopCoroutine("waitForHeist");
		Scripts.GetComponent<ChatManager>().MessageSend(message);
	}

	void RewardHeist() {
		int min = 0;
		int max = 4;
		alias rnd = new alias();
		int option = rnd.Next(min, max);
		string message = "";
		List<string> winners = new List<string>();
		List<string> losers = new List<string>();
		Debug.Log("Reward Heist");
		Debug.Log("Option: " + option);

		if(option == 0){
			message = "Oh no! The cops arrived! No one was able to get anything...";
			for(int i=0;i<users.Count;i++){
				Scripts.GetComponent<Points>().TakePoints(users[i].username, users[i].pointsBet);
			}
		}
		else if(option == 1) {
			for(int i=0;i<users.Count;i++){
				int luck = rnd.Next(0, 10);
				if(luck < 7){Scripts.GetComponent<Points>().TakePoints(users[i].username, users[i].pointsBet);}
				else {
					Scripts.GetComponent<Points>().AddPoints(users[i].username, users[i].pointsBet*4);
					winners.Add(users[i].username);
				}
			}
			message = "Yikes! Cops showed up sooner than expected.";
			if(winners.Count > 0){
				message = message + " But ";
				for(int i=0;i<winners.Count;i++){
					message = message + " : " + winners[i];
				}
				message = message + " : got out with a bag of money!";
			}
			else {message = message + " No one was able to get anything...";}
		}

		else if(option == 2) {
			for(int i=0;i<users.Count;i++){
				int luck = rnd.Next(0, 10);
				if(luck < 5){
					Scripts.GetComponent<Points>().TakePoints(users[i].username, users[i].pointsBet);
					losers.Add(users[i].username);
				}
				else {
					Scripts.GetComponent<Points>().AddPoints(users[i].username, users[i].pointsBet*3);
					winners.Add(users[i].username);
				}
			}
			message = "It was a showdown at the bank today!";
			if (winners.Count > 0 && losers.Count > 0) {
				int max1 = winners.Count;
				int max2 = losers.Count;
				int choice = rnd.Next(0, 3);
				if(choice == 0){
					message = message + " " + winners[rnd.Next(min, max1)] + " sacrificed " + losers[rnd.Next(min, max2)] +
					" to make his escape.";
				}
				else if (choice == 1) {
					message = message + " " + winners[rnd.Next(min, max1)] + " blew the whole bank up to get out!!" +
						" Regrettably " + losers[rnd.Next(min, max2)] + " died in the explosion.";
				}

				else if (choice == 2) {
					message = message + " " + winners[rnd.Next(min, max1)] + " injured 3 cops, killed 5 ducks, and took " +
						losers[rnd.Next(min, max2)] + "'s leg as a trophy.";
				}
			}
			else if(winners.Count > 0){
				message = message + " But ";
				for(int i=0;i<winners.Count;i++){
					message = message + " : " + winners[i];
				}
				message = message + " : got out with money in their pockets.";
			}

			else{message = message + "But the cops lost two men but took out the whole heist group!";}
		}

		else if(option == 3) {
			for(int i=0;i<users.Count;i++){
				int luck = rnd.Next(0, 10);
				if(luck < 3){
					Scripts.GetComponent<Points>().TakePoints(users[i].username, users[i].pointsBet);
					losers.Add(users[i].username);
				}
				else {
					Scripts.GetComponent<Points>().AddPoints(users[i].username, users[i].pointsBet*2);
					winners.Add(users[i].username);
				}
			}
			message = "Cops could hardly handle the heist group today.";
			if (winners.Count > 0 && losers.Count > 0) {
				int max1 = winners.Count;
				int max2 = losers.Count;
				int choice = rnd.Next(0, 3);
				if(choice == 0){
					message = message + " " + winners[rnd.Next(min, max1)] + " stole " + losers[rnd.Next(min, max2)] +
					"'s pet chicken on the way out.";
				}
				else if (choice == 1) {
					message = message + " " + winners[rnd.Next(min, max1)] + " flew a helicopter out of there!" +
						" Regrettably " + losers[rnd.Next(min, max2)] + " didn't make the flight...";
				}

				else if (choice == 2) {
					message = message + " " + winners[rnd.Next(min, max1)] + " shot an RPG at the cops!! However, " +
						losers[rnd.Next(min, max2)] + " accidently jumped in front of it.";
				}
			}
			else if(winners.Count > 0){
				message = message + " But ";
				for(int i=0;i<winners.Count;i++){
					message = message + " : " + winners[i];
				}
				message = message + " : outsmarted the cops and stole their cars on the way out.";
			}

			else{message = message + "But the cops launched a suprise attack and got everone!";}
		}

		Scripts.GetComponent<ChatManager>().MessageSend(message);
	}
}
