using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Arrow : MonoBehaviour
{
    [Header("Gameplay")]
    public float maxStemLength = 30f;

    private float stemLength;
    [Header("Triangle Renderer")]
    public float stemWidth;
    public float tipLength;
    public float tipWidth;

    [System.NonSerialized]
    public List<Vector3> verticesList;
    [System.NonSerialized]
    public List<int> trianglesList;

    Mesh mesh;

    void Start()
    {
        //Make sure Mesh Renderer has a material
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        MoveToMouse();
        GenerateArrow();
    }

    void MoveToMouse()
    {
        //Get mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        //Get difference
        Vector3 objectPos = transform.position;
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        //Set desired angle
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //Set desired length
        float distance = Mathf.Sqrt(Mathf.Pow(mousePos.x,2) + Mathf.Pow(mousePos.y,2));
        if(distance < maxStemLength)
        {
            stemLength = distance;
        } else
        {
            stemLength = maxStemLength;
        }
    }

    void GenerateArrow()
    {
        //Setup
        verticesList = new List<Vector3>();
        trianglesList = new List<int>();

        //Stem setup
        Vector3 stemOrigin = Vector3.zero;
        float stemHalfWidth = stemWidth / 2f;

        //Stem points
        verticesList.Add(stemOrigin);
        verticesList.Add(verticesList[0] + (stemLength * Vector3.right) + (stemHalfWidth * Vector3.up));
        verticesList.Add(verticesList[0] + (stemLength * Vector3.right) + (stemHalfWidth * Vector3.down));

        //Stem triangles
        trianglesList.Add(0);
        trianglesList.Add(1);
        trianglesList.Add(2);

        //Tip setup
        Vector3 tipOrigin = stemLength * Vector3.right;
        float tipHalfWidth = tipWidth / 2;

        //Tip points
        verticesList.Add(tipOrigin + (tipHalfWidth * Vector3.up));
        verticesList.Add(tipOrigin + (tipHalfWidth * Vector3.down));
        verticesList.Add(tipOrigin + (tipLength * Vector3.right));

        //Tip triangle
        trianglesList.Add(3);
        trianglesList.Add(5);
        trianglesList.Add(4);

        //Assign lists to mesh.
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
    }

    public float GetAngle()
    {
        return Mathf.Deg2Rad * transform.eulerAngles.z;
    }

    public float GetLength()
    {
        return stemLength;
    }
}