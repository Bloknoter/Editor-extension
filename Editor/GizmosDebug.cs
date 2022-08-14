using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EditorExtension.Debugging
{
    public class GizmosDebug
    {
        public static void DrawStar(Vector2 point, float size)
        {
            DrawStar(point, size, Color.white);
        }

        public static void DrawStar(Vector2 point, float size, Color color)
        {
            Debug.DrawLine(new Vector2(point.x - size / 2, point.y), new Vector2(point.x + size / 2, point.y), color, 120);
            Debug.DrawLine(new Vector2(point.x, point.y - size / 2), new Vector2(point.x, point.y + size / 2));
            Debug.DrawLine(new Vector2(point.x - size / 3, point.y - size / 3), new Vector2(point.x + size / 3, point.y + size / 3), color, 120);
            Debug.DrawLine(new Vector2(point.x + size / 3, point.y - size / 3), new Vector2(point.x - size / 3, point.y + size / 3), color, 120);
        }

        public static void DrawVector(Vector2 start, Vector2 end)
        {
            DrawVector(start, end, Color.white);
        }

        public static void DrawVector(Vector2 start, Vector2 end, Color color)
        {
            
            Debug.DrawLine(start, end, color, 120);
            float angle = Mathf.Atan2((start - end).y, (start - end).x);

            float angle1 = angle - 2 * Mathf.PI + Mathf.PI / 8f;
            Vector2 first = end + new Vector2(Mathf.Cos(angle1), Mathf.Sin(angle1)) * 0.5f;
            Debug.DrawLine(end, first, color, 120);

            angle1 = angle - 2 * Mathf.PI - Mathf.PI / 8f;
            Vector2 second = end + new Vector2(Mathf.Cos(angle1), Mathf.Sin(angle1)) * 0.5f;
            Debug.DrawLine(end, second, color, 120);

            Debug.DrawLine(first, second, color, 120);

            Vector2 inArrowVector = end - (first + (second - first) / 2);
            for (int i = 0; i < 5; i++)
            {
                Vector2 delta = (second - first) * (i + 1) * 0.1f;
                Debug.DrawLine(first + delta, first + delta + inArrowVector * ((i + 1) / 5f), color, 120);
            }
            for (int i = 0; i < 5; i++)
            {
                Vector2 delta = (second - first) * (i + 6) * 0.1f;
                Debug.DrawLine(first + delta, first + delta + inArrowVector * ((4 - i) / 5f), color, 120);
            }
        }
    }
}
