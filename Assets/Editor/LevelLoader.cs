using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections;
using System.Text;
using System.IO;


public class LevelLoader {

	public enum PrefabType { None, BlueCube, GoldenCube, ExtraLife };

	[MenuItem("MyTools/LoadLevel")]
	public static void LoadLevel()
    {
		DeGenerate();

		PrefabType prefabType = PrefabType.None;
		var path = EditorUtility.OpenFilePanel("Select File", "", "lvl");

		if (path.Length != 0)
		{
			StreamReader theReader = new StreamReader(path, Encoding.Default);
			string line;
			using (theReader)
			{
				// While there's lines left in the text file, do this:
				do
				{
					line = theReader.ReadLine();

					if (line != null)
					{
						if (line == "Blue")
						{
							prefabType = PrefabType.BlueCube;
						}
						else if (line == "Gold")
						{
							prefabType = PrefabType.GoldenCube;
						}
						else if (line == "Life")
						{
							prefabType = PrefabType.ExtraLife;
						}
						else if (line != "")
						{
							PlaceObject(line, prefabType);
						}
					}
				}
				while (line != null);
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
			}
		}
	}

	public static void PlaceObject(string line, PrefabType prefabType)
	{
		GameObject levelObject = null;

		Object blueCube = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/BlueCube.prefab", typeof(GameObject));
		Object goldenCube = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/GoldenCube.prefab", typeof(GameObject));
		Object ribbon = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Ribbon.prefab", typeof(GameObject));

		GameObject level = GameObject.Find("Level");
		if (level == null)
		{
			level = new GameObject("Level");
		}

		var lines = line.Split(',');

		Vector3 Position = new Vector3(float.Parse(lines[0]), float.Parse(lines[1]), float.Parse(lines[2]));
		switch (prefabType)
		{
			case PrefabType.BlueCube:
				levelObject = GameObject.Instantiate(blueCube, Position, Quaternion.identity) as GameObject;
				levelObject.transform.parent = level.transform;
				break;
			case PrefabType.GoldenCube:
				levelObject = GameObject.Instantiate(goldenCube, Position, Quaternion.identity) as GameObject;
				levelObject.transform.parent = level.transform;
				break;
			case PrefabType.ExtraLife:
				levelObject = GameObject.Instantiate(ribbon, Position, Quaternion.identity) as GameObject;
				levelObject.transform.parent = level.transform;
				break;
			default:
				break;
		}
	}

	[MenuItem("MyTools/DeleteObjects")]
	public static void DeGenerate()
	{
		GameObject level = GameObject.Find("Level");
		if (level != null)
		{
			while (level.transform.childCount > 0)
			{
				GameObject.DestroyImmediate(level.transform.GetChild(0).gameObject);
			}
		}
	}

}
