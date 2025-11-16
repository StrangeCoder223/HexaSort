using _Project.Code.Infrastructure.Data;
using TMPro;
using UnityEngine;

namespace _Project.Code.UI
{
    public class GoalWidget : MonoBehaviour
    {
        [field:SerializeField]
        protected TextMeshProUGUI GoalText { get; private set; }
        
        public virtual void Initialize(GoalData goalData)
        {
            GoalText.text = goalData.TargetAmount.ToString();
        }
    }
}