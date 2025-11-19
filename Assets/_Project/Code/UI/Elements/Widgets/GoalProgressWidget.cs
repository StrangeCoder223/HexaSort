using _Project.Code.Infrastructure.Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Code.UI
{
    public class GoalProgressWidget : GoalWidget
    {
        [SerializeField] private Slider _progress;
        
        public override void Initialize(GoalData goalData, Sprite icon)
        {
            base.Initialize(goalData, icon);
            
            goalData.CurrentAmount.Subscribe(current => RefreshProgress(current, goalData.TargetAmount)).AddTo(this);
        }

        private void RefreshProgress(int current, int target)
        {
            _progress.value = (float)current / target;
            GoalText.text = current + "/" + target;
        }
    }
}