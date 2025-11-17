using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Factories;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class LevelGenerator
    {
        private readonly IGameFactory _gameFactory;
        private readonly IConfigService _configService;
        private readonly IPersistentService _persistent;

        public LevelGenerator(IPersistentService persistent, IConfigService configService, IGameFactory gameFactory)
        {
            _persistent = persistent;
            _configService = configService;
            _gameFactory = gameFactory;
        }

        public async void Generate(int level)
        {
            LevelConfig levelConfig = _configService.ForLevel(level);
            GeneratorConfig generatorConfig = _configService.ForGenerator();
            SessionData sessionData = _persistent.Data.Progress.SessionData;
            
            float hexRadius = generatorConfig.HexRadius;
            float hexWidth = hexRadius * generatorConfig.HexWidthMultiplier;
            float hexHeight = hexRadius * Mathf.Sqrt(3f);
            float horizontalSpacing = hexWidth * generatorConfig.HorizontalSpacingMultiplier;
            
            Vector3 gridCenter = CalculateGridCenter(levelConfig, horizontalSpacing, hexHeight, generatorConfig.ColumnOffsetMultiplier);
            Vector3 targetCenter = Vector3.zero;
            
            Vector3 offset = targetCenter - gridCenter;
            
            for (int i = 0; i < levelConfig.Cells.Count; i++)
            {
                if (levelConfig.Cells[i].Type == HexCellType.Locked)
                    continue;
                
                CellConfig cellConfig = levelConfig.Cells[i];
                
                int x = i % levelConfig.Width;
                int y = i / levelConfig.Width;
                
                Vector3 cellPosition = GetCellPosition(x, y, horizontalSpacing, hexHeight, offset, generatorConfig.ColumnOffsetMultiplier);
                
                Cell emptyCell = await _gameFactory.CreateEmptyCell();
                emptyCell.transform.position = cellPosition;
                
                HexStack hexStack = await _gameFactory.CreateHexStack(cellConfig.ColorStack);
                emptyCell.Occupy(hexStack);
            }
        }

        private Vector3 GetCellPosition(int x, int y, float horizontalSpacing, float hexHeight, Vector3 offset, float oddColumnOffsetMultiplier)
        {
            //if cell odd, add offset for hex grid
            float offsetY = (x % 2 == 1) ? hexHeight * oddColumnOffsetMultiplier : 0f;
            
            Vector3 position = new Vector3(x * horizontalSpacing, 0f, y * hexHeight + offsetY);
            
            return position + offset;
        }

        private Vector3 CalculateGridCenter(LevelConfig levelConfig, float horizontalSpacing, float hexHeight, float oddColumnOffsetMultiplier)
        {
            float gridWidth = (levelConfig.Width - 1) * horizontalSpacing;

            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            for (int y = 0; y < levelConfig.Height; y++)
            {
                for (int x = 0; x < levelConfig.Width; x++)
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