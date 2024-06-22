using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 initialPosition;
    [SerializeField] private GameObject arrow;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            transform.position = initialPosition;
            rb.simulated = !rb.simulated;
            if (rb.simulated)
            {
                arrow.SetActive(false);
                Arrow a = arrow.GetComponent<Arrow>();
                rb.velocity = new Vector2((float) Mathf.Cos(a.GetAngle()), (float) Mathf.Sin(a.GetAngle())) * a.GetLength() * 3;
            }
            else
            {
                arrow.SetActive(true);
            }
        }
    }
}
