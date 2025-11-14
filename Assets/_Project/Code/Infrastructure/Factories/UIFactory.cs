using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Services.AssetProvider;
using _Project.Code.UI;
using Cysharp.Threading.Tasks;
using Reflex.Injectors;
using UnityEngine;

namespace _Project.Code.Infrastructure.Factories
{
    public class UIFactory : ObjectFactory
    {
        public UIFactory(IAssetProvider assetProvider) : base(assetProvider) { }

        public async UniTask<GoalWidget> CreateGoalWidget(GoalData goalData, RectTransform parent = null)
        {
            GoalWidget goalWidget = await InstantiateInjectedObject<GoalWidget>(RuntimeConstants.AssetLabels.GoalWidget, parent);
            goalWidget.Initialize(goalData);
            
            return goalWidget;
        }
        
    }
}