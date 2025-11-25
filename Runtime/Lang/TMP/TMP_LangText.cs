using Extension.Test;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace Lang {
    
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_LangText: MonoBehaviour {

        public static Dictionary<Language, TMP_FontAsset> Fonts { private get; set; }
        private static TMP_FontAsset GetFont(Language pLanguage, Language pMainLanguage = Language.English) {
            if (Fonts.TryGetValue(pLanguage, out var font))
                return font;
            if (Fonts.TryGetValue(pMainLanguage, out font))
                return font;
            return null;
        }
            
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
           var font = GetFont(LanguageManager.LangPack, LanguageManager.MainLang);
           if (font != null)
               _text.font = font;
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