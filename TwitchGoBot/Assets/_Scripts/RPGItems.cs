using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPGItems : MonoBehaviour {

	public RPG.Item Potion ()
	{
		RPG.Item potion = new RPG.Item ( "Potion", 1, 25 );

		return potion;
	}

}
