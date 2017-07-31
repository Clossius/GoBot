using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
 
public class FileManager : MonoBehaviour {

	//Load settings
	public StreamReader LoadSettings(string fileName) 
	{
		string filePath = Application.dataPath + "/Resources/" + fileName + ".txt";

		StreamReader fileObject = new StreamReader(filePath);

		return fileObject;
	}

	// Save settings.
	public void SaveSettings ( string fileName, string data ) {

		string filePath = Application.dataPath + "/Resources/" + fileName + ".txt";
		bool exist = File.Exists(filePath  );

		if( !exist ){ System.IO.File.WriteAllText( filePath, data ); }

		else { File.WriteAllText(filePath, data); }

		}
}