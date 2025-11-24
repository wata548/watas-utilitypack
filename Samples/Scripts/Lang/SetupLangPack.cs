using System.IO;
using System.Text.RegularExpressions;
using Extension.Test;
using Lang;
using UnityEngine;

namespace Lang {
    public class SetupLangPack: MonoBehaviour {

        [SerializeField]
        private TMP_LangText _target;

        [TestMethod(pRuntimeOnly: true)]
        private void ChangeLangPack(Language pLang = Language.English) {
            LanguageManager.LangPack = pLang;
        }

        [TestMethod]
        private void ChangeContext(string pContext) => 
            _target.Text = pContext;
        
        private void Awake() {
            LanguageManager.MainLang = Language.English;
            LanguageManager.LangPack = Language.Korean;
            var context = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "LangPack/LangPack.csv"));
            LanguageManager.Table = new CSVPack(context, LanguageManager.MainLang);
        }
    }
}