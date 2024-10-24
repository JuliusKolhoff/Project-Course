using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject
{
	public float[] Layer1Teeth = new float[GridManager.Width + GridManager.Width * GridManager.Height];
	public float[] Layer1Torque = new float[GridManager.Width + GridManager.Width * GridManager.Height];
	public float[] Layer2Teeth = new float[GridManager.Width + GridManager.Width * GridManager.Height];
	public float[] Layer2Torque = new float[GridManager.Width + GridManager.Width * GridManager.Height];
	public Vector2 MotorPosition;
	public float MotorTeeth;
	public Vector2 GoalPosition;
}
