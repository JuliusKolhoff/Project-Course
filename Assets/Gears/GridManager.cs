using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	[SerializeField] private int _width, _height;
	[SerializeField] private Tile _tilePrefab;
	[SerializeField] private Transform _cam;
	[SerializeField] private Transform _parent;
	[SerializeField] private float rotationSpeed = 2f;

	private Dictionary<Vector2, Tile> _tiles;

	void Start()
	{
		GenerateGrid();
		GetTileAtPosition(new(0, 0)).NewGear();
	}
	private void Update()
	{
		if (Input.GetKey(KeyCode.R))
		{
			Tile.RotatedTiles.Clear();
			GetTileAtPosition(new(0, 0)).Rotate(rotationSpeed,10);
		}
	}
	void GenerateGrid()
	{
		_tiles = new Dictionary<Vector2, Tile>();
		for (int x = 0; x < _width; x++)
		{
			for (int y = 0; y < _height; y++)
			{
				var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity,_parent);
				spawnedTile.name = $"Tile {x} {y}";

				var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
				spawnedTile.Init(isOffset);


				_tiles[new Vector2(x, y)] = spawnedTile;
			}
		}

		_cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
		LinkAdjacentTiles();
	}
	void LinkAdjacentTiles()
	{
		foreach (var tileEntry in _tiles)
		{
			var tilePos = tileEntry.Key;
			var tile = tileEntry.Value;

			var adjacentTiles = GetAdjacentTiles((int)tilePos.x, (int)tilePos.y);
			tile.AdjacentTiles.AddRange(adjacentTiles);
		}
	}


	public Tile GetTileAtPosition(Vector2 pos)
	{
		if (_tiles.TryGetValue(pos, out var tile)) return tile;
		return null;
	}
	public Tile GetTileAtIndex(int i,int j)
	{
		return GetTileAtPosition(new Vector2(i , j));
	}
	public List<Tile> GetAdjacentTiles(int i,int j)
	{
		List<Tile> tile = new();
		if (i > 0)
			tile.Add(GetTileAtIndex(i - 1, j));
		if(i < _width)
			tile.Add(GetTileAtIndex(i + 1, j));
		if (j > 0)
			tile.Add(GetTileAtIndex(i, j - 1));
		if(j < _height)
			tile.Add(GetTileAtIndex(i, j + 1));
		return tile;
	}
}