using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public GameObject Obj1, Obj2;
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, Obj1.transform.position);
        lineRenderer.SetPosition(1, Obj2.transform.position);
    }
    public GameObject TravelThrough(GameObject current,Energy energy)
    {
        if(current == Obj1)
        {
            return Obj2;
        }
        else
        {
            return Obj1;
        }
    }
	public GameObject GetOtherObject(GameObject current)
	{
		if (current == Obj1)
		{
			return Obj2;
		}
		else
		{
			return Obj1;
		}
	}
}
