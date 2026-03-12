using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extension.Test {
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestMethodAttribute: Attribute {
        public readonly int Priority;
        public readonly string Name;
        public readonly bool RuntimeOnly;
        public readonly bool AllowFoldOut;

        public TestMethodAttribute(int pPriority, string pName = "", bool pRuntimeOnly = false, bool pAllowFoldOut = true) =>
            (Name, Priority, RuntimeOnly, AllowFoldOut) = (pName, pPriority, pRuntimeOnly, pAllowFoldOut);
        
        public TestMethodAttribute(string pName = "", int pPriority = 0, bool pRuntimeOnly = false, bool pAllowFoldOut = true) =>
            (Name, Priority, RuntimeOnly, AllowFoldOut) = (pName, pPriority, pRuntimeOnly, pAllowFoldOut);
        
    }

    public class MethodComparer<T> : IEqualityComparer<(MethodInfo, T)> {
        public bool Equals((MethodInfo, T) x, (MethodInfo, T) y) {
            return x.Item1.Name == y.Item1.Name
                   && x.Item1.ReturnType == y.Item1.ReturnType
                   && x.Item1.GetParameters().Select(p => p.ParameterType)
                       .SequenceEqual(y.Item1.GetParameters().Select(p => p.ParameterType));
        }

        public int GetHashCode((MethodInfo, T) pObj) {

            var parameters = pObj.Item1.GetParameters();
            
            if(parameters.Length == 0)
                return HashCode.Combine(pObj.Item1.Name, pObj.Item1.ReturnType);
            
            var hash = pObj.Item1.GetParameters()
                .Select(p => p.ParameterType.GetHashCode())
                .Aggregate((lhs, rhs) => (lhs * 123) ^ rhs);
            
            return HashCode.Combine(pObj.Item1.Name, pObj.Item1.ReturnType, hash);
        }
    }
}