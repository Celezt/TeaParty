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
    [SerializeField] private AudioSource[] _audioSource;
    [SerializeField] private SerializableDictionary<string, SpriteRenderer> _sprites = new SerializableDictionary<string, SpriteRenderer>();

    private bool _isInitialized;
    private int _oldTopVisibleCharacterCount;
    private int _oldBottomVisibleCharacterCount;

    public void Clear()
    {
        Refresh();
        
        foreach (var sprite in _sprites.Values)
        {
            sprite.enabled = false;
        }
    }

    public void Refresh()
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

    public void UpdateBinder()
    {
        if (_binder == null)
            return;

        _binder.OnEnterDialogueClip.RemoveListener(OnEnterClip);
        _binder.OnProcessDialogueClip.RemoveListener(OnProcessClip);
        _binder.OnExitDialogueClip.RemoveListener(OnExitClip);
        _binder.OnDeleteTimeline.RemoveListener(Refresh);
        _binder.OnEnterDialogueClip.AddListener(OnEnterClip);
        _binder.OnProcessDialogueClip.AddListener(OnProcessClip);
        _binder.OnExitDialogueClip.AddListener(OnExitClip);
        _binder.OnDeleteTimeline.AddListener(Refresh);
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
                if (asset.Actor != "You")
                {
                    if (_sprites.TryGetValue(asset.Actor, out SpriteRenderer sprite))
                    {
                        sprite.enabled = true;
                    }

                    foreach (var (name, hideSprite) in _sprites.Where(x => x.Key.Trim() != asset.Actor))
                    {
                        hideSprite.enabled = false;
                    }
                }

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
                int visibleCharacterCount = Mathf.CeilToInt(_bottomText.textInfo.characterCount * percentage);
                _bottomText.maxVisibleCharacters = visibleCharacterCount;


                if (visibleCharacterCount != _oldBottomVisibleCharacterCount)
                {
                    _audioSource[1].PlayOneShot(_audioSource[1].clip);
                }

                _oldBottomVisibleCharacterCount = visibleCharacterCount;
            }
            else
            {
                int visibleCharacterCount = Mathf.CeilToInt(_topText.textInfo.characterCount * percentage);
                _topText.maxVisibleCharacters = visibleCharacterCount;

                if (visibleCharacterCount != _oldTopVisibleCharacterCount)
                {
                    _audioSource[1].PlayOneShot(_audioSource[0].clip);
                }

                _oldTopVisibleCharacterCount = visibleCharacterCount;
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
}
