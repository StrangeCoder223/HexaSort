using DG.Tweening;
using UnityEngine;

namespace _Project.Code.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class BaseScreen : MonoBehaviour
    {
        protected Canvas Canvas => _canvas;
    
        [SerializeField] private RectTransform _scaleTransform;
        private Canvas _canvas;
    
        private readonly float _scaleDuration = 0.2f;

        public virtual void Initialize()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
        }

        public virtual void Show()
        {
            _canvas.enabled = true;
            
            _scaleTransform.localScale = Vector3.zero;
            _scaleTransform.DOScale(1f, _scaleDuration);
        }

        public virtual void Hide()
        {
            _scaleTransform.DOScale(0f, _scaleDuration).OnComplete(() => gameObject.SetActive(false));
        }
    }
}