using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UXFExamples;


public class TimeKeeper
{
	protected CancellationToken _linkedCt;


	public TimeKeeper(CancellationToken linkedToken)
	{
		_linkedCt = linkedToken;
	}

	public async Task Delayer(float seconds)
	{
		Debug.Log("No time");

		await Task.Delay((int)(seconds * 1000), _linkedCt);

		Debug.LogError("TIME UP");
	}


	public async Task WaitForOtherTask(Task task)
	{
		while (!task.IsCompleted && _linkedCt.IsCancellationRequested == false)
		{
			await Task.Yield();
		}
	}


}


public class KeyKeeper
{
	protected CancellationToken _linkedCt;


	public KeyKeeper(CancellationToken linkedToken)
	{
		_linkedCt = linkedToken;
	}

	public async Task<bool> WaitForKey(KeyCode keyCode)
	{
		while (!Input.GetKeyDown(keyCode) && _linkedCt.IsCancellationRequested == false)
		{
			Debug.Log("No Key");
			await Task.Yield();
		}

		Debug.LogError("WE KEYED A THING");
		return true;
	}



}


