using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Celezt.DialogueSystem;
using TMPro;
using UnityEngine.UI;
using System.Linq;

[ExecuteInEditMode]
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueSystemBinder _binder;
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private CanvasGroup _top;
    [SerializeField] private CanvasGroup _bottom;
    [SerializeField] private TextMeshProUGUI _topText;
    [SerializeField] private TextMeshProUGUI _bottomText;
    [SerializeField] private TextMeshProUGUI _topActor;
    [SerializeField] private TextMeshProUGUI _bottomActor;
    [SerializeField] private CanvasGroup _actions;
    [SerializeField] private float _textScroll = 1.0f;

    private bool _isInitialized;

    private void Awake()
    {
        if (_isInitialized == false)
            Initialize();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (_isInitialized == false)
            Initialize();
#endif
    }

    private void Initialize()
    {
        _isInitialized = true;

        Refresh();

        _binder.OnCreateTrackMixer.AddListener(() => Refresh());
        _binder.OnEnterClip.AddListener(OnEnterClip);
        _binder.OnProcessClip.AddListener(OnProcessClip);
        _binder.OnExitClip.AddListener(OnExitClip);
    }

    private void OnEnterClip(DialogueSystemBinder.Callback callback)
    {
        if (callback.Behaviour is DialogueBehaviour behaviour)
        {
            if (callback.Index == 0)   // The first is currently not playing anything or is currently the bottom.
            {
                _bottom.alpha = 1;
                _bottomText.text = behaviour.Text;
                _bottomActor.text = behaviour.Actor;
            }
            else
            {
                _top.alpha = 1;
                _topText.text = behaviour.Text;
                _topActor.text = behaviour.Actor;
            }
        }
    }

    private void OnProcessClip(DialogueSystemBinder.Callback callback)
    {
        if (callback.Behaviour is DialogueBehaviour behaviour)
        {
            double startTime = callback.Start;
            double endTime = callback.End;
            double currentTime = callback.Time;

            float percentage = Mathf.Clamp01((float)((currentTime - startTime) / (endTime - _textScroll - startTime)));

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
    }
}
