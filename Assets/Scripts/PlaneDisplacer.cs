using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Events;

public class PlaneDisplacer : MonoBehaviour
{
    [HideInInspector] public UnityEvent FinishDisplacementEvent = new();

    [SerializeField] private SplineFollower _splineFollower;
    [SerializeField] private float _offsetXDuration;
    [SerializeField] private Ease _offsetXEasing;
    [SerializeField] private float _offsetYDuration;
    [SerializeField] private Ease _offsetYEasing;
    [SerializeField] private float _offsetZDuration;
    [SerializeField] private Ease _offsetZEasing;
    [SerializeField] private float _startFollowSpeed;
    [SerializeField] private float _targetFollowSpeed;

    private const int _yOffsetIndex = 0;
    private const int _xOffsetIndex = 1;

    private Tween _offsetXTween;
    private Tween _offsetYTween;
    private Tween _offsetZTween;

    public void HandleSpawn(SplineComputer splineComputer, float percent)
    {
        _splineFollower.spline = splineComputer;
        _splineFollower.RebuildImmediate();
        _splineFollower.SetPercent(percent);

        _splineFollower.offsetModifier.keys[_yOffsetIndex].blend = 1f;
        _offsetXTween = DOTween.To(
                () => _splineFollower.offsetModifier.keys[_xOffsetIndex].blend,
                (value) => _splineFollower.offsetModifier.keys[_xOffsetIndex].blend = value,
                -1f,
                _offsetXDuration
            )
            .SetEase(_offsetXEasing)
            .SetLoops(-1, LoopType.Yoyo)
            .Play();

        _splineFollower.offsetModifier.keys[_yOffsetIndex].blend = 1f;
        _offsetYTween = DOTween.To(
                () => _splineFollower.offsetModifier.keys[_yOffsetIndex].blend,
                (value) => _splineFollower.offsetModifier.keys[_yOffsetIndex].blend = value,
                0f,
                _offsetYDuration
            )
            .SetEase(_offsetYEasing)
            .OnComplete(() => {
                if (_offsetYDuration < _offsetZDuration)
                    FinishDisplacementEvent.Invoke();
            })
            .Play();

        _splineFollower.followSpeed = _startFollowSpeed;
        _offsetZTween = DOTween.To(
                () => _splineFollower.followSpeed,
                (value) => _splineFollower.followSpeed = value,
                _targetFollowSpeed,
                _offsetZDuration
            )
            .SetEase(_offsetZEasing)
            .OnComplete(() => {
                if (_offsetZDuration < _offsetYDuration)
                    FinishDisplacementEvent.Invoke();
            })
            .Play();

        _splineFollower.enabled = true;
    }

    public void HandleDestroying()
    {
        _splineFollower.enabled = false;
        _offsetXTween.Kill();
        _offsetYTween.Kill();
        _offsetZTween.Kill();
    }
}
