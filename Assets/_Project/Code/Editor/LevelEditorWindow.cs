using _Project.Code.Infrastructure.Configs;
using UnityEditor;
using UnityEngine;

namespace _Project.Code.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Hex Grid Editor")]
        public static void Open() => GetWindow<LevelEditorWindow>("Hex Grid");
        
        private LevelConfig activeLevel;
        private float hexRadius = 30f;   // радиус гекса
        private int selectedType = 1;    // тип плитки для "рисования"
        private Vector2 scroll;
        
        protected void OnGUI()
        {
            activeLevel = (LevelConfig)EditorGUILayout.ObjectField("Active Level", activeLevel, typeof(LevelConfig), false);

            if (activeLevel == null)
            {
                if (GUILayout.Button("Create New Level")) CreateNewLevel();
                return;
            }

            activeLevel.EnsureSize();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grid Size:", GUILayout.Width(70));
            int newWidth = EditorGUILayout.IntField("Width", activeLevel.Width, GUILayout.Width(250));
            int newHeight = EditorGUILayout.IntField("Height", activeLevel.Height, GUILayout.Width(250));
            GUILayout.EndHorizontal();

            if (newWidth != activeLevel.Width || newHeight != activeLevel.Height)
            {
                activeLevel.Width = Mathf.Clamp(newWidth, 1, 20);
                activeLevel.Height = Mathf.Clamp(newHeight, 1, 20);
                activeLevel.EnsureSize();
                EditorUtility.SetDirty(activeLevel);
            }

            hexRadius = EditorGUILayout.Slider("Hex Size", hexRadius, 15f, 50f);
            selectedType = EditorGUILayout.Popup("Paint Type", selectedType, System.Enum.GetNames(typeof(HexCellType)));

            GUILayout.Space(5);
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(false), GUILayout.MaxHeight(600));
            DrawHexGrid(activeLevel);
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace(); // Пустое пространство внизу

            if (GUILayout.Button("Save")) EditorUtility.SetDirty(activeLevel);
        }

        private void DrawHexGrid(LevelConfig level)
        {
            Handles.BeginGUI();
            
            // Размеры для flat-top гексагонов (плоская сторона сверху/снизу)
            float hexWidth = hexRadius * 2f;                    // полная ширина гекса
            float hexHeight = hexRadius * Mathf.Sqrt(3f);      // высота гекса
            float horizontalSpacing = hexWidth * 0.75f;        // расстояние между центрами по горизонтали
            Vector2 start = new Vector2(50, 50);

            // Проходим по всем ячейкам сетки
            for (int y = 0; y < level.Height; y++)
            {
                for (int x = 0; x < level.Width; x++)
                {
                    // Смещение для нечетных столбцов по вертикали (чтобы получилась сотовая структура)
                    float offsetY = (x % 2 == 1) ? hexHeight * 0.5f : 0f;
                    
                    // Вычисляем позицию центра гексагона
                    Vector2 center = start + new Vector2(
                        x * horizontalSpacing, 
                        y * hexHeight + offsetY
                    );
                    
                    DrawHexCell(center, hexRadius, level.GetCellType(x, y), x, y);
                }
            }

            Handles.EndGUI();
        }

        private void DrawHexCell(Vector2 center, float radius, HexCellType type, int gx, int gy)
        {
            Vector3[] pts = GetHexPoints(center, radius);

            // Заливка
            Handles.color = GetColor(type, gx, gy);
            Handles.DrawAAConvexPolygon(pts);

            // Контур (коричневый цвет как на изображении)
            Handles.color = new Color(0.6f, 0.4f, 0.3f); // коричневый
            Handles.DrawAAPolyLine(3f, pts[0], pts[1], pts[2], pts[3], pts[4], pts[5], pts[0]);

            // Превью стопки цветов для Occupied клеток
            if (type == HexCellType.Occupied)
            {
                DrawColorStackPreview(center, radius, gx, gy);
            }

            // Текст по центру
            if (hexRadius > 20f)
            {
                GUIStyle style = new GUIStyle(EditorStyles.miniLabel) 
                { 
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 8
                };
                
                // Для платных клеток показываем стоимость
                if (type == HexCellType.Paid)
                {
                    int cost = activeLevel.GetCellCost(gx, gy);
                    GUIStyle costStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 10,
                        normal = { textColor = new Color(0.8f, 0.5f, 0f) }
                    };
                    GUI.Label(new Rect(center.x - 25, center.y - 7, 50, 14), $"${cost}", costStyle);
                }
                // Для занятых клеток превью показывает всё, текст не нужен
                else if (type != HexCellType.Occupied)
                {
                    GUI.Label(new Rect(center.x - 20, center.y - 6, 40, 12), $"{gx},{gy}", style);
                }
            }

            // Обработка клика
            if (Event.current.type == EventType.MouseDown)
            {
                if (PointInPolygon(Event.current.mousePosition, pts))
                {
                    // Левый клик - меняем тип клетки
                    if (Event.current.button == 0)
                    {
                        HexCellType newType = (HexCellType)selectedType;
                        activeLevel.SetCell(gx, gy, newType);
                        
                        // Если тип Paid - открываем окно для ввода стоимости
                        if (newType == HexCellType.Paid)
                        {
                            int currentCost = activeLevel.GetCellCost(gx, gy);
                            Vector2 popupPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                            CellCostPopup.Show(popupPosition, currentCost, (cost) =>
                            {
                                activeLevel.SetCellCost(gx, gy, cost);
                                EditorUtility.SetDirty(activeLevel);
                                Repaint();
                            });
                        }
                        // Если тип Occupied - открываем окно для настройки стопки цветов
                        else if (newType == HexCellType.Occupied)
                        {
                            ColorStack currentStack = activeLevel.GetCellColorStack(gx, gy);
                            Vector2 popupPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                            ColorStackPopup.Show(popupPosition, currentStack, (stack) =>
                            {
                                activeLevel.SetCellColorStack(gx, gy, stack);
                                EditorUtility.SetDirty(activeLevel);
                                Repaint();
                            });
                        }
                        
                        GUI.changed = true;
                        Event.current.Use();
                        Repaint();
                    }
                    // Правый клик - редактируем параметры
                    else if (Event.current.button == 1)
                    {
                        Vector2 popupPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                        
                        // Для Paid клеток - редактируем стоимость
                        if (type == HexCellType.Paid)
                        {
                            int currentCost = activeLevel.GetCellCost(gx, gy);
                            CellCostPopup.Show(popupPosition, currentCost, (cost) =>
                            {
                                activeLevel.SetCellCost(gx, gy, cost);
                                EditorUtility.SetDirty(activeLevel);
                                Repaint();
                            });
                        }
                        // Для Occupied клеток - редактируем стопку цветов
                        else if (type == HexCellType.Occupied)
                        {
                            ColorStack currentStack = activeLevel.GetCellColorStack(gx, gy);
                            ColorStackPopup.Show(popupPosition, currentStack, (stack) =>
                            {
                                activeLevel.SetCellColorStack(gx, gy, stack);
                                EditorUtility.SetDirty(activeLevel);
                                Repaint();
                            });
                        }
                        
                        Event.current.Use();
                        Repaint();
                    }
                }
            }
        }

        private void DrawColorStackPreview(Vector2 center, float radius, int gx, int gy)
        {
            ColorStack stack = activeLevel.GetCellColorStack(gx, gy);
            if (stack?.Colors == null || stack.Colors.Count == 0) return;

            // Размеры области превью
            float previewWidth = radius * 1.2f;
            float maxPreviewHeight = radius * 1.2f;
            int colorCount = Mathf.Min(stack.Colors.Count, 10); // Максимум 10 полосок
            float stripHeight = maxPreviewHeight / colorCount;
            
            // Начальная позиция (снизу вверх, как стопка)
            float startY = center.y + (maxPreviewHeight / 2f);

            for (int i = 0; i < colorCount; i++)
            {
                HexColor hexColor = stack.Colors[i];
                Color unityColor = HexColorToUnityColor(hexColor);
                
                float yPos = startY - (i + 1) * stripHeight;
                Rect stripRect = new Rect(
                    center.x - previewWidth / 2f, 
                    yPos,
                    previewWidth, 
                    stripHeight - 1 // -1 для небольшого зазора
                );

                // Рисуем полоску цвета
                EditorGUI.DrawRect(stripRect, unityColor);
                
                // Тонкая обводка
                Handles.BeginGUI();
                Handles.color = new Color(0, 0, 0, 0.3f);
                Handles.DrawAAPolyLine(1f,
                    new Vector3(stripRect.x, stripRect.y),
                    new Vector3(stripRect.xMax, stripRect.y),
                    new Vector3(stripRect.xMax, stripRect.yMax),
                    new Vector3(stripRect.x, stripRect.yMax),
                    new Vector3(stripRect.x, stripRect.y)
                );
                Handles.EndGUI();
            }

            // Если цветов больше 10, показываем "..."
            if (stack.Colors.Count > 10)
            {
                GUIStyle moreStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 8,
                    normal = { textColor = Color.black }
                };
                GUI.Label(new Rect(center.x - 15, startY - stripHeight / 2 - 5, 30, 10), "...", moreStyle);
            }
        }

        private Vector3[] GetHexPoints(Vector2 center, float radius)
        {
            Vector3[] pts = new Vector3[6];
            // Плоские стороны сверху/снизу, острые углы слева/справа
            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * (60 * i); // 0° делает верхнюю/нижнюю стороны плоскими
                pts[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
            return pts;
        }

        private bool PointInPolygon(Vector2 point, Vector3[] poly)
        {
            bool inside = false;
            int j = poly.Length - 1;
            for (int i = 0; i < poly.Length; j = i++)
            {
                Vector2 pi = poly[i];
                Vector2 pj = poly[j];
                if (((pi.y > point.y) != (pj.y > point.y)) &&
                    (point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y + 1e-6f) + pi.x))
                    inside = !inside;
            }
            return inside;
        }

        private void CreateNewLevel()
        {
            var level = ScriptableObject.CreateInstance<LevelConfig>();
            level.EnsureSize();
            string path = EditorUtility.SaveFilePanelInProject("Create Hex Level", "NewHexLevel", "asset", "Select location");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(level, path);
                AssetDatabase.SaveAssets();
                activeLevel = level;
            }
        }

        private Color GetColor(HexCellType type, int x = -1, int y = -1)
        {
            switch (type)
            {
                case HexCellType.Locked:
                    return new Color(0.3f, 0.3f, 0.3f);  // темно-серый
                case HexCellType.Empty:
                    return Color.white;  // белый как на изображении
                case HexCellType.Paid:
                    return new Color(1f, 0.9f, 0.5f);  // светло-желтый
                case HexCellType.Occupied:
                    return new Color(0.8f, 0.8f, 1f);  // светло-голубой по умолчанию
                default:
                    return Color.white;
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
    }
}
