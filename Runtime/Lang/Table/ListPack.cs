using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Lang {
    public class ListPack: ILangTable {

       //==================================================||Fields 
        private readonly IReadOnlyDictionary<string, string[]> _table;
        private readonly IReadOnlyList<Language> _allowLanguages;
        
       //==================================================||Constructors 
        public ListPack(IEnumerable<MatchLangToContext> pPacks, Language pMainLanguage) {

            var packs = pPacks.Select(pack => (pack.Language, ToWords(pack.Context)));
            var length = Enum.GetValues(typeof(Language)).Length;
            var table = new Dictionary<string, string[]>();
            var mainPack = packs.First(pack => pack.Item1 == pMainLanguage).Item2.ToList();
            foreach (var word in mainPack) {
                table.Add(word, new string[length]);
                table[word][(int)pMainLanguage] = word;
            }

            var allowLanguages = new List<Language>();
            foreach (var pack in packs) {
                allowLanguages.Add(pack.Item1);
                
                if(pack.Item1 == pMainLanguage)
                    continue;

                var idx = 0;
                foreach (var word in pack.Item2) {
                    var key = mainPack[idx];
                    table[key][(int)pack.Item1] = word;
                    idx++;
                }
                
            }

            _table = table;
            _allowLanguages = allowLanguages;
            
            IEnumerable<string> ToWords(string pContext) =>
                pContext.Split('\n').Select(line => line.Replace("\\n", "\n"));
        }
        
       //==================================================||Methods 
        public static ListPack Generate(string pFormatPath, Language pMainLanguage) {
            
            var result = new List<MatchLangToContext>();
            foreach (var lang in (Language[])Enum.GetValues(typeof(Language))) {
                var path = string.Format(pFormatPath, lang);
                if (!File.Exists(path))
                    continue;

                var context = File.ReadAllText(path);
                result.Add(new(context, lang));
            }
            return new ListPack(result, pMainLanguage);
        }
 
        public string Text(Language pLang, string pContext) {
            if(_table.TryGetValue(pContext, out string[] value))
                return value[(int)pLang];
            return pContext;
        }

        public IEnumerable<Language> AllowLanguages() =>
            _allowLanguages;

        //==================================================||Datas 
        public struct MatchLangToContext {
            public string Context;
            public Language Language;
            
            public MatchLangToContext(string pContext, Language pLanguage) {
                Context = pContext;
                Language = pLanguage;
            }
        }
    }
}