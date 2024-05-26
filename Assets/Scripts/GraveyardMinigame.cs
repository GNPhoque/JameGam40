using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GraveyardMinigame : MonoBehaviour
{
	public static event Action<int> OnEnergyValueChanged;
	public static Action OnGraveyardMinigameOpen;

	[SerializeField] GameObject graveyardMinigameFrame;
	[SerializeField] GraveyardMinigameCell cellPrefab;
	[SerializeField] Transform grid;
	[SerializeField] public Shovel currentShovel;
	[SerializeField] SerializedDictionary<DiggableLimb, int> weightedDiggableLimbPrefabs;
	//[SerializeField] DiggableLimb[] diggableLimbPrefabs;
	[SerializeField] DiggableLimb diggableBonusPrefab;
	[SerializeField] SpriteRenderer energyBar;
	[SerializeField] TMP_Text energyText;

	[SerializeField] int maxGravesPerDay;
	[SerializeField] int gravesOpenedThisDay;
	[SerializeField] int limbsCount;
	[SerializeField] int bonusCount;
	[SerializeField] int maxLimbTryPosition;
	[SerializeField] int maxBonusTryPosition;
	[SerializeField] int startEnergy;
	[SerializeField] int _currentEnergy;
	[SerializeField] int gridX;
	[SerializeField] int gridY;
	[SerializeField] bool DEBUG_generateGridInEditMode;
	[SerializeField] bool DEBUG_HideGrid;

	[Header("Animation")]
	[Header("Drop In")]
	[SerializeField] AnimationCurve dropInAnimationCurve;
	[SerializeField] float dropInAnimationOffset;
	[SerializeField] float dropInAnimationMult;
	[SerializeField] float dropInAnimationDuration;
	[SerializeField] float currentDropInAnimationDuration;
	[SerializeField] bool dropInAnimating;

	[Header("Drop Out")]
	[SerializeField] AnimationCurve dropOutAnimationCurve;
	[SerializeField] float dropOutAnimationOffset;
	[SerializeField] float dropOutAnimationMult;
	[SerializeField] float dropOutAnimationDuration;
	[SerializeField] float currentDropOutAnimationDuration;
	[SerializeField] bool dropOutAnimating;

	[Header("Limb Curve")]
	[SerializeField] float limbCurveDuration;
	[SerializeField] float currentLimbCurveDuration;
	[SerializeField] Vector2 limbCurveMiddle;
	[SerializeField] Vector2 limbCurveEnd;
	[SerializeField] bool limbCurveAnimating;
	[SerializeField] bool waitForEndOfCurveToDropOut;


	List<DiggableLimb> currentLimbsToAnimate = new List<DiggableLimb>();
	List<DiggableLimb> diggableLimbs = new List<DiggableLimb>();
	List<DiggableLimb> diggableBonus = new List<DiggableLimb>();
	GraveyardMinigameCell[,] cells = new GraveyardMinigameCell[0, 0];
	GraveyardMinigameCell[,] cellsCoveringLimbs = new GraveyardMinigameCell[0, 0];

	int currentEnergy
	{
		get { return _currentEnergy; }
		set
		{
			_currentEnergy = value;
			OnEnergyValueChanged?.Invoke(value);
			energyBar.size = new Vector2(1, value / (float)startEnergy);
			energyText.text = value.ToString();
		}
	}


	private void Start()
	{
		GraveyardMinigameCell.OnCellClicked += OnCellClicked;
		GraveyardMinigameCell.OnCellHoverIn += OnCellHoveredIn;
		GraveyardMinigameCell.OnCellHoverOut += OnCellHoveredOut;
		GraveyardMinigameCell.OnCellRevealed += OnCellRevealed;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) GameManager.instance.CloseGraveyardMinigame();

		if (dropInAnimating)
		{
			currentDropInAnimationDuration += Time.deltaTime;
			transform.position = new Vector3(18, dropInAnimationCurve.Evaluate(currentDropInAnimationDuration / dropInAnimationDuration) * dropInAnimationMult + dropInAnimationOffset);
			if (currentDropInAnimationDuration > dropInAnimationDuration)
			{
				dropInAnimating = false;
				OnGraveyardMinigameOpen?.Invoke();
			}
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
			foreach (DiggableLimb limb in currentLimbsToAnimate)
			{
				currentLimbCurveDuration += Time.deltaTime;
				limb.transform.position = LimbAnimationBezierQuadratic(currentLimbCurveDuration / limbCurveDuration, limb.digOutAnimationStartPosition); //update limb position

				if (currentLimbCurveDuration > limbCurveDuration)
				{
					limb.gameObject.SetActive(false);
					if (diggableLimbs.Count == 0) GameManager.instance.CloseGraveyardMinigame();
					
					limbCurveAnimating = false;
					if (waitForEndOfCurveToDropOut)
					{
						waitForEndOfCurveToDropOut = false;
						dropOutAnimating = true;
					}
				} 
			}
			if (!limbCurveAnimating) currentLimbsToAnimate.Clear();
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
			cell.ShowTarget();
		}
	}

	void OnCellHoveredOut(GraveyardMinigameCell clicked)
	{
		foreach (var cell in GetValidTargets(clicked))
		{
			cell.HideTarget();
		}
	}

	void OnCellClicked(GraveyardMinigameCell clicked)
	{
		if (dropInAnimating || dropOutAnimating || diggableLimbs.Count == 0) return;
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
		if (limbCurveAnimating) waitForEndOfCurveToDropOut = true;
		else dropOutAnimating = true;
	}

	void ResetGrid()
	{
		ClearGrid();

		//Spawn new cells
		for (int x = 0; x < gridX; x++)
		{
			float xPos = (-(gridX / 2) + x) * cellPrefab.transform.localScale.x;
			if (gridX % 2 == 0) xPos += .5f;
			for (int y = 0; y < gridY; y++)
			{
				float yPos = (-(gridY / 2) + y) * cellPrefab.transform.localScale.y;
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
			//DiggableLimb limb = diggableLimbPrefabs[Random.Range(0, diggableLimbPrefabs.Length)];
			DiggableLimb limb = GetWeightedDiggableLimb();
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

		//if (limb.isBonus) currentEnergy += limb.energyBonus; stamina bonus
		if (limb.isBonus) //Dig up a random unrevealed Cell
		{
			while (true)
			{
				int x = Random.Range(0, gridX);
				int y = Random.Range(0, gridY);
				
				if (!cells[x, y].isRevealed)
				{
					cells[x, y].DigUp(50);
					return;
				}
			}
		}
		else //Limb dug up!
		{
			diggableLimbs.Remove(limb);
			GameManager.instance.AddLimb(limb.inventoryLimbName);
			limb.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 50;
			currentLimbsToAnimate.Add(limb);
			limb.digOutAnimationStartPosition = limb.transform.position;
			currentLimbCurveDuration = 0f;
			limbCurveAnimating = true;
		}
	}

	Vector2 LimbAnimationBezierQuadratic(float t, Vector2 startPosition)
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		Vector2 ret = uu * startPosition;
		ret += 2 * u * t * limbCurveMiddle;
		ret += tt * limbCurveEnd;

		return ret;
	}

	public void SetWeightedDiggableLimb(Grave grave)
	{
		weightedDiggableLimbPrefabs = grave.weightedDiggableLimbPrefabs;
	}

	DiggableLimb GetWeightedDiggableLimb()
	{
		float sumOfWeights = 0;
		foreach (var weightedObject in weightedDiggableLimbPrefabs)
		{
			sumOfWeights += weightedObject.Value;
		}

		DiggableLimb selected = weightedDiggableLimbPrefabs.First().Key;
		float randChoice = Random.Range(0, sumOfWeights);
		float weightSum = 0;

		foreach (var weightedObject in weightedDiggableLimbPrefabs)
		{
			weightSum += weightedObject.Value;
			if (randChoice <= weightSum)
			{
				selected = weightedObject.Key;
				break;
			}
		}

		return selected;
	}
}