using System.Collections.Generic;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Factories;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class StackOfferSpawner : MonoBehaviour
    {
        [SerializeField] private float _spacing;
        
        private List<StackDrag> _stackDrags = new();
        private StackOfferGenerator _stackOfferGenerator;
        private IGameFactory _gameFactory;

        [Inject]
        private void Construct(StackOfferGenerator stackOfferGenerator, IGameFactory gameFactory)
        {
            _stackOfferGenerator = stackOfferGenerator;
            _gameFactory = gameFactory;
        }

        public async UniTaskVoid Initialize()
        {
            await Spawn();
        }

        private async UniTask Spawn()
        {
            List<ColorStack> colorStacks = _stackOfferGenerator.GenerateWeightedOffers();

            for (int i = 0; i < colorStacks.Count; i++)
            {
                HexStack hexStack = await _gameFactory.CreateDraggableHexStack(colorStacks[i]);
                hexStack.transform.SetParent(transform);
                hexStack.transform.localPosition = new Vector3(i * _spacing, 0, 0);

                StackDrag stackDrag = hexStack.GetComponent<StackDrag>();
                stackDrag.DraggedSuccessful += RemoveStack;
                _stackDrags.Add(stackDrag);
            }
        }

        private async void RemoveStack(StackDrag stackDrag)
        {
            stackDrag.DraggedSuccessful -= RemoveStack;
            
            _stackDrags.Remove(stackDrag);
            
            if (_stackDrags.Count > 0)
                return;
            
            await Spawn();
        }
    }
}