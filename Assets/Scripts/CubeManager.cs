using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeManager : MonoBehaviour {

	private static CubeManager instance;

	public static CubeManager Instance
	{
		get { return instance ?? (instance = new GameObject("CubeManager").AddComponent<CubeManager>()); }
	}

	private List<GameObject> mCubeList;
	private List<Vector3> mVectorList;



	void Awake()
	{
		// First we check if there are any other instances conflicting
		if (instance != null && instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		// Here we save our singleton instance
		instance = this;
		mCubeList = new List<GameObject>();
		mVectorList = new List<Vector3>();
	}

	public void Add(GameObject gameObj)
	{

		if (!mCubeList.Contains(gameObj) && (gameObj != null))
		{
			mCubeList.Add(gameObj);
			mVectorList.Add(gameObj.transform.position);
		}
	}

	public void updateCube(GameObject gameObj)
	{
		for(int i = 0; i < mCubeList.Count; ++i)
		{
			if (mCubeList[i] == gameObj)
			{
				mVectorList[i] = gameObj.transform.position;
				break;
			}
		}
	}

	public GameObject getCube(Vector3 position)
	{
		GameObject returnObj = null;
		for (int i = 0; i < mVectorList.Count; ++i)
		{
			if (mVectorList[i] == position)
			{
				returnObj = mCubeList[i];
				break;
			}
		}

		return returnObj;
	}

	public bool getIfCubeGrounded(Vector3 position)
	{
		bool returnBool = false;
		GameObject returnObj = null;
		for (int i = 0; i < mVectorList.Count; ++i)
		{
			if (mVectorList[i] == position)
			{
				returnObj = mCubeList[i];
				break;
			}
		}

		if (returnObj != null)
		{
			CubeController cubeController = returnObj.GetComponent<CubeController>();
			if (cubeController != null)
			{
				returnBool = cubeController.isGroundedCube();
			}
		}

		return returnBool;
	}
}
