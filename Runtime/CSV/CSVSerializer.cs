using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;

namespace CSVData {
    public static partial class CSV {
        public static string Serialize<T>(List<List<T>> pData) where T: class {

            var result = new StringBuilder();
            foreach (var row in pData) {
                var flag = false;
                
                foreach (var cell in row) {
                    
                    if (flag)
                        result.Append(',');
                    else 
                        flag = true;
                    result.Append($"\"{cell}\"");
                }

                result.AppendLine();
            }

            return result.ToString();
        }
    }
}