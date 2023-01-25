using UnityEngine;


[RequireComponent(typeof(InterpolatedTransformUpdater))]
public class InterpolatedTransform : MonoBehaviour, IInterpolatedTransform
{
    [SerializeField] private Transform _view;


    private TransformData[] _lastTransforms;
    private int _newTransformIndex;
    private int _oldTransformIndex => _newTransformIndex == 0 ? 1 : 0;


    private void OnEnable()
    {
        ForgetPreviousTransforms();
    }
    private void FixedUpdate()
    {
        SetTransform(_view,_lastTransforms[_newTransformIndex]);
    }
    private void Update()
    {
        SetInterpolatedTransform(_view, _lastTransforms[_oldTransformIndex], _lastTransforms[_newTransformIndex]);
    }


    public void ForgetPreviousTransforms()
    {
        _lastTransforms = new TransformData[2];
        TransformData data = new TransformData(transform);
        _lastTransforms[0] = data;
        _lastTransforms[1] = data;

        _newTransformIndex = 0;
    }
    public void LateFixedUpdate()
    {
        _newTransformIndex = _oldTransformIndex;
        _lastTransforms[_newTransformIndex] = new TransformData(transform);
    }


    private static void SetTransform(Transform target, TransformData data)
    {
        target.position = data.position;
        target.rotation = data.rotation;
    }
    private static void SetInterpolatedTransform(Transform target, TransformData olderData, TransformData newestData)
    {
        target.position = Vector3.Lerp(olderData.position, newestData.position, IntepolationController.InterpolationFactor);
        target.rotation = Quaternion.Slerp(olderData.rotation, newestData.rotation, IntepolationController.InterpolationFactor);
    }
    private struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformData(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
        }
    }
}
