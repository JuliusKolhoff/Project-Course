using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
	public List<Wire> plusConnections = new();
	public List<Wire> minusConnections = new();

	public float Resistance;
	public GameObject _plus, _minus;
}
