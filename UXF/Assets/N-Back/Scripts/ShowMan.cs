using TMPro;
using UnityEngine;

public class ShowMan : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI TextField;
	[SerializeField] private ParticleSystem SuccessHitParticles;
	[SerializeField] private ParticleSystem SuccessNoResponseParticles;

	private void ClearTextField()
	{
		DisplayText();
	}

	public void DisplayVariable(string variable)
	{
		DisplayText(variable);
	}
	
	public void MissedResponse()
	{
		DisplayText("MISSED");
	}
	
	public void InCorrectResponse()
	{
		DisplayText("NOPE");
	}

	private void DisplayText(string displayText = "")
	{
		TextField.text = displayText;
	}

	public void CorrectResponse()
	{
		ClearTextField();
		PlayParticles(SuccessHitParticles);
	}

	public void CorrectNoResponse()
	{
		ClearTextField();
		PlayParticles(SuccessNoResponseParticles);
	}
	
	private static void PlayParticles(ParticleSystem particles)
	{
		particles.Play();
	}
}
