using System.Collections.Generic;
using System.Linq;
using CSVData;

namespace Lang {
    public class CSVPack: ILangTable {

        private readonly IReadOnlyDictionary<string, List<string>> _table;
        
        public CSVPack(string pContext, Language pMainLanguage) {
            _table = CSV.Parse(pContext).ToDictionary(row => row[(int)pMainLanguage], row => row);
            
        }

        public string Text(Language pLang, string pContext) {
            if(_table.ContainsKey(pContext) && _table[pContext].Count > (int)pLang)
                return _table[pContext][(int)pLang];
            return pContext;
        }
    }
}