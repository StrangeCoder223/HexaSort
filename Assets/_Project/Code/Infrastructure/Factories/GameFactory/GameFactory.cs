using System.Collections.Generic;
using _Project.Code.Gameplay;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Services.AssetProvider;
using _Project.Code.Infrastructure.Services.ConfigService;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Infrastructure.Factories
{
    public class GameFactory : ObjectFactory, IGameFactory
    { 
        private readonly IConfigService _configService;

        public GameFactory(IAssetProvider assetProvider, IConfigService configService) : base(assetProvider)
        {
            _configService = configService;
        }

        public async UniTask<Cell> CreateEmptyCell(CellData cellData)
        {
            Cell cell = await InstantiateInjectedObject<Cell>(RuntimeConstants.AssetLabels.CellPrefab);
            cell.Construct(cellData);

            return cell;
        }
        
        public async UniTask<HexStack> CreateHexStack(ColorStack colorStack)
        {
            HexStack hexStack = await InstantiateInjectedObject<HexStack>(RuntimeConstants.AssetLabels.HexStackPrefab);

            List<Hex> hexes = new List<Hex>();
            for (int i = 0; i < colorStack.Colors.Count; i++)
            {
                ColorConfig colorConfig = _configService.ForHexColor(colorStack.Colors[i]);
                
                Hex hex = await InstantiateInjectedObject<Hex>(RuntimeConstants.AssetLabels.HexPrefab);
                hex.Initialize(colorConfig);
                
                hexes.Add(hex);
            }
            
            hexStack.Construct(hexes);
            hexStack.Initialize();

            return hexStack;
        }

        public async UniTask<HexStack> CreateDraggableHexStack(ColorStack colorStack)
        {
            HexStack hexStack = await CreateHexStack(colorStack);
            hexStack.gameObject.AddComponent<BoxCollider>();
            hexStack.gameObject.AddComponent<StackDrag>().Construct(hexStack);
            
            return hexStack;
        }
    }
}