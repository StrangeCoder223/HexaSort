using Cysharp.Threading.Tasks;
using UniRx;

namespace _Project.Code.UI
{
    public class MoneyWidget : CurrencyWidget
    {
        public override ReactiveProperty<int> Currency => PersistentService.Persistent.Progress.Money;
    }
}