using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPGPlayer : MonoBehaviour {

	public RPG.Player CreateWarrior ( string userN )
	{
		string username = userN;
		string title = "Warrior";
		int maxhp = 80;
		int hp = 80;
		int mp = 0;
		int maxmp = 0;
		int exp = 0;
		int lvl = 1;
		int atk = 11;
		int def = 1;
		int mdef = 0;
		int matk = 0;
		int points = 0;

		List<RPG.Item> items = new List<RPG.Item>(  );
		List<RPG.Attack> attacks = new List<RPG.Attack>(  );

		RPG.Player player = new RPG.Player ( username, title, hp, mp, exp, lvl, atk, def, mdef, matk, points, items, attacks );

		return player;
	}

	public RPG.Player CreateGuardian ( string userN )
	{
		string username = userN;
		string title = "Guardian";
		int maxhp = 100;
		int hp = 100;
		int mp = 0;
		int maxmp = 0;
		int exp = 0;
		int lvl = 1;
		int atk = 8;
		int def = 3;
		int mdef = 3;
		int matk = 0;
		int points = 0;

		List<RPG.Item> items = new List<RPG.Item>(  );
		List<RPG.Attack> attacks = new List<RPG.Attack>(  );

		RPG.Player player = new RPG.Player ( username, title, hp, mp, exp, lvl, atk, def, mdef, matk, points, items, attacks );

		return player;
	}

	public RPG.Player CreateMage ( string userN )
	{
		string username = userN;
		string title = "Mage";
		int maxhp = 50;
		int hp = 50;
		int mp = 25;
		int maxmp = 25;
		int exp = 0;
		int lvl = 1;
		int atk = 3;
		int def = 0;
		int mdef = 2;
		int matk = 5;
		int points = 0;

		List<RPG.Item> items = new List<RPG.Item>(  );
		List<RPG.Attack> attacks = new List<RPG.Attack>(  );

		RPG.Player player = new RPG.Player ( username, title, hp, mp, exp, lvl, atk, def, mdef, matk, points, items, attacks );

		return player;
	}

}
