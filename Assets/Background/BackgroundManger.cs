using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManger : MonoBehaviour
{
	[SerializeField] private int _width, _height;
	[SerializeField] private float _distance;
	[SerializeField] private bool _skipOddTiles, _setCameraBackground;
	[SerializeField] private BackgroundIcon _iconPrefab;
	[SerializeField] private Sprite _iconSprite;
	[SerializeField] private Color _iconColor;

	void Start()
    {
        if (_setCameraBackground)
        {
			Camera.main.backgroundColor = _iconColor;
		}
		GenerateIconGrid();
	}

	void GenerateIconGrid()
	{
		for (int x = -_width; x <= _width; x++)
		{
			for (int y = -_height; y <= _height; y++)
			{
				if(((x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0)) && _skipOddTiles)
                {
					continue;
                }
				var spawnedIcon = Instantiate(_iconPrefab, new Vector3(x * _distance, y * _distance), Quaternion.identity);
				spawnedIcon.name = $"Tile {x} {y}";
				spawnedIcon.Init(_iconSprite, _iconColor);
			}
		}
	}
}
