#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSVData.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace CSVData {
    public class CSVWindow: EditorWindow {
        private SerializedObject _serializedObject;
        private SerializedProperty _serializedProperty;
        
        [FormerlySerializedAs("_targetPage")] [SerializeField]
        private List<string> _targetSheet = new();
        
        [MenuItem("Window/Utilities/CSV")]
        private static void Init() {
            var window = GetWindow<CSVWindow>("CSV Title");
            window.Show();
            if(String.IsNullOrEmpty(EditorUserSettings.GetConfigValue("CSV_EnvPath")))
                EditorUserSettings.SetConfigValue("CSV_EnvPath", ".env");
            if(String.IsNullOrEmpty(EditorUserSettings.GetConfigValue("CSV_SavePath")))
                EditorUserSettings.SetConfigValue("CSV_SavePath", "Resources");
            window._targetSheet = EditorUserSettings.GetConfigValue("CSV_TargetSheet")
                .Split(',')
                .ToList();
        }

        private void OnEnable() {
            _serializedObject = new SerializedObject(this);
            _serializedProperty = _serializedObject.FindProperty("_targetSheet");
        }

        private void Load(bool pGenerateCode = false) {
            var envPath = Path.Combine(Application.dataPath, EditorUserSettings.GetConfigValue("CSV_EnvPath"));
            var ssKey = File.ReadAllText(envPath);
            var ssPath = EditorUserSettings.GetConfigValue("CSV_SheetId");
            var savePath = Path.Combine(Application.dataPath, EditorUserSettings.GetConfigValue("CSV_SavePath"));
            foreach (var sheet in _targetSheet) {
                if(string.IsNullOrWhiteSpace(sheet))
                    continue;
                var data = SpreadSheet.LoadData(ssPath, sheet, ssKey);
                var context = CSV.Serialize(data);
                File.WriteAllText(Path.Join(savePath, $"{sheet}.csv"), context);
                if(pGenerateCode)
                    CSV.GenerateCode(sheet, data);
                Debug.Log($"Success to save {sheet}");
            }
            AssetDatabase.Refresh();
        }

        private void GenerateEnum() {
            var envPath = Path.Combine(Application.dataPath, EditorUserSettings.GetConfigValue("CSV_EnvPath"));
            var ssKey = File.ReadAllText(envPath);
            var ssPath = EditorUserSettings.GetConfigValue("CSV_SheetId");
            var data = SpreadSheet.LoadData(ssPath, EditorUserSettings.GetConfigValue("CSV_TypeSheet"), ssKey);
            CSV.GenerateCode("Enums", data, CSVDataStyle.Enum);
        }

        private void InputField(string pName, string pLabel) {
            var temp = EditorGUILayout.TextField(pLabel, EditorUserSettings.GetConfigValue(pName));
            EditorUserSettings.SetConfigValue(pName, temp);
        }
        
        private void OnGUI() {
             GUILayout.Label("Enter SpreadSheet Id\n(https://docs.google.com/spreadsheets/d/{id}/edit)");
             InputField("CSV_SheetId", "spread sheet path");
            
            GUILayout.Space(50);
            GUILayout.Label("Enter .env file path");
            InputField("CSV_EnvPath", "env file path(local)");
            
            GUILayout.Space(50);
            EditorUserSettings.SetConfigValue("CSV_AutoSave", GUILayout.Toggle(EditorUserSettings.GetConfigValue("CSV_AutoSave") == "T", "Load data on play") ? "T" : "F");
            InputField("CSV_SavePath", "save data path(local)");

            _serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_serializedProperty, true);
            var isChanged = EditorGUI.EndChangeCheck();
            _serializedObject.ApplyModifiedProperties();
            if (isChanged)
                EditorUserSettings.SetConfigValue("CSV_TargetSheet", string.Join(',',_targetSheet));    
            if (GUILayout.Button("Save"))
                Load();
            if (GUILayout.Button("GenerateClass"))
                Load(true);
            
            GUILayout.Space(50);
            InputField("CSV_TypeSheet", "TypeSheet");
            if (GUILayout.Button("GenerateEnum"))
                GenerateEnum();
        }
    }
}
#endif
