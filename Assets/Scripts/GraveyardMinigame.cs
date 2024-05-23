using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GraveyardMinigame : MonoBehaviour
{
	[SerializeField] GameObject graveyardMinigameFrame;
	[SerializeField] GraveyardMinigameCell cellPrefab;
	[SerializeField] Transform grid;
	[SerializeField] Shovel currentShovel;
	[SerializeField] DiggableLimb[] diggableLimbPrefabs;
	[SerializeField] DiggableLimb diggableBonusPrefab;

	[SerializeField] int limbsCount;
	[SerializeField] int bonusCount;
	[SerializeField] int maxLimbTryPosition;
	[SerializeField] int maxBonusTryPosition;
	[SerializeField] int startEnergy;
	[SerializeField] int currentEnergy;
	[SerializeField] int cellWidth;
	[SerializeField] int cellHeight;
	[SerializeField] int gridX;
	[SerializeField] int gridY;
	[SerializeField] bool DEBUG_generateGridInEditMode;
	[SerializeField] bool DEBUG_HideGrid;

	[Header("Animation")]
	[SerializeField] AnimationCurve dropInAnimationCurve;
	[SerializeField] float dropInAnimationOffset;
	[SerializeField] float dropInAnimationMult;
	[SerializeField] float dropInAnimationDuration;
	[SerializeField] float currentDropInAnimationDuration;
	[SerializeField] bool dropInAnimating;

	[SerializeField] AnimationCurve dropOutAnimationCurve;
	[SerializeField] float dropOutAnimationOffset;
	[SerializeField] float dropOutAnimationMult;
	[SerializeField] float dropOutAnimationDuration;
	[SerializeField] float currentDropOutAnimationDuration;
	[SerializeField] bool dropOutAnimating;

	DiggableLimb currentLimbToAnimate;
	Vector2 currentLimbToAnimateStartPosition;
	[SerializeField] float limbCurveDuration;
	[SerializeField] float currentLimbCurveDuration;
	[SerializeField] Vector2 limbCurveMiddle;
	[SerializeField] Vector2 limbCurveEnd;
	[SerializeField] bool limbCurveAnimating;


	List<DiggableLimb> diggableLimbs = new List<DiggableLimb>();
	List<DiggableLimb> diggableBonus = new List<DiggableLimb>();
	GraveyardMinigameCell[,] cells = new GraveyardMinigameCell[0, 0];
	GraveyardMinigameCell[,] cellsCoveringLimbs = new GraveyardMinigameCell[0, 0];


	private void Start()
	{
		GraveyardMinigameCell.OnCellClicked += OnCellClicked;
		GraveyardMinigameCell.OnCellHoverIn += OnCellHoveredIn;
		GraveyardMinigameCell.OnCellHoverOut += OnCellHoveredOut;
		GraveyardMinigameCell.OnCellRevealed += OnCellRevealed;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) Exit();

		if (dropInAnimating)
		{
			currentDropInAnimationDuration += Time.deltaTime;
			transform.position = new Vector3(18, dropInAnimationCurve.Evaluate(currentDropInAnimationDuration / dropInAnimationDuration) * dropInAnimationMult + dropInAnimationOffset);
			if (currentDropInAnimationDuration > dropInAnimationDuration) dropInAnimating = false;
		}

		if (dropOutAnimating)
		{
			currentDropOutAnimationDuration += Time.deltaTime;
			transform.position = new Vector3(18, dropOutAnimationCurve.Evaluate(currentDropOutAnimationDuration / dropOutAnimationDuration) * dropOutAnimationMult + dropOutAnimationOffset);
			if (currentDropOutAnimationDuration > dropOutAnimationDuration)
			{
				dropOutAnimating = false;
				ClearGrid();
			}
		}

		if (limbCurveAnimating)
		{
			currentLimbCurveDuration += Time.deltaTime;
			currentLimbToAnimate.transform.position = LimbAnimationBezierQuadratic(currentLimbCurveDuration / limbCurveDuration);
			if (currentLimbCurveDuration > limbCurveDuration)
			{
				limbCurveAnimating = false;
				currentLimbToAnimate.gameObject.SetActive(false);
			}
		}
	}

	//Used for testing displays in editor
	private void OnValidate()
	{
		if (Application.isPlaying) return;
		if (!DEBUG_generateGridInEditMode)
		{
			ClearGrid();
			return;
		}

		ResetGrid();
	}

	#region MOUSE
	void OnCellHoveredIn(GraveyardMinigameCell clicked)
	{
		foreach (var cell in GetValidTargets(clicked))
		{
			cell.Highlight();
		}
	}

	void OnCellHoveredOut(GraveyardMinigameCell clicked)
	{
		foreach (var cell in GetValidTargets(clicked))
		{
			cell.ShowDurability();
		}
	}

	void OnCellClicked(GraveyardMinigameCell clicked)
	{
		if (currentEnergy < currentShovel.energyCost) return; //TODO : Add sound
		currentEnergy -= currentShovel.energyCost;

		foreach (var cell in GetValidTargets(clicked))
		{
			cell.DigUp(cell.incomingShovelDamage);
		}

		if (currentEnergy <= 0) GameManager.instance.CloseGraveyardMinigame();
	}
	#endregion

	[ContextMenu("ResetGrid")]
	public void Show()
	{
		print("Showing GraveyardMinigame");
		currentDropInAnimationDuration = 0;
		dropInAnimating = true;
		ResetGrid();
		SetupLimbs();
		SetupBonus();
		currentEnergy = startEnergy;
	}

	public void Exit()
	{
		currentDropOutAnimationDuration = 0;
		dropOutAnimating = true;
	}

	void ResetGrid()
	{
		ClearGrid();

		//Spawn new cells
		for (int x = 0; x < gridX; x++)
		{
			float xPos = -(gridX / 2) + x;
			if (gridX % 2 == 0) xPos += .5f;
			for (int y = 0; y < gridY; y++)
			{
				float yPos = -(gridY / 2) + y;
				if (gridY % 2 == 0) yPos += .5f;
				GraveyardMinigameCell cell = Instantiate(cellPrefab, new Vector3(xPos, yPos, 0) + grid.position, Quaternion.identity, grid);
				cell.position = new Vector2Int(x, y);
				cell.gameObject.name = $"Cell {x},{y}";
				cells[x, y] = cell;
				if(DEBUG_HideGrid) cell.gameObject.SetActive(false);
			}
		}
	}

	void ClearGrid()
	{
		for (int i = 0; i < grid.childCount; i++)
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.delayCall += () =>
			{
				DestroyImmediate(grid.GetChild(0).gameObject);
			};
#else
			Destroy(grid.GetChild(i).gameObject);
#endif
		}
		cells = new GraveyardMinigameCell[gridX, gridY];
		cellsCoveringLimbs = new GraveyardMinigameCell[gridX, gridY];
	}

	void SetupLimbs()
	{
		diggableLimbs.Clear();
		int tryCount = 0;
		int placedLimbs = 0;

		for (int i = 0; i < limbsCount; i++)
		{
			DiggableLimb limb = diggableLimbPrefabs[Random.Range(0, diggableLimbPrefabs.Length)];
			while (tryCount < maxLimbTryPosition)
			{
				Vector2Int position = new Vector2Int(Random.Range(-limb.coveredPositionsSetup.Min(x => x.x), gridX - limb.coveredPositionsSetup.Max(x => x.x)),
													Random.Range(-limb.coveredPositionsSetup.Min(x => x.y), gridY - limb.coveredPositionsSetup.Max(x => x.y)));
				if (IsLimbPositionValid(position, limb))
				{
					DiggableLimb spawned = Instantiate(limb, cells[position.x,position.y].transform.position, Quaternion.identity, grid);
					diggableLimbs.Add(spawned);
					foreach (var pos in limb.coveredPositionsSetup)
					{
						Vector2Int coveringPosition = new Vector2Int(position.x + pos.x, position.y + pos.y);
						cellsCoveringLimbs[coveringPosition.x, coveringPosition.y] = cells[coveringPosition.x, coveringPosition.y];
						spawned.coveredPositions.Add(coveringPosition);
					}
					Debug.Log($"{limb.name} placed at : {position}");
					placedLimbs++;
					break;
				}
				else
				{
					Debug.Log($"Bad position : {position}");
					tryCount++;
				}
			}
		}
		Debug.Log($"Placed {placedLimbs} out of {limbsCount} expected");
	}

	void SetupBonus()
	{
		diggableBonus.Clear();
		int tryCount = 0;
		int placedBonus = 0;

		for (int i = 0; i < bonusCount; i++)
		{
			while (tryCount < maxBonusTryPosition)
			{
				Vector2Int position = new Vector2Int(Random.Range(-diggableBonusPrefab.coveredPositionsSetup.Min(x => x.x), gridX - diggableBonusPrefab.coveredPositionsSetup.Max(x => x.x)),
													Random.Range(-diggableBonusPrefab.coveredPositionsSetup.Min(x => x.y), gridY - diggableBonusPrefab.coveredPositionsSetup.Max(x => x.y)));
				if (IsLimbPositionValid(position, diggableBonusPrefab))
				{
					DiggableLimb spawned = Instantiate(diggableBonusPrefab, cells[position.x,position.y].transform.position, Quaternion.identity, grid);
					diggableBonus.Add(spawned);
					foreach (var pos in diggableBonusPrefab.coveredPositionsSetup)
					{
						Vector2Int coveringPosition = new Vector2Int(position.x + pos.x, position.y + pos.y);
						cellsCoveringLimbs[coveringPosition.x, coveringPosition.y] = cells[coveringPosition.x, coveringPosition.y];
						spawned.coveredPositions.Add(coveringPosition);
					}
					Debug.Log($"Bonus placed at : {position}");
					placedBonus++;
					break;
				}
				else
				{
					Debug.Log($"Bad position : {position}");
					tryCount++;
				}
			}
		}
		Debug.Log($"Placed {placedBonus} out of {bonusCount} expected");
	}

	bool IsLimbPositionValid(Vector2Int position, DiggableLimb limb)
	{
		foreach (var target in limb.coveredPositionsSetup)
		{
			Vector2Int targetPos = position + target;
			//Check if position is in grid and not already covering another limb
			if ((targetPos.x < 0 || targetPos.x >= gridX || targetPos.y < 0 || targetPos.y >= gridY) || cellsCoveringLimbs[targetPos.x, targetPos.y] != null) return false;
		}
		return true;
	}

	List<GraveyardMinigameCell> GetValidTargets(GraveyardMinigameCell from)
	{
		List<GraveyardMinigameCell> targets = new List<GraveyardMinigameCell>();
		foreach (var target in currentShovel.targets)
		{
			Vector2Int targetPos = from.position + target.Key;
			if (targetPos.x < 0 || targetPos.x >= gridX || targetPos.y < 0 || targetPos.y >= gridY) continue;
			if (cells[targetPos.x, targetPos.y].isRevealed) continue;

			targets.Add(cells[targetPos.x, targetPos.y]);
			cells[targetPos.x, targetPos.y].incomingShovelDamage = target.Value;
		}
		return targets;
	}

	public void OnCellRevealed(GraveyardMinigameCell revealedCell)
	{
		DiggableLimb limb = diggableLimbs.FirstOrDefault(x => x.coveredPositions.Contains(revealedCell.position));
		if (limb == null)
		{
			limb = diggableBonus.FirstOrDefault(x => x.coveredPositions.Contains(revealedCell.position));
			if (limb == null) return; //No hidden object revealed
		}

		foreach (var item in limb.coveredPositions)
		{
			if (cells[item.x, item.y].isRevealed == false) return; //limb is still partially covered
		}

		if (limb.isBonus) currentEnergy += limb.energyBonus;
		else
		{
			//TODO : Add limb to inventory
			Debug.Log("TODO : Add limb to inventory");
			limb.GetComponent<SpriteRenderer>().sortingOrder = 50;
			currentLimbToAnimate = limb;
			currentLimbToAnimateStartPosition = limb.transform.position;
			limbCurveAnimating = true;
		}
	}

	Vector2 LimbAnimationBezierQuadratic(float t)
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		Vector2 ret = uu * currentLimbToAnimateStartPosition;
		ret += 2 * u * t * limbCurveMiddle;
		ret += tt * limbCurveEnd;

		return ret;
	}
}