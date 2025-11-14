using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Services.ConfigService;

namespace _Project.Code.Infrastructure.Services.PersistentService
{
    public class PersistentService : IPersistentService
    {
        public PersistentData Persistent { get; set; }

        private IConfigService _configService;
        
        public PersistentService(IConfigService configService) => _configService = configService;
        
        public PersistentData CreateDefaultPersistent()
        {
            MetaConfig metaConfig = _configService.ForMeta();
            
            PersistentData persistent = new PersistentData()
            {
                Progress = new()
                {
                    Level = 1,
                    Life = new(metaConfig.MaxLife),
                    LifeRestoreTime = new(0),
                    Money = new(0)
                },
                Options = new()
            };
            
            return persistent;
        }
    }
}