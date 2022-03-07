using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UXFExamples;


public class TimeKeeper
{
	protected CancellationTokenSource _linkedCt;


	public TimeKeeper(CancellationTokenSource linkedToken)
	{
		_linkedCt = linkedToken;
	}

	public async Task Delayer(float seconds)
	{
		Debug.Log("No time");
		
		await Task.Delay((int)(seconds * 1000), _linkedCt.Token);
		
		_linkedCt.Cancel();
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
	protected CancellationTokenSource _linkedCt;


	public KeyKeeper(CancellationTokenSource linkedToken)
	{
		_linkedCt = linkedToken;
	}

	public async Task<bool> WaitForKey(KeyCode keyCode)
	{
		while (!Input.GetKeyDown(keyCode) && _linkedCt.IsCancellationRequested == false)
		{
			try
			{
				Debug.Log("No Key");
				await Task.Yield();
			}
			catch (OperationCanceledException e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
		
		_linkedCt.Cancel();
		Debug.LogError("WE KEYED A THING");
		return true;
	}

	public async Task<bool> WaitForKey2(KeyCode keyCode)
	{
		var keyTask = Task.Run(() =>
		{
			while (!Input.GetKeyDown(keyCode))
			{
				Debug.Log("No key");
			}
			
			Debug.Log("We keyed a thing");
			_linkedCt.Cancel();
		});

		await keyTask;
		return true;
	}


}


