using System;
using DG.Tweening;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class StackDrag : MonoBehaviour
    {
        public event Action<StackDrag> DraggedSuccessful;
        
        private float _dragHeight = 2f;
        private float _zOffset = -1f;
        private float _highlightRayOffsetZ = -0.5f;
        private bool _dragging;
        
        private Vector3 _originalPosition;
        private Tween _tween;
        private Cell _lastCell;
        private HexStack _hexStack;

        public void Construct(HexStack hexStack)
        {
            _hexStack = hexStack;
        }
        
        private void OnMouseDown()
        {
            if (IsTweenPlaying())
                return;
            
            _originalPosition = transform.position;
        }

        private void OnMouseDrag()
        {
            if (IsTweenPlaying())
                return;
            
            Vector3 newPosition = GetMouseWorldPosition();
            newPosition.y = _originalPosition.y + _dragHeight;
            transform.position = Vector3.MoveTowards(transform.position, newPosition, 20f);
            _dragging = true;
        }
        
        private void OnMouseUp()
        {
            if (IsTweenPlaying())
                return;
            
            _tween.Kill(true);
            _dragging = false;

            if (_lastCell != null)
            {
                _lastCell.Highlight(false);
                _lastCell.Occupy(_hexStack);
                
                DraggedSuccessful?.Invoke(this);
                Destroy(this);
            }
            else
                _tween = transform.DOMove(_originalPosition, 0.2f);
        }

        private bool IsTweenPlaying()
        {
            return _tween != null && _tween.active;
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z - _zOffset;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        private void Update()
        {
            if (_dragging)
            {
                if (Physics.Raycast(transform.position + (Vector3.forward * _highlightRayOffsetZ), Vector3.down, out RaycastHit hit, Mathf.Infinity))
                {
                    Debug.DrawLine(transform.position + (Vector3.forward * _highlightRayOffsetZ), hit.point, Color.green);
                    
                    _lastCell?.Highlight(false);
                    
                    if (!hit.transform.TryGetComponent(out Cell cell) || cell.IsOccupied)
                        return;
                    
                    _lastCell = cell;
                    _lastCell?.Highlight(true);
                    
                    return;
                }
                
                _lastCell?.Highlight(false);
                _lastCell = null;
            }
            else
            {
                _lastCell?.Highlight(false);
                _lastCell = null;
            }
        }
    }
}