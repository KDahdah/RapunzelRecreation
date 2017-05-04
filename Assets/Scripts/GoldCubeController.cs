using UnityEngine;
using System.Collections;

public class GoldCubeController : MonoBehaviour {

	void Awake()
	{
		CubeManager.Instance.Add(gameObject);
	}
}
