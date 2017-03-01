using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FigureController : MonoBehaviour 
{
	public enum MoveDirections { Left, Right, Down };
	public enum MoveStatus { Waiting, Move, Cancel };
	public MoveStatus currentMoveStatus;

	public delegate void MoveStatusDelegate(bool moveConfirmed);
	public static event MoveStatusDelegate EMoveStatusDetermined;
	public delegate void RotateStatusDelegate(bool rotateConfirmed, int figureX, int figureY);
	public static event RotateStatusDelegate ERotateStatusDetermined;

	public delegate void DroppedDelegate(List<SettledBlock> settledBlocks);
	public static event DroppedDelegate EFigureSettled;

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
				timeSinceLastMoveDown += Time.deltaTime;
			}
			yield return new WaitForFixedUpdate();
		}
	}

	

	void Update()
	{
		if (dropped)
		{
			if (Input.GetKeyDown(KeyCode.W))
				TryRotate();

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

	void TryMove(MoveDirections direction){
		int newX = currentX;
		int newY = currentY;

		if (direction == MoveDirections.Down)
			newY = currentY - 1;
		if (direction == MoveDirections.Left)
			newX = currentX - 1;
		if (direction == MoveDirections.Right)
			newX = currentX + 1;
		
		bool movePossible = true;
		foreach (FigureBlock block in myBlocks){
			if (!block.TryMoveWithFigure(newX,newY)){
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

	void TryRotate(){
		bool rotatePossible = true;
		foreach (FigureBlock block in myBlocks)
		{
			if (!block.TryRotateAroundFigure(currentX,currentY))
			{
				rotatePossible = false;
				break;
			}
		}
		if (ERotateStatusDetermined != null)
			ERotateStatusDetermined(rotatePossible, currentX,currentY);
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
		Destroy(this.gameObject);
	}

}
