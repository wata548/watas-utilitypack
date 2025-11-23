using System;

namespace Extension.Test {
    public static class ExParse {
        public static object ParseToObject(Type pTargetType, string pValue) {
            if (pTargetType == typeof(string))
                return pValue;
            if (pTargetType.IsEnum)
                return Enum.Parse(pTargetType, pValue);
            var parse = pTargetType.GetMethod("Parse", new[] { typeof(string) });
            return parse?.Invoke(null, new[] { pValue })
                   ?? throw new ArgumentException($"{pTargetType} Type didn't have 'parse(string)'method");
        } 
    }
}