using System;
using _Project.Code.Infrastructure.Data;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class Cell : MonoBehaviour
    {
        public event Action<Cell> Occupied;
        
        public bool IsOccupied => _hexStack != null;
        public HexStack HexStack => _hexStack;
        public int X => _cellData.X;
        public int Y => _cellData.Y;
        
        [SerializeField] private MeshRenderer _cellRenderer;
        
        private HexStack _hexStack;
        private float _stackOffset = 0.125f;
        private CellData _cellData;
            
        public void Construct(CellData cellData)
        {
            _cellData = cellData;
        }
        
        public void Occupy(HexStack hexStack)
        {
            if (_hexStack != null || hexStack.HexCount <= 0)
                return;
            
            _hexStack = hexStack;
            _hexStack.transform.SetParent(transform);
            _hexStack.transform.localPosition = Vector3.up * _stackOffset;
            _cellData.StackColors = _hexStack.GetColors();
            
            Occupied?.Invoke(this);
        }

        public void Highlight(bool highlight)
        {
            if (highlight)
                _cellRenderer.material.EnableKeyword("_EMISSION");
            else
                _cellRenderer.material.DisableKeyword("_EMISSION");
        }

        public void UpdateStackColors()
        {
            if (_hexStack != null)
            {
                _cellData.StackColors = _hexStack.GetColors();
            }
        }

        public void TryClear()
        {
            if (_hexStack == null)
                return;
            
            if (_hexStack.HexCount == 0)
            {
                Destroy(_hexStack.gameObject);
                _hexStack = null;
                _cellData.StackColors.Clear();
            }
        }
    }
}