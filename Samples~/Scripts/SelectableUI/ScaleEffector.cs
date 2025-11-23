using SelectableUI;
using UnityEngine;

namespace SelectableUI {
    public class ScaleEffector: SelectEffector {
        
        public override void OnSelected() {
            transform.localScale = Vector3.one * 1.5f;
        }

        public override void OnUnSelected() {
            transform.localScale = Vector3.one;
        }
    }
}