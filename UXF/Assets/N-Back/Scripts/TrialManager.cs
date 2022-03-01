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
	private readonly List<string> _previousCharacters = new();
	private string _current;

	[Range(0, 100)] public int PercentageNLikely = 60;

	public ParticleSystem SuccessParticles;
	private bool _keyed;
	private const float _missedTrialInterval = 0.5f;


	private CancellationTokenSource _cts = new CancellationTokenSource();


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
	}


	public async void StartTrial(Trial trial)
	{
		await TrialRunner(trial);
	}


	private async Task TrialRunner(Trial trial)
	{
		_current = GetWeighedRandomCharacter(trial);

		TextField.text = _current;
		_keyed = false;

		_trialDuration = trial.block.settings.GetFloat("trialDuration");
		await Task.Delay((int)(_trialDuration * 1000));

		TextField.text = null;

		if (_keyed == false && CanUseNBack(trial) == true && _current == _previousCharacters[^nBack])
		{
			TextField.text = "MISSED";

			trial.block.settings.SetValue("trialDuration", _trialDuration + ChangeTrialTime);
			trial.block.settings.SetValue("interTrialInterval", _interTrialInterval + ChangeITI);
			_trialDuration = trial.block.settings.GetFloat("trialDuration");
			_interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");

			await Task.Delay((int)(_missedTrialInterval * 1000));

			TextField.text = "";
		}

		_interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");

		await Task.Delay((int)(_interTrialInterval * 1000));


		var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
		var finished = await Task.WhenAny(Delayer(linkedCts.Token), Delayer1(linkedCts.Token));
		await finished;
		linkedCts.Cancel();
		linkedCts.Dispose();
		_cts = new CancellationTokenSource();

		_previousCharacters.Add(_current);
		trial.session.EndCurrentTrial();

	}


	private async Task Delayer1(CancellationToken ct)
	{
		await Task.Delay(5000, ct);
		Debug.Log("Time's up!");
	}


	private async Task Delayer(CancellationToken ct)
	{
		while (!Input.GetKeyDown(KeyCode.Return) && ct.IsCancellationRequested == false)
		{
			Debug.Log("No key yet");
			await Task.Yield();
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			Debug.Log("Pressee le qui");
		}
		else if (ct.IsCancellationRequested == true)
		{
			Debug.Log("Cancellated");
		}
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


	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Space))
		{
			return;
		}

		if (Session.instance.CurrentTrial.number < nBack)
		{
			SuccessParticles.Play();
			TextField.text = "";
			return;
		}

		var settings = Session.instance.CurrentBlock.settings;

		if (_current == _previousCharacters[^ nBack])
		{
			SuccessParticles.Play();
			TextField.text = "";

			if (_trialDuration - ChangeTrialTime > 0)
			{
				settings.SetValue("trialDuration", _trialDuration - ChangeTrialTime);
			}

			if (_interTrialInterval - ChangeITI > 0.1f)
			{
				settings.SetValue("interTrialInterval", _interTrialInterval - ChangeITI);
			}
		}
		else
		{
			TextField.text = "NOPE!";

			settings.SetValue("trialDuration", _trialDuration + ChangeTrialTime);
			settings.SetValue("interTrialInterval", _interTrialInterval + ChangeITI);
		}

		_trialDuration = settings.GetFloat("trialDuration");
		_interTrialInterval = settings.GetFloat("interTrialInterval");

		_keyed = true;
	}


	private void OnDisable()
	{
		_cts.Cancel();
	}
}
