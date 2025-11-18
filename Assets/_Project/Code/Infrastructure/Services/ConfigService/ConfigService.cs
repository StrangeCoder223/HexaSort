using System.Linq;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Services.AssetProvider;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Infrastructure.Services.ConfigService
{
    public class ConfigService : IConfigService
    {
        private GameConfig _gameConfig;
        private readonly IAssetProvider _assetProvider;
        
        public ConfigService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }
        
        public async UniTask Load()
        {
            _gameConfig = await _assetProvider.LoadAsset<GameConfig>(RuntimeConstants.AssetLabels.GameConfig);
        }

        public MetaConfig ForMeta()
        {
            return _gameConfig.Meta;
        }
        
        public LevelConfig ForLevel(int level)
        {
            if (_gameConfig.Levels.Count < level)
                return null;
            
            return _gameConfig.Levels[level - 1];
        }

        public ColorConfig ForHexColor(HexColor hexColor)
        {
            return _gameConfig.Colors.FirstOrDefault(x => x.HexColor == hexColor);
        }

        public GeneratorConfig ForGenerator()
        {
            return _gameConfig.Generator;
        }
    }
}