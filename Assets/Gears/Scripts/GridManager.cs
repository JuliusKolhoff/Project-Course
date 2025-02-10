using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class GridManager : MonoBehaviour
{
	public int Width, Height;
	[SerializeField] private TMP_Text WidthSliderText, HeightSliderText, TorqueSliderText,RpmSliderText;
	[SerializeField] private Tile _tilePrefab;
	[SerializeField] private Transform _cam;
	[SerializeField] private GameObject _level1Partent;
	[SerializeField] private GameObject _level2Partent;
	[SerializeField] private Transform _canvas;
	[SerializeField] private GameObject _popupTextPrefab;
	[SerializeField] private float startRotationSpeed = 2f;
	[SerializeField] private bool applyMachineEfficiency = false;

	public bool PlaceMotor { get; set; } = false;
	public bool PlaceGoal { get; set; } = false;

	private bool _won;
	private int _exitTimer;
	private int _exitTime = 50;
	private bool _loading;
	private string _currentLevel;

	public Vector2 MotorPosition { get; set; } = new(0,0);
	public float MotorTorque { get; set; } = 10f;
	public List<Vector2> GoalPositions { get; set; } = new();
	public float MinTorque { get; set; }
	public float MinRpm { get; set; }

	private GameObject _popup;
	[SerializeField] private GameObject _popupButton;

	private Dictionary<Vector2, Tile>[] _tiles = new Dictionary<Vector2, Tile>[2];
	private TextAsset[] _textAssets;
	private Dictionary<int, string> _popupTexts = new();
	public void SetTilesVisible(bool value)
	{
		if(_level1Partent != null)
		{
			_level1Partent.SetActive(value);
		}
		if (_level2Partent != null)
		{
			_level2Partent.SetActive(value);
		}
	}
	public bool HasEnabledPopup()
	{
		return _popup != null && _popup.activeSelf;
	}
	public void SetPopupEnabled(bool value)
	{
		_popup.SetActive(value);
	}
	public void SetPopupButtonEnabled(bool value)
	{
		_popupButton.SetActive(value);
	}
	public bool IsPopupButtonEnabled()
	{
		return _popupButton.activeSelf;
	}
	void Start()
	{
		_textAssets = Resources.LoadAll<TextAsset>("Popups/");
		foreach(var t in _textAssets) 
		{
			_popupTexts.Add(int.Parse(t.name.Split("_")[0]),t.text);
		}
		LoadState(SceneLoader.LoadParameter);
		SetPopupButtonEnabled(false);
		int index = int.Parse(SceneLoader.LoadParameter.Split("_")[0]);
		string text = _popupTexts.GetValueOrDefault(index, null);
		if (!string.IsNullOrEmpty(text))
		{
			SetPopupButtonEnabled(true);
			_popup = Instantiate(_popupTextPrefab, _canvas);
			_popup.GetComponent<Popup>().Controller = this;
			_popup.GetComponent<TMP_Text>().text = text;
			SetTilesVisible(false);
		}
	}
	public void SetRpmFromSlider(Slider slider)
	{
		MinRpm = slider.value;
		RpmSliderText.text = $"Rpm: {MinRpm}";
	}
	public void SetTorqueFromSlider(Slider slider)
	{
		MinTorque = slider.value;
		TorqueSliderText.text = $"Torque: {MinTorque}";
	}
	public void SetWidthFromSlider(Slider slider)
	{
		Width = (int)slider.value;
		WidthSliderText.text = $"Width: {Width}";
		ResizeGrid(Width, Height);
	}
	public void SetHeightFromSlider(Slider slider)
	{
		Height = (int)slider.value;
		HeightSliderText.text = $"Height: {Height}";
		ResizeGrid(Width, Height);
	}
	public void SetPlaceGoal(bool value)
	{
		PlaceGoal = value;
	}
	public void NewGoalPosition(Vector2 position)
	{
		GoalPositions.Add(position);
		PlaceGoal = false;
	}
	public void SaveState(string name)
	{
#if UNITY_EDITOR
		Level level = ScriptableObject.CreateInstance<Level>();
		level.Init(this);
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				if (GetTileAtIndex(x, y, 0) == null)
				{
					continue;
				}
				if(GetTileAtIndex(x, y, 0).HasGear())
				{
					level.Layer1Teeth[x + Width * y] = GetTileAtIndex(x, y, 0).Teeth;
					level.Layer1Torque[x + Width * y] = GetTileAtIndex(x, y, 0).MinTorque;
					level.Layer1Rpm[x + Width * y] = GetTileAtIndex(x, y, 0).MinRpm;
				}
				else
				{
					level.Layer1Teeth[x+Width * y] = 0;
					level.Layer1Torque[x + Width * y] = 0;
					level.Layer1Rpm[x + Width * y] = 0;
				}
				if (GetTileAtIndex(x, y, 1).HasGear())
				{
					level.Layer2Teeth[x + Width * y] = GetTileAtIndex(x, y, 1).Teeth;
					level.Layer2Torque[x + Width * y] = GetTileAtIndex(x, y, 1).MinTorque;
					level.Layer2Rpm[x + Width * y] = GetTileAtIndex(x, y, 1).MinRpm;
				}
				else
				{
					level.Layer2Teeth[x + Width * y] = 0;
					level.Layer2Torque[x + Width * y] = 0;
					level.Layer2Rpm[x + Width * y] = 0;
				}


			}
		}
		level.MotorPosition = MotorPosition;
		level.MotorTorque = MotorTorque;
		if(GetTileAtPosition(MotorPosition, 0))
		{
			level.MotorTeeth = GetTileAtPosition(MotorPosition, 0).Teeth;
		}
		else
		{
			level.MotorTeeth = 0;
		}
		
		level.GoalPositions = GoalPositions.ToArray();
		level.Width = Width;
		level.Height = Height;
		AssetDatabase.CreateAsset(level , $"Assets/Gears/Resources/Levels/{name}.asset");
#endif
	}

	public void LoadState(string name)
	{
		//Reformat level name
		_currentLevel = name;
		Level[] levels = Resources.LoadAll<Level>("Levels/");
		foreach (Level levelInstance in levels)
		{
			string levelName = levelInstance.name;
			int index = int.Parse(levelName.Split("_")[0]);
			if (index == int.Parse(name.Split("_")[0])){
				name = levelName;
				break;
			}
		}
		//Load Level
		Level level = Resources.Load<Level>($"Levels/{name}");
		MotorPosition = level.MotorPosition;
		GoalPositions = level.GoalPositions.ToList();
		ResizeGrid(level.Width,level.Height);
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				if (new Vector2(x, y).Equals(level.MotorPosition) && level.MotorTeeth != 0)
				{
					GetTileAtIndex(x, y, 0).NewMotor(level.MotorTeeth);
				}
				else
				{
					if (level.Layer1Teeth[x + Width * y] != 0)
					{
						if (level.Layer1Rpm == null || level.Layer1Rpm.Length == 0)
						{
							GetTileAtIndex(x, y, 0).NewGear(level.Layer1Teeth[x + Width * y], level.Layer1Torque[x + Width * y], 0, true);

						}
						else
						{
							GetTileAtIndex(x, y, 0).NewGear(level.Layer1Teeth[x + Width * y], level.Layer1Torque[x + Width * y], level.Layer1Rpm[x + Width * y], true);

						}
					}
					if (level.Layer2Teeth[x + Width * y] != 0)
					{
						if(level.Layer2Rpm == null || level.Layer2Rpm.Length == 0)
						{
							GetTileAtIndex(x, y, 1).NewGear(level.Layer2Teeth[x + Width * y], level.Layer2Torque[x + Width * y], 0, true);

						}
						else
						{
							GetTileAtIndex(x, y, 1).NewGear(level.Layer2Teeth[x + Width * y], level.Layer2Torque[x + Width * y], level.Layer2Rpm[x + Width * y], true);

						}
					}
				}
			}
		}
		MotorPosition = level.MotorPosition;
		MotorTorque = level.MotorTorque;
		GoalPositions = level.GoalPositions?.ToList();
		EnableLayer(0);
	}
	private void ResizeGrid(int width, int height)
	{
		Destroy(_level1Partent);
		Destroy(_level2Partent);
		MotorPosition = new(-1, -1);
		_level1Partent = new GameObject("TilesLevel1");
		_level2Partent = new GameObject("TilesLevel2");
		Width = width;
		Height = height;
		List<Vector2> tempList = new List<Vector2>(GoalPositions);
		foreach (Vector2 position in tempList)
        {
			if (position.x > Width || position.y > Height)
			{
				GoalPositions.Remove(position);
			}
		}
        
		GenerateGrid(0);
		GenerateGrid(1);
		LinkAdjacentTiles(0);
		LinkAdjacentTiles(1);
		EnableLayer(0);
	}
	
	public bool IncludeMachineEfficiency()
	{
		return applyMachineEfficiency;
	}
	private void Update()
	{
		
		if (_won)
		{
			_exitTimer++;
		}
		if (_exitTimer < _exitTime)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				EnableLayer(0);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				EnableLayer(1);
			}


			if (Input.GetKey(KeyCode.R) && _level1Partent.activeSelf && _level2Partent.activeSelf)
			{
				if (GetTileAtPosition(MotorPosition, 0) && GetTileAtPosition(MotorPosition, 0).Teeth != 0)
				{
					if (GoalPositions?.Count != 0)
					{
						foreach (Vector2 position in GoalPositions)
						{
							if (GetTileAtPosition(position, 0) == null)
							{
								continue;
							}
							GetTileAtPosition(position, 0).CurrentRotation = 0;

						}
					}
					foreach (Tile tile in Tile.RotatedTiles)
					{
						if (tile.HasGear())
						{
							tile.CurrentRotation = 0;
							tile.CurrentTorque = 0;
						}
					}
					Tile.RotatedTiles.Clear();
					if (MotorTorque == 0)
					{
						MotorTorque = 1;
					}
					GetTileAtPosition(MotorPosition, 0).Rotate(startRotationSpeed, MotorTorque);
					if (GoalPositions?.Count != 0)
					{
						bool allGoalsRotating = true;
						foreach (Vector2 position in GoalPositions)
						{
							if (GetTileAtPosition(position, 0) == null)
							{
								continue;
							}
							if (Mathf.Abs(GetTileAtPosition(position, 0).CurrentRotation) < GetTileAtPosition(position, 0).MinRpm || 
								GetTileAtPosition(position, 0).CurrentTorque < GetTileAtPosition(position, 0).MinTorque ||
								GetTileAtPosition(position, 0).CurrentRotation == 0)
							{
								allGoalsRotating = false;
							}
						}
						if (allGoalsRotating)
						{
							Debug.Log("Won");
							
							_won = true;
						}
					}
				}

			}
			else
			{
				foreach (Tile tile in Tile.RotatedTiles)
				{
					tile.ResetRotationGradient(Time.deltaTime);
				}
			}
		}
		else if(!_loading)
		{
			_loading = true;
			SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();
			string[] level = SceneLoader.LoadParameter.Split("_");
			string nextLevel = (int.Parse(level[0]) + 1) + "_" + level[1];
			SceneLoader.LoadParameter = nextLevel;
			sceneLoader.LoadNextScene("Gears", 0);
		}
		
		
		
	}
	public void RestartLevel()
	{
		LoadState(_currentLevel);
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
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity,parent);
				spawnedTile.name = $"Tile {x} {y}";
				spawnedTile.Layer = layer;
				spawnedTile.Canvas = _canvas;
				
				var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
				if (layer == 1)
				{
					isOffset = !isOffset;
				}
				spawnedTile.Init(isOffset,this);


				_tiles[layer][new Vector2(x, y)] = spawnedTile;
			}
		}
		
		_cam.transform.position = new Vector3((float)Width / 2 - 0.5f, (float)Height / 2 - 0.5f, -10);
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

	public void SetPlaceMotor(bool value)
	{
		PlaceMotor = value;
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
		if(i < Width - 1)
			tile.Add(GetTileAtIndex(i + 1, j, layer));
		if (j > 0)
			tile.Add(GetTileAtIndex(i, j - 1, layer));
		if(j < Height - 1)
			tile.Add(GetTileAtIndex(i, j + 1, layer));
		return tile;
	}
}