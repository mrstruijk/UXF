using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowMan : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _textField;
	[SerializeField] private ParticleSystem _particles;

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
		PlayParticles();
	}

	private void PlayParticles()
	{
		_particles.Play();
	}
}
