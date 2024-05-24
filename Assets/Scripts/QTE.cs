using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// QTE game is composed by N series of sequences composed by QTESequenceSize keys
public class QTE : MonoBehaviour
{

    public int QTESequenceSize = 5;
    public int QTESequenceTime = 20;

    public GameObject BGKey;

    public List<List<BGKey>> sequences {get; private set;}

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
        sequences = new List<List<BGKey>>();
    } 

    // Update is called once per frame
    void Update()
    {
        // _sequenceRemainingTime -= Time.deltaTime;
        if (_sequenceRemainingTime <= 0)
            _gameStarted = false;
        if (!_gameStarted)
            return;
        if (!Input.anyKeyDown)
            return;
        BGKey currentKey = sequences[_currentSequence][_currentSequenceKey];
        if (!Input.GetKeyDown(sequences[_currentSequence][_currentSequenceKey].Key))
        {
            currentKey.startShake();
            return;
        }
        currentKey.validateKey();
        sequences[_currentSequence][_currentSequenceKey].validateKey();
        if (_currentSequenceKey + 1 == sequences[_currentSequence].Count)
        {
            _sequenceRemainingTime = QTESequenceTime;
            _currentSequenceKey = 0;
            if (_currentSequence + 1 == sequences.Count)
            {
                _gameStarted = false;
                return;
            }
            _currentSequence += 1;
            ChangeSequence(_currentSequence);
            return;
        }
        _currentSequenceKey += 1;

    }

    public void StartGame()
    {
        // count number or sequences to do
        int seqNumbers = 0;
        foreach (SnapPositions snaper in SnapPositions.parentSnapers)
        {
            if (snaper.snappedTo != null)
                seqNumbers += 1;
        }
        if (seqNumbers == 0)
            return;

        drawQTE(seqNumbers);
        ChangeSequence(0);
        _currentSequence = 0;
        _currentSequenceKey = 0;
        _sequenceRemainingTime = QTESequenceTime;
        _gameStarted = true;
    }

    private void ChangeSequence(int newSequence)
    {
        if (newSequence > 0)
        {
            // deactivate prévious sequence
            foreach (BGKey key in sequences[newSequence - 1])
            {
                key.gameObject.SetActive(false);
            }
        }
        foreach (BGKey key in sequences[newSequence])
        {
            key.gameObject.SetActive(true);
        }
    }

    private void drawQTE(int seriesNumber)
    {
        sequences.Clear();
        for(int i = 0; i < seriesNumber; i++)
        {
            List<BGKey> keyList = new List<BGKey>();
            for (int j = 0; j < QTESequenceSize; j++)
            {
                KeyCode drawed = keycodes[Random.Range(0, keycodes.Count)];
                GameObject newKey = Instantiate(BGKey, transform);
                BGKey keyScript = newKey.GetComponent<BGKey>();
                keyScript.ActivateWithKey(drawed, transform.position + new Vector3(j * 1.25f, 0, 0));
                keyList.Add(keyScript);
                newKey.SetActive(false);
            }

            sequences.Add(keyList);
        }
    }
}
