using System.Collections.Generic;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Factories;
using _Project.Code.Infrastructure.Services.ConfigService;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Gameplay.Generators
{
    public class LevelGenerator
    {
        public IReadOnlyList<Cell> GeneratedCells => _generatedCells;
        
        private Transform _gridCenterTransform;
        private readonly IGameFactory _gameFactory;
        private readonly IConfigService _configService;
        private readonly List<Cell> _generatedCells;

        public LevelGenerator(IConfigService configService, IGameFactory gameFactory)
        {
            _configService = configService;
            _gameFactory = gameFactory;
            _generatedCells = new List<Cell>();
        }

        public void Initialize(Transform gridCenterTransform)
        {
            _gridCenterTransform = gridCenterTransform;
        }

        public async UniTask GenerateFor(LevelData levelData)
        {
            GeneratorConfig generatorConfig = _configService.ForGenerator();
            
            HexGridLayout gridLayout = new HexGridLayout(generatorConfig, levelData.Width, levelData.Height);
            Vector3 gridOffset = _gridCenterTransform.position - gridLayout.CalculateGridCenter();
            
            foreach (CellData cellData in levelData.Cells)
            {
                Cell cell = await GenerateCell(cellData, gridLayout, gridOffset);
                _generatedCells.Add(cell);
            }
        }
        
        private async UniTask<Cell> GenerateCell(CellData cellData, HexGridLayout gridLayout, Vector3 gridOffset)
        {
            Vector3 cellPosition = gridLayout.GetCellPosition(cellData.Position.x, cellData.Position.y) + gridOffset;
             
            Cell cell = await _gameFactory.CreateEmptyCell(cellData);
            cell.transform.position = cellPosition;

            if (cellData.StackColors.Count > 0)
            {
                HexStack hexStack = await _gameFactory.CreateHexStack(new ColorStack(cellData.StackColors));
                cell.Occupy(hexStack);
            }
            
            return cell;
        }
    }
}