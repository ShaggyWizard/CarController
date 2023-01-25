using UnityEngine;


public class IntepolationController : MonoBehaviour
{
    public static float InterpolationFactor => _interpolationFactor;


    private static float _interpolationFactor;


    private float[] _lastFixedUpdateTimes;
    private int _newTimeIndex;
    private int _oldTimeIndex => _newTimeIndex == 0 ? 1 : 0;


    public void Start()
    {
        _lastFixedUpdateTimes = new float[2];
        _newTimeIndex = 0;
    }
    private void FixedUpdate()
    {
        _newTimeIndex = _oldTimeIndex;
        _lastFixedUpdateTimes[_newTimeIndex] = Time.fixedTime;
    }
    private void Update()
    {
        float newerTime = _lastFixedUpdateTimes[_newTimeIndex];
        float olderTime = _lastFixedUpdateTimes[_oldTimeIndex];

        if (newerTime != olderTime)
            _interpolationFactor = (Time.time - newerTime) / (newerTime - olderTime);
        else
            _interpolationFactor = 1;
    }
}
