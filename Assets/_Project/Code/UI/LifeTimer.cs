using System;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using Reflex.Attributes;
using UniRx;
using UnityEngine;

namespace _Project.Code.UI
{
    public class LifeTimer : MonoBehaviour
    {
        private CompositeDisposable _timerDisposable;
        private IPersistentService _persistent;
        private IConfigService _configService;

        [Inject]
        private void Construct(IPersistentService persistent, IConfigService configService)
        {
            _persistent = persistent;
            _configService = configService;
        }

        private void Awake() => _persistent.Persistent.Progress.Life.Subscribe(TryStartTimer).AddTo(this);

        private void OnDestroy() => _timerDisposable?.Dispose();

        private void TryStartTimer(int lifeAmount)
        {
            int maxLife = _configService.ForMeta().MaxLife;
            
            if (lifeAmount >= maxLife)
                return;
            
            var lifeRestoreTime = _persistent.Persistent.Progress.LifeRestoreTime;
            
            if (lifeRestoreTime.Value <= 0) 
                lifeRestoreTime.Value = _configService.ForMeta().LifeRestoreTime;

            _timerDisposable = new();
            
            Observable.Interval(TimeSpan.FromSeconds(1f))
                .Subscribe(x =>
                {
                    lifeRestoreTime.Value--;
                    if (lifeRestoreTime.Value <= 0)
                    {
                        _timerDisposable?.Dispose();
                        _persistent.Persistent.Progress.Life.Value++;
                    }
                })
                .AddTo(_timerDisposable);
        }
    }
}