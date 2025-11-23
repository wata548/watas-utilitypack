using System;
using System.Collections.Generic;

namespace Extension.Test {
    public static class ExType {
        public static IEnumerable<Type> GetSuperTypes(this Type pType) {
            var superTypes = new List<Type>();
            var targetType = pType;
            while (pType != null) {
                superTypes.Add(pType);
                pType = pType.BaseType;
            }

            return superTypes;
        }
    }
}