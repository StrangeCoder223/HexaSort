using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using _Project.Code.Infrastructure.Services.SaveLoadService;
using _Project.Code.Infrastructure.Services.SceneLoader;
using Reflex.Attributes;
using Reflex.Core;

namespace _Project.Code
{
    public class BootstrapEntry : SceneEntry
    {
        private ISaveLoadService _saveLoadService;
        private IConfigService _configService;
        private IPersistentService _persistentService;
        private ISceneLoader _sceneLoader;

        [Inject]
        private void Construct(IConfigService configService, ISaveLoadService saveLoadService, IPersistentService persistentService,
            ISceneLoader sceneLoader)
        {
            _configService = configService;
            _saveLoadService = saveLoadService;
            _persistentService = persistentService;
            _sceneLoader = sceneLoader;
        }

        public override async void Initialize()
        {
            BootServices();

            await _sceneLoader.LoadScene(RuntimeConstants.Scenes.Game);
        }

        private void BootServices()
        {
            _configService.Load();
            
            PersistentData savedData = _saveLoadService.Load();

            if (savedData == null) 
                _persistentService.Persistent = _persistentService.CreateDefaultPersistent();
            else
                _persistentService.Persistent = savedData;
        }
    }
}