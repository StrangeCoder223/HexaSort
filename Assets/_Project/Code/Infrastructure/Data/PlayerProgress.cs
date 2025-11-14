using System;
using _Project.Code.Infrastructure.Configs;
using UniRx;

namespace _Project.Code.Infrastructure.Data
{
    [Serializable]
    public class PlayerProgress
    {
        public int Level;
        public ReactiveProperty<int> Money;
        public ReactiveProperty<int> Life;
        public ReactiveProperty<int> LifeRestoreTime;
        
        public SessionData SessionData;
    }

    [Serializable]
    public class SessionData
    {
        public GoalData Goal;
    }

    [Serializable]
    public class GoalData
    {
        public HexColor TargetColor;
        public int Amount;
    }
}