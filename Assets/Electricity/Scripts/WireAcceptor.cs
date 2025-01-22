using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireAcceptor : MonoBehaviour
{
	[SerializeField] private BoardObject parent;
	[SerializeField] private bool _isPositive;
	public void AddWire(Wire wire)
	{
		if (_isPositive)
		{
			parent.PlusConnections.Add(wire);
		}
		else
		{
			parent.MinusConnections.Add(wire);

		}
	}
	public bool IsPositive()
	{
		return _isPositive;
	}
	public void RemoveWire(Wire wire)
	{
		if (_isPositive)
		{
			parent.PlusConnections.Remove(wire);
		}
		else
		{
			parent.MinusConnections.Remove(wire);

		}
	}
}
