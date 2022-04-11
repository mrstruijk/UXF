﻿using System.Collections;
using UnityEngine;

namespace UXF.UI
{
    public class FadeImage : MonoBehaviour
    {

        public float duration = 1f;
        public AnimationCurve curve;
        public CanvasGroup group;

        public void BeginFade()
        {
            StopAllCoroutines();
            StartCoroutine(FadeSequence());
        }

        IEnumerator FadeSequence()
        {
            float startTime = Time.time;
            float t = 0;
            
            while ((t = (Time.time - startTime) / duration) < 1)
            {
                group.alpha = curve.Evaluate(t);
                yield return null;
            }

            group.alpha = 0;
        }

    }

}