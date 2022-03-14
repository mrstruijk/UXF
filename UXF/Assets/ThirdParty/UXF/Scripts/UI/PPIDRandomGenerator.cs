using UnityEngine;
using UnityEngine.UI;

namespace UXF.UI
{
    public class PPIDRandomGenerator : MonoBehaviour
    {
        public InputField inputField;

        UIController uiController;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            uiController = GetComponentInParent<UIController>();
        }


        public void EnterRandom()
        {
            inputField.text = uiController.GenerateUniquePPID();
        }

    }

}