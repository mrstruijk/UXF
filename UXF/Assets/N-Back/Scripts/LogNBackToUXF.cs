using UnityEngine;
using UXF;

public class LogNBackToUXF : MonoBehaviour
{
    [SerializeField] private Session Session;

    public void CorrectHit()
    {
        if (Session.hasInitialised)
        {
            Session.CurrentTrial.result["Result"] = "Correct_Hit";
        }
    }
    
    public void CorrectNoResponse()
    {
        if (Session.hasInitialised)
        {
            Session.CurrentTrial.result["Result"] = "Correct_No_Response";
        }
    }
    
    public void Missed()
    {
        if (Session.hasInitialised)
        {
            Session.CurrentTrial.result["Result"] = "Failed_Missed";
        }
    }
    
    public void Wrong()
    {
        if (Session.hasInitialised)
        {
            Session.CurrentTrial.result["Result"] = "Failed_Wrong_Hit";
        }
    }

    private void LogResponseTime()
    {
        if (Session.hasInitialised)
        {
            // var rt = _session.CurrentTrial.
        }
    }
}
