using System.Collections.Generic;

namespace Lang {
    public interface ILangTable {
        string Text(Language pLang, string pContext);
        IEnumerable<Language> AllowLanguages();
    }   
}

