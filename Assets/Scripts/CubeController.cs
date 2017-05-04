using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour {

	enum CubeState
	{
		Bottom,
		Static,
		Shaking,
		Falling,
		Sliding,
		Dead
	}

	float timeOfFalling = 0;
	CubeState cubeState;
	Vector3 targetPosition;
	GameObject heldCube = null;
	// Use this for initialization
	void Awake () {
		targetPosition = transform.position;
		CubeManager.Instance.Add(gameObject);
		if (transform.position.y == 0)
		{
			cubeState = CubeState.Bottom;
		}
		else
		{
			cubeState = CubeState.Static;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (CheckForCubesBelow())
		{
			gameObject.GetComponent<Renderer>().material.color = Color.blue;
		}
		else
		{
			gameObject.GetComponent<Renderer>().material.color = Color.white;
		}

		if (cubeState == CubeState.Static)
		{
			if (!CheckForCubesBelow())
			{
				cubeState = CubeState.Shaking;
				timeOfFalling = Time.time + .5f;
				GameObject targetCube = CubeManager.Instance.getCube(transform.position + Vector3.up);
				if (targetCube != null)
				{
					heldCube = targetCube;
				}
			}
		}
		else if (cubeState == CubeState.Shaking)
		{
			Shaking();
		}
		else if (cubeState == CubeState.Falling)
		{
			Falling();
		}
	}

	void Shaking()
	{
		if (CheckForCubesBelow())
		{
			cubeState = CubeState.Static;
		}
		else if (Time.time >= timeOfFalling)
		{
			cubeState = CubeState.Falling;
		}
	}

	void Falling()
	{
		transform.position += (Vector3.down * .1f);
		CubeManager.Instance.updateCube(gameObject);

		if (CheckForCubesBelow())
		{
			cubeState = CubeState.Static;
		}

	}

	bool CheckForCubesBelow()
	{
		bool returnVal = false;
		Vector3 positionToCheck = transform.position;
		positionToCheck.y -= 1;
		
		for (int x = -1; x <= 1; ++x)
		{
			if (CubeManager.Instance.getIfCubeGrounded(new Vector3(positionToCheck.x + x, positionToCheck.y, positionToCheck.z )))
			{
				returnVal = true;
				break;
			}
		}

		if (!returnVal)
		{
			for (int z = -1; z <= 1; ++z)
			{
				if (CubeManager.Instance.getIfCubeGrounded(new Vector3(positionToCheck.x, positionToCheck.y, positionToCheck.z + z)))
				{
					returnVal = true;
					break;
				}
			}
		}

		return returnVal;
	}

	public void SetTargetPosition(Vector3 newTargetPosition)
	{
		cubeState = CubeState.Sliding;
		targetPosition = newTargetPosition;
	}

	public void UpdatePosition(Vector3 targetDirection)
	{
		transform.position += targetDirection * .1f;

		if (heldCube != null)
		{
			heldCube.SendMessage("UpdatePosition", targetDirection);
		}
	}

	public void SetEndPosition()
	{
		cubeState = CubeState.Static;
		transform.position = targetPosition;
		CubeManager.Instance.updateCube(gameObject);
		
		if (heldCube != null)
		{
			heldCube.SendMessage("SetEndPosition");
			heldCube = null;
		}
	}

	public void SetHeldCube(Vector3 targetDirection)
	{
		cubeState = CubeState.Sliding;
		targetPosition = transform.position + targetDirection;
		GameObject targetCube = CubeManager.Instance.getCube(transform.position + targetDirection);
		if (targetCube != null)
		{
			heldCube = targetCube;
			heldCube.SendMessage("SetHeldCube", targetDirection);
		}
	}

	public bool isGroundedCube()
	{
		return ((cubeState == CubeState.Static) || (cubeState == CubeState.Bottom) || (cubeState == CubeState.Sliding));
	}
}
