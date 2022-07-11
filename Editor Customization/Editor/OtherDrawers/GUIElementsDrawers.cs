using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorExtension.CuriousInspector.GUIElementsDrawers 
{
    /*public interface IDrawer
    {
        void Draw();
    }*/
    public class TabDrawer/* : IDrawer*/
    {
        private string[] tabs;
        public TabDrawer(string[] _tabs)
        {
            tabs = _tabs;
        }

        public int Draw(int selected)
        {
            return GUILayout.Toolbar(selected, tabs);
        }
    }
}
