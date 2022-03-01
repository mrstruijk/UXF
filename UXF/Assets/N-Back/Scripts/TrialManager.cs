using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UXF;
using Random = UnityEngine.Random;


public class TrialManager : MonoBehaviour
{
	[Range(0, 10)] public int nBack = 1;
	public bool UseLetters;
	private float _trialDuration = 2f;
	private float _interTrialInterval = 0.25f;
	public TextMeshProUGUI TextField;
	public float ChangeTrialTime = 0.25f;
	public float ChangeITI = 0.1f;

	private List<string> _characterList = new();
	[SerializeField] private List<string> _previousCharacters;
	private string _current;

	[Range(0, 100)] public int PercentageNLikely = 60;

	public ParticleSystem SuccessParticles;
	private const float _successTrialInterval = 0.5f;
	private const float _missedTrialInterval = 0.5f;


	private CancellationTokenSource _cts = new CancellationTokenSource();

	private int numberofTrials;


	private void Start()
	{
		_cts = new CancellationTokenSource();
	}


	public void GetInitialValuesFromSettings(Session session)
	{
		if (UseLetters == true)
		{
			_characterList = session.settings.GetStringList("letters");
		}
		else
		{
			_characterList = session.settings.GetStringList("numbers");
		}

		var numPracticeTrials = session.settings.GetInt("n_practice_trials");
		var numMainTrials = session.settings.GetInt("n_main_trials");
		numberofTrials = numPracticeTrials + numMainTrials;
		_previousCharacters = new List<string>(new string[numberofTrials]);
	}


	public async void StartTrial(Trial trial)
	{
		await TrialRunner(trial);
	}


	private async Task TrialRunner(Trial trial)
	{
		_current = GetWeighedRandomCharacter(trial);
		TextField.text = _current;

		_previousCharacters[trial.number - 1] = _current;

		_trialDuration = trial.block.settings.GetFloat("trialDuration");
		var trialCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
		var trialFinished = await Task.WhenAny(WaitForKeyPress(trialCts.Token, trial), Missed(trialCts.Token, trial, _trialDuration));
		await trialFinished;

		TextField.text = null;

		_interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");
		await Delayer(trialCts.Token, _interTrialInterval);

		trialCts.Cancel();
		trialCts.Dispose();

		trial.session.EndCurrentTrial();
	}


	private static async Task Delayer(CancellationToken ct, float seconds)
	{
		await Task.Delay((int)(seconds * 1000), ct);
	}


	private async Task Missed(CancellationToken ct, Trial trial, float trialDuration)
	{
		await Delayer(ct, trialDuration);

		if (trial.number <= 1)
		{
			return;
		}

		if (_current == _previousCharacters[^nBack])
		{
			TextField.text = "MISSED";

			await Delayer(ct, _missedTrialInterval);

			trial.block.settings.SetValue("trialDuration", _trialDuration + ChangeTrialTime);
			trial.block.settings.SetValue("interTrialInterval", _interTrialInterval + ChangeITI);
			_trialDuration = trial.block.settings.GetFloat("trialDuration");
			_interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");

			TextField.text = "";
		}
	}


	private async Task WaitForKeyPress(CancellationToken ct, Trial trial)
	{
		while (!Input.GetKeyDown(KeyCode.Space) && ct.IsCancellationRequested == false)
		{
			// Debug.Log("No key yet");
			await Task.Yield();
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("Pressee le qui");

			if (trial.number <= nBack)
			{
				InCorrectResponse(ct, trial);
			}
			else if (_current == _previousCharacters[^ nBack])
			{
				CorrectResponse(ct, trial);
			}
			else
			{
				InCorrectResponse(ct, trial);
			}

			_trialDuration = trial.settings.GetFloat("trialDuration");
			_interTrialInterval = trial.settings.GetFloat("interTrialInterval");
		}
		else if (ct.IsCancellationRequested == true)
		{
			Debug.Log("Cancellated");
		}
	}


	private async void InCorrectResponse(CancellationToken ct, Trial trial)
	{
		TextField.text = "NOPE!";

		trial.settings.SetValue("trialDuration", _trialDuration + ChangeTrialTime);
		trial.settings.SetValue("interTrialInterval", _interTrialInterval + ChangeITI);

		await Delayer(ct, _missedTrialInterval);
	}



	private async void CorrectResponse(CancellationToken ct, Trial trial)
	{
		TextField.text = "";
		SuccessParticles.Play();

		if (_trialDuration - ChangeTrialTime > 0)
		{
			trial.settings.SetValue("trialDuration", _trialDuration - ChangeTrialTime);
		}

		if (_interTrialInterval - ChangeITI > 0.1f)
		{
			trial.settings.SetValue("interTrialInterval", _interTrialInterval - ChangeITI);
		}

		await Delayer(ct, _successTrialInterval);
	}


	private string GetWeighedRandomCharacter(Trial trial)
	{
		if (CanUseNBack(trial) == false)
		{
			return _characterList[Random.Range(0, _characterList.Count)];
		}

		if (IsNBackTrial() == true)
		{
			return _previousCharacters[^nBack];
		}

		List<string> excludedList = new();
		excludedList.AddRange(_characterList.Where(characters => characters != _previousCharacters[^nBack]));
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


	private void OnDisable()
	{
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



