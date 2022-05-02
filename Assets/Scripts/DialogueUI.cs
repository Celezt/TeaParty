using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Celezt.DialogueSystem;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private CanvasGroup _top;
    [SerializeField] private CanvasGroup _bottom;
    [SerializeField] private TextMeshProUGUI _topText;
    [SerializeField] private TextMeshProUGUI _bottomText;
    [SerializeField] private TextMeshProUGUI _topActor;
    [SerializeField] private TextMeshProUGUI _bottomActor;

    private DialogueSystemBinder _binder;

    private enum Order
    {
        Top,
        Bottom,
    }

    private void Awake()
    {
        if (_binder == null)
            Initialize();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (_binder == null)
            Initialize();
#endif
    }

    private void Initialize()
    {
        _binder = GetComponent<DialogueSystemBinder>();

        Refresh();

        _binder.OnCreateTrackMixer.AddListener(() => Refresh());
        _binder.OnEnterClip.AddListener(OnEnterClip);
        _binder.OnExitClip.AddListener(OnExitClip);
    }

    private void OnNextButtonPressed()
    {

    }

    private void OnEnterClip(DialogueSystemBinder.Callback callback)
    {
        if (callback.Behaviour is ParagraphBehaviour behaviour)
        {
            if (callback.Binder[0].Mixer.IsAvailable ||
               (callback.UserData != null && (Order)callback.UserData == Order.Bottom))   // The first is currently not playing anything or is currently the bottom.
            {
                _bottom.alpha = 1;
                _bottomText.text = behaviour.Text;
                _bottomActor.text = behaviour.Actor;
                callback.UserData = Order.Bottom;
            }
            else
            {
                _top.alpha = 1;
                _topText.text = behaviour.Text;
                _topActor.text = behaviour.Actor;
                callback.UserData = Order.Top;
            }
        }
    }

    private void OnExitClip(DialogueSystemBinder.Callback callback)
    {
        if (callback.UserData == null)
            return;

        switch ((Order)callback.UserData)
        {
            case Order.Bottom:
                _bottom.alpha = 0;
                _bottomText.text = null;
                _bottomActor.text = null;
                break;
            case Order.Top:
                _top.alpha = 0;
                _topText.text = null;
                _topActor.text = null;
                break;
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

        for (int i = 0; i < _binder.TrackCount; i++)
            _binder.SetUserData(i, null);
    }
}
