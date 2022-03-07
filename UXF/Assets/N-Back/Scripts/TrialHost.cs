using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UXF;
using Object = System.Object;
using Random = UnityEngine.Random;


public class TrialHost : MonoBehaviour
{
	/*
	[SerializeField] private ShowMan _showMan;
	[SerializeField] private TrialGenerator _trialGenerator;
	private TimeKeeper _timeKeeper;
	private KeyKeeper _keyKeeper;
	[SerializeField] private int nBack;
	[SerializeField] private int PercentageNLikely;
	private string _current;
	[SerializeField] private List<string> _previousCharacters;

	[SerializeField] private float _trialDuration = 1f;
	[SerializeField] private float _interTrialInterval = 1f;


	private void Awake()
	{


	}


	private void Start()
	{
		_previousCharacters = new List<string>(new string[ _trialGenerator.numberofTrials]);
	}


	public async void Trials(Trial trial)
	{
		await Trialling(trial);
	}


	private async Task Trialling(Trial trial)
	{
		_current = GetWeighedRandomCharacter(trial);
		_showMan.DisplayText(_current);
		_previousCharacters[trial.number - 1] = _current;


		var cts = new CancellationTokenSource();
		var linked = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
		_timeKeeper = new TimeKeeper(linked);
		_keyKeeper = new KeyKeeper(linked);

		Task delayed = null;
		Debug.LogFormat("1");
		Task<bool> keyer;
		Debug.LogFormat("2"); 
		//var trialFinished = await Task.WhenAny(delayed = _timeKeeper.Delayer(_trialDuration), keyer = _keyKeeper.WaitForKey(KeyCode.Space));
		//var trialFinished = await Task.WhenAny(_timeKeeper.Delayer(_trialDuration));
		var completedTasks = await Task.WhenAny(keyer = _keyKeeper.WaitForKey2(KeyCode.Space), _timeKeeper.Delayer(_trialDuration));
		Debug.LogFormat("3");
		
		Debug.LogFormat("4");
		linked.Cancel();
		Debug.LogFormat("5");
		linked.Dispose();
		Debug.LogFormat("6");
		
		Debug.LogFormat("7");
		var keypressed = keyer.Result;

		Debug.LogFormat("8");
		_showMan.DisplayText("");

		if (keypressed == true)
		{
			_showMan.PlayParticles();
		}

		

		trial.session.EndCurrentTrial();
		Debug.LogFormat("9");
	}



	private string GetWeighedRandomCharacter(Trial trial)
	{
		if (CanUseNBack(trial) == false)
		{
			return _trialGenerator._characterList[Random.Range(0, _trialGenerator._characterList.Count)];
		}

		if (IsNBackTrial() == true)
		{
			return _previousCharacters[^nBack];
		}

		List<string> excludedList = new();
		excludedList.AddRange(_trialGenerator._characterList.Where(characters => characters != _previousCharacters[^nBack]));
		var current = excludedList[Random.Range(0, excludedList.Count)];
		return current;
	}


	private bool CanUseNBack(Trial trial)
	{
		return trial.number > nBack;
	}


	private bool IsNBackTrial()
	{
		return Random.Range(0, 101) < PercentageNLikely;
	}
	*/
}
