using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowMan : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _textField;
	[SerializeField] private ParticleSystem _particles;


	public void DisplayText(string displayText)
	{
		displayText ??= "";
		_textField.text = displayText;
	}


	public void PlayParticles()
	{
		_particles.Play();
	}
}
