using UnityEngine;
using UnityEngine.UI;

namespace SelectableUI {
    [RequireComponent(typeof(Selectable))]
    public abstract class SelectEffector : MonoBehaviour {
        public virtual void OnSelected(){}
        public virtual void OnUnSelected(){}
    }
}