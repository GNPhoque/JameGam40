using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraveyardMinigame : MonoBehaviour
{
	[SerializeField] GameObject graveyardMinigameFrame;
	[SerializeField] GraveyardMinigameCell cellPrefab;
	[SerializeField] Transform grid;
	[SerializeField] Shovel currentShovel;
	[SerializeField] DiggableLimb[] diggableLimbPrefabs;
	[SerializeField] List<DiggableLimb> diggableLimbs;

	[SerializeField] int limbsCount;
	[SerializeField] int maxLimbTryPosition;
	[SerializeField] int startEnergy;
	[SerializeField] int currentEnergy;
	[SerializeField] int cellWidth;
	[SerializeField] int cellHeight;
	[SerializeField] int gridX;
	[SerializeField] int gridY;
	[SerializeField] bool generateGridInEditMode;

	GraveyardMinigameCell[,] cells = new GraveyardMinigameCell[0, 0];
	GraveyardMinigameCell[,] cellsCoveringLimbs = new GraveyardMinigameCell[0, 0];

	private void Start()
	{
		GraveyardMinigameCell.OnCellClicked += OnCellClicked;
		GraveyardMinigameCell.OnCellHoverIn += OnCellHoveredIn;
		GraveyardMinigameCell.OnCellHoverOut += OnCellHoveredOut;
	}

	//Used for testing displays in editor
	private void OnValidate()
	{
		if (Application.isPlaying) return;
		if (!generateGridInEditMode)
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
		foreach (var cell in GetValidTargets(clicked))
		{
			cell.DigUp(cell.incomingShovelDamage);
		}
	}
	#endregion

	[ContextMenu("ResetGrid")]
	public void Show()
	{
		print("Showing GraveyardMinigame");
		graveyardMinigameFrame.SetActive(true);
		ResetGrid();
		SetupLimbs();
		currentEnergy = startEnergy;
	}

	public void Exit()
	{
		graveyardMinigameFrame.SetActive(false);
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
		int tryCount = 0;
		int placedLimbs = 0;
		for (int i = 0; i < limbsCount; i++)
		{
			DiggableLimb limb = diggableLimbPrefabs[Random.Range(0, diggableLimbPrefabs.Length)];
			while (tryCount < maxLimbTryPosition)
			{
				Vector2Int position = new Vector2Int(Random.Range(-limb.coveredPositions.Min(x => x.x), gridX - limb.coveredPositions.Max(x => x.x)),
													Random.Range(-limb.coveredPositions.Min(x => x.y), gridY - limb.coveredPositions.Max(x => x.y)));
				if (IsLimbPositionValid(position, limb))
				{
					diggableLimbs.Add(Instantiate(limb, cells[position.x,position.y].transform.position, Quaternion.identity, grid));
					foreach (var pos in limb.coveredPositions)
					{
						Vector2Int coveringPosition = new Vector2Int(position.x + pos.x, position.y + pos.y);
						cellsCoveringLimbs[coveringPosition.x, coveringPosition.y] = cells[coveringPosition.x, coveringPosition.y];
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

	bool IsLimbPositionValid(Vector2Int position, DiggableLimb limb)
	{
		foreach (var target in limb.coveredPositions)
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
			targets.Add(cells[targetPos.x, targetPos.y]);
			cells[targetPos.x, targetPos.y].incomingShovelDamage = target.Value;
		}
		return targets;
	}
}