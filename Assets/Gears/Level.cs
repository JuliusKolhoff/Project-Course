using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject
{
	public float[] Layer1Teeth = new float[GridManager.Width + GridManager.Width * GridManager.Height];
	public float[] Layer1Torque = new float[GridManager.Width + GridManager.Width * GridManager.Height];
	public float[] Layer2Teeth = new float[GridManager.Width + GridManager.Width * GridManager.Height];
	public float[] Layer2Torque = new float[GridManager.Width + GridManager.Width * GridManager.Height];
	public float[] MotorPosition = new float[2];
	public float[] GloatPosition = new float[2];
}
