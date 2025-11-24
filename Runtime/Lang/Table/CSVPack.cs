using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSVData;

namespace Lang {
    public class CSVPack: ILangTable {

        //==================================================||Fields 
        private readonly IReadOnlyDictionary<string, string[]> _table;
        private readonly IEnumerable<Language> _allowLanguages;
        
        //==================================================||Constructors 
        public CSVPack(string pPath): this(File.ReadAllText(pPath), LanguageManager.MainLang) { }
        public CSVPack(string pContext, Language pMainLanguage): this(CSV.Parse(pContext), pMainLanguage) {}
        public CSVPack(List<List<string>> pContext, Language pMainLanguage) {
            var packs = pContext.Select(row => ((Language)Enum.Parse(typeof(Language), row[0]), row.Skip(1)));
            if (packs.All(pack => pack.Item1 != pMainLanguage))
                return;

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
        }

        public IEnumerable<Language> AllowLanguages() =>
            _allowLanguages;
        //==================================================||Methods 
        public string Text(Language pLang, string pContext) {
            if(_table.TryGetValue(pContext, out string[] value))
                return value[(int)pLang];
            return pContext;
        }

    }
}