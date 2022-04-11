using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UXF;
using Random = UnityEngine.Random;


public class TrialManager : MonoBehaviour
{
    [SerializeField] private SessionGenerator s_sessionGenerator;
    [SerializeField] private ShowMan s_showMan;
    [SerializeField] private AudioMan s_audioMan;
    [SerializeField] private LogNBackToUXF s_log;

    private string _current;
    private float _interTrialInterval;
    private Coroutine _keyPressedCR;

    private int _nBack;

    private int _percentageNLikely;

    private List<string> _previousCharacters;
    private long _rt;

    private long _startTime;
    private long _stopTime;

    private float _trialDuration;
    private Coroutine _trialDurationCR;


    private void Awake()
    {
        if (s_sessionGenerator == null)
        {
            s_sessionGenerator = FindObjectOfType<SessionGenerator>();
        }

        if (s_showMan == null)
        {
            s_showMan = FindObjectOfType<ShowMan>();
        }

        if (s_audioMan == null)
        {
            s_audioMan = FindObjectOfType<AudioMan>();
        }

        if (s_log == null)
        {
            s_log = FindObjectOfType<LogNBackToUXF>();
        }
    }


    public void StartTrial(Trial trial)
    {
        StartCoroutine(TrialRunner(trial));
    }


    private IEnumerator TrialRunner(Trial trial)
    {
        if (trial.numberInBlock == 1)
        {
            if (trial.block.number == 1)
            {
                s_showMan.DisplayText("Practice trials");
            }
            else if (trial.block.number == 2)
            {
                s_showMan.DisplayText("Main trials start now");
            }

            yield return new WaitForSeconds(2.5f);
        }

        _nBack = Session.instance.settings.GetInt("NBack");

        _current = GetWeighedRandomCharacter(trial);

        s_showMan.DisplayText(_current);

        _percentageNLikely = Session.instance.settings.GetInt("percentageNBack");

        _trialDuration = trial.block.settings.GetFloat("trialDuration");

        _keyPressedCR = StartCoroutine(WaitForKeyPress(trial));
        _trialDurationCR = StartCoroutine(WaitForTaskDuration(trial, _trialDuration));

        _startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        while (_keyPressedCR != null && _trialDurationCR != null)
        {
            yield return null;
        }

        _interTrialInterval = trial.block.settings.GetFloat("interTrialInterval");

        yield return new WaitForSeconds(_interTrialInterval);

        if (_previousCharacters == null || _previousCharacters.Count == 0)
        {
            _previousCharacters = new List<string>();
        }

        _previousCharacters.Insert(0, _current);

        trial.session.EndCurrentTrial();
    }


    private IEnumerator WaitForTaskDuration(Trial trial, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        s_log.TrialDuration(_trialDuration);
        LogRT();

        if (_keyPressedCR != null)
        {
            StopCoroutine(_keyPressedCR);
        }

        if (trial.number <= _nBack)
        {
            s_showMan.CorrectNoResponse();
            s_audioMan.PlaySuccessNoHit();
            s_log.CorrectNoResponse();
        }
        else if (_current == _previousCharacters[_nBack - 1])
        {
            s_showMan.MissedResponse();
            s_audioMan.PlayMiss();
            s_log.Missed();
        }
        else
        {
            s_showMan.CorrectNoResponse();
            s_audioMan.PlaySuccessNoHit();
            s_log.CorrectNoResponse();
        }

        _trialDurationCR = null;
    }


    private IEnumerator WaitForKeyPress(Trial trial)
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        s_log.TrialDuration(_trialDuration);
        LogRT();

        if (_trialDurationCR != null)
        {
            StopCoroutine(_trialDurationCR);
        }

        if (trial.number <= _nBack)
        {
            s_showMan.InCorrectResponse();
            s_audioMan.PlayWrong();
            s_log.Wrong();
        }
        else if (_current == _previousCharacters[_nBack - 1])
        {
            s_showMan.CorrectResponse();
            s_audioMan.PlaySuccessHit();
            s_log.CorrectHit();
        }
        else
        {
            s_showMan.InCorrectResponse();
            s_audioMan.PlayWrong();
            s_log.Wrong();
        }

        _keyPressedCR = null;
    }


    private void LogRT()
    {
        _rt = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startTime;
        s_log.ResponseTime(_rt);
    }


    private string GetWeighedRandomCharacter(Trial trial)
    {
        string current = null;
        var chance = Random.Range(0, 101);

        if (CanUseNBack(trial) == false)
        {
            current = s_sessionGenerator.CharacterList[Random.Range(0, s_sessionGenerator.CharacterList.Count)];
        }
        else if (CanUseNBack(trial) == true && IsNBackTrial(chance) == true) // && _previousCharacters.Count > _nBack)
        {
            current = _previousCharacters[_nBack - 1];
        }
        else if (CanUseNBack(trial) == true && IsNBackTrial(chance) == false) // && _previousCharacters.Count > _nBack)
        {
            var excludedList = s_sessionGenerator.CharacterList.Where(character => character != _previousCharacters[_nBack - 1]).ToList();
            current = excludedList[Random.Range(0, excludedList.Count)];
        }
        else
        {
            Debug.LogErrorFormat("This shouldn't happen. CanUseNBack == {0}, IsNBackTrial == {1}", CanUseNBack(trial), IsNBackTrial(chance));
        }

        return current;
    }


    private bool CanUseNBack(Trial trial)
    {
        return trial.number > _nBack;
    }


    private bool IsNBackTrial(int chance)
    {
        return chance < _percentageNLikely;
    }


    private void OnDisable()
    {
        if (_previousCharacters is {Count: > 0})
        {
            _previousCharacters.Reverse();
        }

        StopAllCoroutines();
    }
}