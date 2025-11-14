using UnityEngine;

namespace _Project.Code.Infrastructure.Services.AssetProvider
{
    public interface IAssetProvider
    {
        T LoadAsset<T>(string path) where T : Object;
    }
}