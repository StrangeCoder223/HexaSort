using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Code.UI
{
    public class LoadingScreen : BaseScreen
    {
        public float Progress
        {
            get => _progress;
            set
            {
                if (value < 0) return;
                
                _progress = value;
                RefreshBar();
            }
        }

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _percentsLabel;
        [SerializeField] private Slider _progressBar;
        private float _progress;
        private const float FadeDuration = 0.5f;

        public override void Initialize()
        {
            base.Initialize();
            DontDestroyOnLoad(gameObject);
        }

        public override void Show()
        {
            Canvas.enabled = true;
            if (_canvasGroup.alpha >= 1f) return;
            
            _canvasGroup.DOFade(1f, FadeDuration);
        }

        public override void Hide()
        {
            if (_canvasGroup.alpha <= 0f) return;

            _canvasGroup.DOFade(0f, FadeDuration).OnComplete(() =>
            {
                Canvas.enabled = false;
            });
        }

        private void RefreshBar()
        {
            _progressBar.value = _progress;
            _percentsLabel.text = _progress * 100 + "%";
        }
    }
}