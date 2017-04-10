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

	public static event UnityEngine.Events.UnityAction EFigureHoveredForOneSecond;
	public delegate void SettledTimeDelegate(float timeSinceDropped);
	public static event SettledTimeDelegate EFigureSettledTimeClocked;


	public static bool frozen = false;
	public static bool accelerated = false;
	
	
	const float frequencyModifierChangePerGenLevel = -0.085f;
	float genFrequencyModifier
	{
		get { return TetrisManager.Instance.generatorLevel * frequencyModifierChangePerGenLevel; }
	}

	//const float acceleratedTimeMultiplier = 15f;
	const float acceleratedFrequencyModifier = -0.92f;

    public TetrominoTypes tetrominoType;

    public List<Vector2> figureBlockOffsets
    {
        get
        {
            List<Vector2> blockCoords = new List<Vector2>();
            foreach (FigureBlock block in myBlocks)
                blockCoords.Add(block.getBlockOffsets);

            return blockCoords;
        }
    }

    float timeSinceDropped = 0;

	[SerializeField]
	Color figureColor;

	[SerializeField]
	int currentX;
	[SerializeField]
	int currentY;


	List<FigureBlock> myBlocks;

	bool dropped = false;

	float baseMoveDownFrequency = 1f;
	float softDropFrequencyModifier= -0.97f;
	bool softDropToggled = false;

	float moveSidewaysEveryNSeconds = 0.04f;
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

		TetrisManager.ETetrisEndClear += DisposeFigure;

		currentX = startX;
		currentY = startY;
		transform.SetParent(Grid.Instance.gridGroup, false);
		transform.SetAsLastSibling();
		GetComponent<RectTransform>().anchoredPosition = Grid.Instance.GetCellPositionInGrid(currentX, currentY);

		foreach (FigureBlock block in myBlocks)
			block.SnapToParent();

		StartCoroutine(MoveDownRoutine());
		StartCoroutine(TimeSinceDroppedRoutine());

		//FloatingText.CreateFloatingText("Figure dropped!", Color.black, 20, 2f, transform);
	}

	public void Initialize()
	{
		myBlocks = new List<FigureBlock>(transform.GetComponentsInChildren<FigureBlock>());

		System.Array blockTypes = System.Enum.GetValues(typeof(BlockType));

		//BlockType randomType = (BlockType)blockTypes.GetValue(Random.Range(0, blockTypes.Length));

		foreach (FigureBlock block in myBlocks)
			block.Initialize();
			//block.GetComponent<Image>().color = figureColor;
	}

	void DisposeFigure()
	{
		EMoveStatusDetermined = null;
		ERotateStatusDetermined = null;
		TetrisManager.ETetrisEndClear -= DisposeFigure;
		GameObject.Destroy(this.gameObject);
	}

	IEnumerator TimeSinceDroppedRoutine()
	{
		float hoverTickFrequencyInSecs = 1f;
		float timeUntilNextHoverTick = hoverTickFrequencyInSecs;
		while (true)
		{
			if (!frozen)
			{
				timeSinceDropped += TetrisManager.tetrisDeltaTime;
				timeUntilNextHoverTick -= TetrisManager.tetrisDeltaTime;
			}
			if (timeUntilNextHoverTick <= 0)
			{
				if (EFigureHoveredForOneSecond!=null) EFigureHoveredForOneSecond();
				timeUntilNextHoverTick = hoverTickFrequencyInSecs;
			}
			yield return new WaitForFixedUpdate();
		}
	}

	IEnumerator MoveDownRoutine()
	{
		float timeSinceLastMoveDown = 0;
		while (true)
		{
			if (!frozen)
			{
				float actingFrequencyModifier = 0;
				if (softDropToggled) actingFrequencyModifier = softDropFrequencyModifier;
				else
					if (accelerated) actingFrequencyModifier = acceleratedFrequencyModifier;
				else
					actingFrequencyModifier = genFrequencyModifier;

				if (timeSinceLastMoveDown >= baseMoveDownFrequency + actingFrequencyModifier)
				{
					TryMove(MoveDirections.Down);
					timeSinceLastMoveDown = 0;
				}
				/*
				float timeMultiplier;
				if (accelerated)
					timeMultiplier = acceleratedTimeMultiplier;
				else
					timeMultiplier = 1 + TetrisManager.Instance.currentSpeedMultiplierModifier;
					*/
				timeSinceLastMoveDown += TetrisManager.tetrisDeltaTime;// * timeMultiplier;
			}
			yield return new WaitForFixedUpdate();
		}
	}

	

	void Update()
	{
		if (dropped && !TetrisManager.paused)
		{
			if (Input.GetKeyDown(KeyCode.W))
				TryRotateFigure();

			if (Input.GetKeyDown(KeyCode.S))
			{
				softDropToggled = true;
				frozen = false;
			}
			if (Input.GetKeyUp(KeyCode.S))
				softDropToggled = false;

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
		int moveDistance = 0;//Compiler made me assign this
		for (int checkedDistance = 1; checkedDistance <= 2; checkedDistance++)
		{
			moveDistance = checkedDistance;

			canRotateAfterShift = true;
			moveDirection = MoveDirections.Left;
			if (checkShiftedRotationPossible.Invoke(moveDirection, checkedDistance))
				break;
			moveDirection = MoveDirections.Right;
			if (checkShiftedRotationPossible.Invoke(moveDirection, checkedDistance))
				break;
			moveDirection = MoveDirections.Up;
			if (checkShiftedRotationPossible.Invoke(moveDirection, checkedDistance))
				break;
			moveDirection = MoveDirections.Down;
			if (checkShiftedRotationPossible.Invoke(moveDirection, checkedDistance))
				break;

			canRotateAfterShift = false;
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
			GetComponent<RectTransform>().anchoredPosition = Grid.Instance.GetCellPositionInGrid(currentX, currentY);
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

		if (EFigureSettledTimeClocked != null)
			EFigureSettledTimeClocked(timeSinceDropped);
		if (EFigureSettled != null)
			EFigureSettled(figureSettledBlocks);
		DisposeFigure();
	}
	/*
	void HandlePlayerBlueGainFX(int blueGain)
	{
		PlayerShipModel.EPlayerGainedBlueEnergy -= HandlePlayerBlueGainFX;
		if (blueGain > 0)
		{
			FloatingText.CreateFloatingText(blueGain.ToString(), Color.cyan, 40, 1.5f, transform);
			ParticleDB.Instance.CreateSettledFigureParticles(transform.position);
		}
	}*/
}
