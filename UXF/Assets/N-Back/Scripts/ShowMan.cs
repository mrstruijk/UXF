using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ShowMan : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _textField;
	[SerializeField] private ParticleSystem _successHitParticles;
	[SerializeField] private ParticleSystem _successNoResponseParticles;

	public void ClearTextField()
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
		_textField.text = displayText;
	}

	public void CorrectResponse()
	{
		ClearTextField();
		PlayParticles(_successHitParticles);
	}

	public void CorrectNoResponse()
	{
		ClearTextField();
		PlayParticles(_successNoResponseParticles);
	}
	
	private static void PlayParticles(ParticleSystem particles)
	{
		particles.Play();
	}
}
