using System;
using _Project.Code.Infrastructure.Services.PersistentService;
using _Project.Code.Infrastructure.Services.SceneLoader;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Code.UI
{
    public class MetaScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Button _playButton;
        
        private IPersistentService _persistent;
        private ISceneLoader _sceneLoader;

        [Inject]
        private void Construct(IPersistentService persistent, ISceneLoader sceneLoader)
        {
            _persistent = persistent;
            _sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            _levelText.text = "Level " + _persistent.Data.Progress.Level;
            _playButton.onClick.AddListener(TryPlayLevel);
        }

        private async void TryPlayLevel()
        {
            if (_persistent.Data.Progress.Life.Value <= 0)
                return;
            
            await _sceneLoader.LoadScene(RuntimeConstants.Scenes.Game);
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveAllListeners();
        }
    }
}
