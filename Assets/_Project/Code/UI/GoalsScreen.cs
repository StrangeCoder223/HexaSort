using _Project.Code.Infrastructure.Services.PersistentService;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Code.UI
{
    public class GoalsScreen : BaseScreen
    {
        [SerializeField] private RectTransform _goalsContainer;
        private GameObject _prefab;
        private IPersistentService _persistentService;

        [Inject]
        private void Construct(IPersistentService persistent)
        {
            _persistentService = persistent;
        }

        public override void Show()
        {
            base.Show();

            GoalWidget goal = Instantiate(_prefab, _goalsContainer).GetComponent<GoalWidget>();
            goal.Initialize();
            
        }
    }

    public class GoalWidget : MonoBehaviour
    {
        private IPersistentService _persistent;

        [Inject]
        private void Construct(IPersistentService persistent)
        {
            _persistent = persistent;
        }

        public void Initialize()
        {
            _persistent.Persistent.Progress.SessionData.Goal
        }
    }
}