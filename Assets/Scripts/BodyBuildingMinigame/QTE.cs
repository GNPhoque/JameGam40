using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// QTE game is composed by N series of sequences composed by QTESequenceSize keys
public class QTE : MonoBehaviour
{
    public event Action<string> QTESuccess;
    public event Action<string> QTEFailure;

    public event Action<string> QTESequenceSuccess;
    public event Action<string> QTESequenceFailure;

    public event Action<string> QTEGameSuccess;

    public int QTESequenceSize = 5;
    public int QTESequenceTime = 20;

    public List<List<KeyCode>> sequences {get; private set;}

    private int _currentSequence = 0;
    private int _currentSequenceKey = 0;

    private bool _gameStarted = false;
    private float _sequenceRemainingTime = 0.0f;

    List<KeyCode> keycodes;

    private void Start()
    {
        keycodes = new List<KeyCode>();

        for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z; i++)
            keycodes.Add((KeyCode)i);
        sequences = new List<List<KeyCode>>();

        StartGame(4);
    } 

    // Update is called once per frame
    void Update()
    {
        if (!_gameStarted)
            return;
        Debug.Log(sequences[_currentSequence][_currentSequenceKey].ToString());
        _sequenceRemainingTime -= Time.deltaTime;
        if (_sequenceRemainingTime <= 0)
        {
            _gameStarted = false;
            QTESequenceFailure?.Invoke("");
        }
        if (!Input.anyKeyDown)
            return;
        if (!Input.GetKeyDown(sequences[_currentSequence][_currentSequenceKey]))
        {
            QTEFailure?.Invoke("");
            return;
        }
        QTESuccess?.Invoke("");
        if (_currentSequenceKey + 1 == sequences[_currentSequence].Count)
        {
            QTESequenceSuccess?.Invoke("");
            _sequenceRemainingTime = QTESequenceTime;
            _currentSequenceKey = 0;
            if (_currentSequence + 1 == sequences.Count)
            {
                _gameStarted = false;
                QTEGameSuccess?.Invoke("");
                return;
            }
            _currentSequence += 1;
            return;
        }
        _currentSequenceKey += 1;

    }

    public void StartGame(int seriesNumber)
    {
        drawQTE(seriesNumber);
        _currentSequence = 0;
        _currentSequenceKey = 0;
        _sequenceRemainingTime = QTESequenceTime;
        _gameStarted = true;
    }

    private void TryKey()
    {
        foreach (KeyCode vKey in keycodes)
        {
            if (Input.GetKeyDown(vKey))
            {
                Debug.Log(vKey.ToString());
            }
        }
    }

    private void drawQTE(int seriesNumber)
    {
        sequences.Clear();
        for(int i = 0; i < seriesNumber; i++)
        {
            List<KeyCode> keyList = new List<KeyCode>();
            for (int j = 0; j < QTESequenceSize; j++)
                keyList.Add(keycodes[UnityEngine.Random.Range(0, keycodes.Count)]);
            sequences.Add(keyList);
        }
    }
}
