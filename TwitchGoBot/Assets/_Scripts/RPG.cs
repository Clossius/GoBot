using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPG : MonoBehaviour {

	public class Player 
	{
		public string username;
		public string title;
		public int maxhp;
		public int hp;
		public int mp;
		public int maxmp;
		public int exp;
		public int lvl;
		public int atk;
		public int def;
		public int mdef;
		public int matk;
		public int points;

		public List<Item> items;
		public List<Attack> attacks;

		public Player ( string u, string t, int h, int m, int e, int l, int a, int d, int md, int ma, int poi, List<Item> it, List<Attack> att)
		{
			username = u;
			title = t;
			maxhp = h;
			hp = h;
			mp = m;
			maxmp = m;
			exp = e;
			lvl = l;
			atk = a;
			def = d;
			mdef = md;
			matk = ma;
			points = poi;

			items = it;
			attacks = att;
		}
	}

	public class Monster 
	{
		public string username;
		public string title;
		public int maxhp;
		public int hp;
		public int mp;
		public int maxmp;
		public int exp;
		public int lvl;
		public int atk;
		public int def;
		public int mdef;
		public int matk;

		public List<Item> items;
		public List<Attack> attacks;

		public Monster ( string u, string t, int h, int m, int e, int l, int a, int d, int md, int ma, List<Item> it, List<Attack> att)
		{
			username = u;
			title = t;
			maxhp = h;
			hp = h;
			mp = m;
			maxmp = m;
			exp = e;
			lvl = l;
			atk = a;
			def = d;
			mdef = md;
			matk = ma;

			items = it;
			attacks = att;
		}
	}

	public class Item 
	{
		public string name;
		public int amount;
		public int dropRate;

		public Item ( string n, int a, int dr )
		{
			name = n;
			amount = a;
			dropRate = dr;
		}
	}

	public class Store 
	{
		public string item;
		public int cost;

		public Store ( string it, int c )
		{
			item = it;
			cost = c;
		}
	}

	public class Attack
	{
		string name;
		string type; //Types, Normal, Magical, Stun, Buff, Heal
		int dmg;

		public Attack ( string n, string t, int d )
		{
			name = n;
			type = t;
			dmg = d;
		}
	}


	string FileName = "RPG";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
