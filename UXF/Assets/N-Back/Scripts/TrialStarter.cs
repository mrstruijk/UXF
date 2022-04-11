using UnityEngine;
using UXF;


public class TrialStarter : MonoBehaviour
{
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