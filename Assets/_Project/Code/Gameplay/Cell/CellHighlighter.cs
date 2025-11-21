using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class CellHighlighter
    {
        public Cell HighlightedCell => _highlightedCell;
        
        private const float RayDistance = Mathf.Infinity;
        private Cell _highlightedCell;
        
        
        public void UpdateHighlight(Vector3 position, float rayOffsetZ)
        {
            Cell targetCell = GetCellUnderCursor(position, rayOffsetZ);
            SetHighlightedCell(targetCell);
        }
        
        public void ClearHighlight()
        {
            SetHighlightedCell(null);
        }
        
        private Cell GetCellUnderCursor(Vector3 position, float rayOffsetZ)
        {
            Vector3 rayOrigin = position + Vector3.forward * rayOffsetZ;
            
            if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, RayDistance))
                return null;
            
            Debug.DrawLine(rayOrigin, hit.point, Color.green);
            
            if (!hit.transform.TryGetComponent(out Cell cell) || cell.IsOccupied)
                return null;
            
            return cell;
        }
        
        private void SetHighlightedCell(Cell newCell)
        {
            if (_highlightedCell == newCell)
                return;
            
            _highlightedCell?.Highlight(false);
            _highlightedCell = newCell;
            _highlightedCell?.Highlight(true);
        }
    }
}

