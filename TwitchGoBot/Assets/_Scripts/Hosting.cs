using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Irc;
using System.Linq;

public class Hosting : MonoBehaviour {

	public class Host 
	{
		string name;

		public Host ( string n ) 
		{
			name = n;
		}
	}

	List<Host> hostList = new List<Host>();
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
