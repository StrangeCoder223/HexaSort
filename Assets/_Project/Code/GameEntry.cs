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
        
        private StackOfferSpawner _stackOfferSpawner;
        private LevelGenerator _levelGenerator;
        private IPersistentService _persistent;
        private IConfigService _configService;

        [Inject]
        private void Construct(IPersistentService persistent, IConfigService configService, LevelGenerator levelGenerator, 
            StackOfferSpawner offerSpawner)
        {
            _persistent = persistent;
            _configService = configService;
            _levelGenerator = levelGenerator;
            _stackOfferSpawner = offerSpawner;
        }
        
        public override void Initialize()
        {
            InitializeGameplay();
            InitializeUI();
        }

        private async void InitializeGameplay()
        {
            _levelGenerator.Initialize();
            
            await _levelGenerator.Generate();
            _stackOfferSpawner.Initialize().Forget();
        }

        private void InitializeUI()
        {
            _screens.ForEach(x => x.Initialize());
        }
    }
}
