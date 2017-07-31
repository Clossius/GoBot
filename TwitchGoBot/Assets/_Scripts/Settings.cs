using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;

public class Settings : MonoBehaviour {

	string settingsFile = "Settings";
	public InputField botChannelInputField;
	public Text botChannelText;
	string botName = "n/a";
	public InputField oauthInputField;
	public Text oauthText;
	string oauth = "n/a";
	public InputField channelInputField;
	public Text channelText;
	string channelName = " n/a";
	public InputField pointsNameInputField;
	public Text pointsNameText;
	string pointsName = "Points";
	public InputField pointsToGainInputField;
	public Text pointsToGainText;
	public Toggle usePoints;
	int pointsToGive;
	public Toggle playSound;
	public InputField costToBuyExtraMoveVoteInputField;
	public int costToBuyExtraMoveVote = 100;
	public Toggle canBuyExtraMoveVote;
	public Toggle rewardVotingToggle;
	public int rewardVotingAmount;
	public InputField rewardVotingInputField;
	public Toggle useRanksToggle;
	public Toggle use8Ball;
	public Toggle useHeist;
	public Toggle useBetting;
	public Toggle useQuote;
	public Toggle greetToggle;
	public Toggle customGreetToggle;
	public Toggle useSubsToggle;
	public Toggle bonusPointsToggle;
	public Toggle subsBonusToggle;

	bool gameLoaded = false;


	// Use this for initialization
	void Start () {
		pointsToGainText.text = "1";
		LoadSettings(settingsFile);
	}
	
	// Update is called once per frame
	void Update () {
		botChannelText.text = "Bot Channel: " + botName;
		if(oauth != null && oauth != "" && oauth != "n/a"){
			if(oauth.ElementAt(5) == ':') {oauthText.text = "Token: " + oauth[0] + oauth[1] +oauth[2] +oauth[3] +oauth[4] +oauth[5] +oauth[6] +oauth[7] +oauth[8] +oauth[9] +oauth[10] +oauth[11] +oauth[12];}
		}
		channelText.text = "Channel: ";
		if(channelName !=null){for(int i=1;i<channelName.Length;i++) {
			if(i==1){channelText.text = channelText.text + Char.ToUpper(channelName[i]);}
			else {channelText.text = channelText.text + channelName[i];}
		}}
		pointsNameText.text = "Points Name: " + pointsName;
		pointsToGainText.text = "Points Given : " + pointsToGive.ToString();
	}



	//Load settings
	void LoadSettings(string fileName) 
	{
		StreamReader fileObject = GameObject.Find("_Scripts").GetComponent<FileManager>().LoadSettings( fileName );

		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				if (line != null && line[0] != '/') {
					string[] entries = line.Split(':');
					Debug.Log(entries[0] + " " + entries[1]);
					if (entries[0] == "BotUserName") {botName = entries[1];}
					else if (entries[0] == "oauthToken") {
							if(entries[1] != "n/a"){oauth = entries[1] + ":" + entries[2];}
						}
					else if (entries[0] == "ChannelToJoin") {channelName = entries[1];}
					else if (entries[0] == "pointsName") {pointsName = entries[1];}
					else if (entries[0] == "rewardvotingamount") {
						rewardVotingAmount = Int32.Parse(entries[1]);
						GameObject.Find("_Scripts").GetComponent<TwitchPlays>().ChangeVotingReward(rewardVotingAmount);
					}
					else if (entries[0] == "rewardvoting") {
						if(entries[1] == "true"){rewardVotingToggle.isOn = true;}
						else if(entries[1] == "false"){rewardVotingToggle.isOn = false;}
					}
					else if (entries[0] == "usePoints") {
						if(entries[1] == "true"){usePoints.isOn = true;}
						else if(entries[1] == "false"){usePoints.isOn = false;}
					}
					else if (entries[0] == "useRanks") {
						if(entries[1] == "true"){useRanksToggle.isOn = true;}
						else if(entries[1] == "false"){useRanksToggle.isOn = false;}
					}
					else if (entries[0] == "playSound") {
						if(entries[1] == "true"){playSound.isOn = true;}
						else if(entries[1] == "false"){playSound.isOn = false;}
					}
					else if (entries[0] == "amountOfPointsToGive") {
						pointsToGive = Int32.Parse(entries[1]);
						GameObject.Find("_Scripts").GetComponent<Points>().PointsToBeAdded(pointsToGive);
					}
					else if (entries[0] == "canBuyExtraMove") {
						if(entries[1] == "true"){canBuyExtraMoveVote.isOn = true;}
						else if(entries[1] == "false"){canBuyExtraMoveVote.isOn = false;}
						//GameObject.Find("_Scripts").GetComponent<Points>().UsePoints(usePoints.isOn);
					}
					else if (entries[0] == "costToBuyExtraMoveVoteText") {
						costToBuyExtraMoveVote = Int32.Parse(entries[1]);
						GameObject.Find("_Scripts").GetComponent<TwitchPlays>().SetCostToBuyExtraMoveVote(costToBuyExtraMoveVote);
					}
					else if (entries[0] == "use8ball") {
						if(entries[1] == "true"){use8Ball.isOn = true;}
						else if(entries[1] == "false"){use8Ball.isOn = false;}
					}
					else if (entries[0] == "useheist") {
						if(entries[1] == "true"){useHeist.isOn = true;}
						else if(entries[1] == "false"){useHeist.isOn = false;}
					}
					else if (entries[0] == "usebetting") {
						if(entries[1] == "true"){useBetting.isOn = true;}
						else if(entries[1] == "false"){useBetting.isOn = false;}
					}
					else if (entries[0] == "usequote") {
						if(entries[1] == "true"){useQuote.isOn = true;}
						else if(entries[1] == "false"){useQuote.isOn = false;}
					}
					else if (entries[0] == "usegreet") {
						if(entries[1] == "true"){greetToggle.isOn = true;}
						else if(entries[1] == "false"){greetToggle.isOn = false;}
					}
					else if (entries[0] == "usecustomgreet") {
						if(entries[1] == "true"){customGreetToggle.isOn = true;}
						else if(entries[1] == "false"){customGreetToggle.isOn = false;}
					}
					else if (entries[0] == "usesubs") {
						if(entries[1] == "true"){useSubsToggle.isOn = true;}
						else if(entries[1] == "false"){useSubsToggle.isOn = false;}
					}
					else if (entries[0] == "usebonuspoints") {
						if(entries[1] == "true"){bonusPointsToggle.isOn = true;}
						else if(entries[1] == "false"){bonusPointsToggle.isOn = false;}
					}
					else if (entries[0] == "usesubsbonus") {
						if(entries[1] == "true"){subsBonusToggle.isOn = true;}
						else if(entries[1] == "false"){subsBonusToggle.isOn = false;}
					}
				}
				}
				while (line != null);
			fileObject.Close();
			gameLoaded = true;

		}
		GameObject.Find("_Scripts").GetComponent<Points>().GetPointsName(pointsName);
	}

	public void SaveSettings () {
		string settingsText = "";
		if(gameLoaded){
			settingsText = settingsText + "BotUserName:" + botName + "\n";
			settingsText = settingsText + "oauthToken:" + oauth + "\n";
			settingsText = settingsText + "ChannelToJoin:" + channelName + "\n";
			settingsText = settingsText + "pointsName:" + pointsName + "\n";
			if(usePoints.isOn) {settingsText = settingsText + "usePoints:" + "true" + "\n";}
			else {settingsText = settingsText + "usePoints:" + "false" + "\n";}
			if(playSound.isOn) {settingsText = settingsText + "playSound:" + "true" + "\n";}
			else {settingsText = settingsText + "playSound:" + "false" + "\n";}
			settingsText = settingsText + "amountOfPointsToGive:" + pointsToGive.ToString() + "\n";
			if(canBuyExtraMoveVote.isOn) {settingsText = settingsText + "canBuyExtraMove:" + "true" + "\n";}
			else {settingsText = settingsText + "canBuyExtraMove:" + "false" + "\n";}
			settingsText = settingsText + "costToBuyExtraMoveVoteText:" + costToBuyExtraMoveVote.ToString() + "\n";
			if(rewardVotingToggle.isOn) {settingsText = settingsText + "rewardvoting:" + "true" + "\n";}
			else {settingsText = settingsText + "rewardvoting:" + "false" + "\n";}
			settingsText = settingsText + "rewardvotingamount:" + rewardVotingAmount.ToString() + "\n";
			if(useRanksToggle.isOn) {settingsText = settingsText + "useRanks:" + "true" + "\n";}
			else {settingsText = settingsText + "useRanks:" + "false" + "\n";}
			if(use8Ball.isOn) {settingsText = settingsText + "use8ball:" + "true" + "\n";}
			else {settingsText = settingsText + "use8ball:" + "false" + "\n";}
			if(useHeist.isOn) {settingsText = settingsText + "useheist:" + "true" + "\n";}
			else {settingsText = settingsText + "useheist:" + "false" + "\n";}
			if(useBetting.isOn) {settingsText = settingsText + "usebetting:" + "true" + "\n";}
			else {settingsText = settingsText + "usebetting:" + "false" + "\n";}
			if(useQuote.isOn) {settingsText = settingsText + "usequote:" + "true" + "\n";}
			else {settingsText = settingsText + "usequote:" + "false" + "\n";}
			if(greetToggle.isOn) {settingsText = settingsText + "usegreet:" + "true" + "\n";}
			else {settingsText = settingsText + "usegreet:" + "false" + "\n";}
			if(customGreetToggle.isOn) {settingsText = settingsText + "usecustomgreet:" + "true" + "\n";}
			else {settingsText = settingsText + "usecustomgreet:" + "false" + "\n";}
			if(useSubsToggle.isOn) {settingsText = settingsText + "usesubs:" + "true" + "\n";}
			else {settingsText = settingsText + "usesubs:" + "false" + "\n";}
			if(bonusPointsToggle.isOn) {settingsText = settingsText + "usebonuspoints:" + "true" + "\n";}
			else {settingsText = settingsText + "usebonuspoints:" + "false" + "\n";}
			if(subsBonusToggle.isOn) {settingsText = settingsText + "usesubsbonus:" + "true" + "\n";}
			else {settingsText = settingsText + "usesubsbonus:" + "false" + "\n";}

			GameObject.Find("_Scripts").GetComponent<FileManager>().SaveSettings(settingsFile, settingsText);
		}
	}

	public void GetOauthButtonPressed () {
		Application.OpenURL("https://twitchapps.com/tmi/");
	}

	void OnGUI() {
		if(botChannelInputField.isFocused && botChannelInputField.text != "" && Input.GetKey(KeyCode.Return)) {
         	BotUserNameChange();
			botChannelInputField.text = "";
    	 }

		if(oauthInputField.isFocused && oauthInputField.text != "" && Input.GetKey(KeyCode.Return)) {
			OauthTokenChange();
			oauthInputField.text = "";
    	 }

		if(channelInputField.isFocused && channelInputField.text != "" && Input.GetKey(KeyCode.Return)) {
			ChannelToJoinChanged();
			channelInputField.text = "";
    	 }

		if(pointsNameInputField.isFocused && pointsNameInputField.text != "" && Input.GetKey(KeyCode.Return)) {
			PointsNameChange();
			pointsNameInputField.text = "";
    	 }

		if(pointsToGainInputField.isFocused && pointsToGainInputField.text != "" && Input.GetKey(KeyCode.Return)) {
			PointsToGiveChange();
			pointsToGainInputField.text = "";
    	 }

		if(costToBuyExtraMoveVoteInputField.isFocused && costToBuyExtraMoveVoteInputField.text != "" && Input.GetKey(KeyCode.Return)) {
			costToBuyExtraMoveVoteChanged();
			GameObject.Find("_Scripts").GetComponent<TwitchPlays>().SetCostToBuyExtraMoveVote(costToBuyExtraMoveVote);
			costToBuyExtraMoveVoteInputField.text = "";
    	 }

		if(rewardVotingInputField.isFocused && rewardVotingInputField.text != "" && Input.GetKey(KeyCode.Return)) {
			RewardVotingAmountChanged();
			rewardVotingInputField.text = "";
    	 }
 	}

 	public void RewardVotingAmountChanged () {
 		GameObject.Find("_Scripts").GetComponent<TwitchPlays>().ChangeVotingReward(rewardVotingAmount);
 		SaveSettings();
 	}

	public void BotUserNameChange() {
		botName = botChannelInputField.textComponent.text;
		SaveSettings();
	}

	public string GetBotUserName() {
		return botName;
	}


	public void OauthTokenChange() {
		oauth = oauthInputField.textComponent.text;
		SaveSettings();
	}

	public string GetOauthToken() {
		return oauth;
	}


	public void ChannelToJoinChanged() {
		string channel = channelInputField.textComponent.text;
		channelName = "#";
		for(int i=0;i<channel.Length;i++){channelName = channelName + Char.ToLower(channel[i]);}
		SaveSettings();
	}

	public string GetChannelToJoinName() {
		return channelName;
	}


	public void PointsNameChange() {
		pointsName = pointsNameInputField.textComponent.text;
		SaveSettings();
		GameObject.Find("_Scripts").GetComponent<Points>().GetPointsName(pointsName);
	}

	public string GetPointsName() {
		return pointsName;
	}


	public void PointsToGiveChange() {
		pointsToGive = Int32.Parse(pointsToGainInputField.textComponent.text);
		GameObject.Find("_Scripts").GetComponent<Points>().PointsToBeAdded(pointsToGive);
		SaveSettings();
	}

	public int GetPointsToGive() {
		return pointsToGive;
		SaveSettings();
	}

	public void OnUsePointsToggle() {
		SaveSettings();
	}

	public void OnToggle() {
		SaveSettings();
	}

	void costToBuyExtraMoveVoteChanged() {
		costToBuyExtraMoveVote = Int32.Parse(costToBuyExtraMoveVoteInputField.textComponent.text);
		GameObject.Find("_Scripts").GetComponent<TwitchPlays>().SetCostToBuyExtraMoveVote(costToBuyExtraMoveVote);
		SaveSettings();
	}

	public void OnCostToButExtraMoveToggle() {
		SaveSettings();
	}

}
