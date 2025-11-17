using System.Collections.Generic;
using System.Linq;
using _Project.Code.Infrastructure.Configs;
using DG.Tweening;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class HexStack : MonoBehaviour
    {
        public Hex TopHex => _hexes[^1];
        public int HexCount => _hexes.Count;
        
        private List<Hex> _hexes;
        private readonly Vector3 _hexStep = Vector3.up * 0.13f;

        public void Construct(List<Hex> hexes)
        {
            _hexes = hexes;
        }
        
        public void Initialize()
        {
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
                newHexes[i].transform.DOLocalMove(upperStep + i * _hexStep, 0.2f).SetDelay(i * 0.1f);
            }
        }

        public List<Hex> RemoveUpper()
        {
            HexColor targetColor = TopHex.Color;
            
            List<Hex> removedHexes = new List<Hex>();
            
            for (int i = _hexes.Count - 1; i > 0; i--)
            {
                if (_hexes[i].Color != targetColor)
                    break;
                
                removedHexes.Add(_hexes[i]);
            }
            
            _hexes.RemoveRange(_hexes.Count - 1, removedHexes.Count);

            return removedHexes;
        }

        public List<HexColor> GetColors()
        {
            List<HexColor> hexColors = new List<HexColor>();
            _hexes.ForEach(x => hexColors.Add(x.Color));

            return hexColors;
        }
    }
}