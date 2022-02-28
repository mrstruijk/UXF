using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class Task : MonoBehaviour
{
    [FormerlySerializedAs("_characterList")] public List<string> CharacterList = new List<string>(){"0","1","2","3","4","5","6","7","8","9"};
    public float WaitTime = 2f;
    public TextMeshProUGUI TextField;

    [SerializeField] private List<string> _previousCharacters = new();
    
    private void Start()
    {
        StartCoroutine(TaskCR());
    }

    private IEnumerator TaskCR()
    {
        for (;;)
        {
            yield return new WaitForSeconds(WaitTime);
            var current = CharacterList[Random.Range(0, CharacterList.Count)];
            
            _previousCharacters.Add(current);
            
            TextField.text = current;
        }
    }
}
