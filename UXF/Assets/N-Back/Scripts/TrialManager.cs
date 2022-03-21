using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UXF;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


public class TrialManager : MonoBehaviour
{
	[Header("Other Components")]
	[SerializeField] private TrialGenerator TrialGenerator;
	[SerializeField] private ShowMan ShowMan;
	[SerializeField] private LogNBackToUXF Log;
	
	[Header("Trial Settings")]
	[Range(1, 10)] public int NBack = 1;
	[Range(0, 100)] public int PercentageNLikely = 60;
	
	private float _trialDuration = 2f;
	private float _interTrialInterval = 0.25f;

	private List<string> _previousCharacters;
	private string _current;
	
	private Coroutine _keyPressedCR;
	private Coroutine _trialDurationCR;


	public void StartTrial(Trial trial)
	{ 
		StartCoroutine(TrialRunner(trial));
	}


	private IEnumerator TrialRunner(Trial trial)
	{
		_current = GetWeighedRandomCharacter(trial);

		ShowMan.DisplayVariable(_current);

		_trialDuration = trial.block.settings.GetFloat("trialDuration");

		_keyPressedCR = StartCoroutine(WaitForKeyPress(trial));
		_trialDurationCR = StartCoroutine(WaitForTaskDuration(trial, _trialDuration));

		while (_keyPressedCR != null && _trialDurationCR != null)
		{
			yield return null;
		}

		_interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");

		yield return new WaitForSeconds(_interTrialInterval);
		
		if (_previousCharacters == null || _previousCharacters.Count == 0)
		{
			_previousCharacters = new List<string>();
		}

		_previousCharacters.Insert(0, _current);

		trial.session.EndCurrentTrial();
	}

	private IEnumerator WaitForTaskDuration(Trial trial, float seconds)
	{
		yield return new WaitForSeconds(seconds);

		if (_keyPressedCR != null)
		{
			StopCoroutine(_keyPressedCR);
		}
		
		if (trial.number <= NBack)
		{
			ShowMan.CorrectNoResponse();
			Log.CorrectNoResponse();
		}
		else if (_current == _previousCharacters[NBack - 1])
		{
			ShowMan.MissedResponse();
			Log.Missed();
		}
		else
		{
			ShowMan.CorrectNoResponse();
			Log.CorrectNoResponse();
		}
		
		_trialDurationCR = null;
	}

	
	private IEnumerator WaitForKeyPress(Trial trial)
	{
		while (!Input.GetKeyDown(KeyCode.Space))
		{
			yield return null;
		}

		if (_trialDurationCR != null)
		{
			StopCoroutine(_trialDurationCR);
		}

		if (trial.number <= NBack)
		{
			ShowMan.InCorrectResponse();
			Log.Wrong();
		}
		else if (_current == _previousCharacters[NBack - 1])
		{
			ShowMan.CorrectResponse();
			Log.CorrectHit();
		}
		else
		{
			ShowMan.InCorrectResponse();
			Log.Wrong();
		}
			
		_keyPressedCR = null;
	}


	private string GetWeighedRandomCharacter(Trial trial)
	{
		string current = null;
		var chance = Random.Range(0, 101);
		
		if (CanUseNBack(trial) == false)
		{
			current = TrialGenerator._characterList[Random.Range(0, TrialGenerator._characterList.Count)];
		}
		else if (CanUseNBack(trial) == true && IsNBackTrial(chance) == true) // && _previousCharacters.Count > _nBack)
		{
			current = _previousCharacters[NBack - 1];
		}
		else if (CanUseNBack(trial) == true && IsNBackTrial(chance) == false) // && _previousCharacters.Count > _nBack)
		{
			var excludedList = TrialGenerator._characterList.Where(character => character != _previousCharacters[NBack - 1]).ToList();
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
		return trial.number > NBack;
	}


	private bool IsNBackTrial(int chance)
	{
		return chance < PercentageNLikely;
	}


	private void OnDisable()
	{
		if (_previousCharacters is {Count: > 0})
		{
			_previousCharacters.Reverse();
		}
		
		StopAllCoroutines();
	}
}