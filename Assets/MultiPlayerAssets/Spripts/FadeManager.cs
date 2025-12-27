using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private Image _coverScreen;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private Ease _ease;

    private readonly CompositeMotionHandle _handles = new(1);

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn(System.Action fadeEndCallback = null)
    {
        _handles.Cancel();
        LMotion.Create(1f, 0f, _fadeDuration)
            .WithEase(_ease)
            .WithOnComplete(() => 
                {
                    _coverScreen.gameObject.SetActive(false);
                    fadeEndCallback?.Invoke();
                })
            .BindToColorA(_coverScreen)
            .AddTo(_handles);
    }

    public void FadeOut(System.Action fadeEndCallback = null)
    {
        _handles.Cancel();
        _coverScreen.gameObject.SetActive(true);
        LMotion.Create(0f, 1f, _fadeDuration)
            .WithEase(_ease)
            .WithOnComplete(() => fadeEndCallback?.Invoke())
            .BindToColorA(_coverScreen)
            .AddTo(_handles);
    }

    private void OnDestroy()
    {
        _handles.Cancel();
    }
}
