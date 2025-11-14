using Cysharp.Threading.Tasks;

namespace _Project.Code.Infrastructure.Services.SceneLoader
{
    public interface ISceneLoader
    {
        UniTask LoadScene(string scene);
    }
}