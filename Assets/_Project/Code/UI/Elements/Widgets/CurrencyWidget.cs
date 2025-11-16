using _Project.Code.Infrastructure.Services.PersistentService;
using Reflex.Attributes;
using TMPro;
using UniRx;
using UnityEngine;

namespace _Project.Code.UI
{
    public abstract class CurrencyWidget : MonoBehaviour
    {
        public virtual ReactiveProperty<int> Currency { get; protected set; }
        protected IPersistentService PersistentService { get; private set; }

        [SerializeField] private TextMeshProUGUI _currencyText; 
        
        [Inject]
        private void Construct(IPersistentService persistentService)
        {
            PersistentService = persistentService;
        }

        protected virtual void Awake()
        {
            Currency.Subscribe(RefreshCurrency).AddTo(this);
        }

        protected virtual void RefreshCurrency(int amount)
        {
            _currencyText.text = amount.ToString();
        }
    }
}