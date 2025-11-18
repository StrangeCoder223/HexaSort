using _Project.Code.Infrastructure.Factories;
using _Project.Code.Infrastructure.Services.PersistentService;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Code.UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private RectTransform _goalsProgressContainer;
        
        private IPersistentService _persistentService;
        private IUIFactory _uiFactory;

        [Inject]
        private void Construct(IPersistentService persistent, IUIFactory uiFactory)
        {
            _persistentService = persistent;
            _uiFactory = uiFactory;
        }
        
        public async void Initialize()
        {
            foreach (var goal in _persistentService.Data.Progress.LevelData.Goals)
            {
                GoalProgressWidget goalProgressWidget = await _uiFactory.CreateGoalProgressWidget(goal.Value, _goalsProgressContainer);
            }
        }
    }
}