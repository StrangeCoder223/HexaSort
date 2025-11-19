using _Project.Code.Infrastructure.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Code.UI
{
    public class GoalWidget : MonoBehaviour
    {
        [field:SerializeField]
        protected TextMeshProUGUI GoalText { get; private set; }

        [SerializeField] private Image _icon;
        
        public virtual void Initialize(GoalData goalData, Sprite sprite)
        {
            _icon.sprite = sprite;
            GoalText.text = goalData.TargetAmount.ToString();
        }
    }
}