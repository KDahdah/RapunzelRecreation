using UnityEngine;
using System.Collections;
using System;

public class KnightController : MonoBehaviour {

	enum KnightState
	{
		Idle,
		Moving,
		Pausing,
		ClimbingDown,
		ClimbingUp,
		ClimbingAcross,
		ClimbingDiagonal,
		ClimbingCorner,
		Hanging,
		HoldingBlock,
		Pulling,
		Falling,
		JumpingUp,
		JumpingDown,
		Dead
	}

	KnightState knightState = KnightState.Idle;
	Vector3 currentDirection = Vector3.forward;
	Vector3 targetDirection = Vector3.zero;
	Vector3 targetPosition = Vector3.zero;
	GameObject heldCube = null;
	float timeToUnpause = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{


		switch (knightState)
		{
			case KnightState.Idle:
				Idle();
				break;
			case KnightState.Moving:
				Moving();
				break;
			case KnightState.ClimbingDown:
				ClimbingDown();
				break;
			case KnightState.Hanging:
				Hanging();
				break;
			case KnightState.ClimbingUp:
				ClimbingUp();
				break;
			case KnightState.ClimbingAcross:
				ClimbingAcross();
				break;
			case KnightState.ClimbingCorner:
				ClimbingCorner();
				break;
			case KnightState.ClimbingDiagonal:
				ClimbingDiagonal();
				break;
			case KnightState.HoldingBlock:
				HoldingBlock();
				break;
			case KnightState.Pulling:
				Pulling();
				break;
			case KnightState.Falling:
				Falling();
				break;
			case KnightState.JumpingUp:
				JumpingUp();
				break;
			case KnightState.JumpingDown:
				JumpingDown();
				break;
			case KnightState.Pausing:
				Pausing();
				break;
		}
	}

	void ContinueDirectionMove()
	{
		if (CheckMovement())
		{
			targetPosition = targetDirection + transform.position;
			if ((CubeManager.Instance.getCube(targetPosition) == null) && (CubeManager.Instance.getCube(targetPosition + Vector3.down) != null))
			{
				knightState = KnightState.Moving;
			}
			else if (CubeManager.Instance.getCube(targetPosition) != null)
			{
				currentDirection = targetDirection;
				knightState = KnightState.Pausing;
				timeToUnpause = Time.time + 0.1f;
			}
		}
	}

	bool CheckMovement()
	{
		bool returnVal = false;
		if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical")))
		{
			if (Input.GetAxis("Horizontal") >= 1)
			{
				targetDirection = Vector3.left;
				returnVal = true;
			}
			else if (Input.GetAxis("Horizontal") <= -1)
			{
				targetDirection = Vector3.right;
				returnVal = true;
			}
		}
		else
		{
			if (Input.GetAxis("Vertical") >= 1)
			{
				targetDirection = Vector3.back;
				returnVal = true;
			}
			else if (Input.GetAxis("Vertical") <= -1)
			{
				targetDirection = Vector3.forward;
				returnVal = true;
			}
		}

		return returnVal;
	}

	void Idle()
	{
		if (Input.GetKey(KeyCode.Space))
		{

			heldCube = CubeManager.Instance.getCube(transform.position + currentDirection);
			if (heldCube != null)
			{
				transform.position += currentDirection * .25f;
				knightState = KnightState.HoldingBlock;
			}
		}
		else if (CheckMovement())
		{
			targetPosition = targetDirection + transform.position;
			if (CubeManager.Instance.getCube(targetPosition) == null)
			{
				currentDirection = targetDirection;
				if (CubeManager.Instance.getCube(targetPosition + Vector3.down) == null)
				{
					if (CubeManager.Instance.getCube(targetPosition + (Vector3.down * 2)) != null)
					{
						targetPosition += Vector3.down;
						knightState = KnightState.JumpingDown;
					}
					else
					{
						currentDirection = -targetDirection;
						targetPosition = transform.position + (targetDirection * .75f) + (Vector3.down * .75f);
						knightState = KnightState.ClimbingDown;
					}
				}
				else
				{
					knightState = KnightState.Moving;
				}
			}
			else if (targetDirection != currentDirection)
			{
				currentDirection = targetDirection;
				knightState = KnightState.Pausing;
				timeToUnpause = Time.time + 0.1f;
			}
			else if (CubeManager.Instance.getCube(targetPosition + Vector3.up) == null)
			{
				targetPosition = transform.position + targetDirection + Vector3.up;
				knightState = KnightState.JumpingUp;
			}
		}
	}

	void Moving()
	{
		transform.position += (targetDirection * .1f);

		if (Vector3.Distance(transform.position, targetPosition) <= 0.01f)
		{
			transform.position = targetPosition;
			knightState = KnightState.Idle;

			ContinueDirectionMove();
		}
	}

	void ClimbingDown()
	{
		Vector3 halfwayVector = targetPosition;
		halfwayVector.y = transform.position.y;

		if (Vector3.Distance(halfwayVector, transform.position) > 0.1f)
		{
			transform.position += (targetDirection * .15f);
		}
		else
		{
			transform.position = halfwayVector;
			if (Vector3.Distance(targetPosition, transform.position) > 0.1f)
			{
				transform.position += (Vector3.down * .15f);
			}
			else
			{
				transform.position = targetPosition;
				knightState = KnightState.Hanging;
			}
		}
	}

	void ClimbingAcross()
	{
		transform.position += (targetDirection * .1f);

		if (Vector3.Distance(transform.position, targetPosition) <= 0.01f)
		{
			transform.position = targetPosition;
			knightState = KnightState.Hanging;
		}
	}

	void ClimbingCorner()
	{
		Vector3 compareVector1 = Vector3.Scale(targetPosition, targetDirection);
		Vector3 compareVector2 = Vector3.Scale(transform.position, targetDirection);

		if (Vector3.Distance(compareVector1, compareVector2) > 0.1f)
		{
			transform.position += targetDirection * .15f;
		}
		else
		{
			transform.position -= compareVector2;
			transform.position += compareVector1;

			float diff = Vector3.Distance(targetPosition, transform.position);
			if (Vector3.Distance(targetPosition, transform.position) > 0.1f)
			{

				Vector3 compareVector3 = Vector3.Normalize(targetPosition - transform.position);
				transform.position += compareVector3 * .15f;
			}
			else
			{
				transform.position = targetPosition;
				knightState = KnightState.Hanging;
			}
		}

	}

	void ClimbingDiagonal()
	{
		Vector3 compareVector1 = Vector3.Scale(targetPosition, targetDirection);
		Vector3 compareVector2 = Vector3.Scale(transform.position, targetDirection);

		if (Vector3.Distance(compareVector1, compareVector2) > 0.01f)
		{
			transform.position += targetDirection * .05f;
		}
		else
		{
			transform.position -= compareVector2;
			transform.position += compareVector1;

			float diff = Vector3.Distance(targetPosition, transform.position);
			if (Vector3.Distance(targetPosition, transform.position) > 0.01f)
			{

				Vector3 compareVector3 = Vector3.Normalize(targetPosition - transform.position);
				transform.position += compareVector3 * .05f;
			}
			else
			{
				transform.position = targetPosition;
				knightState = KnightState.Hanging;
			}
		}

	}

	void Hanging()
	{
		if (CheckMovement())
		{
			var centerPosition = transform.position + (currentDirection * -.25f) + (Vector3.down * .25f);
			if (targetDirection == currentDirection)
			{
				if ((CubeManager.Instance.getCube(centerPosition + targetDirection + Vector3.up) == null) && (CubeManager.Instance.getCube(centerPosition + Vector3.up) == null))
				{
					targetPosition = centerPosition + targetDirection + Vector3.up;
					knightState = KnightState.ClimbingUp;
				}
			}
			else if (targetDirection != (currentDirection * -1))
			{
				if (CubeManager.Instance.getCube(centerPosition + targetDirection) != null)
				{
					targetPosition = transform.position + (currentDirection * -.25f) + (targetDirection * .25f);
					currentDirection = targetDirection;
					knightState = KnightState.ClimbingDiagonal;
				}
				else if (CubeManager.Instance.getCube(centerPosition + targetDirection + currentDirection) != null)
				{
					targetPosition = transform.position + targetDirection;
					knightState = KnightState.ClimbingAcross;
				}
				else
				{
					targetPosition = transform.position + (currentDirection * .75f) + (targetDirection * .75f);
					currentDirection = -targetDirection;
					knightState = KnightState.ClimbingCorner;
				}
			}
		}
	}

	void ClimbingUp()
	{
		Vector3 halfwayVector = transform.position;
		halfwayVector.y = targetPosition.y;

		if (Vector3.Distance(halfwayVector, transform.position) > 0.1f)
		{
			transform.position += (Vector3.up * .15f);
		}
		else
		{
			transform.position = halfwayVector;
			if (Vector3.Distance(targetPosition, transform.position) > 0.1f)
			{
				transform.position += (targetDirection * .15f);
			}
			else
			{
				transform.position = targetPosition;
				knightState = KnightState.Idle;
			}
		}
	}

	void HoldingBlock()
	{
		if (Input.GetKeyUp(KeyCode.Space))
		{
			heldCube = null;
			transform.position -= currentDirection * .25f;
			knightState = KnightState.Idle;
		}		
		else if (CheckMovement())
		{
			if (currentDirection == targetDirection)
			{
				heldCube.SendMessage("SetHeldCube", targetDirection);
				targetPosition = targetDirection + transform.position;
				knightState = KnightState.Pulling;
			}
			else if (currentDirection == -targetDirection)
			{
				if (CubeManager.Instance.getCube(heldCube.transform.position + (targetDirection*2)) == null)
				{
					heldCube.SendMessage("SetTargetPosition", heldCube.transform.position + targetDirection);
					targetPosition = targetDirection + transform.position;
					knightState = KnightState.Pulling;
				}
			}
		}
	}

	void Pulling()
	{
		transform.position += (targetDirection * .1f);
		heldCube.SendMessage("UpdatePosition", targetDirection);

		if (Vector3.Distance(transform.position, targetPosition) <= 0.01f)
		{
			heldCube.SendMessage("SetEndPosition");
			transform.position = targetPosition;

			if (CubeManager.Instance.getCube(transform.position - (currentDirection * .25f) + Vector3.down) == null)
			{
				heldCube = null;
				transform.position += (Vector3.up * .25f) - (currentDirection * .25f) - (targetDirection * .25f);
				currentDirection = -targetDirection;
				knightState = KnightState.Falling;
			}
			else
			{
				knightState = KnightState.HoldingBlock;
			}
		}
	}

	void Falling()
	{
		transform.position += Vector3.down * .1f;

		Vector3 checkVector = transform.position + (currentDirection * .75f) + (Vector3.down * .25f);
		if (CubeManager.Instance.getCube(checkVector) != null)
		{
			knightState = KnightState.Hanging;
		}
	}

	void JumpingUp()
	{
		Vector3 verticalVector1 = Vector3.Scale(targetDirection, transform.position);
		verticalVector1.y = targetPosition.y;

		Vector3 verticalVector2 = Vector3.Scale(targetDirection, targetPosition);
		verticalVector2.y = targetPosition.y;

		if (Vector3.Distance(verticalVector1, verticalVector2) >= .5f)
		{
			transform.position += (Vector3.up * .2f);
			transform.position += (targetDirection * .1f);
		}
		else if (Vector3.Distance(verticalVector1, verticalVector2) > .01f)
		{
			transform.position += (Vector3.down * .05f);
			transform.position += (targetDirection * .1f);
		}
		else
		{
			transform.position = targetPosition;
			knightState = KnightState.Idle;
		}
	}

	void JumpingDown()
	{
		Vector3 verticalVector1 = Vector3.Scale(targetDirection, transform.position);
		verticalVector1.y = targetPosition.y;

		Vector3 verticalVector2 = Vector3.Scale(targetDirection, targetPosition);
		verticalVector2.y = targetPosition.y;

		if (Vector3.Distance(verticalVector1, verticalVector2) >= .5f)
		{
			transform.position += (Vector3.up * .05f);
			transform.position += (targetDirection * .1f);
		}
		else if (Vector3.Distance(verticalVector1, verticalVector2) > .01f)
		{
			transform.position += (Vector3.down * .2f);
			transform.position += (targetDirection * .1f);
		}
		else
		{
			transform.position = targetPosition;
			knightState = KnightState.Idle;
		}
	}
	
	void Pausing()
	{
		if (Time.time >= timeToUnpause)
		{
			knightState = KnightState.Idle;
		}
	}
}
