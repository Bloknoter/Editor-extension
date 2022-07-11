using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EditorExtension.Debugging
{
    public class Measure : EditorWindow
    {
        [MenuItem("Extension/Measure")]
        private static void GetWindow()
        {
            GetWindow(typeof(Measure), false, "Measure", true).position = new Rect(500, 100, 550, 500);
        }

        private Vector2 scrollPos;

        private Transform first;

        private Transform second;

        private int dimension;

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            first = (Transform)EditorGUILayout.ObjectField("First", first, typeof(Transform), true);
            second = (Transform)EditorGUILayout.ObjectField("Second", second, typeof(Transform), true);
            dimension = EditorGUILayout.Popup(new GUIContent("Dimension"), dimension, new string[] { "2D", "3D" });

            EditorGUILayout.Space();

            if(first != null && second != null)
            {
                float value = 0;
                if(dimension == 0)
                {
                    value = Vector2.Distance(first.position, second.position);
                }
                else
                {
                    value = Vector3.Distance(first.position, second.position);
                }
                EditorGUILayout.LabelField($"DISTANCE: {value}");
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("DIFFERENCE");
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField($"x: {Mathf.Abs(first.position.x - second.position.x)}");
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField($"y: {Mathf.Abs(first.position.y - second.position.y)}");
                if (dimension == 1)
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField($"z: {Mathf.Abs(first.position.z - second.position.z)}");
                }
                EditorGUI.indentLevel--;
                if (dimension == 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    value = 0;
                    var dir = (Vector2)second.position - (Vector2)first.position;
                    value = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    EditorGUILayout.LabelField($"ANGLE: {value}");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Select all fields");
            }

            EditorGUILayout.EndScrollView();
        }

        private void Update()
        {
            Repaint();
        }
    }
}
