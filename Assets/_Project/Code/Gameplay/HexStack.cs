using System.Collections.Generic;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Services.PersistentService;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class HexStack : MonoBehaviour
    {
        public Hex TopHex => _hexes[^1];
        public int HexCount => _hexes.Count;
        
        private List<Hex> _hexes;
        private readonly Vector3 _hexStep = Vector3.up * 0.13f;
        private IPersistentService _persistentService;

        [Inject]
        private void Construct(IPersistentService persistentService)
        {
            _persistentService = persistentService;
        }
        
        public void Initialize(List<Hex> hexes)
        {
            _hexes = hexes;
            
            for (int i = 0; i < _hexes.Count; i++)
            {
                _hexes[i].transform.SetParent(transform);
                _hexes[i].transform.localPosition = _hexStep * i;
            }
        }

        public void Fill(List<Hex> newHexes)
        {
            Vector3 upperStep = _hexStep * _hexes.Count; 
            
            _hexes.AddRange(newHexes);
            
            for (int i = 0; i < newHexes.Count; i++)
            {
                newHexes[i].transform.SetParent(transform);
                newHexes[i].transform.localPosition = (upperStep + i * _hexStep);
            }
        }

        public async UniTask<List<Hex>> RemoveUpperAnimated(Vector3 targetStackTopPosition, int targetStackCount)
        {
            List<Hex> removedHexes = RemoveUpperHexes();
            
            await AnimateHexesTransfer(removedHexes, targetStackTopPosition, targetStackCount);

            return removedHexes;
        }
        
        private List<Hex> RemoveUpperHexes()
        {
            HexColor targetColor = TopHex.Color;
            List<Hex> removedHexes = new List<Hex>();
            
            for (int i = _hexes.Count - 1; i >= 0; i--)
            {
                if (_hexes[i].Color != targetColor)
                    break;
                
                removedHexes.Add(_hexes[i]);
            }
            
            removedHexes.ForEach(x => _hexes.Remove(x));
            return removedHexes;
        }

        private async UniTask AnimateHexesTransfer(List<Hex> hexes, Vector3 targetPosition, int targetStackCount)
        {
            const float ArcHeight = 2f;
            const float AnimationDuration = 0.5f;
            const float DelayBetweenHexes = 0.08f;
            const float RotationAmount = -180f;

            List<UniTask> animationTasks = new List<UniTask>();
            
            for (int i = 0; i < hexes.Count; i++)
            {
                Hex hex = hexes[i];
                hex.transform.SetParent(null);

                Vector3 start = hex.transform.position;
                Vector3 end = targetPosition + _hexStep * (targetStackCount + i);
                Vector3[] path = CreateParabolicPath(start, end, ArcHeight);
                
                // Всегда вращаем по Z оси
                Vector3 rotationAxis = new Vector3(0f, 0f, RotationAmount);
                
                Tween moveTween = hex.transform
                    .DOPath(path, AnimationDuration, PathType.CatmullRom)
                    .SetEase(Ease.InOutQuad)
                    .SetDelay(i * DelayBetweenHexes);
                
                Tween rotateTween = hex.transform
                    .DORotate(rotationAxis, AnimationDuration, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.InOutQuad)
                    .SetDelay(i * DelayBetweenHexes);
                
                animationTasks.Add(moveTween.AsyncWaitForCompletion().AsUniTask());
                animationTasks.Add(rotateTween.AsyncWaitForCompletion().AsUniTask());
            }
            
            await UniTask.WhenAll(animationTasks);
        }

        private Vector3[] CreateParabolicPath(Vector3 start, Vector3 end, float arcHeight)
        {
            Vector3 direction = (end - start).normalized;
            Vector3 control1 = start + Vector3.up * arcHeight * 0.5f + direction * 0.3f;
            Vector3 control2 = end + Vector3.up * arcHeight * 0.7f - direction * 0.3f;
            
            return new[] { start, control1, control2, end };
        }

        public List<HexColor> GetColors()
        {
            List<HexColor> hexColors = new List<HexColor>();
            _hexes.ForEach(x => hexColors.Add(x.Color));

            return hexColors;
        }

        public int GetTopColorCount()
        {
            if (_hexes.Count == 0)
                return 0;

            HexColor topColor = TopHex.Color;
            int count = 0;

            for (int i = _hexes.Count - 1; i >= 0; i--)
            {
                if (_hexes[i].Color != topColor)
                    break;

                count++;
            }

            return count;
        }

        public async UniTask DestroyTopHexes(int count)
        {
            if (count <= 0 || count > _hexes.Count)
                return;

            List<Hex> hexesToDestroy = new List<Hex>();

            for (int i = 0; i < count; i++)
            {
                int index = _hexes.Count - 1 - i;
                if (index >= 0)
                {
                    hexesToDestroy.Add(_hexes[index]);
                }
            }

            for (int i = 0; i < hexesToDestroy.Count; i++)
            {
                Hex hex = hexesToDestroy[i];
                _hexes.Remove(hex);
                
                await hex.transform.DOScale(0f, 0.03f)
                    .SetEase(Ease.InBack)
                    .AsyncWaitForCompletion()
                    .AsUniTask();
                
                var goals = _persistentService.Data.Progress.LevelData.Goals;
                if (goals.ContainsKey(hex.Color))
                {
                    goals[hex.Color].CurrentAmount.Value++;
                }
                
                if (goals.ContainsKey(HexColor.Any))
                {
                    goals[HexColor.Any].CurrentAmount.Value++;
                }
                
                Destroy(hex.gameObject);

                await UniTask.WaitForEndOfFrame();
            }
        }
    }
}