using System;
using System.Collections.Generic;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Factories;
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
        private List<GoalWidget> _goalWidgets;
        private IUIFactory _uiFactory;

        [Inject]
        private void Construct(IPersistentService persistent, IUIFactory uiFactory)
        {
            _persistentService = persistent;
            _uiFactory = uiFactory;
        }

        public override async void Show()
        {
            base.Show();

            _goalWidgets = new List<GoalWidget>();

            foreach (var goal in _persistentService.Data.Progress.LevelData.Goals)
            {
                GoalWidget goalWidget = await _uiFactory.CreateGoalWidget(goal.Value, _goalsContainer);
                _goalWidgets.Add(goalWidget);
            }
        }

        public override void Hide()
        {
            base.Hide();
            
            _goalWidgets.ForEach(x => Destroy(x.gameObject));
            _goalWidgets.Clear();
        }
    }
}