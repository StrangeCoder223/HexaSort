using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class Cell : MonoBehaviour, IProgressWriter
    {
        public event Action<Cell> Occupied;
        
        public bool IsOccupied => _hexStack != null;
        public HexStack HexStack => _hexStack;
        public Vector2Int Position { get; private set; }
        
        [SerializeField] private MeshRenderer _cellRenderer;
        
        private HexStack _hexStack;
        private float _stackOffset = 0.125f;
            
        public void Construct(Vector2Int position)
        {
            Position = position;
        }
        
        public void Occupy(HexStack hexStack)
        {
            if (_hexStack != null || hexStack.HexCount <= 0)
                return;
            
            _hexStack = hexStack;
            _hexStack.transform.SetParent(transform);
            _hexStack.transform.localPosition = Vector3.up * _stackOffset;
            
            Occupied?.Invoke(this);
        }

        public void Highlight(bool highlight)
        {
            if (highlight)
                _cellRenderer.material.EnableKeyword("_EMISSION");
            else
                _cellRenderer.material.DisableKeyword("_EMISSION");
        }

        public void TryClear()
        {
            if (_hexStack == null)
                return;
            
            if (_hexStack.HexCount == 0)
            {
                Destroy(_hexStack.gameObject);
                _hexStack = null;
            }
        }

        public void WriteProgress(PlayerProgress progress)
        {
            CellData cellData = progress.LevelData.Cells.FirstOrDefault(x => x.Position == Position);
            cellData.StackColors = _hexStack != null ? _hexStack.GetColors() : new List<HexColor>();
        }
    }
}