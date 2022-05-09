using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Celezt.DialogueSystem;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

[ExecuteInEditMode]
public class DialogueUI : MonoBehaviour
{
    public DialogueSystemBinder Binder
    {
        get => _binder;
        set
        {
            if (_binder != value)
            {
                _binder = value;
                UpdateBinder();
            }
        }
    }

    [SerializeField] private DialogueSystemBinder _binder;
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private CanvasGroup _top;
    [SerializeField] private CanvasGroup _bottom;
    [SerializeField] private TextMeshProUGUI _topText;
    [SerializeField] private TextMeshProUGUI _bottomText;
    [SerializeField] private TextMeshProUGUI _topActor;
    [SerializeField] private TextMeshProUGUI _bottomActor;
    [SerializeField] private CanvasGroup _actions;

    private bool _isInitialized;

    public void UpdateBinder()
    {
        if (_binder == null)
            return;

        _binder.OnEnterClip.RemoveListener(OnEnterClip);
        _binder.OnProcessClip.RemoveListener(OnProcessClip);
        _binder.OnExitClip.RemoveListener(OnExitClip);
        _binder.OnEnterClip.AddListener(OnEnterClip);
        _binder.OnProcessClip.AddListener(OnProcessClip);
        _binder.OnExitClip.AddListener(OnExitClip);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (_isInitialized == false)
        {
            Refresh();
            _isInitialized = true;
        }
#endif
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void CreateAssetWhenReady()
    {
        foreach (var ui in FindObjectsOfType<DialogueUI>())
            ui.Refresh();
    }
#endif

    private void OnEnterClip(DialogueSystemBinder.Callback callback)
    {
        if (callback.Asset is DialogueAsset asset)
        {
            if (callback.Index == 0)   // The first is currently not playing anything or is currently the bottom.
            {
                _bottom.alpha = 1;
                _bottomText.text = asset.Text;
                _bottomActor.text = asset.Actor;
            }
            else
            {
                _top.alpha = 1;
                _topText.text = asset.Text;
                _topActor.text = asset.Actor;
            }
        }
    }

    private void OnProcessClip(DialogueSystemBinder.Callback callback)
    {
        if (callback.Asset is DialogueAsset asset)
        {
            double startTime = callback.Start;
            double endTime = callback.End;
            double currentTime = callback.Time;

            float percentage = Mathf.Clamp01((float)((currentTime - startTime) / (endTime - asset.EndOffset - startTime)));

            if (callback.Index == 0)
            {
                _bottomText.maxVisibleCharacters = Mathf.CeilToInt(_bottomText.textInfo.characterCount * percentage);
            }
            else
            {
                _topText.maxVisibleCharacters = Mathf.CeilToInt(_topText.textInfo.characterCount * percentage);
            }
        }
    }

    private void OnExitClip(DialogueSystemBinder.Callback callback)
    {
        if (callback.Index == 0)
        {
            _bottom.alpha = 0;
            _bottomText.text = null;
            _bottomActor.text = null;
        }
        else
        {
            _top.alpha = 0;
            _topText.text = null;
            _topActor.text = null;
        }
    }

    private void Refresh()
    {
        _top.alpha = 0;
        _bottom.alpha = 0;
        _topText.text = null;
        _bottomText.text = null;
        _topActor.text = null;
        _bottomActor.text = null;

        _topText.maxVisibleCharacters = 0;
        _bottomText.maxVisibleCharacters = 0;

        UpdateBinder();
    }
}
