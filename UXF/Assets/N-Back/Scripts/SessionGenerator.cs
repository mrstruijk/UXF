using UnityEngine;
using UXF;

public class SessionGenerator : MonoBehaviour
{
    public void GenerateExperiment(Session session)
    {
        var numPracticeTrials = session.settings.GetInt("n_practice_trials");
        var practiceBlock = session.CreateBlock(numPracticeTrials);
        practiceBlock.settings.SetValue("trialDuration", 2);
        
        var numMainTrials = session.settings.GetInt("n_main_trials");
        var mainBlock = session.CreateBlock(numMainTrials); // block 2
        mainBlock.settings.SetValue("trialDuration", 1);
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