using System.Collections.Generic;
using _Project.Code.Gameplay;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using Cysharp.Threading.Tasks;

namespace _Project.Code.Infrastructure.Factories
{
    public interface IGameFactory
    {
        IReadOnlyList<IProgressWriter> ProgressWriters { get; }
        IReadOnlyList<IProgressReader> ProgressReaders { get; }
        
        UniTask<HexStack> CreateHexStack(ColorStack colorStack);
        UniTask<Cell> CreateEmptyCell(CellData cellData);
        UniTask<HexStack> CreateDraggableHexStack(ColorStack colorStack);
        void Clear();
    }
}