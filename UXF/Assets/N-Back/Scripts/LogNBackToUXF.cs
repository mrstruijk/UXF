using UnityEngine;
using UXF;

public class LogNBackToUXF : MonoBehaviour
{
    [SerializeField] private Session _session;

    public void CorrectHit()
    {
        if (_session.hasInitialised)
        {
            _session.CurrentTrial.result["Result"] = "Correct_Hit";
        }
    }
    
    public void CorrectNoResponse()
    {
        if (_session.hasInitialised)
        {
            _session.CurrentTrial.result["Result"] = "Correct_No_Response";
        }
        
    }
    
    public void Missed()
    {
        if (_session.hasInitialised)
        {
            _session.CurrentTrial.result["Result"] = "Failed_Missed";
        }
    }
    
    public void Wrong()
    {
        if (_session.hasInitialised)
        {
            _session.CurrentTrial.result["Result"] = "Failed_Wrong_Hit";
        }
    }

    private void LogResponseTime()
    {
        if (_session.hasInitialised)
        {
            // var rt = _session.CurrentTrial.
        }
    }
}
