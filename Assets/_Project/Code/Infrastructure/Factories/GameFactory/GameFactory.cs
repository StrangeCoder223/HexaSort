using _Project.Code.Infrastructure.Services.AssetProvider;

namespace _Project.Code.Infrastructure.Factories
{
    public class GameFactory : ObjectFactory, IGameFactory
    {
        public GameFactory(IAssetProvider assetProvider) : base(assetProvider) { }
        
        
    }
}