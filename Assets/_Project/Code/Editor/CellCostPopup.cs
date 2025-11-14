using UnityEditor;
using UnityEngine;

namespace _Project.Code.Editor
{
    public class CellCostPopup : EditorWindow
    {
        private int cost = 100;
        private System.Action<int> onConfirm;

        public static void Show(Vector2 position, int currentCost, System.Action<int> onConfirm)
        {
            var window = CreateInstance<CellCostPopup>();
            window.cost = currentCost > 0 ? currentCost : 100;
            window.onConfirm = onConfirm;
            window.titleContent = new GUIContent("Стоимость клетки");
            
            // Позиционируем окно рядом с курсором
            var rect = new Rect(position.x, position.y, 250, 100);
            window.position = rect;
            window.ShowPopup();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Укажите стоимость открытия клетки:", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(5);
            
            cost = EditorGUILayout.IntField("Стоимость:", cost);
            cost = Mathf.Max(0, cost); // Не меньше 0
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Отмена", GUILayout.Width(80)))
            {
                Close();
            }
            
            if (GUILayout.Button("OK", GUILayout.Width(80)))
            {
                onConfirm?.Invoke(cost);
                Close();
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // Закрытие на Escape
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Close();
                Event.current.Use();
            }
            
            // Подтверждение на Enter
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                onConfirm?.Invoke(cost);
                Close();
                Event.current.Use();
            }
        }
    }
}
