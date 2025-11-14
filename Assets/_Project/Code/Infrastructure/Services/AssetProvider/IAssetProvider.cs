using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Infrastructure.Services.AssetProvider
{
    public interface IAssetProvider
    {
        UniTask<T> LoadAsset<T>(string path) where T : Object;
    }
}