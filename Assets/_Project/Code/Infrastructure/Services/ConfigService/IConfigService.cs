using _Project.Code.Infrastructure.Configs;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Infrastructure.Services.ConfigService
{
    public interface IConfigService
    {
        LevelConfig ForLevel(int level);
        MetaConfig ForMeta();
        UniTask Load();
        HexConfig ForHex(HexColor hexColor);
        GeneratorConfig ForGenerator();
    }
}