using System;
using _Project.Code.Gameplay;
using _Project.Code.Infrastructure.Services.PersistentService;
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
        private LevelSwitcher _levelSwitcher;
        private IPersistentService _persistent;

        [Inject]
        private void Construct(IPersistentService persistent, LevelSwitcher levelSwitcher)
        {
            _persistent = persistent;
            _levelSwitcher = levelSwitcher;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            _nextButton.onClick.AddListener(async () => await _levelSwitcher.NextLevel());
            
            _persistent.Data.Progress.LevelData.Goals.Values.ForEach(x =>
            {
                x.CurrentAmount.Subscribe(_ => TryShowWin()).AddTo(this);
            });
        }

        private void OnDestroy() => _nextButton.onClick.RemoveAllListeners();

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
    }
}