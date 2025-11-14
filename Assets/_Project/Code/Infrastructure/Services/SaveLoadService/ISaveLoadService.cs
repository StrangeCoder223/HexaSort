using _Project.Code.Infrastructure.Data;

namespace _Project.Code.Infrastructure.Services.SaveLoadService
{
    public interface ISaveLoadService
    {
        public void Save();
        public PersistentData Load();
    }
}