using System.Collections.Generic;
using System.Linq;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Services.PersistentService;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class HexStack : MonoBehaviour
    {
        public Hex TopHex => _hexes[^1];
        public int HexCount => _hexes.Count;
        public Vector3 TopPosition => transform.position;
        
        private List<Hex> _hexes;
        private IPersistentService _persistentService;
        private HexTransferAnimator _animator;
        private readonly Vector3 _hexStep = Vector3.up * 0.13f;

        [Inject]
        private void Construct(IPersistentService persistentService, HexTransferAnimator animator)
        {
            _persistentService = persistentService;
            _animator = animator;
        }
        
        public void Initialize(List<Hex> hexes)
        {
            _hexes = hexes;
            PositionHexes(_hexes, startIndex: 0);
        }

        public void Fill(List<Hex> newHexes)
        {
            int startIndex = _hexes.Count;
            _hexes.AddRange(newHexes);
            PositionHexes(newHexes, startIndex);
        }
        
        public List<Hex> RemoveUpper()
        {
            int count = GetTopColorCount();
            if (count == 0)
                return new List<Hex>();
            
            List<Hex> removedHexes = _hexes.GetRange(_hexes.Count - count, count);
            _hexes.RemoveRange(_hexes.Count - count, count);
            
            return removedHexes;
        }
        
        private void PositionHexes(List<Hex> hexes, int startIndex)
        {
            for (int i = 0; i < hexes.Count; i++)
            {
                hexes[i].transform.SetParent(transform);
                hexes[i].transform.localPosition = _hexStep * (startIndex + i);
            }
        }

        public List<HexColor> GetColors()
        {
            return _hexes.Select(hex => hex.Color).ToList();
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

            int startIndex = _hexes.Count - count;
            List<Hex> hexesToDestroy = _hexes.GetRange(startIndex, count);
            _hexes.RemoveRange(startIndex, count);

            for (int i = hexesToDestroy.Count - 1; i >= 0; i--)
            {
                Hex hex = hexesToDestroy[i];
                
                await _animator.AnimateDestruction(hex, duration: 0.05f);
                UpdateGoals(hex.Color);
                Destroy(hex.gameObject);
                await UniTask.WaitForEndOfFrame();
            }
        }
        
        private void UpdateGoals(HexColor hexColor)
        {
            var goals = _persistentService.Data.Progress.LevelData.Goals;
            
            if (goals.ContainsKey(hexColor))
                goals[hexColor].CurrentAmount.Value++;
            
            if (goals.ContainsKey(HexColor.Any))
                goals[HexColor.Any].CurrentAmount.Value++;
        }
    }
}