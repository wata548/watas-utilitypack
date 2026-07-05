#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Extension.Test {
    public class TestMethodInfo {
        public bool FoldOut { get; set; }
        public readonly IReadOnlyList<(MethodInfo Method, TestMethodAttribute Attribute)> Datas;

        public TestMethodInfo(bool pFold, List<(MethodInfo Method, TestMethodAttribute Attribute)> pDatas) {
            FoldOut = pFold;
            Datas = pDatas;
        }
    }
    
    public class TestWindow: EditorWindow {
        private static Dictionary<Type, TestMethodInfo> _methods;
        private Vector2 _scroll; 
        
        [MenuItem("Window/Utilities/Test")]
        private static void Init() {
            var window = GetWindow<TestWindow>();
            window.Show();
        }

        private static void MethodInit() {
            const BindingFlags FLAG = BindingFlags.Static 
                                      | BindingFlags.Public 
                                      | BindingFlags.NonPublic;
                            
            _methods ??= AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(asm => asm.GetTypes().Where(type => type.IsSealed && type.IsAbstract))
                .Select(type => (type, new TestMethodInfo(true, type
                        .HaveAttributeMethods(FLAG, new MethodComparer<TestMethodAttribute>())
                        .OrderBy(data => data.Attribute.AllowFoldOut)
                        .ThenByDescending(data => data.Item2.Priority)
                        .ToList())
                    )
                ).Where(data => data.Item2.Datas.Count > 0)
                .ToDictionary(kvp => kvp.type, kvp => kvp.Item2);
                        
        }
        
        private void OnGUI() {
            MethodInit();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var (type, _) in _methods) {
                GUILayout.Label(type.Name);
                Class(type);
            }
            EditorGUILayout.EndScrollView();
        }

        private void Class(Type pType) {
            bool generatedFoldOut = false;
            foreach (var targetButton in _methods[pType].Datas) {
                if (!generatedFoldOut && targetButton.Attribute.AllowFoldOut) {
                    generatedFoldOut = true;
                    _methods[pType].FoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(_methods[pType].FoldOut, "TestFunction");
                    if (!_methods[pType].FoldOut) {
                        
                        EditorGUILayout.EndFoldoutHeaderGroup();
                        return;
                    }
                }
                                        
                SetPropertyField(targetButton.Method);
                        
                var buttonName = targetButton.Attribute.Name;
                if (string.IsNullOrEmpty(buttonName))
                    buttonName = targetButton.Method.Name;
                        
                if (!Application.isPlaying && targetButton.Attribute.RuntimeOnly) {
                    EditorGUI.BeginDisabledGroup(true);
                    buttonName += "(RuntimeOnly)";
                }
                                        
                if (GUILayout.Button(buttonName)) {
                        
                    targetButton.Method.Invoke(null, GetParameterValue(targetButton.Method));
                }
                                        
                if (!Application.isPlaying && targetButton.Attribute.RuntimeOnly)
                    EditorGUI.EndDisabledGroup();
                        
            }
            if(generatedFoldOut)
                EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private Dictionary<MethodInfo, List<(ParameterInfo Info, string Value)>> _parameters = new();
                
        private void SetPropertyField(MethodInfo pMethod) {
                    
            _parameters.TryAdd(pMethod, new());
            int idx = 0;
                        
            foreach (var parameter in pMethod.GetParameters()) {
                var input = "";
                            
        
                if (idx < _parameters[pMethod].Count) {
        
                    var content = _parameters[pMethod][idx].Value;
                    input = EditorGUILayout.TextField($"{parameter.Name}({parameter.ParameterType.Name})", content);
                    _parameters[pMethod][idx] = (parameter, input);
                }
                else {
                    input = EditorGUILayout.TextField($"{parameter.Name}({parameter.ParameterType.Name})", parameter.HasDefaultValue ? parameter.DefaultValue!.ToString() : "");
                    _parameters[pMethod].Add((parameter, input));
                }
        
                idx++;
            }
        }
        
        private object[] GetParameterValue(MethodInfo pMethod) {
            var parameterList = new List<Object>();
                            
            foreach (var parameter in _parameters[pMethod]) {
        
                var parameterType = parameter.Info.ParameterType;
        
                object parsedValue = ExParse.ParseToObject(parameterType, parameter.Value);
                parameterList.Add(parsedValue);
            }
        
            return parameterList.ToArray();
        }
    }
}
#endif