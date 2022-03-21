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
	[Header("Other Components")]
	[SerializeField] private TrialGenerator _trialGenerator;
	[SerializeField] private ShowMan _showMan;
	[SerializeField] private LogNBackToUXF _log;
	
	[Header("Trial Settings")]
	[Range(1, 10)] public int _nBack = 1;
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
		
		try
		{
			var trialCt = trialCts.Token;
			var keyPressed = WaitForKeyPress(trialCt);
			var trialDuration = Delayer(trialCt, _trialDuration);
			var finishedTrial = await Task.WhenAny(keyPressed, trialDuration);
			await finishedTrial;
			trialCts.Cancel();
			// var keyWasPressed = await TimedKeyboardWait(trialCts, _trialDuration);

			_showMan.ClearTextField();

			if (keyPressed.IsCanceled == false)
			{
				if (keyPressed.Result == true)
				{
					if (trial.number <= _nBack)
					{
						_showMan.InCorrectResponse();
						_log.Wrong();
					}
					else if (_current == _previousCharacters[_nBack - 1])
					{
						_showMan.CorrectResponse();
						_log.CorrectHit();
					}
					else
					{
						_showMan.InCorrectResponse();
						_log.Wrong();
					}
				}
				else
				{
					if (trial.number <= _nBack)
					{
						_showMan.CorrectNoResponse();
						_log.CorrectNoResponse();
					}
					else if (_current == _previousCharacters[_nBack - 1])
					{
						_showMan.MissedResponse();
						_log.Missed();
					}
					else
					{
						_showMan.CorrectNoResponse();
						_log.CorrectNoResponse();
					}
				}
			}


			_interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");
			await Delayer(trialCt, _interTrialInterval);

			if (_previousCharacters == null || _previousCharacters.Count == 0)
			{
				_previousCharacters = new List<string>();
			}

			_previousCharacters.Insert(0, _current);

			//trialCts.Cancel();
		}
		finally
		{
			trialCts.Dispose();
		}
		
		trial.session.EndCurrentTrial();
	}

	

	private async Task Delayer(CancellationToken ct, float seconds)
	{
		try
		{
			await Task.Delay((int) (seconds * 1000), ct);
		}
		finally
		{
			//cts?.Cancel();
			//cts?.Dispose();
		}
	}
	

	private async Task<bool> WaitForKeyPress(CancellationToken ct)
	{
		while (!Input.GetKeyDown(KeyCode.Space) && ct.IsCancellationRequested == false)
		{
			await Task.Yield();
		}
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			//ct?.Cancel();
			return true;
		}
		
		//if (cts.IsCancellationRequested)
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
		else if (CanUseNBack(trial) == true && IsNBackTrial(chance) == true) // && _previousCharacters.Count > _nBack)
		{
			current = _previousCharacters[_nBack - 1];
		}
		else if (CanUseNBack(trial) == true && IsNBackTrial(chance) == false) // && _previousCharacters.Count > _nBack)
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
		if (_previousCharacters != null && _previousCharacters.Count > 0)
		{
			_previousCharacters.Reverse();
		}
	
		//_cts?.Cancel();
		//_cts?.Dispose();
	}
}