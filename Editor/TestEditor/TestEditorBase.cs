#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Extension.Test {
    public abstract class TestEditorBase: Editor {
        
        protected bool _isFoldOut = true;
        private Dictionary<MethodInfo, List<(ParameterInfo Info, string Value)>> _parameters = new();
        
        protected void SetPropertyField(MethodInfo pMethod) {
            
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

        protected object[] GetParameterValue(MethodInfo pMethod) {
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