using System.Collections.Generic;

namespace _Project.Code.Gameplay
{
    public class HexGridNavigator
    {
        private static readonly (int, int)[] EvenOffsets = { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (1, -1) };
        private static readonly (int, int)[] OddOffsets = { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, 1), (1, 1) };
        
        private readonly Dictionary<(int, int), Cell> _cellsCache;

        public HexGridNavigator(IEnumerable<Cell> cells)
        {
            _cellsCache = new Dictionary<(int, int), Cell>();
            
            foreach (Cell cell in cells)
            {
                _cellsCache[(cell.Position.x, cell.Position.y)] = cell;
            }
        }

        public List<Cell> GetNeighborCells(Cell cell)
        {
            List<Cell> neighbors = new List<Cell>();
            (int, int)[] offsets = GetOffsetsForColumn(cell.Position.x);

            foreach ((int dx, int dy) in offsets)
            {
                if (_cellsCache.TryGetValue((cell.Position.x + dx, cell.Position.y + dy), out Cell neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public IEnumerable<Cell> GetAllCells()
        {
            return _cellsCache.Values;
        }

        private (int, int)[] GetOffsetsForColumn(int x)
        {
            return x % 2 == 0 ? EvenOffsets : OddOffsets;
        }
    }
}

