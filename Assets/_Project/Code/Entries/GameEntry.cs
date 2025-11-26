using System;
using System.Collections.Generic;
using _Project.Code.Gameplay;
using _Project.Code.Gameplay.Generators;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Factories;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using _Project.Code.UI;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;

namespace _Project.Code
{
    public class GameEntry : SceneEntry
    {
        [SerializeField] private Transform _gridCenterTransform;
        [SerializeField] private List<BaseScreen> _screens;
        [SerializeField] private HexStackTransfer _hexStackTransfer;
        [SerializeField] private Hud _hud;
        
        private StackOfferSpawner _stackOfferSpawner;
        private LevelGenerator _levelGenerator;
        private IPersistentService _persistent;
        private IConfigService _configService;
        private IGameFactory _gameFactory;

        [Inject]
        private void Construct(IPersistentService persistent, IConfigService configService, IGameFactory gameFactory, LevelGenerator levelGenerator, 
            StackOfferSpawner offerSpawner)
        {
            _persistent = persistent;
            _configService = configService;
            _levelGenerator = levelGenerator;
            _gameFactory = gameFactory;
            _stackOfferSpawner = offerSpawner;
        }
        
        public override async UniTask Initialize()
        {
            InitializeData();
            await InitializeGameplay();
            InitializeUI();
        }

        private void OnDestroy()
        {
            _gameFactory.Clear();
        }

        private async UniTask InitializeGameplay()
        {
            _levelGenerator.Initialize(_gridCenterTransform);
            
            await _levelGenerator.GenerateFor(_persistent.Data.Progress.LevelData);
            await _hexStackTransfer.Initialize();
            
            _stackOfferSpawner.Initialize().Forget();
        }

        private void InitializeUI()
        {
            _screens.ForEach(x => x.Initialize());
            _hud.Initialize();
        }

        private void InitializeData()
        {
            if (_persistent.Data.Progress.LevelData == null)
            { 
                InitializeLevelData();
            }
        }
        
        private void InitializeLevelData()
        {
            LevelConfig levelConfig = _configService.ForLevel(_persistent.Data.Progress.Level);
            
            _persistent.Data.Progress.LevelData = new LevelData
            {
                Width = levelConfig.Width,
                Height = levelConfig.Height,
                Cells = CreateCellDataList(levelConfig),
                Goals = CreateGoalDataDictionary(levelConfig)
            };
        }

        private List<CellData> CreateCellDataList(LevelConfig levelConfig)
        {
            List<CellData> cells = new List<CellData>();

            for (int i = 0; i < levelConfig.Cells.Count; i++)
            {
                CellConfig cellConfig = levelConfig.Cells[i];
                
                if (cellConfig.Type == HexCellType.Locked)
                    continue;

                cells.Add(new CellData
                {
                    Position = new (i % levelConfig.Width, i / levelConfig.Width),
                    Cost = cellConfig.Cost,
                    StackColors = cellConfig.ColorStack.Colors
                });
            }

            return cells;
        }

        private Dictionary<HexColor, GoalData> CreateGoalDataDictionary(LevelConfig levelConfig)
        {
            Dictionary<HexColor, GoalData> goals = new Dictionary<HexColor, GoalData>();

            foreach (var goal in levelConfig.Goals)
            {
                goals.TryAdd(goal.TargetColor, new GoalData
                {
                    HexColor = goal.TargetColor,
                    CurrentAmount = new(0),
                    TargetAmount = goal.Amount
                });
            }

            return goals;
        }
    }
}
