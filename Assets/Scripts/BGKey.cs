using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BGKey : MonoBehaviour
{
    // shake duration
    public float shakeTime = 0.3f;
    // max displacement distance in one axis (x, y)
    public float shakeDisplacementStrength = 0.05f;
    // the number of displacement in shakeTime
    public int shakeNumber = 10;

    public TextMeshProUGUI KeyText;
    private SpriteRenderer _bg;

    private bool _shakeStarted = false;
    public KeyCode Key;

    private void Awake()
    {
        _bg = GetComponent<SpriteRenderer>();
    }

    public void ActivateWithKey(KeyCode key, Vector3 position)
    {
        _bg.color = Color.gray;
        KeyText.text = key.ToString();
        transform.position = position;
        Key = key;
    }

    public void Deactivate()
    {
    }

    public void validateKey()
    {
        _bg.color = Color.green;
    }

    public void startShake()
    {
        if (_shakeStarted)
            return;
        StartCoroutine(shake());
    }

    public IEnumerator shake()
    {
        float currentShakeTime = shakeTime;
        Vector3 startPosition = transform.position;
        int remainingShakes = shakeNumber;
        _shakeStarted = true;

        while (remainingShakes > 0)
        {
            currentShakeTime -= Time.deltaTime;
            if ((shakeNumber * currentShakeTime) / shakeTime < remainingShakes)
            {
                float xDisplacement = Random.Range(startPosition.x - shakeDisplacementStrength, startPosition.x + shakeDisplacementStrength);
                float yDisplacement = Random.Range(startPosition.y - shakeDisplacementStrength, startPosition.y + shakeDisplacementStrength);

                transform.position = new Vector3(xDisplacement, yDisplacement, 0);
                remainingShakes -= 1;
            }
            yield return new WaitForEndOfFrame();
        }

        transform.position = startPosition;
        _shakeStarted = false;
        Debug.Log("end shake");
    }
}
