using System.Collections.Generic;
using UnityEngine;

namespace _Project.Code.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = RuntimeConstants.AssetLabels.GameConfig, menuName = "Configs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public MetaConfig Meta;
        public List<LevelConfig> Levels;
    }
}