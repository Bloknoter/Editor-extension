using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

using CuriousAttributes;
using EditorExtension.CuriousInspector.GUIContainers;
using EditorExtension.CuriousInspector.AttributeHandlers;

namespace EditorExtension.CuriousInspector
{
    [CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
    public class CuriousInspector : Editor
    {
        private Type currType;

        private List<FieldInfo> fieldInfos;
        private List<SerializedProperty> serializedProperties;
        private List<List<IHandler>> handlers;
        private List<DrawSettings> settings;

        private List<MethodInfo> methodInfos;

        private Container MainContainer;

        private void OnEnable()
        {
            serializedProperties = new List<SerializedProperty>();
            handlers = new List<List<IHandler>>();
            settings = new List<DrawSettings>();
            fieldInfos = new List<FieldInfo>();
            methodInfos = new List<MethodInfo>();
            currType = target.GetType();
            /*Type baseType = currType.BaseType;
            do
            {
                fieldInfos.AddRange(baseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
                methodInfos.AddRange(baseType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            } while (baseType != null);*/
            fieldInfos.AddRange(currType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            methodInfos.AddRange(currType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

            /*List<FieldInfo> existingFields = new List<FieldInfo>();
            for(int i = 0; i < fieldInfos.Count;i++)
            {
                if(!existingFields.Contains(fieldInfos[i]))
                {
                    existingFields.Add(fieldInfos[i]);
                }
            }
            fieldInfos = existingFields;

            List<MethodInfo> existingMethods = new List<MethodInfo>();
            for (int i = 0; i < methodInfos.Count; i++)
            {
                if (!existingMethods.Contains(methodInfos[i]))
                {
                    existingMethods.Add(methodInfos[i]);
                }
            }
            methodInfos = existingMethods;*/

            for (int i = 0; i < fieldInfos.Count; i++)
            {
                SerializedProperty serializedProperty = serializedObject.FindProperty(fieldInfos[i].Name);

                Log($"field: {fieldInfos[i]}, serializedProperty: {serializedProperty != null}");
                if (serializedProperty != null)
                {
                    Log("Gathering information about the field...");
                    serializedProperties.Add(serializedProperty);

                    MyAttribute[] custattributes = (MyAttribute[])fieldInfos[i].GetCustomAttributes<MyAttribute>(true);
                    List<IHandler> currhandlers = new List<IHandler>();

                    for (int h = 0; h < custattributes.Length; h++)
                    {
                        IHandler handler = custattributes[h].GetHandler();
                        handler.Initialize(custattributes[h]);
                        currhandlers.Add(handler);
                    }

                    handlers.Add(currhandlers);

                    DrawSettings drawSettings = new DrawSettings();
                    settings.Add(drawSettings);

                }
                else
                {
                    Log("No serialized property found, removing field from drawing list...");
                    fieldInfos.RemoveAt(i);
                    i--;
                }
            }

            MainContainer = new Container("Main");

            for (int i = 0; i < handlers.Count; i++)
            {
                bool selectedtoanothercontainer = false;
                DrawSettings drawSettings = new DrawSettings();
                FieldContainer fieldContainer = new FieldContainer(serializedProperties[i], fieldInfos[i], drawSettings);
                for (int h = 0; h < handlers[i].Count; h++)
                {
                    handlers[i][h].SetDrawSettings(drawSettings);
                    if (handlers[i][h] is IContainerHandler)
                    {
                        selectedtoanothercontainer = true;
                        IContainerHandler containerHandler = (IContainerHandler)handlers[i][h];

                        if (MainContainer.ContainsContainerLocaly(containerHandler.ContainerName))
                        {
                            Container container = MainContainer.GetContainerLocalyorNull(containerHandler.ContainerName);
                            container.AddComponent(fieldContainer);
                        }
                        else
                        {
                            Container container = ContainerFabric.GetContainer(containerHandler);
                            container.AddComponent(fieldContainer);
                            MainContainer.AddComponent(container);
                        }
                    }
                    if (handlers[i][h] is ISingleLocalContainerHandler)
                    {
                        selectedtoanothercontainer = true;
                        ISingleLocalContainerHandler containerHandler = (ISingleLocalContainerHandler)handlers[i][h];

                        if (MainContainer.ContainsContainerLocaly<SingleLocalContainer>())
                        {
                            SingleLocalContainer container = MainContainer.GetContainerLocalyorNull<SingleLocalContainer>();
                            container.AddComponentandInitIt(fieldContainer, containerHandler);
                        }
                        else
                        {
                            SingleLocalContainer container = (SingleLocalContainer)ContainerFabric.GetContainer(containerHandler);
                            container.AddComponentandInitIt(fieldContainer, containerHandler);
                            MainContainer.AddComponent(container);
                        }
                    }
                }
                Log($"field: {fieldInfos[i]}, drawsettings: {drawSettings},\n selectedtoanothercontainer: {selectedtoanothercontainer}");
                if (!selectedtoanothercontainer)
                {
                    MainContainer.AddComponent(fieldContainer);
                }
            }

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            MainContainer.Draw();

            for (int i = 0; i < methodInfos.Count; i++)
            {
                // Debug.Log($"{memberInfos[i].MemberType} {i} : {memberInfos[i]}");
                IHandler[] handlers = DrawUtility.GetHandlersfromAttributesofTypeandInitIt<MyAttribute>(methodInfos[i]);
                if (handlers.Length > 0)
                {
                    DrawSettings drawSettings = new DrawSettings();

                    for (int a = 0; a < handlers.Length; a++)
                    {
                        handlers[a].SetDrawSettings(drawSettings);
                    }
                    if (DrawUtility.Button(drawSettings))
                    {
                        methodInfos[i].Invoke(target, null);
                    }
                }

            }

            serializedObject.ApplyModifiedProperties();
        }


        private void Log(string message)
        {
            /*if (SettingsData.GetSettingsData().InspectorDebug)
                Debug.Log(message);*/
        }
    }
}
