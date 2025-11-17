using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Code.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = RuntimeConstants.AssetLabels.GameConfig, menuName = "Configs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public GeneratorConfig Generator;
        public MetaConfig Meta;
        public List<LevelConfig> Levels;
        public List<ColorConfig> Colors;
    }

    [Serializable]
    public class GeneratorConfig
    {
        public int MaxOffersCount = 3;
        public int MinStackHeight = 2;
        public int MaxStackHeight = 6;
        
        public float PureStackChance = 0.3f;
        public float NewColorChance = 0.1f;
        
        public float HexRadius = 0.5f;
        public float HexWidthMultiplier = 2f;
        public float HorizontalSpacingMultiplier = 0.75f;
        public float ColumnOffsetMultiplier = 0.5f;
    }

    [Serializable]
    public class ColorConfig
    {
        public HexColor HexColor;
        public Color MeshColor;
    }
}