using System.Collections.Generic;
using _Project.Code.Gameplay;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using _Project.Code.UI;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;

namespace _Project.Code
{
    public class GameEntry : SceneEntry
    {
        [SerializeField] private List<BaseScreen> _screens;
        private IPersistentService _persistent;
        private IConfigService _configService;
        private LevelGenerator _levelGenerator;

        [Inject]
        private void Construct(IPersistentService persistent, IConfigService configService, LevelGenerator levelGenerator)
        {
            _persistent = persistent;
            _configService = configService;
            _levelGenerator = levelGenerator;
        }
        
        public override void Initialize()
        {
            InitializeGameplay();
            InitializeUI();
        }

        private void InitializeGameplay()
        {
            _levelGenerator.Generate(_persistent.Data.Progress.Level);
        }

        private void InitializeUI()
        {
            _screens.ForEach(x => x.Initialize());
        }
    }
}
