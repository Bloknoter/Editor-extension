using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EditorExtension
{
    public class EditorLogger
    {
        public enum OperationResultType { Success, Interrupted, Failure }

        public enum InfoType { Message, Warning, Error }

        public static void LogOperationResult(string operationName, OperationResultType logType)
        {
            LogOperationResult(operationName, "", logType);
        }

        public static void LogOperationResult(string operationName, string message, OperationResultType logType)
        {
            switch (logType)
            {
                case OperationResultType.Success:
                    Debug.Log($"Operation \"{operationName}\" finished with result <color=green><b>\"Success\"</b></color>. {message}");
                    break;

                case OperationResultType.Interrupted:
                    Debug.Log($"Operation \"{operationName}\" finished with result <color=yellow><b>\"Interrupted\"</b></color>. {message}");
                    break;

                case OperationResultType.Failure:
                    Debug.Log($"Operation \"{operationName}\" finished with result <color=red><b>\"Failure\"</b></color>. {message}");
                    break;
            }
        }

        public static void Log(string info, InfoType infoType)
        {
            switch (infoType)
            {
                case InfoType.Message:
                    Debug.Log($"{info}");
                    break;
                case InfoType.Warning:
                    Debug.Log($"<color=yellow><b>Warning!</b></color> {info}");
                    break;

                case InfoType.Error:
                    Debug.Log($"<color=red><b>Error: </b></color> {info}");
                    break;
            }
        }
    }
}
