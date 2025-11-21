using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class HexTransferAnimator : MonoBehaviour
    {
        [Header("Transfer Animation")]
        [SerializeField] private float _arcHeight = 2f;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private float _delayBetweenHexes = 0.08f;
        [SerializeField] private float _rotationAmount = -180f;
        [SerializeField] private AnimationCurve _arcHeightCurve;
        
        [Header("Stack Settings")]
        [SerializeField] private float _hexStepY = 0.13f;
        
        private Vector3 HexStep => Vector3.up * _hexStepY;

        public async UniTask AnimateTransfer(List<Hex> hexes, Vector3 targetStackPosition, int targetStackCount)
        {
            List<UniTask> animationTasks = new List<UniTask>();
            
            for (int i = 0; i < hexes.Count; i++)
            {
                Hex hex = hexes[i];
                hex.transform.SetParent(null);
                
                float delay = i * _delayBetweenHexes;
                Vector3 endPosition = targetStackPosition + HexStep * (targetStackCount + i);
                
                Tween moveTween = CreateMovementTween(hex, endPosition, delay);
                Tween rotateTween = CreateRotationTween(hex, delay);
                
                animationTasks.Add(moveTween.AsyncWaitForCompletion().AsUniTask());
                animationTasks.Add(rotateTween.AsyncWaitForCompletion().AsUniTask());
            }
            
            await UniTask.WhenAll(animationTasks);
        }
        
        public async UniTask AnimateDestruction(Hex hex, float duration = 0.03f)
        {
            await hex.transform.DOScale(0f, duration)
                .SetEase(Ease.InBack)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
        
        private Tween CreateMovementTween(Hex hex, Vector3 endPosition, float delay)
        {
            Vector3 startPosition = hex.transform.position;
            
            return DOVirtual.Float(0f, 1f, _animationDuration, progress =>
            {
                float arcOffset = _arcHeightCurve.Evaluate(progress) * _arcHeight;
                hex.transform.position = Vector3.Lerp(startPosition, endPosition, progress) + Vector3.up * arcOffset;
            })
            .SetEase(Ease.Linear)
            .SetDelay(delay);
        }
        
        private Tween CreateRotationTween(Hex hex, float delay)
        {
            Vector3 rotationAxis = new Vector3(0f, 0f, _rotationAmount);
            
            return hex.transform
                .DORotate(rotationAxis, _animationDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutQuad)
                .SetDelay(delay);
        }
    }
}

