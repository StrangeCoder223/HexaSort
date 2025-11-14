using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Infrastructure.Services.AssetProvider
{
    public class AssetProvider : IAssetProvider
    {
        public async UniTask<T> LoadAsset<T>(string path) where T : Object
        {
            var resourceRequest = Resources.LoadAsync<T>(path).ToUniTask();

            Object result = await resourceRequest;

            return result as T;
        }
    }
}