using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SelectableUI {
    public static class SelectUIManager {

        private static Selectable _focus;

        public static void SetFocus(Selectable pUI) {
            _focus = pUI;
            _focus.GetComponent<SelectEffector>()?.OnSelected();
        }
        
        public static void MoveFocus(Vector2Int pInput) {
            if (_focus == null)
                return;
            if ((pInput.x == 0) == (pInput.y == 0))
                return;
            
            var temp = pInput switch {
                { x: > 0 } =>
                    _focus.FindSelectableOnRight(),
                { x: < 0 } =>
                    _focus.FindSelectableOnLeft(),
                { y: > 0 } =>
                    _focus.FindSelectableOnUp(),
                { y: < 0 } =>
                    _focus.FindSelectableOnDown(),
                _ =>
                    null
            };
            
            if (temp == null)
                return;
            
            _focus.GetComponent<SelectEffector>()?.OnUnSelected();
            _focus = temp;
            _focus.GetComponent<SelectEffector>()?.OnSelected();
        } 
    }
}