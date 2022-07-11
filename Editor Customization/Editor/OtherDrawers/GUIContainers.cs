using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using EditorExtension.CuriousInspector.AttributeHandlers;
using EditorExtension.CuriousInspector.Settings;

namespace EditorExtension.CuriousInspector.GUIContainers
{
    internal class ContainerDebug
    {
        public static void Log(string message)
        {
            if (SettingsData.GetSettingsData().ContainersDebug)
                Debug.Log(message);
        }
    }
    public interface IComponent
    {
        void Draw();
    }
    public class Container : IComponent
    {
        public Container(string newName)
        {
            if (newName == null || newName == "")
            {
                throw new System.Exception("You trying to create Container with null or empty name which is disallowed");
            }
            else
                name = newName;
            IsDrawn = true;
        }
        protected List<IComponent> components = new List<IComponent>();

        private string name;
        public string Name
        {
            get { return name; }
        }
        public bool IsDrawn;

        public bool ContainsContainerLocaly(string _Name)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is Container && ((Container)components[i]).Name == _Name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsContainerLocaly<T>() where T : Container
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                {
                    return true;
                }
            }
            return false;
        }

        public Container GetContainerLocalyorNull(string _Name)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is Container && ((Container)components[i]).Name == _Name)
                {
                    return (Container)components[i];
                }
            }
            return null;
        }

        public T GetContainerLocalyorNull<T>() where T : Container
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                {
                    return (T)components[i];
                }
            }
            return null;
        }

        public void AddComponent(IComponent newcomponent)
        {
            if (newcomponent != null)
            {
                components.Add(newcomponent);
                ContainerDebug.Log($"New component {newcomponent} added to container {Name}");
            }
            else
                Debug.LogError("You are trying to add null component to container. Procedure is canceled");
        }

        public virtual void Draw()
        {
            ContainerDebug.Log($"Drawing {Name} container");
            if (IsDrawn)
            {
                for (int i = 0; i < components.Count; i++)
                {
                    components[i].Draw();
                }
            }
        }

        public override string ToString()
        {
            return $"'{Name}' container";
        }
    }

    public class FieldContainer : IComponent
    {
        private SerializedProperty serializedProperty;
        private FieldInfo fieldInfo;
        private DrawSettings drawSettings;
        public FieldContainer(SerializedProperty _serializedProperty, FieldInfo _fieldInfo, DrawSettings _drawSettings)
        {
            if (_serializedProperty != null)
                serializedProperty = _serializedProperty;
            else
                throw new System.Exception("You are trying to create field container with null SerializedProperty. Procedure is canceled");
            if (_fieldInfo != null)
                fieldInfo = _fieldInfo;
            else
                throw new System.Exception("You are trying to create field container with null System.FieldInfo. Procedure is canceled");
            if (_drawSettings != null)
                drawSettings = _drawSettings;
            else
                drawSettings = new DrawSettings();
        }

        public DrawSettings DrawSettings
        {
            get { return drawSettings; }
            set
            {
                if (value != null)
                    drawSettings = value;
            }
        }

        public void Draw()
        {
            ContainerDebug.Log($"Drawing {fieldInfo.Name} field container");
            DrawUtility.PropertyField(serializedProperty, fieldInfo, drawSettings);
        }

        public override string ToString()
        {
            return $"'{fieldInfo.Name}' field";
        }
    }

    #region Overriden Containers

    public class SingleLocalContainer : Container
    {
        public SingleLocalContainer(string newName) : base(newName)
        {

        }

        public virtual void AddComponentandInitIt(IComponent newcomponent, ISingleLocalContainerHandler handler)
        {
            base.AddComponent(newcomponent);
        }
    }

    public class GroupContainer : Container
    {

        public GroupContainer(string newName) : base(newName)
        {
            IsDrawn = false;
        }

        public override void Draw()
        {
            ContainerDebug.Log($"Drawing {Name} group container");
            EditorGUILayout.Space(7);

            GUILayout.BeginVertical("Tooltip");

            GUIContent expandercontent = new GUIContent(Name);
            if (IsDrawn)
                expandercontent.image = EditorGUIUtility.IconContent("IN foldout act on").image;
            else
                expandercontent.image = EditorGUIUtility.IconContent("IN foldout").image;
            TextAnchor textAnchor = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            if (GUILayout.Button(expandercontent))
            {
                IsDrawn = !IsDrawn;
            }
            GUI.skin.button.alignment = textAnchor;

            if(IsDrawn)
            {
                for (int i = 0; i < components.Count; i++)
                {
                    components[i].Draw();
                }
            }

            GUILayout.EndVertical();

            EditorGUILayout.Space(7);
        }
    }

    public class TabContainer : SingleLocalContainer
    {
        private int selected;
        private List<string> tabs;

        public TabContainer(string newName) : base(newName)
        {
            tabs = new List<string>();
            selected = 0;
        }

        public override void AddComponentandInitIt(IComponent newcomponent, ISingleLocalContainerHandler handler)
        {  
            string tab = handler.ContainerName;
            if(ContainsContainerLocaly(tab))
            {
                GetContainerLocalyorNull(tab).AddComponent(newcomponent);
            }
            else
            {
                Container container = new Container(handler.ContainerName);
                AddComponent(container);
                container.AddComponent(newcomponent);
                tabs.Add(handler.ContainerName);
            }
            ContainerDebug.Log($"Adding {newcomponent} component to the tab {handler.ContainerName}");
        }

        public override void Draw()
        {
            ContainerDebug.Log($"Drawing {Name} tab container");
            GUILayout.BeginVertical("Tooltip");
            selected = GUILayout.Toolbar(selected, tabs.ToArray());
            components[selected].Draw();
            GUILayout.EndVertical();
            EditorGUILayout.Space(7);
        }
    }

    #endregion

    public class ContainerFabric
    {
        public static Container GetContainer(IHandler handler)
        {
            if (handler is ExpandGroupHandler)
                return new GroupContainer(((ExpandGroupHandler)handler).ContainerName);
            if (handler is TabHandler)
                return new TabContainer("Tab");
            return null;
        }
    }
}
