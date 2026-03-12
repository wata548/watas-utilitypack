#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Extension.Test {
    
    [CustomEditor(typeof(ScriptableObject), true)]
    public class TestEditor3: TestEditor2 {}
}

#endif
