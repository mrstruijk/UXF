using System.Collections.Generic;
using mrstruijk;
using UnityEngine;
using UXF;


public class SessionGenerator : MonoBehaviour
{
    [DisableEditing] public List<string> CharacterList = new();


    public void GenerateExperiment(Session session)
    {
        SetSettingForSession(session);

        SetSettingsForPracticeBlock(session);

        SetSessionsForMainBlock(session);
    }


    private void SetSettingForSession(Session session)
    {
        var useLetters = session.settings.GetBool("useLetters");
        CharacterList = session.settings.GetStringList(useLetters == true ? "letters" : "numbers");

        var nback = session.settings.GetInt("NBack");
        session.settings.SetValue("NBack", nback);

        var wrong = session.settings.GetString("displayText_wrong");
        session.settings.SetValue("displayText_wrong", wrong);

        var missed = session.settings.GetString("displayText_missed");
        session.settings.SetValue("displayText_missed", missed);
    }


    private static void SetSettingsForPracticeBlock(Session session)
    {
        var numPracticeTrials = session.settings.GetInt("n_practice_trials");
        var practiceBlock = session.CreateBlock(numPracticeTrials);
        var durationPracticeTrial = session.settings.GetFloat("practice_trialDuration");
        practiceBlock.settings.SetValue("trialDuration", durationPracticeTrial);
        var backgroundColor = session.settings.GetString("practice_color");
        practiceBlock.settings.SetValue("backgroundColor", backgroundColor);
    }


    private static void SetSessionsForMainBlock(Session session)
    {
        var numMainTrials = session.settings.GetInt("n_main_trials");
        var mainBlock = session.CreateBlock(numMainTrials); // block 2
        var durationMainTrial = session.settings.GetFloat("main_trialDuration");
        mainBlock.settings.SetValue("trialDuration", durationMainTrial);
        var backgroundColor = session.settings.GetString("main_color");
        mainBlock.settings.SetValue("backgroundColor", backgroundColor);
    }
}