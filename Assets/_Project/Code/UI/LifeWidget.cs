using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace _Project.Code.UI
{
    public class LifeWidget : CurrencyWidget
    {
        public override ReactiveProperty<int> Currency => PersistentService.Data.Progress.Life;

        [SerializeField] private TextMeshProUGUI _timerText;
        
        protected override void Awake()
        {
            base.Awake();
            
            PersistentService.Data.Progress.LifeRestoreTime.Subscribe(x =>
            {
                _timerText.text = TimeSpan.FromSeconds(x).ToString(@"mm\:ss");
            }).AddTo(this);
        }
    }
}