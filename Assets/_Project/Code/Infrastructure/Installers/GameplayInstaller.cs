using _Project.Code.Gameplay;
using Reflex.Core;
using UnityEngine;

namespace _Project.Code.Infrastructure.Installers
{
    public class GameplayInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddTransient(typeof(LevelGenerator));
        }
    }
}