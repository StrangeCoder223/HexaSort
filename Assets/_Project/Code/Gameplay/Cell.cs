using _Project.Code.Infrastructure.Data;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class Cell : MonoBehaviour
    {
        public bool IsOccupied => _hexStack != null;
        
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
        }

        public void Highlight(bool highlight)
        {
            if (highlight)
                _cellRenderer.material.EnableKeyword("_EMISSION");
            else
                _cellRenderer.material.DisableKeyword("_EMISSION");
        }
    }
}