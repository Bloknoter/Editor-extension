using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;


using CuriousAttributes;

namespace EditorExtension.CuriousInspector.AttributeHandlers 
{
    public interface IHandler
    {
        void Initialize(IAttribute attribute);
        void SetDrawSettings(DrawSettings settings);
    }

    public interface IContainerHandler : IHandler
    {
        string ContainerName { get; }
    }

    public interface ISingleLocalContainerHandler : IHandler
    {
        string ContainerName { get; }
    }

    public class DrawSettings
    {
        public DrawSettings()
        {
            backgroundColor = Color.white;
            textColor = Color.black;
            label = new GUIContent() { text = "" };
            displayonly = false;
            display = true;
        }
        public Color backgroundColor;
        public Color textColor;
        public GUIContent label;
        public bool displayonly;
        public bool display;

        public override string ToString()
        {
            return $"BackgroundColor: {backgroundColor} \nTextColor: {textColor} \nLabel.text: {label.text}  \nDisplay: {display}" +
            $" \nDisplayOnly: {displayonly}";
        }
    }

    #region Group Handlers

    public class ExpandGroupHandler : IContainerHandler
    {
        private string groupName;
        public void Initialize(IAttribute attribute)
        {
            ExpandGroup tab = (ExpandGroup)attribute;
            groupName = tab.groupname;
        }

        public void SetDrawSettings(DrawSettings settings)
        {

        }

        public string ContainerName
        {
            get { return groupName; }
        }
    }

    public class TabHandler : ISingleLocalContainerHandler
    {
        private string tabName;
        public void Initialize(IAttribute attribute)
        {
            Tab tab = (Tab)attribute;
            tabName = tab.groupname;
        }

        public void SetDrawSettings(DrawSettings settings)
        {

        }

        public string ContainerName
        {
            get { return tabName; }
        }
    }
    #endregion

    #region Method Handlers
    public class ButtonHandler : IHandler
    {
        private Button button;
        public void Initialize(IAttribute attribute)
        {
            button = (Button)attribute;
        }

        public void SetDrawSettings(DrawSettings drawSettings)
        {
            drawSettings.label.text = button.name;
        }

    }
    #endregion

    #region CustomizationHandlers
    public class BackgroundColorHandler : IHandler
    {
        private BackgroundColor backgroundColor;
        public void Initialize(IAttribute attribute)
        {
            backgroundColor = (BackgroundColor)attribute;
        }

        public void SetDrawSettings(DrawSettings settings)
        {
            settings.backgroundColor = backgroundColor.color;
        }
    }

    public class TextColorHandler : IHandler
    {
        private TextColor textColor;
        public void Initialize(IAttribute attribute)
        {
            textColor = (TextColor)attribute;
        }

        public void SetDrawSettings(DrawSettings settings)
        {
            settings.textColor = textColor.color;
        }
    }

    public class LabelHandler : IHandler
    {
        private Label label;
        public void Initialize(IAttribute attribute)
        {
            label = (Label)attribute;
        }

        public void SetDrawSettings(DrawSettings settings)
        {
            settings.label.text = label.name;
        }
    }

    public class PrefabHandler : IHandler
    {
        public void Initialize(IAttribute attribute)
        {
            
        }

        public void SetDrawSettings(DrawSettings settings)
        {
            string text = settings.label.text;
#if UNITY_EDITOR
            settings.label.image = EditorGUIUtility.IconContent("Prefab Icon", "prefab").image;
            settings.label.text = text;
            settings.label.tooltip = "prefab";
            settings.textColor = new Color(0.2599235f, 0.5969429f, 0.9339623f);
#endif
        }
    }

    public class SceneHandler : IHandler
    {
        public void Initialize(IAttribute attribute)
        {

        }

        public void SetDrawSettings(DrawSettings settings)
        {
            string text = settings.label.text;
#if UNITY_EDITOR
            settings.label.image = EditorGUIUtility.IconContent("UnityLogo", "scene").image;
            settings.label.text = text;
            settings.label.tooltip = "scene";
#endif
        }
    }
    #endregion

    #region Displaying Handlers

    public class DisplayOnlyHandler : IHandler
    {
        public void Initialize(IAttribute attribute)
        {

        }

        public void SetDrawSettings(DrawSettings settings)
        {
            settings.displayonly = true;
        }
    }

    public class HideHandler : IHandler
    {
        public void Initialize(IAttribute attribute)
        {

        }

        public void SetDrawSettings(DrawSettings settings)
        {
            settings.display = false;
        }
    }

    #endregion
}
