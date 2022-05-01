using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using Celezt.DialogueSystem;
using System;
using System.Linq;

[ExecuteInEditMode]
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private PlayableDirector _director;

    private Button _nextButton;
    private VisualElement _top;
    private VisualElement _bottom;
    private Label _topText;
    private Label _bottomText;
    private VisualElement _root;
    [NonSerialized]
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
        _root = GetComponent<UIDocument>().rootVisualElement;
        _binder = GetComponent<DialogueSystemBinder>();

        _nextButton = _root.Q<Button>("Next");
        _top = _root.Q<VisualElement>("Top");
        _bottom = _root.Q<VisualElement>("Bottom");
        _topText = _top.Q<Label>("Text");
        _bottomText = _bottom.Q<Label>("Text");

        _top.style.visibility = Visibility.Hidden;
        _bottom.style.visibility = Visibility.Hidden;
        _topText.text = "";
        _bottomText.text = "";

        _nextButton.clicked += OnNextButtonPressed;
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
                _bottom.style.visibility = Visibility.Visible;
                _bottomText.text = behaviour.Text;
                callback.UserData = Order.Bottom;
            }
            else
            {
                _top.style.visibility = Visibility.Visible;
                _topText.text = behaviour.Text;
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
                _bottom.style.visibility = Visibility.Hidden;
                _bottomText.text = "";
                break;
            case Order.Top:
                _top.style.visibility = Visibility.Hidden;
                _topText.text = "";
                break;
        }
    }
}
