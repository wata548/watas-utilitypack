using UnityEngine;

namespace Lang {
    public static class LanguageManager {
        
        
       //==================================================||Fields 
       private static ILangTable _table;
       private static Language _mainLang = Language.English;
       private static Language _langPack = Language.English; 
        
       //==================================================||Properties 
       public static ILangTable Table {
           get => _table;
           set {
               _table = value;
               NeedUpdateFrame = Time.frameCount + 1;   
           }
       }
       
       public static Language MainLang {
           get => _mainLang;
           set {
               _mainLang = value;
               _langPack = _mainLang;
           }
       }

       public static int NeedUpdateFrame { get; private set; }
       
        public static Language LangPack {
            get => _langPack;
            set {
                if(_langPack == value)
                    return;
                _langPack = value;
                NeedUpdateFrame = Time.frameCount + 1;
                Debug.Log($"Changed: {_langPack}");
            }
        }

       //==================================================||Methods 

        public static string ApplyLang(this string pContext) =>
            _table.Text(LangPack, pContext);
    }
}