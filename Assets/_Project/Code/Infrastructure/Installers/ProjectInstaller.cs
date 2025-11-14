using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Services.AssetProvider;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using _Project.Code.Infrastructure.Services.SaveLoadService;
using _Project.Code.Infrastructure.Services.SceneLoader;
using _Project.Code.UI;
using Reflex.Core;
using UnityEngine;

namespace _Project.Code.Infrastructure.Installers
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private LoadingScreen _loadingScreen;
        
        private IConfigService _configService;
        private IPersistentService _persistentService;
        private ISaveLoadService _saveLoadService;
        private IAssetProvider _assetProvider;
        private ISceneLoader _sceneLoader;
    
        public void InstallBindings(ContainerBuilder builder)
        {
            _assetProvider = new AssetProvider();
            _configService = new ConfigService(_assetProvider);
            _persistentService = new PersistentService(_configService);
            _saveLoadService = new SaveLoadService(_persistentService);
            _sceneLoader = CreateSceneLoader();
            
            
            builder.AddSingleton(_configService, typeof(IConfigService));
            builder.AddSingleton(_persistentService, typeof(IPersistentService));
            builder.AddSingleton(_saveLoadService, typeof(ISaveLoadService));
            builder.AddSingleton(_sceneLoader, typeof(ISceneLoader));
        }

        private ISceneLoader CreateSceneLoader()
        {
            LoadingScreen loadingScreen = Instantiate(_loadingScreen);
            loadingScreen.Initialize();
            
            return new SceneLoader(loadingScreen);
        }
    }
}