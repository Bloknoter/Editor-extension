using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using EditorExtension.CuriousInspector.AttributeHandlers;

namespace CuriousAttributes
{
    public interface IAttribute
    {
        IHandler GetHandler();
    }

    public abstract class MyAttribute : Attribute, IAttribute
    {
        public abstract IHandler GetHandler();
    }

    #region Method Attributes

    [AttributeUsage(AttributeTargets.Method)]
    public class Button : MyAttribute, IAttribute
    {
        public readonly string name;
        private ButtonHandler handler;

        #region Constructors
        public Button(string _name)
        {
            name = _name;
        }
        #endregion

        public override IHandler GetHandler()
        {
            if(handler == null)
            {
                handler = new ButtonHandler();
            }
            return handler;
        }
    }

    #endregion

    #region GroupAttributes

    public abstract class GroupAttribute : MyAttribute
    {
        public string groupname { get; protected set; }
        public override abstract IHandler GetHandler();
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ExpandGroup : GroupAttribute
    {
        public ExpandGroup(string _groupname)
        {
            groupname = _groupname;
        }

        public override IHandler GetHandler()
        {
            return new ExpandGroupHandler();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Tab : GroupAttribute
    {
        public Tab(string _tabname) 
        {
            groupname = _tabname;
        }

        public override IHandler GetHandler()
        {
            return new TabHandler();
        }
    }

    #endregion

    #region CustomizationAttributes

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class BackgroundColor : MyAttribute
    {
        public readonly Color color;
        private BackgroundColorHandler handler;
        #region Constructors
        public BackgroundColor(float r, float g, float b)
        {
            color = new Color(r, g, b, 1f);
        }
        #endregion

        public override IHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new BackgroundColorHandler();
            }
            return handler;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TextColor : MyAttribute
    {
        public readonly Color color;
        private TextColorHandler handler;
        #region Constructors
        public TextColor(float r, float g, float b)
        {
            color = new Color(r, g, b, 1f);
        }
        #endregion

        public override IHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new TextColorHandler();
            }
            return handler;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Label : MyAttribute
    {
        public readonly string name;
        private LabelHandler handler;
        #region Constructors
        public Label(string _name)
        {
            name = _name;
        }
        #endregion

        public override IHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new LabelHandler();
            }
            return handler;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Prefab : MyAttribute
    {
        private PrefabHandler handler;
        #region Constructors
        public Prefab()
        {
            
        }
        #endregion

        public override IHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new PrefabHandler();
            }
            return handler;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Scene : MyAttribute
    {
        private SceneHandler handler;
        #region Constructors
        public Scene()
        {

        }
        #endregion

        public override IHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new SceneHandler();
            }
            return handler;
        }
    }

    #endregion

    #region Displaying Attributes

    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayOnly : MyAttribute
    {
        private DisplayOnlyHandler handler;
        #region Constructors
        public DisplayOnly()
        {

        }
        #endregion

        public override IHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new DisplayOnlyHandler();
            }
            return handler;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Hide : MyAttribute
    {
        private HideHandler handler;
        #region Constructors
        public Hide()
        {

        }
        #endregion

        public override IHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new HideHandler();
            }
            return handler;
        }
    }

    /*public class SerializeAllFieldsAttribute : MyAttribute
    {
        public override IHandler GetHandler()
        {
            return new SerializeAllFieldsHandler();
        }
    }*/

    #endregion

}

