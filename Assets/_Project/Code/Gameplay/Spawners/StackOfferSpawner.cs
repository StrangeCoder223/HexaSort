using System.Collections.Generic;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Factories;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class StackOfferSpawner : MonoBehaviour
    {
        [SerializeField] private float _spacing = 1f;
        [SerializeField] private float _showAnimationDuration = 0.4f;
        [SerializeField] private Vector3 _showPosition;
        [SerializeField] private Vector3 _hidePosition;
        
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
            await SpawnOffers();
        }

        private void OnDestroy()
        {
            UnsubscribeFromAllStacks();
        }

        private async UniTask SpawnOffers()
        {
            HideInstantly();
            
            List<ColorStack> colorStacks = _stackOfferGenerator.GenerateWeightedOffers();
            await CreateAndPositionStacks(colorStacks);
            
            await ShowWithAnimation();
        }

        private async UniTask CreateAndPositionStacks(List<ColorStack> colorStacks)
        {
            for (int i = 0; i < colorStacks.Count; i++)
            {
                HexStack hexStack = await _gameFactory.CreateDraggableHexStack(colorStacks[i]);
                PositionStack(hexStack, i);
                SubscribeToStackDrag(hexStack);
            }
        }

        private void PositionStack(HexStack hexStack, int index)
        {
            hexStack.transform.SetParent(transform);
            hexStack.transform.localPosition = new Vector3(index * _spacing, 0, 0);
        }

        private void SubscribeToStackDrag(HexStack hexStack)
        {
            StackDrag stackDrag = hexStack.GetComponent<StackDrag>();
            stackDrag.DraggedSuccessful += OnStackDragged;
            _stackDrags.Add(stackDrag);
        }

        private void OnStackDragged(StackDrag stackDrag)
        {
            stackDrag.DraggedSuccessful -= OnStackDragged;
            _stackDrags.Remove(stackDrag);
            
            if (_stackDrags.Count == 0)
                SpawnOffers().Forget();
        }

        private void UnsubscribeFromAllStacks()
        {
            foreach (StackDrag stackDrag in _stackDrags)
            {
                if (stackDrag != null)
                    stackDrag.DraggedSuccessful -= OnStackDragged;
            }
            
            _stackDrags.Clear();
        }

        private void HideInstantly()
        {
            transform.position = _hidePosition;
        }

        private async UniTask ShowWithAnimation()
        {
            await transform.DOMove(_showPosition, _showAnimationDuration)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
    }
}