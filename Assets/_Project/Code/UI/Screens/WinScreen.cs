using _Project.Code.Gameplay;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using _Project.Code.Infrastructure.Services.SceneLoader;
using Reflex.Attributes;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Code.UI
{
    public class WinScreen : BaseScreen
    {
        [SerializeField] private Button _nextButton;
        
        private IPersistentService _persistent;
        private IConfigService _configService;
        private ISceneLoader _sceneLoader;

        [Inject]
        private void Construct(IPersistentService persistent, IConfigService configService, ISceneLoader sceneLoader)
        {
            _persistent = persistent;
            _configService = configService;
            _sceneLoader = sceneLoader;
        }

        public override void Initialize()
        {
            base.Initialize();

            _nextButton.onClick.AddListener(NextLevel);
            _persistent.Data.Progress.LevelData.Goals.Values.ForEach(x =>
            {
                x.CurrentAmount.Subscribe(_ => TryShowWin()).AddTo(this);
            });
        }

        private void TryShowWin()
        {
            if (Canvas.enabled)
                return;
            
            int goalsAmount = _persistent.Data.Progress.LevelData.Goals.Count;
            int completedGoals = 0;

            _persistent.Data.Progress.LevelData.Goals.ForEach(x =>
            {
                if (x.Value.CurrentAmount.Value >= x.Value.TargetAmount)
                {
                    completedGoals++;
                }
            });

            if (completedGoals >= goalsAmount)
            {
                Show();
            }
        }
        
        private async void NextLevel()
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