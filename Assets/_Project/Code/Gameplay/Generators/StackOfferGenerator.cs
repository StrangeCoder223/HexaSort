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
            
            List<HexColor> availableColors = colorWeights.Count > 0 
                ? colorWeights.Keys.ToList() 
                : GetAvailableColors(level);

            return GenerateOffers(availableColors, colorWeights.Count > 0 ? colorWeights : null);
        }
        
        private List<ColorStack> GenerateOffers(List<HexColor> availableColors, Dictionary<HexColor, int> weights = null)
        {
            List<ColorStack> offers = new List<ColorStack>(_generatorConfig.MaxOffersCount);
            
            for (int i = 0; i < _generatorConfig.MaxOffersCount; i++)
            {
                offers.Add(GenerateStack(availableColors, weights));
            }

            return offers;
        }
        
        private ColorStack GenerateStack(List<HexColor> availableColors, Dictionary<HexColor, int> weights)
        {
            int stackHeight = Random.Range(_generatorConfig.MinStackHeight, _generatorConfig.MaxStackHeight + 1);
            bool isPureStack = Random.value < _generatorConfig.PureStackChance;

            List<HexColor> colors = new List<HexColor>(stackHeight);
            
            if (isPureStack)
            {
                HexColor color = ChooseColor(availableColors, weights);
                colors.AddRange(Enumerable.Repeat(color, stackHeight));
            }
            else
            {
                for (int i = 0; i < stackHeight; i++)
                {
                    colors.Add(ChooseColor(availableColors, weights));
                }
            }

            return new ColorStack(colors);
        }
        
        private HexColor ChooseColor(List<HexColor> availableColors, Dictionary<HexColor, int> weights)
        {
            if (weights != null)
                return ChooseWeightedColor(weights);

            if (Random.value < _generatorConfig.NewColorChance)
            {
                var newColors = Enum.GetValues(typeof(HexColor))
                    .Cast<HexColor>()
                    .Where(c => c != HexColor.Any && !availableColors.Contains(c))
                    .ToList();
                
                if (newColors.Count > 0)
                    return newColors[Random.Range(0, newColors.Count)];
            }
            
            return availableColors[Random.Range(0, availableColors.Count)];
        }
        
        private HexColor ChooseWeightedColor(Dictionary<HexColor, int> colorWeights)
        {
            int totalWeight = colorWeights.Values.Sum();
            int randomValue = Random.Range(0, totalWeight);
            
            int currentWeight = 0;
            foreach (var (color, weight) in colorWeights)
            {
                currentWeight += weight;
                if (randomValue < currentWeight)
                    return color;
            }
            
            return colorWeights.Keys.First();
        }
        
        private List<HexColor> GetAvailableColors(LevelData level)
        {
            List<HexColor> colors = GetTopColorsFromCells(level);
            
            if (colors.Count == 0)
                colors = GetGoalColors(level);
            
            if (colors.Count == 0)
                colors = Enum.GetValues(typeof(HexColor))
                    .Cast<HexColor>()
                    .Where(c => c != HexColor.Any)
                    .Take(3)
                    .ToList();

            return colors;
        }
        
        private List<HexColor> GetTopColorsFromCells(LevelData level)
        {
            return level.Cells
                .Where(cell => cell.StackColors.Count > 0)
                .Select(cell => cell.StackColors[^1])
                .Distinct()
                .ToList();
        }
        
        private List<HexColor> GetGoalColors(LevelData level)
        {
            if (level.Goals == null || level.Goals.Count == 0)
                return new List<HexColor>();

            return level.Goals.Keys
                .Where(color => color != HexColor.Any)
                .ToList();
        }
        
        private Dictionary<HexColor, int> GetTopColorWeights(LevelData level)
        {
            return level.Cells
                .Where(cell => cell.StackColors.Count > 0)
                .Select(cell => cell.StackColors[^1])
                .GroupBy(color => color)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}