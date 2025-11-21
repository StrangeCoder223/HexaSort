using System.Collections.Generic;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Factories;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class LevelGenerator
    {
        public IReadOnlyList<Cell> GeneratedCells => _generatedCells;
        
        private readonly IGameFactory _gameFactory;
        private readonly IConfigService _configService;
        private readonly IPersistentService _persistent;
        private readonly List<Cell> _generatedCells;
        private Transform _gridCenterTransform;

        public LevelGenerator(IPersistentService persistent, IConfigService configService, IGameFactory gameFactory)
        {
            _persistent = persistent;
            _configService = configService;
            _gameFactory = gameFactory;
            _generatedCells = new List<Cell>();
        }

        public void Initialize(Transform gridCenterTransform)
        {
            _gridCenterTransform = gridCenterTransform;
            
            if (_persistent.Data.Progress.LevelData == null)
            {
                LevelConfig levelConfig = _configService.ForLevel(_persistent.Data.Progress.Level);
                InitializeLevelData(levelConfig);
            }
        }

        public async UniTask Generate()
        {
            GeneratorConfig generatorConfig = _configService.ForGenerator();
            LevelData levelData = _persistent.Data.Progress.LevelData;
            
            HexGridLayout gridLayout = new HexGridLayout(generatorConfig, levelData.Width, levelData.Height);
            Vector3 gridOffset = _gridCenterTransform.position - gridLayout.CalculateGridCenter();
            
            foreach (CellData cellData in levelData.Cells)
            {
                Cell cell = await CreateCell(cellData, gridLayout, gridOffset);
                _generatedCells.Add(cell);
            }
        }
        
        private async UniTask<Cell> CreateCell(CellData cellData, HexGridLayout gridLayout, Vector3 gridOffset)
        {
            Vector3 cellPosition = gridLayout.GetCellPosition(cellData.X, cellData.Y) + gridOffset;
            
            Cell cell = await _gameFactory.CreateEmptyCell(cellData);
            cell.transform.position = cellPosition;

            if (cellData.StackColors.Count > 0)
            {
                HexStack hexStack = await _gameFactory.CreateHexStack(new ColorStack(cellData.StackColors));
                cell.Occupy(hexStack);
            }
            
            return cell;
        }

        private void InitializeLevelData(LevelConfig levelConfig)
        {
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
                    X = i % levelConfig.Width,
                    Y = i / levelConfig.Width,
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
    
    public class HexGridLayout
    {
        private readonly float _horizontalSpacing;
        private readonly float _hexHeight;
        private readonly float _oddColumnOffsetMultiplier;
        private readonly int _width;
        private readonly int _height;

        public HexGridLayout(GeneratorConfig config, int width, int height)
        {
            float hexRadius = config.HexRadius;
            float hexWidth = hexRadius * config.HexWidthMultiplier;
            
            _hexHeight = hexRadius * Mathf.Sqrt(3f);
            _horizontalSpacing = hexWidth * config.HorizontalSpacingMultiplier;
            _oddColumnOffsetMultiplier = config.ColumnOffsetMultiplier;
            _width = width;
            _height = height;
        }

        public Vector3 GetCellPosition(int x, int y)
        {
            float zOffset = GetColumnZOffset(x);
            return new Vector3(x * _horizontalSpacing, 0f, y * _hexHeight + zOffset);
        }

        public Vector3 CalculateGridCenter()
        {
            float gridWidth = (_width - 1) * _horizontalSpacing;
            (float minZ, float maxZ) = CalculateZBounds();
            float gridHeight = maxZ - minZ;
            
            return new Vector3(gridWidth * 0.5f, 0f, minZ + gridHeight * 0.5f);
        }

        private (float minZ, float maxZ) CalculateZBounds()
        {
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    float z = y * _hexHeight + GetColumnZOffset(x);
                    if (z < minZ) minZ = z;
                    if (z > maxZ) maxZ = z;
                }
            }

            return (minZ, maxZ);
        }

        private float GetColumnZOffset(int x)
        {
            return (x % 2 == 1) ? _hexHeight * _oddColumnOffsetMultiplier : 0f;
        }
    }
}