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
        private List<Cell> _generatedCells;
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
            
            LevelConfig levelConfig = _configService.ForLevel(_persistent.Data.Progress.Level);
            
            if (_persistent.Data.Progress.LevelData == null)
            {
                _persistent.Data.Progress.LevelData = new()
                {
                    Width = levelConfig.Width,
                    Height = levelConfig.Height,
                    Cells = new(),
                    Goals = new()
                };

                for (int i = 0; i < levelConfig.Cells.Count; i++)
                {
                    if (levelConfig.Cells[i].Type == HexCellType.Locked)
                        continue;

                    int x = i % levelConfig.Width;
                    int y = i / levelConfig.Width;
                    
                    _persistent.Data.Progress.LevelData.Cells.Add(new CellData()
                    {
                        X = x,
                        Y = y,
                        Cost = levelConfig.Cells[i].Cost,
                        StackColors = levelConfig.Cells[i].ColorStack.Colors
                    });
                }
                
                levelConfig.Goals.ForEach(x =>
                {
                    _persistent.Data.Progress.LevelData.Goals.TryAdd(x.TargetColor, new GoalData()
                    {
                        HexColor = x.TargetColor,
                        CurrentAmount = new(0),
                        TargetAmount = x.Amount
                    });
                });
            }
        }

        public async UniTask Generate()
        {
            GeneratorConfig generatorConfig = _configService.ForGenerator();
            LevelData levelData = _persistent.Data.Progress.LevelData;
            
            float hexRadius = generatorConfig.HexRadius;
            float hexWidth = hexRadius * generatorConfig.HexWidthMultiplier;
            float hexHeight = hexRadius * Mathf.Sqrt(3f);
            float horizontalSpacing = hexWidth * generatorConfig.HorizontalSpacingMultiplier;
            
            Vector3 gridCenter = CalculateGridCenter(levelData.Width, levelData.Height, 
                horizontalSpacing, hexHeight, generatorConfig.ColumnOffsetMultiplier);
            
            Vector3 targetCenter = _gridCenterTransform.position;
            
            Vector3 offset = targetCenter - gridCenter;
            
            for (int i = 0; i < levelData.Cells.Count; i++)
            {
                CellData cellData = levelData.Cells[i];
                
                Vector3 cellPosition = GetCellPosition(cellData.X, cellData.Y, horizontalSpacing, hexHeight, offset, generatorConfig.ColumnOffsetMultiplier);
                
                Cell emptyCell = await _gameFactory.CreateEmptyCell(cellData);
                emptyCell.transform.position = cellPosition;

                if (cellData.StackColors.Count > 0)
                {
                    HexStack hexStack = await _gameFactory.CreateHexStack(new ColorStack(cellData.StackColors));
                    emptyCell.Occupy(hexStack);
                }
                
                _generatedCells.Add(emptyCell);
            }
        }

        private Vector3 GetCellPosition(int x, int y, float horizontalSpacing, float hexHeight, Vector3 offset, float oddColumnOffsetMultiplier)
        {
            //if cell odd, add offset for hex grid
            float offsetY = (x % 2 == 1) ? hexHeight * oddColumnOffsetMultiplier : 0f;
            
            Vector3 position = new Vector3(x * horizontalSpacing, 0f, y * hexHeight + offsetY);
            
            return position + offset;
        }

        private Vector3 CalculateGridCenter(int width, int height, float horizontalSpacing, float hexHeight, float oddColumnOffsetMultiplier)
        {
            float gridWidth = (width - 1) * horizontalSpacing;

            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float offsetY = (x % 2 == 1) ? hexHeight * oddColumnOffsetMultiplier : 0f;
                    float z = y * hexHeight + offsetY;

                    if (z < minZ) minZ = z;
                    if (z > maxZ) maxZ = z;
                }
            }

            float gridHeight = maxZ - minZ;
            
            return new Vector3(gridWidth * 0.5f, 0f, minZ + gridHeight * 0.5f);
        }
    }
}