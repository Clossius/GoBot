using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;

public class Subscriptions : MonoBehaviour {

	public class Subsciber 
	{
		public string name;
		public int donation;

		public Subsciber ( string s, int d ) 
		{
			name = s;
			donation = d;
		}
	}

	string fileName = "Subscribers";

	List<Subsciber> subs = new List<Subsciber>();

	// Use this for initialization
	void Start () 
	{
		LoadSubs ();
	}
	
	void LoadSubs ()
	{
		Debug.Log("Loading Subscibers...");
		StreamReader fileObject = new StreamReader(Application.dataPath + "/Resources/" + fileName + ".txt");
		using (fileObject) {
		string line;
			do {line = fileObject.ReadLine();
				
				if (line != null && line[0] != '/') {string[] entries = line.Split(':');
					Subsciber sub = new Subsciber ( entries[0], Int32.Parse( entries[1] ) );
					subs.Add ( sub );
				}
			}
			while (line != null);
			fileObject.Close();

		}
	}

	void SaveSubs ()
	{
		if ( subs.Count > 0 )
		{
			string subsText = "";
			for( int i=0; i<subs.Count; i++ )
			{
				subsText = subsText + subs[i].name + ":" + subs[i].donation + "\n";
			}


			string filePath = Application.dataPath + "/Resources/" + fileName + ".txt";
			File.WriteAllText(filePath, subsText);
			Debug.Log("Saving Subscribers...");
		}
	}

	public void AddSub ( string username, int donation )
	{
		bool isSubsciber = false;
		username = username.ToLower();
		Debug.Log ( "Adding Sub..." );
		for ( int i=0; i<subs.Count; i++ )
		{
			if ( username == subs[i].name )
			{
				isSubsciber = true;
				subs[i].donation = donation;
				GameObject.Find( "_Scripts" ).GetComponent<ChatManager>().MessageSend 
				(
					"Subsciber Edit Complete."
				);
				SaveSubs();
			}
		}

		if ( !isSubsciber )
		{
			Subsciber sub = new Subsciber ( username, donation );
			subs.Add ( sub );
			GameObject.Find( "_Scripts" ).GetComponent<ChatManager>().MessageSend 
				(
					"Subsciber Added."
				);
			SaveSubs();
		}
	}

	public void DeleteSub ( string username )
	{
		username = username.ToLower();

		for ( int i=0; i<subs.Count; i++ )
		{
			if ( subs[i].name == username )
			{
				subs.RemoveAt( i );
				GameObject.Find( "_Scripts" ).GetComponent<ChatManager>().MessageSend 
				(
					"Subsciber Removed."
				);
			}
		}
	}

	public int GetSubDonation ( string username )
	{
		int donation = 0;

		for ( int i=0; i<subs.Count; i++ )
		{
			if ( subs[i].name == username )
			{
				donation = subs[i].donation;
			}
		}

		return donation;
	}
}
