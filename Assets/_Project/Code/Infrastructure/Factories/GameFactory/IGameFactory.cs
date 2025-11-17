using _Project.Code.Gameplay;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Infrastructure.Factories
{
    public interface IGameFactory
    {
        UniTask<HexStack> CreateHexStack(ColorStack colorStack);
        UniTask<Cell> CreateEmptyCell(CellData cellData);
        UniTask<HexStack> CreateDraggableHexStack(ColorStack colorStack);
    }
}