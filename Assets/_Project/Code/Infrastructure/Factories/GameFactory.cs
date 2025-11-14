using _Project.Code.Infrastructure.Services.AssetProvider;

namespace _Project.Code.Infrastructure.Factories
{
    public class GameFactory : ObjectFactory
    {
        public GameFactory(IAssetProvider assetProvider) : base(assetProvider) { }
        
        
    }
}