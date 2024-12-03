using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampController : MonoBehaviour
{
    [SerializeField] private BoardObject _obj;
    private SpriteRenderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
		_renderer.color = Color.white;
	}

    // Update is called once per frame
    void Update()
    {
        if(_obj.Ampere > 0 && _obj.Vuse > 0)
        {
            _renderer.color = Color.yellow;
        }
        else
        {
            _renderer.color = Color.white;
        }
    }
}
