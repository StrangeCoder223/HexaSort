using System;
using System.Collections.Generic;
using _Project.Code.Infrastructure.Configs;
using UniRx;
using UnityEngine.Serialization;

namespace _Project.Code.Infrastructure.Data
{
    [Serializable]
    public class PlayerProgress
    {
        public int Level;
        public ReactiveProperty<int> Money;
        public ReactiveProperty<int> Life;
        public ReactiveProperty<int> LifeRestoreTime;
        public LevelData LevelData;
    }

    [Serializable]
    public class LevelData
    {
        public int Width;
        public int Height;
        public Dictionary<HexColor, GoalData> Goals;
        public List<CellData> Cells;
    }

    [Serializable]
    public class CellData
    {
        public int X;
        public int Y;
        public int Cost;
        public List<HexColor> StackColors;
    }

    [Serializable]
    public class GoalData
    {
        public HexColor HexColor;
        public int TargetAmount;
        public ReactiveProperty<int> CurrentAmount;
    }
}