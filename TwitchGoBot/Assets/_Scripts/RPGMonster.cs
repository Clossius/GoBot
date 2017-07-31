using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPGMonster : MonoBehaviour {

	public RPG.Monster CreateGreenSlime ( int lvl )
	{
		string username = "Green Slime";
		string title = "Slime";
		int maxhp = 100 * ( lvl / 2 );
		int hp = 100 * ( lvl / 2 );
		int mp = 0;
		int maxmp = 0;
		int exp = 10 * ( lvl / 2 );
		int level = 1 * lvl;
		int atk = 10 + ( ( 10 * lvl ) / 4 );
		int def = 0;
		int mdef = 0;
		int matk = 0;

		List<RPG.Item> items = new List<RPG.Item>(  );
		List<RPG.Attack> attacks = new List<RPG.Attack>(  );

		RPG.Monster monster = new RPG.Monster ( username, title, hp, mp, exp, level, atk, def, mdef, matk, items, attacks );

		return monster;
	}

}
