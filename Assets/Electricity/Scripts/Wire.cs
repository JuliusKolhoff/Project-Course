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
        lineRenderer.positionCount = 4;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 start = Obj1.transform.position;
        Vector3 end = Obj2.transform.position;

        WireAcceptor startAcceptor = Obj1.GetComponent<WireAcceptor>();
        WireAcceptor endAcceptor = Obj2.GetComponent<WireAcceptor>();

        float obj1Rotation = Obj1.GetComponent<WireAcceptor>().GetParent().transform.eulerAngles.z;
        float obj2Rotation = Obj2.GetComponent<WireAcceptor>().GetParent().transform.eulerAngles.z;

        Vector3 obj1direction = RotationToDirection(obj1Rotation) * (startAcceptor.IsPositive() ? 1 : -1);
		Vector3 obj2direction = RotationToDirection(obj2Rotation) * (endAcceptor.IsPositive() ? 1 : -1);

		Vector3 intermediate1Pos = start + obj1direction * .5f;
		Vector3 intermediate2Pos = end + obj2direction * .5f;
		
        Vector3 newIntermediate1Pos = intermediate1Pos + obj1direction * 0.05f;
		Vector3 newIntermediate2Pos = intermediate2Pos + obj2direction * 0.05f;
        while (Vector3.Distance(intermediate1Pos, intermediate2Pos) > Vector3.Distance(newIntermediate1Pos, intermediate2Pos) || Vector3.Distance(intermediate2Pos, intermediate1Pos) > Vector3.Distance(newIntermediate2Pos, intermediate1Pos))
        {
            if (Vector3.Distance(intermediate1Pos, intermediate2Pos) > Vector3.Distance(newIntermediate1Pos, intermediate2Pos))
            {
                intermediate1Pos = newIntermediate1Pos;
                newIntermediate1Pos = intermediate1Pos + obj1direction * 0.05f;
            }
            if (Vector3.Distance(intermediate2Pos, intermediate1Pos) > Vector3.Distance(newIntermediate2Pos, intermediate1Pos))
            {
                intermediate2Pos = newIntermediate2Pos;
                newIntermediate2Pos = intermediate2Pos + obj2direction * 0.05f;
            }
        }
		

        if(Vector3.Distance(intermediate1Pos, intermediate2Pos) < 0.1f)
        {
            intermediate2Pos = intermediate1Pos;
        }
		// Set the line points
		lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, intermediate1Pos);
        lineRenderer.SetPosition(2, intermediate2Pos);
        lineRenderer.SetPosition(3, end);
    }
    public GameObject TravelThrough(GameObject current, Energy energy)
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
    public GameObject GetOtherObject(GameObject current)
    {
        if (current == Obj1)
        {
            return Obj2;
        }
        else if (current == Obj2)
        {
            return Obj1;
        }
        else
        {
            Debug.Log("Gameobject not in connection");
            return null;
        }
    }
    public Vector3 RotationToDirection(float rotation) => (Mathf.Abs(rotation) % 360) switch
    {
        0 => Vector3.left,
        90 => Mathf.Sign(rotation) * Vector3.down,
        180 => Vector3.right,
        270 => Mathf.Sign(rotation) * Vector3.up,
        _ => Vector3.left,
    };
}
