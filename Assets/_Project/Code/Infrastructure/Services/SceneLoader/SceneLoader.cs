using _Project.Code.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Code.Infrastructure.Services.SceneLoader
{
    public class SceneLoader : ISceneLoader
    {
        private readonly LoadingScreen _loadingScreen;

        public SceneLoader(LoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }
        
        public async UniTask LoadScene(string scene)
        {
            _loadingScreen.Show();
            
            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(scene);
            while (!loadingOperation.isDone)
            {
                _loadingScreen.Progress = loadingOperation.progress;
                await UniTask.Yield();
            }

            _loadingScreen.Progress = 1f;

            _loadingScreen.Hide();
        }
    }
}