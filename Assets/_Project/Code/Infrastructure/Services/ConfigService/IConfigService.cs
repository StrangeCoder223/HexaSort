using _Project.Code.Infrastructure.Configs;

namespace _Project.Code.Infrastructure.Services.ConfigService
{
    public interface IConfigService
    {
        LevelConfig ForLevel(int level);
        MetaConfig ForMeta();
        void Load();
    }
}