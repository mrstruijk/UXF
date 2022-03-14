using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UXF;


public class TrialGenerator : MonoBehaviour
{
	public List<string> _characterList = new();

	public int NumberOfTrials;
	public bool UseLetters;

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
		NumberOfTrials = numPracticeTrials + numMainTrials;
	}


	public void OpenFirstTrial(Session session)
	{
		session.BeginNextTrial();
	}


	public void TryStartingNextTrial(Trial trial)
	{
		if (trial == trial.session.LastTrial)
		{
			Debug.Log("We done son");
		}
		else if (trial.session.CurrentTrial != trial.session.CurrentBlock.lastTrial)
		{
			trial.session.BeginNextTrialSafe();
		}
		else
		{
			if (trial.session.CurrentBlock == trial.session.blocks[^1])
			{
				return;
			}

			trial.session.GetBlock(trial.session.CurrentBlock.number + 1);
			trial.session.BeginNextTrialSafe();
		}
	}
}
