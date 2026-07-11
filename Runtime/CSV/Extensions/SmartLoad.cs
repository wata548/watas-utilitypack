using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CSVData.Extensions {
    public static partial class SpreadSheet {
        /// <summary>
        /// <p> It depends on csv window's parameter(in editor not runtime)</p>
        /// <p> This function is only workable when CSV save path in Resource directory </p>
        /// <p> CSV window's 'save data path' </p>
        /// </summary>
        /// <param name="pSheet">FileName / Spread sheet's sheet name</param>
        /// <param name="pDirectory">directory path(Resources/{pDirectory})</param>
        public static List<T> SmartLoad<T>(string pSheet, string pDirectory = "") {
            return CSV.DeserializeToList<T>(SmartLoad(pSheet, pDirectory));
        }
        /// <summary>
        /// <p> It depends on csv window's parameter(in editor not runtime)</p>
        /// <p> This function is only workable when CSV save path in Resource directory </p>
        /// <p> CSV window's 'save data path' </p>
        /// </summary>
        /// <param name="pSheet">FileName / Spread sheet's sheet name</param>
        /// <param name="pDirectory">directory path(Resources/{pDirectory})</param>
        public static List<List<string>> SmartLoad(string pSheet, string pDirectory = "") {
            bool isAutoSave = false;
            var context = new List<List<string>>();
#if UNITY_EDITOR
            if (UnityEditor.EditorUserSettings.GetConfigValue("CSV_AutoSave") == "T") {
                isAutoSave = true;
                var envPath = Path.Combine(Application.dataPath, UnityEditor.EditorUserSettings.GetConfigValue("CSV_EnvPath"));
                var ssKey = File.ReadAllText(envPath);
                var ssPath = UnityEditor.EditorUserSettings.GetConfigValue("CSV_SheetId");
                var savePath = Path.Combine(Application.dataPath, UnityEditor.EditorUserSettings.GetConfigValue("CSV_SavePath"));
                context = LoadData(ssPath, pSheet, ssKey);
                var data = CSV.Serialize(context);
                File.WriteAllText(Path.Join(savePath, $"{pSheet}.csv"), data);
            }
#endif
            if (!isAutoSave) {
                var resource = Resources.Load<TextAsset>(Path.Join(pDirectory, pSheet));
                context = CSV.Parse(resource.text);    
            }
            return context;

        }
    }
}
