using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometor : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _maxSpeedText, _currentSpeedText;
    [SerializeField] private Car _car;


    private string _currentSpeedDescription;


    private void Awake()
    {
        _maxSpeedText.text += _car.MaxSpeed.ToString("0.0");
        _currentSpeedDescription = _currentSpeedText.text;
        _car.OnSpeedChange += UpdateSpeedText;
    }


    private void UpdateSpeedText(float newSpeed)
    {
        _currentSpeedText.text = _currentSpeedDescription + newSpeed.ToString("0.0");
    }
}
