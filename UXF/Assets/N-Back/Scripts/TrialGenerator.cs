using System.Collections.Generic;
using UnityEngine;
using UXF;


public class TrialGenerator : MonoBehaviour
{
	public List<string> CharacterList = new();

	public int NumberOfTrials;
	public bool UseLetters;

	public void GetInitialValuesFromSettings(Session session)
	{
		CharacterList = session.settings.GetStringList(UseLetters == true ? "letters" : "numbers");

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
			Debug.Log("We're done");
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
