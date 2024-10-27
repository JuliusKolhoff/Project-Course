using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject
{
	public void Init(GridManager manager)
	{
		Layer1Teeth = new float[manager.Width + manager.Width * manager.Height];
		Layer1Torque = new float[manager.Width + manager.Width * manager.Height];
		Layer2Teeth = new float[manager.Width + manager.Width * manager.Height];
		Layer2Torque = new float[manager.Width + manager.Width * manager.Height];
	}
	public float[] Layer1Teeth;
	public float[] Layer1Torque;
	public float[] Layer2Teeth;
	public float[] Layer2Torque;
	public Vector2 MotorPosition;
	public float MotorTeeth;
	public Vector2 GoalPosition;
	public int Width, Height;
}
