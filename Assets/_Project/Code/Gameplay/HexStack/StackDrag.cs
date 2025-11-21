using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Code.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class StackDrag : MonoBehaviour
    {
        public event Action<StackDrag> DraggedSuccessful;
        
        private const float DragHeight = 2f;
        private const float ZOffset = -1f;
        private const float HighlightRayOffsetZ = -0.5f;
        private const float MoveSpeed = 20f;
        private const float ReturnDuration = 0.2f;
        
        private bool _dragging;
        private Vector3 _originalPosition;
        private Tween _tween;
        private HexStack _hexStack;
        private CellHighlighter _cellHighlighter;

        public void Construct(HexStack hexStack)
        {
            _hexStack = hexStack;
            _cellHighlighter = new CellHighlighter();
        }
        
        private void OnMouseDown()
        {
            if (CantDrag())
                return;
            
            _originalPosition = transform.position;
        }

        private void OnMouseDrag()
        {
            if (CantDrag())
                return;
            
            Vector3 targetPosition = GetMouseWorldPosition();
            targetPosition.y = _originalPosition.y + DragHeight;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed);
            _dragging = true;
        }
        
        private void OnMouseUp()
        {
            if (CantDrag())
                return;
            
            _tween?.Kill(true);
            _dragging = false;

            if (_cellHighlighter.HighlightedCell != null)
            {
                _cellHighlighter.HighlightedCell.Occupy(_hexStack);
                _cellHighlighter.ClearHighlight();
                DraggedSuccessful?.Invoke(this);
                Destroy(this);
            }
            else
            {
                _tween = transform.DOMove(_originalPosition, ReturnDuration);
            }
        }
        
        private bool CantDrag() => IsTweenPlaying() || EventSystem.current.IsPointerOverGameObject();

        private bool IsTweenPlaying() => _tween != null && _tween.active;

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z - ZOffset;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        private void Update()
        {
            if (_dragging)
                _cellHighlighter.UpdateHighlight(transform.position, HighlightRayOffsetZ);
            else
                _cellHighlighter.ClearHighlight();
        }
    }
}