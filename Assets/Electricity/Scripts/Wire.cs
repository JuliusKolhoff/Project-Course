using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
		if(obj1direction == obj2direction)
        {
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
		}else if (obj1direction == -obj2direction)
        {
            //Connection "look" at each other
            if(Vector3.Distance(intermediate1Pos, intermediate2Pos) > Vector3.Distance(intermediate1Pos + obj1direction * 0.0005f, intermediate2Pos))
            {
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
				SetLinePositions(start, intermediate1Pos, intermediate2Pos, end);
			}
            else // Look away from eachother
            {
				//TODO ANDereres Verhalten falls der Anfang zwischen Ende und anderem pol des 2. Objektes ist
				GameObject otherObject;
				Vector3 intermediateHalfPos;
				Vector3 intermediateHalf2Pos;
				Vector3 offsetDirection = RotationToDirection(obj1Rotation + 90) * (startAcceptor.IsPositive() ? 1 : -1);
                //obj2 is in offset direction from obj1 -> offset from obj1 to obj 2
                if(Vector3.Distance(start, end) > Vector3.Distance(start + offsetDirection * 0.0001f, end))
				{
					Vector3 obj1pos = start + obj1direction * 0.5f;
					Vector3 endPos = end + obj2direction * .5f;
					otherObject = endAcceptor.IsPositive() ? endAcceptor.GetParent().GetComponent<BoardObject>().Minus : endAcceptor.GetParent().GetComponent<BoardObject>().Plus;
					Vector3 otherObjPosition = otherObject.transform.position - obj2direction;
					// Other object y must be between the two ends
					if (offsetDirection.x != 0 && ((obj1pos.y < otherObjPosition.y && obj1pos.y > endPos.y) || (obj1pos.y > otherObjPosition.y && obj1pos.y < endPos.y)))
					{
						Debug.Log("Case x");
					}
					else if (offsetDirection.y != 0 && ((obj1pos.x < otherObjPosition.x && obj1pos.x > endPos.x) || (obj1pos.x > otherObjPosition.x && obj1pos.x < endPos.x)))
					{
						Debug.Log("Case y: " + obj1pos.x + "; right End: " + otherObjPosition.x + ";Left End: " + endPos.x);
					}
					else
					{
						intermediateHalfPos = intermediate1Pos + offsetDirection * 0.75f;
						intermediateHalf2Pos = intermediate2Pos + offsetDirection * 0.75f;
						Vector3 newIntermediateHalfPos = intermediateHalfPos + offsetDirection * 0.05f;
						while (Vector3.Distance(intermediateHalfPos, intermediateHalf2Pos) > Vector3.Distance(newIntermediateHalfPos, intermediateHalf2Pos))
						{
							if (Vector3.Distance(intermediateHalfPos, intermediateHalf2Pos) > Vector3.Distance(newIntermediateHalfPos, intermediateHalf2Pos))
							{
								intermediateHalfPos = newIntermediateHalfPos;
								newIntermediateHalfPos = intermediateHalfPos + offsetDirection * 0.05f;
							}

						}
						SetLinePositions(start, intermediate1Pos, intermediateHalfPos, intermediateHalf2Pos, intermediate2Pos, end);
					}



					
				}
                else
                {
					Vector3 obj1pos = start + obj1direction * 0.5f;
					Vector3 endPos = end + obj2direction * .5f;
					otherObject = endAcceptor.IsPositive() ? endAcceptor.GetParent().GetComponent<BoardObject>().Minus : endAcceptor.GetParent().GetComponent<BoardObject>().Plus;
					Vector3 otherObjPosition = otherObject.transform.position - obj2direction;
					// Other object y must be between the two ends
					if (offsetDirection.x != 0 && ((obj1pos.y < otherObjPosition.y && obj1pos.y > endPos.y) || (obj1pos.y > otherObjPosition.y && obj1pos.y < endPos.y)))
					{
						Debug.Log("Case x");
					}
					else if (offsetDirection.y != 0 && ((obj1pos.x < otherObjPosition.x && obj1pos.x > endPos.x) || (obj1pos.x > otherObjPosition.x && obj1pos.x < endPos.x)))
					{
						Debug.Log("Case y: " + obj1pos.x + "; right End: " + otherObjPosition.x + ";Left End: " + endPos.x);
					}
					else
					{
						intermediateHalfPos = intermediate2Pos + offsetDirection * 0.75f;
						intermediateHalf2Pos = intermediate1Pos + offsetDirection * 0.75f;
						Vector3 newIntermediateHalfPos = intermediateHalfPos + offsetDirection * 0.05f;
						while (Vector3.Distance(intermediateHalfPos, intermediateHalf2Pos) > Vector3.Distance(newIntermediateHalfPos, intermediateHalf2Pos))
						{
							if (Vector3.Distance(intermediateHalfPos, intermediateHalf2Pos) > Vector3.Distance(newIntermediateHalfPos, intermediateHalf2Pos))
							{
								intermediateHalfPos = newIntermediateHalfPos;
								newIntermediateHalfPos = intermediateHalfPos + offsetDirection * 0.05f;
							}
						}
						SetLinePositions(start, intermediate1Pos, intermediateHalf2Pos, intermediateHalfPos, intermediate2Pos, end);
					}
				}
					
			}
            
        }
        else//Komplett andere Rotation oben und rechts oder so
        {
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
			SetLinePositions(start, intermediate1Pos, intermediate2Pos, end);
		}
       
		

       
    }
    private void SetLinePositions(Vector3 start, Vector3 intermediate1Pos, Vector3 intermediate2Pos, Vector3 end)
    {
		if (Vector3.Distance(intermediate1Pos, intermediate2Pos) < 0.1f)
		{
			intermediate2Pos = intermediate1Pos;
		}
		// Set the line points
		lineRenderer.positionCount = 4;
		lineRenderer.SetPosition(0, start);
		lineRenderer.SetPosition(1, intermediate1Pos);
		lineRenderer.SetPosition(2, intermediate2Pos);
		lineRenderer.SetPosition(3, end);
	}
	private void SetLinePositions(Vector3 start, Vector3 intermediate1Pos, Vector3 intermediate2Pos,Vector3 intermediate3Pos, Vector3 intermediate4Pos, Vector3 end)
	{
		// Set the line points
		lineRenderer.positionCount = 6;
		lineRenderer.SetPosition(0, start);
		lineRenderer.SetPosition(1, intermediate1Pos);
		lineRenderer.SetPosition(2, intermediate2Pos);
		lineRenderer.SetPosition(3, intermediate3Pos);
		lineRenderer.SetPosition(4, intermediate4Pos);
		lineRenderer.SetPosition(5, end);
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
