using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UXF;
using Debug = UnityEngine.Debug;


public class TrialManager : MonoBehaviour
{
	[SerializeField] private ShowMan _showMan;
	[Range(0, 10)] public int nBack = 1;
	private float _trialDuration = 2f;
	private float _interTrialInterval = 0.25f;
	public TextMeshProUGUI TextField;
	public float ChangeTrialTime = 0.25f;
	public float ChangeITI = 0.1f;

	[SerializeField] public List<string> _previousCharacters;
	private string _current;

	[Range(0, 100)] public int PercentageNLikely = 60;

	public ParticleSystem SuccessParticles;
	private const float _successTrialInterval = 0.5f;
	private const float _missedTrialInterval = 0.5f;


	private CancellationTokenSource _cts = new CancellationTokenSource();

	private int numberofTrials;
	[SerializeField] private TrialGenerator _trialGenerator;


	private void Start()
	{
		_cts = new CancellationTokenSource();
		_previousCharacters = new List<string>();
	}


	public async void StartTrial(Trial trial)
	{
		await TrialRunner(trial);
	}


	private async Task TrialRunner(Trial trial)
	{
		_current = GetWeighedRandomCharacter(trial);

		_showMan.DisplayText(_current);

		_previousCharacters.Add(_current);

		_trialDuration = trial.block.settings.GetFloat("trialDuration");
		var trialCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
		var trialFinished = await Task.WhenAny(WaitForKeyPress(trialCts.Token, trial), Missed(trialCts.Token, trial, _trialDuration));
		await trialFinished;

		_showMan.DisplayText(null);

		/// _interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");
		await Delayer(trialCts.Token, 5);

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

		if (trial.number <= nBack)
		{
			return;
		}

		if (_current == _previousCharacters[_previousCharacters.Count -1 -nBack])
		{
			_showMan.DisplayText("MISSED");

			await Delayer(ct, _missedTrialInterval);

			trial.block.settings.SetValue("trialDuration", _trialDuration + ChangeTrialTime);
			trial.block.settings.SetValue("interTrialInterval", _interTrialInterval + ChangeITI);
			_trialDuration = trial.block.settings.GetFloat("trialDuration");
			_interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");

			_showMan.DisplayText(null);
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
			else if (_current == _previousCharacters[_previousCharacters.Count -1 -nBack])
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
		_showMan.DisplayText("NOPE");

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
		string current;

		if (CanUseNBack(trial) == false)
		{
			current = _trialGenerator._characterList[Random.Range(0, _trialGenerator._characterList.Count)];
			Debug.LogErrorFormat("Canno use Nback, has to be {0}", current);
			return current;
		}

		if (IsNBackTrial() == true)
		{
			current = _previousCharacters[_previousCharacters.Count -1 -nBack];
			Debug.LogErrorFormat("Nback = {0}", current);
			return current;
		}

		List<string> excludedList = new();
		excludedList.AddRange(_trialGenerator._characterList.Where(characters => characters != _previousCharacters[_previousCharacters.Count -1 -nBack]));
		current = excludedList[Random.Range(0, excludedList.Count)];
		Debug.LogErrorFormat("Wanna use new char {0}", current);
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
