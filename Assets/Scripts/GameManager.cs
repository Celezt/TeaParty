using Celezt.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DialogueSystem _system;
    [SerializeField] private DialogueUI _ui;
    [SerializeField] private Dialogue _dialogue;

    private void Start()
    {
        _ui.Binder = _system.Binder;

        _system.Buttons.AddRange(_ui.GetComponentsInChildren<Button>());    // Give all buttons.

        if (_dialogue != null)
        {
            _system.CreateDialogue(_dialogue, "ID");
        }
    }
}
