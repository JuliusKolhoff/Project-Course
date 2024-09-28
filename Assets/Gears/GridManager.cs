using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	[SerializeField] private int _width, _height;
	[SerializeField] private Tile _tilePrefab;
	[SerializeField] private Transform _cam;
	[SerializeField] private GameObject _level1Partent;
	[SerializeField] private GameObject _level2Partent;
	[SerializeField] private float rotationSpeed = 2f;

	private Dictionary<Vector2, Tile>[] _tiles = new Dictionary<Vector2, Tile>[2];

	void Start()
	{
		GenerateGrid(0);
		GenerateGrid(1);
		LinkAdjacentTiles(0);
		LinkAdjacentTiles(1);
		EnableLevel(0);
		GetTileAtPosition(new(0, 0),0).NewGear(4);
		GetTileAtIndex(_width-1,_height-1,0).NewGear(8);
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			EnableLevel(0);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			EnableLevel(1);
		}


		if (Input.GetKey(KeyCode.R))
		{
			Tile.RotatedTiles.Clear();
			GetTileAtPosition(new(0, 0),0).Rotate(rotationSpeed,10);
		}
		if(GetTileAtIndex(_width - 1, _height - 1,0).CurrentRotation != 0)
		{
			Debug.Log("Won");
		}
	}
	private void EnableLevel(int level)
	{
		if(level == 0)
		{
			SetChildTransperency(_level1Partent, 1);
			SetChildTransperency(_level2Partent, .5f);
		}
		else if(level == 1)
		{
			SetChildTransperency(_level1Partent, .5f);
			SetChildTransperency(_level2Partent, 1);
		}
		
	}
	private void SetChildTransperency(GameObject parent,float alpha)
	{
		//SpriteRenderer[] children = parent.GetComponentsInChildren<SpriteRenderer>();
		foreach (BoxCollider2D collider in parent.GetComponentsInChildren<BoxCollider2D>())
		{
			//Try to find the Renderer of a normal Gear
			Transform gear = collider.gameObject.transform.Find("Gear");
			//if no normal Cog was found search for a small one
			if (gear == null)
			{
				gear = collider.gameObject.transform.Find("SmallGear");
			}
			//if no cogs are found following code is not needed
			if (gear != null)
			{
				SpriteRenderer renderer = gear.gameObject.GetComponent<SpriteRenderer>();
				renderer.color = new(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
				if (alpha != 1)//Not in Focused Layer
				{
					renderer.sortingOrder = 1;
				}
				else//In Focused Layer
				{
					renderer.sortingOrder = 2;
				}
			}
			if (alpha != 1)//Not in Focused Layer
			{
				collider.enabled = false;
			}
			else//In Focused Layer
			{
				collider.enabled = true;
			}

		}
	}
	void GenerateGrid(int level)
	{
		_tiles[level] = new Dictionary<Vector2, Tile>();
		Transform parent;
		if(level == 0)
		{
			parent = _level1Partent.transform;
		}
		else
		{
			parent = _level2Partent.transform;
		}
		for (int x = 0; x < _width; x++)
		{
			for (int y = 0; y < _height; y++)
			{
				var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity,parent);
				spawnedTile.name = $"Tile {x} {y}";
				spawnedTile.Level = level;

				var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
				spawnedTile.Init(isOffset);


				_tiles[level][new Vector2(x, y)] = spawnedTile;
			}
		}

		_cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
	}
	void LinkAdjacentTiles(int level)
	{
		foreach (var tileEntry in _tiles[level])
		{
			var tilePos = tileEntry.Key;
			var tile = tileEntry.Value;

			var adjacentTiles = GetAdjacentTiles((int)tilePos.x, (int)tilePos.y, level);
			tile.AdjacentTiles.AddRange(adjacentTiles);
		}
	}


	public Tile GetTileAtPosition(Vector2 pos,int level)
	{
		if (_tiles[level].TryGetValue(pos, out var tile)) return tile;
		return null;
	}
	public Tile GetTileAtIndex(int i,int j,int level)
	{
		return GetTileAtPosition(new Vector2(i , j), level);
	}
	public List<Tile> GetAdjacentTiles(int i,int j,int level)
	{
		List<Tile> tile = new();
		int otherLevel = Mathf.Abs(level - 1);
		tile.Add(GetTileAtIndex(i, j, otherLevel));
		if (i > 0)
			tile.Add(GetTileAtIndex(i - 1, j,level));
		if(i < _width - 1)
			tile.Add(GetTileAtIndex(i + 1, j, level));
		if (j > 0)
			tile.Add(GetTileAtIndex(i, j - 1, level));
		if(j < _height - 1)
			tile.Add(GetTileAtIndex(i, j + 1, level));
		return tile;
	}
}