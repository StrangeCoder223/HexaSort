using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class LevelGenerator
    {
        private readonly IFactory _factory;
        private readonly IConfigService _configService;
        private readonly IPersistentService _persistent;

        public LevelGenerator(IPersistentService persistent, IConfigService configService)
        {
            _persistent = persistent;
            _configService = configService;
        }

        public void Generate(int level)
        {
            LevelConfig levelConfig = _configService.ForLevel(level - 1);
            SessionData sessionData = _persistent.Persistent.Progress.SessionData;

            _factory.CreateCell();  
        }
    }

    public interface IFactory
    {
        GameObject CreateHex();
        GameObject CreateCell();
    }
}