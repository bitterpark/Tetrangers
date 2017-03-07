using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TetrominoTypes { L, ReverseL, RightCorner, LeftCorner, T, Square, Line};

public class FigureController : MonoBehaviour 
{
	public enum MoveDirections { Left, Right, Down, Up};
	public enum MoveStatus { Waiting, Move, Cancel };
	public MoveStatus currentMoveStatus;

	public delegate void MoveStatusDelegate(bool moveConfirmed);
	public static event MoveStatusDelegate EMoveStatusDetermined;
	public delegate void RotateStatusDelegate(bool canRotate, int figureX, int figureY);
	public static event RotateStatusDelegate ERotateStatusDetermined;

	public delegate void DroppedDelegate(List<SettledBlock> settledBlocks);
	public static event DroppedDelegate EFigureSettled;

	public TetrominoTypes tetrominoType;

	public static bool frozen = false;

	[SerializeField]
	Color figureColor;

	[SerializeField]
	int currentX;
	[SerializeField]
	int currentY;


	List<FigureBlock> myBlocks;

	bool dropped = false;

	float moveDownFrequency = 1f;
	float moveDownFrequencyModifier;

	float moveSidewaysEveryNSeconds = 0.05f;
	bool sidewaysMoveReady = true;

	public void DisplayAsNext()
	{
		//Debug.LogFormat("{0} displayed as next figure",name);
		transform.SetParent(FigureSpawner.Instance.nextFigureDisplay,false);
		transform.localPosition = Vector3.zero;
		foreach (FigureBlock block in myBlocks)
			block.SnapToParent();
	}

	public void DropIntoPlay(int startX, int startY)
	{
		//Debug.LogFormat("{0} dropped into play", name);
		dropped = true;

		TetrisManager.ETetrisFinished += DisposeFigure;

		currentX = startX;
		currentY = startY;
		transform.SetParent(Grid.Instance.transform, false);
		transform.SetAsLastSibling();
		transform.localPosition = Grid.Instance.GetCellWorldPosition(currentX, currentY);

		foreach (FigureBlock block in myBlocks)
			block.SnapToParent();

		StartCoroutine(MoveDownRoutine());
	}

	public void Initialize()
	{
		myBlocks = new List<FigureBlock>(transform.GetComponentsInChildren<FigureBlock>());
		foreach (FigureBlock block in myBlocks)
			block.GetComponent<Image>().color = figureColor;
	}

	void DisposeFigure()
	{
		EMoveStatusDetermined = null;
		ERotateStatusDetermined = null;
		TetrisManager.ETetrisFinished -= DisposeFigure;
		GameObject.Destroy(this.gameObject);
	}

	IEnumerator MoveDownRoutine()
	{
		float timeSinceLastMoveDown = 0;
		while (true)
		{
			if (!frozen)
			{
				if (timeSinceLastMoveDown >= moveDownFrequency + moveDownFrequencyModifier)
				{
					TryMove(MoveDirections.Down);
					timeSinceLastMoveDown = 0;
				}
				timeSinceLastMoveDown += TetrisManager.tetrisDeltaTime;
			}
			yield return new WaitForFixedUpdate();
		}
	}

	

	void Update()
	{
		if (dropped && !TetrisManager.paused)
		{
			if (tetrominoType!=TetrominoTypes.Square & Input.GetKeyDown(KeyCode.W))
				TryRotateFigure();

			if (Input.GetKeyDown(KeyCode.S))
			{
				moveDownFrequencyModifier = -0.95f;
				frozen = false;
			}
			if (Input.GetKeyUp(KeyCode.S))
				moveDownFrequencyModifier = 0;

			if (Input.GetKey(KeyCode.A))
				if (sidewaysMoveReady)
				{
					TryMove(MoveDirections.Left);
					StartCoroutine(SidewaysMoveTimerRoutine());
				}
			if (Input.GetKey(KeyCode.D))
				if (sidewaysMoveReady)
				{
					TryMove(MoveDirections.Right);
					StartCoroutine(SidewaysMoveTimerRoutine());
				}
		}
	}

	IEnumerator SidewaysMoveTimerRoutine()
	{
		sidewaysMoveReady = false;
		yield return new WaitForSeconds(moveSidewaysEveryNSeconds);
		sidewaysMoveReady = true;
		yield break;
	}

	void TryRotateFigure()
	{
		if (!TryRegularRotate())
			TryShiftAndRotate();
	}

	void TryShiftAndRotate()
	{
		System.Func<MoveDirections, int, bool> checkShiftedRotationPossible = (MoveDirections dir, int distance) =>
		{
			bool rotatePossible = true;
			int newX, newY;
			GetNewCoordsFromMoveDirection(dir, distance, out newX, out newY);
			foreach (FigureBlock block in myBlocks)
			{
				if (!block.CanRotateAroundPoint(newX, newY))
				{
					rotatePossible = false;
					break;
				}
			}
			return rotatePossible;
		};

		bool canRotateAfterShift = false;
		MoveDirections moveDirection = MoveDirections.Left; //Compiler made me assign this
		int moveDistance = 0;
		for (int checkedDistance = 1; checkedDistance <= 2; checkedDistance++)
		{
			moveDistance = checkedDistance;
			if (checkShiftedRotationPossible.Invoke(MoveDirections.Left, checkedDistance))
			{
				moveDirection = MoveDirections.Left;
				canRotateAfterShift = true;
				break;
			}
			moveDirection = MoveDirections.Right;
			if (checkShiftedRotationPossible.Invoke(MoveDirections.Right, checkedDistance))
			{
				moveDirection = MoveDirections.Right;
				canRotateAfterShift = true;
				break;
			}
			moveDirection = MoveDirections.Up;
			if (checkShiftedRotationPossible.Invoke(MoveDirections.Up, checkedDistance))
			{
				moveDirection = MoveDirections.Up;
				canRotateAfterShift = true;
				break;
			}
		}
		if (canRotateAfterShift)
		{
			TryMove(moveDirection, moveDistance);
			TryRegularRotate();
		}
	}

	bool TryRegularRotate()
	{
		bool rotatePossible = true;
		foreach (FigureBlock block in myBlocks)
		{
			if (!block.TryRotateAroundFigure(currentX, currentY))
			{
				rotatePossible = false;
				break;
			}
		}
		if (ERotateStatusDetermined != null)
			ERotateStatusDetermined(rotatePossible, currentX, currentY);

		return rotatePossible;
	}

	void TryMove(MoveDirections direction)
	{
		TryMove(direction, 1);
	}

	void TryMove(MoveDirections direction, int distance){

		int newX, newY;
		GetNewCoordsFromMoveDirection(direction, distance, out newX, out newY);
		
		bool movePossible = true;
		foreach (FigureBlock block in myBlocks){
			if (!block.TryMoveWithFigure(newX, newY))
			{
				movePossible = false;
				break;
			}
		}
		if (movePossible)
		{
			currentX = newX;
			currentY = newY;
			transform.localPosition = Grid.Instance.GetCellWorldPosition(currentX, currentY);
		}
		if (EMoveStatusDetermined!=null)
			EMoveStatusDetermined(movePossible);

		if (direction == MoveDirections.Down && !movePossible)
			Settle();
		
	}

	

	void GetNewCoordsFromMoveDirection(MoveDirections direction, out int newX, out int newY)
	{
		GetNewCoordsFromMoveDirection(direction, 1, out newX, out newY);
	}

	void GetNewCoordsFromMoveDirection(MoveDirections direction, int moveLength, out int newX, out int newY)
	{
		newX = currentX;
		newY = currentY;

		if (direction == MoveDirections.Down)
			newY = currentY - moveLength;
		if (direction == MoveDirections.Up)
			newY = currentY + moveLength;
		if (direction == MoveDirections.Left)
			newX = currentX - moveLength;
		if (direction == MoveDirections.Right)
			newX = currentX + moveLength;
	}

	void Settle()
	{
		List<SettledBlock> figureSettledBlocks = new List<SettledBlock>();
		foreach (FigureBlock block in myBlocks)
		{
			SettledBlock newSettledBlock;
			if (block.TrySettleBlock(currentX,currentY,out newSettledBlock))
				figureSettledBlocks.Add(newSettledBlock);
			
		}
		if (EFigureSettled != null)
			EFigureSettled(figureSettledBlocks);
		DisposeFigure();
	}

}
