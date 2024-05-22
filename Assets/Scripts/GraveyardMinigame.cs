using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardMinigame : MonoBehaviour
{
	[SerializeField] GameObject graveyardMinigameFrame;
	[SerializeField] GraveyardMinigameCell cellPrefab;
	[SerializeField] Transform grid;
	[SerializeField] Shovel currentShovel;

	[SerializeField] int cellWidth;
	[SerializeField] int cellHeight;
	[SerializeField] int gridX;
	[SerializeField] int gridY;
	[SerializeField] bool generateGridInEditMode;

	GraveyardMinigameCell[,] cells = new GraveyardMinigameCell[0, 0];

	private void Start()
	{
		GraveyardMinigameCell.OnCellClicked += OnCellClicked;
	}

	//Used for testing displays in editor
	private void OnValidate()
	{
		if (!generateGridInEditMode)
		{
			ClearGrid(); 
			return;
		}

		ResetGrid();
	}

	public void Show()
	{
		print("Showing GraveyardMinigame");
		graveyardMinigameFrame.SetActive(true);
		ResetGrid();
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
			for (int y = 0; y < gridY; y++)
			{
				float yPos = -(gridY / 2) + y;
				GraveyardMinigameCell cell = Instantiate(cellPrefab, new Vector3(xPos, yPos, 0) + grid.position, Quaternion.identity, grid);
				cell.position = new Vector2Int(x, y);
				cells[x,y] = cell;
			}
		}
	}

	private void ClearGrid()
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
	}

	void OnCellClicked(GraveyardMinigameCell clicked)
	{
		foreach (var target in currentShovel.targets)
		{
			Vector2Int targetPos = clicked.position + target.Key;
			if (targetPos.x < 0 || targetPos.x >= gridX || targetPos.y < 0 || targetPos.y >= gridY) continue;
			cells[targetPos.x, targetPos.y].DigUp(target.Value);
		}
	}
}