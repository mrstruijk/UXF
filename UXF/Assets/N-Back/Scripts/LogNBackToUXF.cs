using System;
using UnityEngine;
using UXF;


public class LogNBackToUXF : MonoBehaviour
{
    [SerializeField] private Session Session;


    private void Awake()
    {
        if (Session == null)
        {
            Session = FindObjectOfType<Session>();
        }
    }


    public void CorrectHit()
    {
        if (!Session.hasInitialised)
        {
            return;
        }

        Session.CurrentTrial.result["Result"] = "Correct";
        Session.CurrentTrial.result["Action"] = "Hit";
    }


    public void CorrectNoResponse()
    {
        if (!Session.hasInitialised)
        {
            return;
        }

        Session.CurrentTrial.result["Result"] = "Correct";
        Session.CurrentTrial.result["Action"] = "No_Response";
    }


    public void Missed()
    {
        if (!Session.hasInitialised)
        {
            return;
        }

        Session.CurrentTrial.result["Result"] = "Failed";
        Session.CurrentTrial.result["Action"] = "Missed";
    }


    public void Wrong()
    {
        if (!Session.hasInitialised)
        {
            return;
        }

        Session.CurrentTrial.result["Result"] = "Failed";
        Session.CurrentTrial.result["Action"] = "False_Positive";
    }


    public void ResponseTime(long rt)
    {
        if (!Session.hasInitialised)
        {
            return;
        }

        Session.CurrentTrial.result["RT"] = rt;
    }


    public void TrialDuration(float duration)
    {
        if (!Session.hasInitialised)
        {
            return;
        }

        Session.CurrentTrial.result["TrialDuration"] = duration;
    }
}