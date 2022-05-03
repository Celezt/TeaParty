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

    private CanvasGroup[] _actionButtons = new CanvasGroup[6];

    private bool _isInitialized;

    private enum Order
    {
        None,
        Top,
        Bottom,
    }

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

#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnReaload()
    {
        foreach (var ui in FindObjectsOfType<DialogueUI>())
        {
            if (ui._isInitialized == false)
                ui.Initialize();
        }
    }
#endif

    private void Initialize()
    {
        _isInitialized = true;

        CanvasGroup[] children = _actions.GetComponentsInChildren<CanvasGroup>();
        for (int i = 0; i < _actionButtons.Length; i++)
            _actionButtons[i] = children[i];

        Refresh();

        _binder.OnCreateTrackMixer.AddListener(() => Refresh());
        _binder.OnEnterClip.AddListener(OnEnterClip);
        _binder.OnExitClip.AddListener(OnExitClip);
    }

    private void OnEnterClip(DialogueSystemBinder.Callback callback)
    {
        if (callback.Behaviour is DialogueBehaviour behaviour)
        {
            if (!callback.Binder.UserData.Any(x => (Order)x == Order.Bottom))   // The first is currently not playing anything or is currently the bottom.
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

        callback.UserData = Order.None;
    }

    private void Refresh()
    {
        _top.alpha = 0;
        _bottom.alpha = 0;
        _topText.text = null;
        _bottomText.text = null;
        _topActor.text = null;
        _bottomActor.text = null;

        foreach (CanvasGroup child in _actionButtons)
            child.alpha = 0;

        for (int i = 0; i < _binder.TrackCount; i++)
            _binder.SetUserData(i, Order.None);
    }
}
