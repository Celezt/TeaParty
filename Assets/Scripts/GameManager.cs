using Celezt.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DialogueSystem _system;
    [SerializeField] private DialogueUI _ui;
    [SerializeField] private Dialogue _dialogue;

    private void Start()
    {
        _ui.Binder = _system.Binder;

        if (_dialogue != null)
        {
            _system.CreateDialogue(_dialogue, "ID");
        }
    }
}
