using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extension.Test {
    public static class ExAttribute {
        
        public static IEnumerable<(MethodInfo Method, T Attribute)> HaveAttributeMethods<T>(this Type pType, BindingFlags pFlag)
            where T: Attribute => pType?.GetMethods(pFlag)
                .Where(method => method.IsDefined(typeof(T)))
                .Select(method => (
                    method,
                    method.GetCustomAttribute(typeof(T)) as T
                ));
        
        public static IEnumerable<(MethodInfo Method, T Attribute)> HaveAttributeMethods<T>(this Type pType, BindingFlags pFlag, IEqualityComparer<(MethodInfo, T)> pComparer)
            where T: Attribute => pType
                .GetSuperTypes()
                .SelectMany(type => type.HaveAttributeMethods<T>(pFlag))
                .Distinct(pComparer);
    }
}
