using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Code.Infrastructure.Configs
{
    [Serializable]
    public class ColorStack
    {
        public List<HexColor> Colors;

        public ColorStack(List<HexColor> colors)
        {
            Colors = colors;
        }
    }

    [Serializable]
    public class CellConfig
    {
        public HexCellType Type;
        public int Cost;
        public ColorStack ColorStack;

        public CellConfig()
        {
            Type = HexCellType.Locked;
            Cost = 0;
            ColorStack = new ColorStack(new List<HexColor>());
        }
    }

    [CreateAssetMenu(menuName = "Hexa Sort/Level Config", fileName = "NewLevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int Width = 8;
        public int Height = 6;
        public List<CellConfig> Cells = new List<CellConfig>();

        public void EnsureSize()
        {
            int expected = Width * Height;
            
            if (Cells == null)
                Cells = new List<CellConfig>();

            // Добавляем новые ячейки если нужно
            while (Cells.Count < expected)
            {
                Cells.Add(new CellConfig());
            }

            // Удаляем лишние ячейки если нужно
            while (Cells.Count > expected)
            {
                Cells.RemoveAt(Cells.Count - 1);
            }

            // Проверяем, что все ячейки инициализированы
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i] == null)
                {
                    Cells[i] = new CellConfig();
                }
                else if (Cells[i].ColorStack == null)
                {
                    Cells[i].ColorStack = new ColorStack(new List<HexColor>());
                }
            }
        }

        public CellConfig GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) 
                return new CellConfig();
            
            int index = y * Width + x;
            if (index >= Cells.Count)
                return new CellConfig();
            
            if (Cells[index] == null)
                Cells[index] = new CellConfig();
            
            return Cells[index];
        }

        public HexCellType GetCellType(int x, int y)
        {
            return GetCell(x, y).Type;
        }

        public void SetCell(int x, int y, HexCellType type)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return;
            
            int index = y * Width + x;
            if (index >= Cells.Count) return;
            
            if (Cells[index] == null)
                Cells[index] = new CellConfig();
            
            Cells[index].Type = type;
        }

        public int GetCellCost(int x, int y)
        {
            return GetCell(x, y).Cost;
        }

        public void SetCellCost(int x, int y, int cost)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return;
            
            int index = y * Width + x;
            if (index >= Cells.Count) return;
            
            if (Cells[index] == null)
                Cells[index] = new CellConfig();
            
            Cells[index].Cost = cost;
        }

        public ColorStack GetCellColorStack(int x, int y)
        {
            CellConfig cell = GetCell(x, y);
            if (cell.ColorStack == null)
                cell.ColorStack = new ColorStack(new List<HexColor>());
            return cell.ColorStack;
        }

        public void SetCellColorStack(int x, int y, ColorStack colorStack)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return;
            
            int index = y * Width + x;
            if (index >= Cells.Count) return;
            
            if (Cells[index] == null)
                Cells[index] = new CellConfig();
            
            Cells[index].ColorStack = colorStack;
        }
    }

    public enum HexCellType
    {
        Locked = 0,
        Empty = 1,
        Paid = 2,
        Occupied = 3
    }

    public enum HexColor
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Yellow = 3,
        Purple = 4
    }
}