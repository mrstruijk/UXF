using UnityEngine;
using UXF;

public class SessionGenerator : MonoBehaviour
{
    public void GenerateExperiment(Session session)
    {
        var numPracticeTrials = session.settings.GetInt("n_practice_trials");
        var practiceBlock = session.CreateBlock(numPracticeTrials);
        //practiceBlock.settings.SetValue("trialDuration", 2);

        var numMainTrials = session.settings.GetInt("n_main_trials");
        var mainBlock = session.CreateBlock(numMainTrials); // block 2
        //mainBlock.settings.SetValue("trialDuration", 1);
    }
}