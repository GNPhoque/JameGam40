using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// QTE game is composed by N series of sequences composed by QTESequenceSize keys
public class QTE : MonoBehaviour
{

    public int QTESequenceSize = 5;
    public int QTESequenceTime = 5;

    public TextMeshProUGUI timer;

    public GameObject BGKey;

    public List<List<BGKey>> sequences {get; private set;}
    private List<SnapPositions> _toSew;

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
        _toSew = new List<SnapPositions>();
    } 

    // Update is called once per frame
    void Update()
    {
        _sequenceRemainingTime -= Time.deltaTime;
        if (_gameStarted && _sequenceRemainingTime <= 0)
        {
            timer.gameObject.SetActive(false);
            desactivateSequence(_currentSequence);
            _gameStarted = false;
        }
        if (!_gameStarted)
            return;
        timer.text = $"{(int)_sequenceRemainingTime}:{(int)((_sequenceRemainingTime % 1) * 1000)}";
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
            desactivateSequence(_currentSequence);
            _toSew[_currentSequence].sew();
            if (_currentSequence + 1 == sequences.Count)
            {
                timer.gameObject.SetActive(false);
                _gameStarted = false;
                return;
            }
            _currentSequence += 1;
            activateSequence(_currentSequence);
            return;
        }
        _currentSequenceKey += 1;

    }

    public void StartGame()
    {
        if (_gameStarted)
            return;
        // count number or sequences to do
        int seqNumbers = 0;
        _toSew.Clear();
        timer.gameObject.SetActive(true);
        foreach (SnapPositions snaper in SnapPositions.parentSnapers)
        {
            if (snaper.snappedTo == null || snaper.sewed)
                continue;
            seqNumbers += 1;
            _toSew.Add(snaper);
        }
        if (seqNumbers == 0)
            return;

        drawQTE(seqNumbers);
        activateSequence(0);
        _currentSequence = 0;
        _currentSequenceKey = 0;
        _sequenceRemainingTime = QTESequenceTime;
        _gameStarted = true;
    }

    private void desactivateSequence(int seq)
    {
        foreach (BGKey key in sequences[seq])
        {
            key.gameObject.SetActive(false);
        }
    }

    private void activateSequence(int seq)
    {
        foreach (BGKey key in sequences[seq])
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
