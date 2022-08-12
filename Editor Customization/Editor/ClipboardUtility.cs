using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EditorExtension
{
    public class ClipboardUtility
    {
        /*public static void WriteData(object data)
        {
            if (data == null)
            {
                EditorGUIUtility.systemCopyBuffer = "";
                return;
            }
            object[] arraydata = new object[1];
            arraydata[0] = data;
            WriteData(arraydata);
        }*/
        public static void WriteData(object[] data)
        {
            string stringdata = "";
            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != null)
                        stringdata += data[i].ToString();
                    stringdata += "|";
                }
            }
            EditorGUIUtility.systemCopyBuffer = stringdata;
        }

        public static string[] ReadData()
        {
            List<string> alldata = new List<string>();
            string buffer = EditorGUIUtility.systemCopyBuffer;
            string currvalue = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == '|')
                {
                    alldata.Add(currvalue);
                    currvalue = "";
                }
                else
                {
                    currvalue += buffer[i];
                }
            }
            string[] result = new string[alldata.Count];
            alldata.CopyTo(result);
            return result;
        }
    }
}
