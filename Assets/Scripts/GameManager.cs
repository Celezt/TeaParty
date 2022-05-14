using Celezt.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DialogueSystem _system;
    [SerializeField] private DialogueUI _ui;
    [SerializeField] private CanvasGroup _gameplayCanvas;
    [SerializeField] private string _inputID = "default";
    [SerializeField] private List<Dialogue> _dialogues = new List<Dialogue>();

    private void Start()
    {
        _ui.Binder = _system.Binder;

        _system.Buttons.AddRange(_gameplayCanvas.GetComponentsInChildren<ButtonBinder>(true));    // Give all buttons.
    }

    public void StartDialogue(int index)
    {
        if (_dialogues != null)
        {
            _system.CreateDialogue(_dialogues[index], _inputID);
            _system.Play();
        }
    }

    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
