using _Project.Code.Gameplay;
using _Project.Code.Infrastructure.Configs;
using Cysharp.Threading.Tasks;

namespace _Project.Code.Infrastructure.Factories
{
    public interface IGameFactory
    {
        UniTask<HexStack> CreateHexStack(ColorStack colorStack);
        UniTask<Cell> CreateEmptyCell();
    }
}