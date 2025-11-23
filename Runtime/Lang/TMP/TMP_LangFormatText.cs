namespace Lang {
    public class TMP_LangFormatText: TMP_LangText {

       //==================================================||Fields 
        private object[] _params;

       //==================================================||Methods 
        private void Apply() => Apply(_params);
        
        public void Apply(params object[] pParams) {
            _params = pParams;
            base.text = string.Format(text, _params);
        }

        protected override void Refresh() {
           base.Refresh(); 
           Apply();
        }  
    }
}