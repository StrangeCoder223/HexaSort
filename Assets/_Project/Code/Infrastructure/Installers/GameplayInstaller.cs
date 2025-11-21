using _Project.Code.Gameplay;
using Reflex.Core;
using UnityEngine;

namespace _Project.Code.Infrastructure.Installers
{
    public class GameplayInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private StackOfferSpawner _stackOfferSpawner;
        [SerializeField] private HexTransferAnimator _hexTransferAnimator;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(typeof(LevelGenerator));
            containerBuilder.AddSingleton(typeof(StackOfferGenerator));
            containerBuilder.AddSingleton(_stackOfferSpawner, typeof(StackOfferSpawner));
            containerBuilder.AddSingleton(_hexTransferAnimator, typeof(HexTransferAnimator));
        }
    }
}