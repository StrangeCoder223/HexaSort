using System.Collections.Generic;
using _Project.Code.Gameplay;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Services.AssetProvider;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Infrastructure.Factories
{
    public class GameFactory : ObjectFactory, IGameFactory
    {
        public IReadOnlyList<IProgressWriter> ProgressWriters => _progressWriters;
        public IReadOnlyList<IProgressReader> ProgressReaders => _progressReaders;
        
        private readonly List<IProgressWriter> _progressWriters;
        private readonly List<IProgressReader> _progressReaders;
        private readonly IConfigService _configService;
        private readonly IPersistentService _persistent;

        public GameFactory(IAssetProvider assetProvider, IConfigService configService, IPersistentService persistent) : base(assetProvider)
        {
            _configService = configService;
            _persistent = persistent;
            _progressWriters = new List<IProgressWriter>();
            _progressReaders = new List<IProgressReader>();
        }
        
        public void Clear()
        {
            _progressWriters.Clear();
            _progressReaders.Clear();
        }

        public async UniTask<Cell> CreateEmptyCell(CellData cellData)
        {
            Cell cell = await InstantiateInjectedObject<Cell>(RuntimeConstants.AssetLabels.CellPrefab);
            cell.Construct(cellData.Position);
            RegisterWriter(cell);

            return cell;
        }
        
        public async UniTask<HexStack> CreateHexStack(ColorStack colorStack)
        {
            HexStack hexStack = await InstantiateInjectedObject<HexStack>(RuntimeConstants.AssetLabels.HexStackPrefab);

            List<Hex> hexes = new List<Hex>();
            for (int i = 0; i < colorStack.Colors.Count; i++)
            {
                HexConfig colorConfig = _configService.ForHex(colorStack.Colors[i]);
                
                Hex hex = await InstantiateInjectedObject<Hex>(RuntimeConstants.AssetLabels.HexPrefab);
                hex.Initialize(colorConfig);
                
                hexes.Add(hex);
            }
            
            hexStack.Initialize(hexes);

            return hexStack;
        }

        public async UniTask<HexStack> CreateDraggableHexStack(ColorStack colorStack)
        {
            HexStack hexStack = await CreateHexStack(colorStack);
            hexStack.gameObject.AddComponent<BoxCollider>();
            hexStack.gameObject.AddComponent<StackDrag>().Construct(hexStack);
            
            return hexStack;
        }
        
        private void RegisterWriter(IProgressWriter writer)
        {
            if (_progressWriters.Contains(writer) == false) 
                _progressWriters.Add(writer);
        }

        private void RegisterReader(IProgressReader reader)
        {
            if (_progressReaders.Contains(reader) == false)
                _progressReaders.Add(reader);
        }
    }
}