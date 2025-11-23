using System;
using SelectableUI;
using UnityEngine;
using UnityEngine.UI;

namespace SelectableUI {
    public class SelectableUIInput: MonoBehaviour {

        [SerializeField] private Selectable _defaultFocus;

        private void OnEnable() {
            SelectUIManager.SetFocus(_defaultFocus);
        }

        private void Update() {
            var result = Vector2Int.zero;
            result.x = (int)UnityEngine.Input.GetAxisRaw("Horizontal");
            result.y = (int)UnityEngine.Input.GetAxisRaw("Vertical");
            SelectUIManager.MoveFocus(result);
        }
    }
}