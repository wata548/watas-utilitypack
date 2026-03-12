#if UNITY_EDITOR

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Extension.Test {
    
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class TestEditor2: TestEditorBase {
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            var flag = BindingFlags.Instance
                       | BindingFlags.FlattenHierarchy
                       | BindingFlags.Static
                       | BindingFlags.Public
                       | BindingFlags.NonPublic;
            
            var targets = target.GetType()
                .HaveAttributeMethods(flag, new MethodComparer<TestMethodAttribute>())
                .OrderBy(data => data.Attribute.AllowFoldOut)
                .ThenByDescending(data => data.Attribute.Priority);

            if (!targets.Any())
                return;
            
            bool generatedFoldOut = false;
            foreach (var targetButton in targets) {
                if (!generatedFoldOut && targetButton.Attribute.AllowFoldOut) {
                    generatedFoldOut = true;
                    _isFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(_isFoldOut, "TestFunction");
                    if (!_isFoldOut)
                        return;
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

                    var parameterList = GetParameterValue(targetButton.Method);
                    
                    if(targetButton.Method.IsStatic)
                        targetButton.Method.Invoke(null, parameterList);
                    else 
                        targetButton.Method.Invoke(target, parameterList);
                }
                
                if (!Application.isPlaying && targetButton.Attribute.RuntimeOnly)
                    EditorGUI.EndDisabledGroup();

            }
            if(generatedFoldOut)
                EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
#endif