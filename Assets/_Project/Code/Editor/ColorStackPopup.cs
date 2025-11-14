using _Project.Code.Infrastructure.Configs;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Project.Code.Editor
{
    public class ColorStackPopup : EditorWindow
    {
        private ColorStack colorStack;
        private System.Action<ColorStack> onConfirm;
        private Vector2 scrollPosition;
        
        private static readonly HexColor[] AvailableColors = new HexColor[]
        {
            HexColor.Red,
            HexColor.Green,
            HexColor.Blue,
            HexColor.Yellow,
            HexColor.Purple
        };

        public static void Show(Vector2 position, ColorStack currentStack, System.Action<ColorStack> onConfirm)
        {
            var window = CreateInstance<ColorStackPopup>();
            window.colorStack = new ColorStack();
            
            // Копируем текущую стопку
            if (currentStack != null && currentStack.Colors != null)
            {
                window.colorStack.Colors = new List<HexColor>(currentStack.Colors);
            }
            
            // Если стопка пустая, добавляем один цвет по умолчанию
            if (window.colorStack.Colors.Count == 0)
            {
                window.colorStack.Colors.Add(HexColor.Red);
            }
            
            window.onConfirm = onConfirm;
            window.titleContent = new GUIContent("Стопка цветов");
            
            // Позиционируем окно
            var rect = new Rect(position.x, position.y, 300, 400);
            window.position = rect;
            window.ShowPopup();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Настройка стопки цветов:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Кликните по прямоугольнику, чтобы изменить цвет", EditorStyles.miniLabel);
            EditorGUILayout.Space(5);

            // Кнопки управления количеством
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(40)))
            {
                colorStack.Colors.Add(HexColor.Red);
            }
            if (GUILayout.Button("-", GUILayout.Width(40)) && colorStack.Colors.Count > 1)
            {
                colorStack.Colors.RemoveAt(colorStack.Colors.Count - 1);
            }
            EditorGUILayout.LabelField($"Количество: {colorStack.Colors.Count}", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Скролл для стопки цветов
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(250));
            
            // Рисуем прямоугольники сверху вниз
            for (int i = colorStack.Colors.Count - 1; i >= 0; i--)
            {
                DrawColorRect(i);
                GUILayout.Space(2);
            }
            
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            // Кнопки OK и Cancel
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Отмена", GUILayout.Width(80)))
            {
                Close();
            }
            
            if (GUILayout.Button("OK", GUILayout.Width(80)))
            {
                onConfirm?.Invoke(colorStack);
                Close();
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
        }

        private void DrawColorRect(int index)
        {
            Rect rect = GUILayoutUtility.GetRect(280, 40);
            
            // Рисуем цветной прямоугольник
            Color unityColor = HexColorToUnityColor(colorStack.Colors[index]);
            EditorGUI.DrawRect(rect, unityColor);
            
            // Обводка
            Handles.BeginGUI();
            Handles.color = Color.black;
            Handles.DrawAAPolyLine(2f, 
                new Vector3(rect.x, rect.y),
                new Vector3(rect.xMax, rect.y),
                new Vector3(rect.xMax, rect.yMax),
                new Vector3(rect.x, rect.yMax),
                new Vector3(rect.x, rect.y)
            );
            Handles.EndGUI();

            // Номер слоя и название цвета
            string colorName = colorStack.Colors[index].ToString();
            GUI.Label(new Rect(rect.x + 5, rect.y + 5, rect.width - 10, 30), 
                $"#{colorStack.Colors.Count - index}: {colorName}", 
                new GUIStyle(EditorStyles.boldLabel) 
                { 
                    normal = { textColor = GetContrastColor(unityColor) },
                    fontSize = 11
                });

            // Обработка клика - меняем цвет на следующий из палитры
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                // Находим текущий цвет и берем следующий
                int currentIndex = System.Array.IndexOf(AvailableColors, colorStack.Colors[index]);
                int nextIndex = (currentIndex + 1) % AvailableColors.Length;
                colorStack.Colors[index] = AvailableColors[nextIndex];
                
                Event.current.Use();
                Repaint();
            }
        }

        private Color HexColorToUnityColor(HexColor hexColor)
        {
            return hexColor switch
            {
                HexColor.Red => Color.red,
                HexColor.Green => Color.green,
                HexColor.Blue => Color.blue,
                HexColor.Yellow => Color.yellow,
                HexColor.Purple => new Color(0.6f, 0f, 1f),
                _ => Color.white
            };
        }

        private Color GetContrastColor(Color backgroundColor)
        {
            float luminance = 0.299f * backgroundColor.r + 0.587f * backgroundColor.g + 0.114f * backgroundColor.b;
            return luminance > 0.5f ? Color.black : Color.white;
        }
    }
}
