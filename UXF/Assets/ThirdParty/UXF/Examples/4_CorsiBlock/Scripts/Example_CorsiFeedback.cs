using UnityEngine;
using UnityEngine.UI;

// UXF namespace


namespace UXFExamples
{
    /// <summary>
    /// This script controls feedback text
    /// </summary>
    public class Example_CorsiFeedback: MonoBehaviour
    {
        
        Text feedbackText;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            feedbackText = GetComponent<Text>();
            Clear();
        }


        public void ShowFeedback(string text, float delay = 1f)
        {
            feedbackText.text = text;
            Invoke("Clear", delay); // clear after 1 second
        }


        void Clear()
        {
            feedbackText.text = "";
        }

    }
}