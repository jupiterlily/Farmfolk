using UnityEngine;

public class SimulationSpeedController : MonoBehaviour
{
	     public float unboundedTime = 1000;
	     private void Start()
	     {
		          SetSimulationSpeed(1);
	     }
	     void Update()
	     {
		          if (Input.GetKeyDown(KeyCode.Alpha1))
				SetSimulationSpeed(1);
		          if (Input.GetKeyDown(KeyCode.Alpha2))
				SetSimulationSpeed(2);
		          if (Input.GetKeyDown(KeyCode.Alpha3))
				SetSimulationSpeed(3);
		          if (Input.GetKeyDown(KeyCode.Alpha4))
				SetSimulationSpeed(4);
		          if (Input.GetKeyDown(KeyCode.Alpha5))
				SetSimulationSpeed(5);
		          if (Input.GetKeyDown(KeyCode.Alpha6))
				SetSimulationSpeed(6);
		          if (Input.GetKeyDown(KeyCode.Alpha7))
				SetSimulationSpeed(7);
		          if (Input.GetKeyDown(KeyCode.Alpha8))
				SetSimulationSpeed(8);
		          if ( Input.GetKeyDown(KeyCode.Alpha9))
				SetSimulationSpeed(9);
		          if (Input.GetKeyDown(KeyCode.Alpha0))
				SetSimulationSpeed(10);
	     }
	     
	     void SetSimulationSpeed(int speed)
	     {
		          float minuteinseconds = 60;
		          float hourinseconds = minuteinseconds * 60 ;
		          float dayinseconds = hourinseconds*24;
		          switch (speed)
		          {
		          case 1:
		          SimulationConstants.SimulationSpeed = dayinseconds / ((SimulationConstants.DayLength*2)*minuteinseconds); // 1 day in 48 minutes
		          Debug.Log("<color=#6667ab>simulation speed 1</color>");
		          break;
		          case 2:
		          SimulationConstants.SimulationSpeed = dayinseconds / (SimulationConstants.DayLength * minuteinseconds); // 1 day in 24 minutes
		          Debug.Log("<color=#f18aad>simulation speed 2</color>");
		          break;
		          case 3:
		          SimulationConstants.SimulationSpeed = dayinseconds / ((SimulationConstants.DayLength / 4) * minuteinseconds); // 1 day in 6 minutes
		          Debug.Log("<color=#ea6759>simulation speed 3</color>");
		          break;
		          case 4:
		          SimulationConstants.SimulationSpeed = dayinseconds / ((SimulationConstants.DayLength / 8) * minuteinseconds); // 1 day in 3 minutes
		          Debug.Log("<color=#f88f58>simulation speed 4</color>");
		          break;
		          case 5:
		          SimulationConstants.SimulationSpeed = dayinseconds / (minuteinseconds * 1.5f); // 1 day in 90 seconds
		          Debug.Log("<color=#f3c65f>simulation speed 5</color>");
		          break;
		          case 6:
		          SimulationConstants.SimulationSpeed = dayinseconds / (minuteinseconds * 0.75f); // 1 day in 45 seconds
		          Debug.Log("<color=#8bc28c>simulation speed 6</color>");
		          break;
		          case 7:
		          SimulationConstants.SimulationSpeed = dayinseconds / (minuteinseconds / 4); // 1 day in 15 seconds
		          Debug.Log("<color=#5e36cc>simulation speed 7</color>");
		          break;
		          case 8:
		          SimulationConstants.SimulationSpeed = dayinseconds * (SimulationConstants.YearLength / 12); // 1 month in 1 seconds
		          Debug.Log("<color=#5e36cc>simulation speed 8</color>");
		          break;
		          case 9:
		          SimulationConstants.SimulationSpeed = dayinseconds * (SimulationConstants.YearLength / 5); // 1 year in 5 seconds
		          Debug.Log("<color=#5e36cc>simulation speed 9</color>");
		          break;
		          case 10:
		          SimulationConstants.SimulationSpeed = dayinseconds * unboundedTime; // 1000 days per second
		          Debug.Log("<color=#5e36cc>simulation speed 10</color>");
		          break;
		          }
	     }
}
