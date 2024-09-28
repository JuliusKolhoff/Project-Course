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
	[SerializeField] private float startRotationSpeed = 2f;
	[SerializeField] private float startTorque = 10f;
	[SerializeField] private bool applyMachineEfficiency = false;

	private Dictionary<Vector2, Tile>[] _tiles = new Dictionary<Vector2, Tile>[2];

	void Start()
	{
		GenerateGrid(0);
		GenerateGrid(1);
		LinkAdjacentTiles(0);
		LinkAdjacentTiles(1);
		EnableLayer(0);
		GetTileAtPosition(new(0, 0),0).NewGear(4,0);
		GetTileAtIndex(_width-1,_height-1,0).NewGear(8,5);
	}
	public bool IncludeMachineEfficiency()
	{
		return applyMachineEfficiency;
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			EnableLayer(0);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			EnableLayer(1);
		}


		if (Input.GetKey(KeyCode.R))
		{
			Tile.RotatedTiles.Clear();
			GetTileAtPosition(new(0, 0),0).Rotate(startRotationSpeed, startTorque);
		}
		if(GetTileAtIndex(_width - 1, _height - 1,0).CurrentRotation != 0)
		{
			Debug.Log("Won");
		}
	}
	public void StopRotation(string Message)
	{
		Tile.RotatedTiles.Clear();
		GetTileAtPosition(new(0, 0), 0).Rotate(0, 0);
		if(Message != "")
			Debug.Log(Message);
		startRotationSpeed = 0;
	}
	private void EnableLayer(int layer)
	{
		if(layer == 0)
		{
			SetChildTransperency(_level1Partent, 1);
			SetChildTransperency(_level2Partent, .5f);
		}
		else if(layer == 1)
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
	void GenerateGrid(int layer)
	{
		_tiles[layer] = new Dictionary<Vector2, Tile>();
		Transform parent;
		if(layer == 0)
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
				spawnedTile.Layer = layer;

				var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
				if(layer == 1)
				{
					isOffset = !isOffset;
				}
				spawnedTile.Init(isOffset,this);


				_tiles[layer][new Vector2(x, y)] = spawnedTile;
			}
		}

		_cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
	}
	void LinkAdjacentTiles(int layer)
	{
		foreach (var tileEntry in _tiles[layer])
		{
			var tilePos = tileEntry.Key;
			var tile = tileEntry.Value;

			var adjacentTiles = GetAdjacentTiles((int)tilePos.x, (int)tilePos.y, layer);
			tile.AdjacentTiles.AddRange(adjacentTiles);
		}
	}


	public Tile GetTileAtPosition(Vector2 pos,int layer)
	{
		if (_tiles[layer].TryGetValue(pos, out var tile)) return tile;
		return null;
	}
	public Tile GetTileAtIndex(int i,int j,int layer)
	{
		return GetTileAtPosition(new Vector2(i , j), layer);
	}
	public List<Tile> GetAdjacentTiles(int i,int j,int layer)
	{
		List<Tile> tile = new();
		int otherLevel = Mathf.Abs(layer - 1);
		tile.Add(GetTileAtIndex(i, j, otherLevel));
		if (i > 0)
			tile.Add(GetTileAtIndex(i - 1, j,layer));
		if(i < _width - 1)
			tile.Add(GetTileAtIndex(i + 1, j, layer));
		if (j > 0)
			tile.Add(GetTileAtIndex(i, j - 1, layer));
		if(j < _height - 1)
			tile.Add(GetTileAtIndex(i, j + 1, layer));
		return tile;
	}
}