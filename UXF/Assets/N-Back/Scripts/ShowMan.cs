using TMPro;
using UnityEngine;
using UXF;


public class ShowMan : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI s_textField;
    [SerializeField] private ParticleSystem s_successHitParticles;
    [SerializeField] private ParticleSystem s_successNoResponseParticles;

    private string _missed;
    private string _wrong;


    private void Awake()
    {
        if (s_textField == null)
        {
            s_textField = GameObject.FindWithTag("Task_TextField").GetComponent<TextMeshProUGUI>();
        }

        if (s_successHitParticles == null)
        {
            s_successHitParticles = GameObject.FindWithTag("Success_Hit_Particles").GetComponent<ParticleSystem>();
        }

        if (s_successNoResponseParticles == null)
        {
            s_successNoResponseParticles = GameObject.FindWithTag("Success_NoHit_Particles").GetComponent<ParticleSystem>();
        }
    }


    private void ClearTextField()
    {
        ChangeText();
    }


    public void DisplayText(string text)
    {
        ChangeText(text);
    }


    public void MissedResponse()
    {
        if (string.IsNullOrEmpty(_missed))
        {
            _missed = Session.instance.settings.GetString("displayText_missed");
        }

        ChangeText(_missed);
    }


    public void InCorrectResponse()
    {
        if (string.IsNullOrEmpty(_missed))
        {
            _wrong = Session.instance.settings.GetString("displayText_wrong");
        }

        ChangeText(_wrong);
    }


    private void ChangeText(string displayText = "")
    {
        s_textField.text = displayText;
    }


    public void CorrectResponse()
    {
        ClearTextField();
        PlayParticles(s_successHitParticles);
    }


    public void CorrectNoResponse()
    {
        ClearTextField();
        PlayParticles(s_successNoResponseParticles);
    }


    private static void PlayParticles(ParticleSystem particles)
    {
        particles.Play();
    }
}