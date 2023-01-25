using UnityEngine;


public class InterpolatedTransformUpdater : MonoBehaviour
{
    private IInterpolatedTransform _interpolatedTransform;


    private void Awake()
    {
        _interpolatedTransform = GetComponent<IInterpolatedTransform>();
    }
    private void FixedUpdate()
    {
        _interpolatedTransform.LateFixedUpdate();
    }
}
