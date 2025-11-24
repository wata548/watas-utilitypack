using Extension.Test;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lang {
    
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_LangText: MonoBehaviour { 
        
        //==================================================||Fields
        private TMP_Text _text;
        private string _context;
        
       //==================================================||Properties 
        public string Text {
            get => text;
            set => text = value;
        }
        
        public string text {
            get => _text.text;
            set {
                _context = value;
                _text.text = _context.ApplyLang();
            }
        }
        
       //==================================================||Methods 
       protected virtual void Refresh() {
           _text.text = _context.ApplyLang();
       }

       [TestMethod]
       private void Change(string pContext) {
           text = pContext;
       }  
       
       //==================================================||Unity 
        private void OnBecameVisible() {
            Refresh();
        }

        private void Start() {
            _text = GetComponent<TMP_Text>();
            _context = _text.text;
        }
        
        private void Update() {
            if(Time.frameCount == LanguageManager.NeedUpdateFrame)
                Refresh();
        }
    }
}