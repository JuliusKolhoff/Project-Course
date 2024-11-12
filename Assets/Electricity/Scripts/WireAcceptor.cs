using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireAcceptor : MonoBehaviour
{
	[SerializeField] private BoardObject parent;
	[SerializeField] private bool isPositive;
	public void AddWire(Wire wire)
	{
		if (isPositive)
		{
			parent.plusConnections.Add(wire);
		}
		else
		{
			parent.minusConnections.Add(wire);

		}
	}
}
