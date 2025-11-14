using _Project.Code.Infrastructure.Data;

namespace _Project.Code.Infrastructure.Services.PersistentService
{
    public interface IPersistentService
    {
        public PersistentData Data { get; set; }

        public PersistentData CreateDefaultData();
    }
}