using UnityEngine;
using UXF;

public class LogNBackToUXF : MonoBehaviour
{
    [SerializeField] private Session _session;

    public void CorrectHit()
    {
        _session.CurrentTrial.result["Result"] = "Correct_Hit";
    }
    
    public void CorrectNoResponse()
    {
        _session.CurrentTrial.result["Result"] = "Correct_No_Response";
    }
    
    public void Missed()
    {
        _session.CurrentTrial.result["Result"] = "Failed_Missed";
    }
    
    public void Wrong()
    {
        _session.CurrentTrial.result["Result"] = "Failed_Wrong_Hit";
    }

    private void LogResponseTime()
    {
        // var rt = _session.CurrentTrial.
    }
}
