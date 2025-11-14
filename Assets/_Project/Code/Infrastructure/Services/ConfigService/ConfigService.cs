using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Services.AssetProvider;

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
        
        public void Load()
        {
            _gameConfig = _assetProvider.LoadAsset<GameConfig>(RuntimeConstants.AssetLabels.GameConfig);
        }

        public MetaConfig ForMeta()
        {
            return _gameConfig.Meta;
        }
        
        public LevelConfig ForLevel(int level)
        {
            return _gameConfig.Levels[level - 1];
        }
    }
}