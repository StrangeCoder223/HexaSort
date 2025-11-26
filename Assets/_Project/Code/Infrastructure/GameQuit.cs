using System;
using _Project.Code.Infrastructure.Factories;
using _Project.Code.Infrastructure.Services.PersistentService;
using _Project.Code.Infrastructure.Services.SaveLoadService;
using Reflex.Attributes;
using Sirenix.Utilities;
using UnityEngine;

namespace _Project.Code.Infrastructure
{
    public class GameQuit : MonoBehaviour
    {
        private IGameFactory _gameFactory;
        private ISaveLoadService _saveLoad;
        private IPersistentService _persistent;

        [Inject]
        private void Construct(IPersistentService persistent, IGameFactory gameFactory, ISaveLoadService saveLoadService)
        {
            _persistent = persistent;
            _gameFactory = gameFactory;
            _saveLoad = saveLoadService;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            _gameFactory.ProgressWriters.ForEach(x => x.WriteProgress(_persistent.Data.Progress));
            _saveLoad.Save();
        }
    }
}