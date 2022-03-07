using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UXF;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


public class TrialManager : MonoBehaviour
{
	[SerializeField] private TrialGenerator _trialGenerator;
	[SerializeField] private ShowMan _showMan;
	[Range(0, 10)] public int _nBack = 1;
	[Range(0, 100)] public int PercentageNLikely = 60;
	
	private float _trialDuration = 2f;
	private float _interTrialInterval = 0.25f;

	private List<string> _previousCharacters;
	private string _current;
	
	private CancellationTokenSource _cts = new();
	

	private void Start()
	{
		_cts = new CancellationTokenSource();
	}


	public async void StartTrial(Trial trial)
	{
		await TrialRunner(trial);
	}


	private async Task TrialRunner(Trial trial)
	{
		_current = GetWeighedRandomCharacter(trial);

		_showMan.DisplayVariable(_current);

		_trialDuration = trial.block.settings.GetFloat("trialDuration");
		
		var trialCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
		var keyWasPressed = await TimedKeyboardWait(trialCts, _trialDuration);

		_showMan.ClearTextField();

		if (keyWasPressed)
		{
			if (trial.number <= _nBack)
			{
				_showMan.InCorrectResponse();
			}
			else if (_current == _previousCharacters[_nBack -1])
			{
				_showMan.CorrectResponse();
			}
			else
			{
				_showMan.InCorrectResponse();
			}
		}
		else
		{
			if (trial.number <= _nBack)
			{
				_showMan.CorrectResponse();
			}
			else if (_current == _previousCharacters[_nBack -1])
			{
				_showMan.MissedResponse();
			}
			else
			{
				_showMan.CorrectResponse();
			}
		}

		_interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");
		await Delayer(_cts, _interTrialInterval);

		_previousCharacters.Insert(0, _current);
		
		trial.session.EndCurrentTrial();
	}

	private static async Task<bool> TimedKeyboardWait(CancellationTokenSource cts, float duration)
	{
		try
		{
			cts.CancelAfter((int) (duration * 1000));
			
			return await WaitForKeyPress(cts); 
		}
		catch (OperationCanceledException e)
		{
			Debug.Log(e);
			throw;
		}
		finally
		{
			cts.Dispose();
		}
	}
	

	private static async Task Delayer(CancellationTokenSource cts, float seconds)
	{
		await Task.Delay((int)(seconds * 1000), cts.Token);
	}
	

	private static async Task<bool> WaitForKeyPress(CancellationTokenSource cts)
	{
		while (!Input.GetKeyDown(KeyCode.Space) && cts.IsCancellationRequested == false)
		{
			await Task.Yield();
		}
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			cts.Cancel();
			return true;
		}
		
		if (cts.IsCancellationRequested)
		{
			// Debug.Log("No key was hit");
		}

		return false;
	}
	


	private string GetWeighedRandomCharacter(Trial trial)
	{
		string current = null;
		var chance = Random.Range(0, 101);
		
		if (CanUseNBack(trial) == false)
		{
			current = _trialGenerator._characterList[Random.Range(0, _trialGenerator._characterList.Count)];
		}
		else if (CanUseNBack(trial) == true && IsNBackTrial(chance) == true)
		{
			current = _previousCharacters[_nBack - 1];
		}
		else if (CanUseNBack(trial) == true && IsNBackTrial(chance) == false)
		{
			var excludedList = _trialGenerator._characterList.Where(character => character != _previousCharacters[_nBack - 1]).ToList();
			current = excludedList[Random.Range(0, excludedList.Count)];
		}
		else
		{
			Debug.LogErrorFormat("This shouldn't happen. CanUseNBack == {0}, IsNBackTrial == {1}", CanUseNBack(trial), IsNBackTrial(chance));
		}
		
		return current;
	}


	private bool CanUseNBack(Trial trial)
	{
		return trial.number > _nBack;
	}


	private bool IsNBackTrial(int chance)
	{
		return chance < PercentageNLikely;
	}


	private void OnDisable()
	{
		_previousCharacters.Reverse();
		_cts.Cancel();
	}
}

/*
 *	private async void WorkingAsyncCaller()
	{
		var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
		var finished = await Task.WhenAny(WaitForKeyPress(linkedCts.Token), Delayer1(linkedCts.Token));
		await finished;
		linkedCts.Cancel();
		linkedCts.Dispose();
		_cts = new CancellationTokenSource();
	}
 *
 *
 * 	private async Task Delayer1(CancellationToken ct)
	{
		await Task.Delay(5000, ct);
		Debug.Log("Time's up!");
	}
 *
 *
 */
