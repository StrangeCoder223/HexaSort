using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace _Project.Code.UI
{
    public class LifeWidget : CurrencyWidget
    {
        public override ReactiveProperty<int> Currency => PersistentService.Persistent.Progress.Life;

        [SerializeField] private TextMeshProUGUI _timerText;
        
        protected override void Awake()
        {
            base.Awake();
            
            PersistentService.Persistent.Progress.LifeRestoreTime.Subscribe(x =>
            {
                _timerText.text = TimeSpan.FromSeconds(x).ToString(@"mm\:ss");
            }).AddTo(this);
        }
    }
}