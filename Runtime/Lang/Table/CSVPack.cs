using System;
using System.Collections.Generic;
using System.Linq;
using CSVData;

namespace Lang {
    public class CSVPack: ILangTable {

        //==================================================||Fields 
        private readonly Dictionary<string, string[]> _table = new();
        private List<Language> _allowLanguages = new();
        private Language _mainLanguage; 
        
        //==================================================||Constructors 
        public CSVPack(Language pMainLanguage) => 
            _mainLanguage = pMainLanguage;

        //==================================================||Methods 
        public void Add(string pContext) =>
            Add(CSV.Parse(pContext));

        public void Add(List<List<string>> pContext) {
            var packs = pContext.Select(row => ((Language)Enum.Parse(typeof(Language), row[0]), row.Skip(1)));
            if (packs.All(pack => pack.Item1 != _mainLanguage))
                return;

            var length = Enum.GetValues(typeof(Language)).Length;
            var mainPack = packs.First(pack => pack.Item1 == _mainLanguage).Item2.ToList();
            foreach (var word in mainPack) {
                _table.TryAdd(word, new string[length]);
                _table[word][(int)_mainLanguage] = word;
            }

            foreach (var pack in packs) {
                _allowLanguages.Add(pack.Item1);
                
                if(pack.Item1 == _mainLanguage)
                    continue;

                var idx = 0;
                foreach (var word in pack.Item2) {
                    var key = mainPack[idx];
                    _table[key][(int)pack.Item1] = word;
                    idx++;
                }
            }
            _allowLanguages = _allowLanguages.Distinct().OrderBy(lang => lang).ToList();
        }

        public string Text(Language pLang, string pContext) {
            if(_table.TryGetValue(pContext, out string[] value))
                return value[(int)pLang];
            return pContext;
        }

        public IEnumerable<Language> AllowLanguages() => 
            _allowLanguages;
    }
}