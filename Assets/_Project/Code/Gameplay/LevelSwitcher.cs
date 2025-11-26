using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using _Project.Code.Infrastructure.Services.SceneLoader;
using Cysharp.Threading.Tasks;

namespace _Project.Code.Gameplay
{
    public class LevelSwitcher
    {
        private readonly IPersistentService _persistent;
        private readonly ISceneLoader _sceneLoader;
        private readonly IConfigService _configService;

        public LevelSwitcher(IPersistentService persistent, ISceneLoader sceneLoader, IConfigService configService)
        {
            _persistent = persistent;
            _sceneLoader = sceneLoader;
            _configService = configService;
        }
        
        public async UniTask NextLevel()
        {
            _persistent.Data.Progress.LevelData = null;
            
            int nextLevel = _persistent.Data.Progress.Level + 1;

            if (_configService.ForLevel(nextLevel) == null)
                nextLevel = 1;

            _persistent.Data.Progress.Level = nextLevel;
            
            await _sceneLoader.LoadScene(RuntimeConstants.Scenes.Game);
        }
    }
}