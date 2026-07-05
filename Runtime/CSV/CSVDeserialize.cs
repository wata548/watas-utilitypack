using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CSVData.Extensions;
using Object = System.Object;

namespace CSVData {
    public static partial class CSV {

        #region Object

        private static Object Deserialize(string typeName, List<string> headerNames, List<string> values,
            bool ignoreException = true) =>
            Deserialize(Type.GetType(typeName), headerNames, values, ignoreException);
        
        private static T Deserialize<T>(List<string> headerNames, List<string> values,
            bool ignoreException = true) =>
            (T)Deserialize(typeof(T), headerNames, values, ignoreException);
        
        private static Object Deserialize(Type targetType, List<string> headerNames, List<string> values,
            bool ignoreException = true) {

            var defaultReflectionFlag = BindingFlags.Instance 
                                        | BindingFlags.NonPublic
                                        | BindingFlags.Public
                                        | BindingFlags.FlattenHierarchy;
            
            var result = Activator.CreateInstance(targetType);

            for (int i = 0; i < headerNames.Count; i++) {

                var fieldInfo = 
                    targetType.GetField(headerNames[i], defaultReflectionFlag);
                
                if (fieldInfo == null) {

                    var propertyInfo = 
                        targetType.GetProperty(headerNames[i], defaultReflectionFlag);

                    if (propertyInfo == null || !propertyInfo.CanWrite) {
                        if (!ignoreException)
                            throw new MissingFieldException($"{headerNames[i]} isn't exist or can't write. ({propertyInfo})");
                        Debug.LogError($"{headerNames[i]} isn't exist or can't write. ({propertyInfo})");
                        continue;        
                    }

                    var propertyType = propertyInfo.PropertyType;
                    var propertyData = CSharpExtension.Parse(propertyType, values[i]);
                    
                    propertyInfo.SetValue(result, propertyData);
                }

                else {
                    
                    var fieldType = fieldInfo.FieldType;
                    var fieldData = CSharpExtension.Parse(fieldType, values[i]);
                    fieldInfo.SetValue(result, fieldData);
                }
            }

            if(result is ICSVListData target)
                target.PostProcessing();

            return result;
        }

        #endregion

        #region List
        
        //string base
        public static List<T> DeserializeToList<T>(string data, bool ignoreException = true) =>
            DeserializeToList<T>(Parse(data), ignoreException);

        public static Object DeserializeToList(string typeName, string data, bool ignoreException = true) =>
            DeserializeToList(Type.GetType(typeName), Parse(data), ignoreException);
        
        public static Object DeserializeToList(Type type, string data, bool ignoreException = true) =>
            DeserializeToList(type, Parse(data), ignoreException);
        
        //string list base
        public static List<T> DeserializeToList<T>(List<string> data, bool ignoreException = true) =>
            DeserializeToList<T>(Parse(data), ignoreException);
        
        public static Object DeserializeToList(string typeName, List<string> data, bool ignoreException = true) =>
            DeserializeToList(Type.GetType(typeName), Parse(data), ignoreException);

        public static Object DeserializeToList(Type type, List<string> data, bool ignoreException = true) =>
            DeserializeToList(type, Parse(data), ignoreException);
        
        //string 2d list base
        public static List<T> DeserializeToList<T>(List<List<string>> datas, bool ignoreException = true) {
                    
                    if (datas.Count <= 2) {
        
                        if (!ignoreException)
                            throw new Exception("data should big than 2 row(headerName, headerType)");
                        Debug.LogError("data should big than 2 row(headerName, headerType)");
                        return null;
                    }
                    
                    var headerNames = datas[0].Where(header => !string.IsNullOrWhiteSpace(header)).ToList();
        
                    var list = new List<T>();
                    for (int i = 2; i < datas.Count; i++) {
                        if (datas[i].All(string.IsNullOrWhiteSpace))
                            break;
                        var row = Deserialize<T>(headerNames, datas[i], ignoreException);
                        list.Add(row);
                    }
        
                    return list;
                }

        public static Object DeserializeToList(string typeName, List<List<string>> datas, bool ignoreException = true) =>
            DeserializeToList(Type.GetType(typeName), datas, ignoreException);
        
        public static Object DeserializeToList(Type targetType, List<List<string>> datas, bool ignoreException = true) {

            if (datas.Count <= 2) {

                if (!ignoreException)
                    throw new Exception("data should big than 2 row(headerName, headerType)");
                Debug.LogError("data should big than 2 row(headerName, headerType)");
                return null;
            }
            
            var headerNames = datas[0].Where(header => !string.IsNullOrWhiteSpace(header)).ToList();

            var listType = typeof(List<>).MakeGenericType(targetType);
            
            var list = Activator.CreateInstance(listType);
            var addFunction = listType.GetMethod("Add", new Type[] { targetType })!;
            
            for (int i = 2; i < datas.Count; i++) {
                if (datas[i].All(string.IsNullOrWhiteSpace))
                    break;
                var row = Deserialize(targetType, headerNames, datas[i], ignoreException);
                addFunction.Invoke(list, new[] {row});
            }

            return list;
        }
        #endregion

        #region Dictionary
        
        //string input
        public static Object DeserializeToDictionary(string typeName, string data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            DeserializeToDictionary(Type.GetType(typeName), Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        public static Object DeserializeToDictionary(Type type, string data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            DeserializeToDictionary(type, Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        public static Dictionary<TKey, TValue> DeserializeToDictionary<TKey, TValue>(string data, string primaryKey, 
            bool ignoreSomeError  = true) =>
            DeserializeToDictionary<TKey, TValue>(Parse(data), primaryKey, ignoreSomeError);
        
        //1dimension
        public static Object DeserializeToDictionary(string typeName, List<string> data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            DeserializeToDictionary(Type.GetType(typeName), Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        
        public static Object DeserializeToDictionary(Type type, List<string> data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            DeserializeToDictionary(type, Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        public static Dictionary<TKey, TValue> DeserializeToDictionary<TKey, TValue>(List<string> data, string primaryKey, 
            bool ignoreSomeError  = true) =>
            DeserializeToDictionary<TKey, TValue>(Parse(data), primaryKey, ignoreSomeError);
        
        //2dimension
        public static Object DeserializeToDictionary(string typeName, List<List<string>> datas, string primaryKeyName,
            out Type keyType, bool ignoreSomeError = true) =>
            DeserializeToDictionary(Type.GetType(typeName), datas, primaryKeyName, out keyType, ignoreSomeError);

        public static Dictionary<TKey, TValue> DeserializeToDictionary<TKey, TValue>(List<List<string>> datas, string primaryKeyName, bool ignoreSomeError = true) {
            if (datas.Count <= 2) {

                if (!ignoreSomeError)
                    throw new Exception("data should big than 2 row(headerName, headerType)");
                Debug.LogError("data should big than 2 row(headerName, headerType)");

                return null;
            }

            var targetType = typeof(TKey);
            MemberInfo primaryKeyInfo = targetType.GetField(primaryKeyName);
            if (primaryKeyInfo == null) {

                primaryKeyInfo = targetType.GetProperty(primaryKeyName);
                if (primaryKeyInfo == null) {
                    if (!ignoreSomeError)
                        throw new Exception($"This key is wrong '{primaryKeyName}'isn't exist");
                    Debug.LogError($"This key is wrong '{primaryKeyName}'isn't exist");
                    return null;
                }

                if (typeof(TKey) != (primaryKeyInfo as PropertyInfo)!.PropertyType) {
                    if (!ignoreSomeError)
                        throw new Exception($"Key type didn't match({(primaryKeyInfo as PropertyInfo)!.PropertyType})");
                    Debug.LogError($"Key type didn't match({(primaryKeyInfo as PropertyInfo)!.PropertyType})");
                }
            }
            else {
                if (typeof(TKey) != (primaryKeyInfo as FieldInfo)!.FieldType) {
                    if (!ignoreSomeError)
                        throw new Exception($"Key type didn't match({(primaryKeyInfo as FieldInfo)!.FieldType})");
                    Debug.LogError($"Key type didn't match({(primaryKeyInfo as FieldInfo)!.FieldType})");
                }
            }
            
            var headerNames = datas[0].Where(header => !string.IsNullOrWhiteSpace(header)).ToList();
            var result = new Dictionary<TKey, TValue>();

            Dictionary<int, int> a = new();

            for (int i = 2; i < datas.Count; i++) {
                var newValue = Deserialize<TValue>(headerNames, datas[i], ignoreSomeError);

                TKey newKey = default;
                if (primaryKeyInfo is FieldInfo fieldKey)
                    newKey = (TKey)fieldKey.GetValue(newValue)!;
                else if (primaryKeyInfo is PropertyInfo propertyKey)
                    newKey = (TKey)propertyKey.GetValue(newValue)!;
                
                result.Add(newKey, newValue);
            }

            return result;
        }
        
        public static Object DeserializeToDictionary(Type targetType, List<List<string>> datas, string primaryKeyName,
            out Type keyType, bool ignoreSomeError = true) {
            if (datas.Count <= 2) {

                if (!ignoreSomeError)
                    throw new Exception("data should big than 2 row(headerName, headerType)");
                Debug.LogError("data should big than 2 row(headerName, headerType)");

                keyType = null;
                return null;
            }

            MemberInfo primaryKeyInfo = targetType.GetField(primaryKeyName);
            if (primaryKeyInfo == null) {

                primaryKeyInfo = targetType.GetProperty(primaryKeyName);
                if (primaryKeyInfo == null) {
                    if (!ignoreSomeError)
                        throw new Exception($"This key is wrong '{primaryKeyName}'isn't exist");
                    Debug.LogError($"This key is wrong '{primaryKeyName}'isn't exist");
                    keyType = null;
                    return null;
                }

                keyType = (primaryKeyInfo as PropertyInfo)!.PropertyType;
            }
            else
                keyType = (primaryKeyInfo as FieldInfo)!.FieldType;
            
            var headerNames = datas[0].Where(header => !string.IsNullOrWhiteSpace(header)).ToList();
            var dictionaryType = 
                typeof(Dictionary<,>).MakeGenericType(keyType, targetType);
            var result = Activator.CreateInstance(dictionaryType);

            Dictionary<int, int> a = new();
            var addFunction = dictionaryType
                .GetMethod("Add", new[] { keyType, targetType })!;

            for (int i = 2; i < datas.Count; i++) {
                var newValue = Deserialize(targetType, headerNames, datas[i], ignoreSomeError);

                Object newKey = null;

                if (primaryKeyInfo is FieldInfo fieldKey)
                    newKey = fieldKey.GetValue(newValue)!;

                else if (primaryKeyInfo is PropertyInfo propertyKey)
                    newKey = propertyKey.GetValue(newValue)!;
                
                
                addFunction.Invoke(result, new[] { newKey, newValue });
            }

            return result;
        }
        #endregion
    }
}