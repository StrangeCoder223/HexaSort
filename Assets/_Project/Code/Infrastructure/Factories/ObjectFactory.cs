using _Project.Code.Infrastructure.Services.AssetProvider;
using Cysharp.Threading.Tasks;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace _Project.Code.Infrastructure.Factories
{
    public abstract class ObjectFactory
    {
        private readonly IAssetProvider _assetProvider;

        protected ObjectFactory(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }
        
        protected async UniTask<T> InstantiateInjectedObject<T>(string path, RectTransform parent = null) where T : Object
        {
            GameObject prefab = await _assetProvider.LoadAsset<GameObject>(path);

            var instanceTask = await Object.InstantiateAsync(prefab, parent).ToUniTask();
            var result = instanceTask[0];

            GameObjectInjector.InjectRecursive(result, result.scene.GetSceneContainer());
            
            return result.GetComponent<T>();
        }
        
        protected async UniTask<T> InstantiateObject<T>(string path, RectTransform parent = null) where T : Object
        {
            T prefab = await _assetProvider.LoadAsset<GameObject>(path) as T;

            var uniTask = await Object.InstantiateAsync<T>(prefab, parent).ToUniTask();

            return uniTask[0];
        }
    }
}