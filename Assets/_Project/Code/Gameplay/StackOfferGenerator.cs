using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Code.Infrastructure.Configs;
using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Services.ConfigService;
using _Project.Code.Infrastructure.Services.PersistentService;
using Random = UnityEngine.Random;

namespace _Project.Code.Gameplay
{
    public class StackOfferGenerator
    {
        private readonly IPersistentService _persistent;
        private readonly GeneratorConfig _generatorConfig;

        public StackOfferGenerator(IPersistentService persistent, IConfigService configService)
        {
            _persistent = persistent;
            _generatorConfig = configService.ForGenerator();
        }
        
        public List<ColorStack> GenerateWeightedOffers()
        {
            LevelData level = _persistent.Data.Progress.LevelData;
            
            Dictionary<HexColor, int> colorWeights = GetTopColorWeights(level);
            
            if (colorWeights.Count == 0)
            {
                return GenerateOffers();
            }

            List<ColorStack> offers = new List<ColorStack>();
            
            for (int i = 0; i < _generatorConfig.MaxOffersCount; i++)
            {
                offers.Add(GenerateWeightedStack(colorWeights));
            }

            return offers;
        }
        
        private List<ColorStack> GenerateOffers()
        {
            LevelData level = _persistent.Data.Progress.LevelData;
            
            List<HexColor> topColors = GetTopColorsFromCells(level);
            
            if (topColors.Count == 0)
            {
                topColors = GetGoalColors(level);
            }
            
            if (topColors.Count == 0)
            {
                topColors = Enum.GetValues(typeof(HexColor))
                    .Cast<HexColor>()
                    .Take(3)
                    .ToList();
            }

            List<ColorStack> offers = new List<ColorStack>();
            
            for (int i = 0; i < _generatorConfig.MaxOffersCount; i++)
            {
                offers.Add(GenerateSingleStack(topColors));
            }

            return offers;
        }
        
        private ColorStack GenerateSingleStack(List<HexColor> topColors)
        {
            int stackHeight = Random.Range(_generatorConfig.MinStackHeight, _generatorConfig.MaxStackHeight + 1);
            List<HexColor> colors = new List<HexColor>();
            
            bool isPureStack = Random.value < _generatorConfig.PureStackChance;
            
            if (isPureStack)
            {
                HexColor color = ChooseColor(topColors);
                for (int i = 0; i < stackHeight; i++)
                {
                    colors.Add(color);
                }
            }
            else
            {
                for (int i = 0; i < stackHeight; i++)
                {
                    colors.Add(ChooseColor(topColors));
                }
            }

            return new ColorStack(colors);
        }
        
        private HexColor ChooseColor(List<HexColor> topColors)
        {
            if (Random.value < _generatorConfig.NewColorChance)
            {
                var newColors = Enum.GetValues(typeof(HexColor))
                    .Cast<HexColor>()
                    .Except(topColors)
                    .ToList();
                
                if (newColors.Count > 0)
                {
                    return newColors[Random.Range(0, newColors.Count)];
                }
            }
            
            return topColors[Random.Range(0, topColors.Count)];
        }
        
        private List<HexColor> GetTopColorsFromCells(LevelData level)
        {
            HashSet<HexColor> topColors = new HashSet<HexColor>();
            
            foreach (var cell in level.Cells)
            {
                if (cell.StackColors.Count == 0)
                    continue;
                
                HexColor topColor = cell.StackColors[^1];
                topColors.Add(topColor);
            }

            return topColors.ToList();
        }
        
        private List<HexColor> GetGoalColors(LevelData level)
        {
            if (level.Goals == null || level.Goals.Count == 0)
                return new List<HexColor>();

            return level.Goals
                .Select(goal => goal.TargetColor)
                .Distinct()
                .ToList();
        }
        
        private ColorStack GenerateWeightedStack(Dictionary<HexColor, int> colorWeights)
        {
            int stackHeight = Random.Range(_generatorConfig.MinStackHeight, _generatorConfig.MaxStackHeight + 1);
            List<HexColor> colors = new List<HexColor>();
            
            bool isPureStack = Random.value < _generatorConfig.PureStackChance;
            
            if (isPureStack)
            {
                HexColor color = ChooseWeightedColor(colorWeights);
                for (int i = 0; i < stackHeight; i++)
                {
                    colors.Add(color);
                }
            }
            else
            {
                for (int i = 0; i < stackHeight; i++)
                {
                    colors.Add(ChooseWeightedColor(colorWeights));
                }
            }

            return new ColorStack(colors);
        }
        
        private HexColor ChooseWeightedColor(Dictionary<HexColor, int> colorWeights)
        {
            int totalWeight = colorWeights.Values.Sum();
            int randomValue = Random.Range(0, totalWeight);
            
            int currentWeight = 0;
            foreach (var kvp in colorWeights)
            {
                currentWeight += kvp.Value;
                if (randomValue < currentWeight)
                {
                    return kvp.Key;
                }
            }
            
            return colorWeights.Keys.First();
        }
        
        private Dictionary<HexColor, int> GetTopColorWeights(LevelData level)
        {
            Dictionary<HexColor, int> weights = new Dictionary<HexColor, int>();
            
            foreach (var cell in level.Cells)
            {
                if (cell.StackColors.Count == 0)
                    continue;
                
                HexColor topColor = cell.StackColors[^1];
                
                if (weights.ContainsKey(topColor))
                    weights[topColor]++;
                else
                    weights[topColor] = 1;
            }

            return weights;
        }
    }
}