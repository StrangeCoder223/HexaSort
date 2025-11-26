using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Services.AssetProvider;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Code.Infrastructure.Factories
{
    public class UIFactory : ObjectFactory, IUIFactory
    {
        private readonly IConfigService _configService;

        public UIFactory(IAssetProvider assetProvider, IConfigService configService) : base(assetProvider)
        {
            _configService = configService;
        }
        
        public void Clear() { }

        public async UniTask<GoalWidget> CreateGoalWidget(GoalData goalData, RectTransform parent = null)
        {
            GoalWidget goalWidget = await InstantiateInjectedObject<GoalWidget>(RuntimeConstants.AssetLabels.GoalWidget, parent);
            goalWidget.Initialize(goalData, _configService.ForHex(goalData.HexColor).Icon);
            
            return goalWidget;
        }

        public async UniTask<GoalProgressWidget> CreateGoalProgressWidget(GoalData goalData, RectTransform parent = null)
        {
            GoalProgressWidget goalProgressWidget =  await InstantiateInjectedObject<GoalProgressWidget>(RuntimeConstants.AssetLabels.GoalProgressWidget, parent);
            goalProgressWidget.Initialize(goalData, _configService.ForHex(goalData.HexColor).Icon);

            return goalProgressWidget;
        }

        public void CreateSettings()
        {
            throw new System.NotImplementedException();
        }

        public void CreateShop()
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IUIFactory
    {
        UniTask<GoalWidget> CreateGoalWidget(GoalData goalData, RectTransform parent = null);
        void CreateSettings();
        void CreateShop();
        UniTask<GoalProgressWidget> CreateGoalProgressWidget(GoalData goalData, RectTransform parent = null);
    }
}