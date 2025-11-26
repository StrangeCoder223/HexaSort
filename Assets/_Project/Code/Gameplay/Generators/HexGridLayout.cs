using _Project.Code.Infrastructure.Configs;
using UnityEngine;

namespace _Project.Code.Gameplay.Generators
{
    public class HexGridLayout
    {
        private readonly float _horizontalSpacing;
        private readonly float _hexHeight;
        private readonly float _oddColumnOffsetMultiplier;
        private readonly int _width;
        private readonly int _height;

        public HexGridLayout(GeneratorConfig config, int width, int height)
        {
            float hexRadius = config.HexRadius;
            float hexWidth = hexRadius * config.HexWidthMultiplier;
            
            _hexHeight = hexRadius * Mathf.Sqrt(3f);
            _horizontalSpacing = hexWidth * config.HorizontalSpacingMultiplier;
            _oddColumnOffsetMultiplier = config.ColumnOffsetMultiplier;
            _width = width;
            _height = height;
        }

        public Vector3 GetCellPosition(int x, int y)
        {
            float zOffset = GetColumnZOffset(x);
            return new Vector3(x * _horizontalSpacing, 0f, y * _hexHeight + zOffset);
        }

        public Vector3 CalculateGridCenter()
        {
            float gridWidth = (_width - 1) * _horizontalSpacing;
            (float minZ, float maxZ) = CalculateZBounds();
            float gridHeight = maxZ - minZ;
            
            return new Vector3(gridWidth * 0.5f, 0f, minZ + gridHeight * 0.5f);
        }

        private (float minZ, float maxZ) CalculateZBounds()
        {
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    float z = y * _hexHeight + GetColumnZOffset(x);
                    if (z < minZ) minZ = z;
                    if (z > maxZ) maxZ = z;
                }
            }

            return (minZ, maxZ);
        }

        private float GetColumnZOffset(int x)
        {
            return (x % 2 == 1) ? _hexHeight * _oddColumnOffsetMultiplier : 0f;
        }
    }
}