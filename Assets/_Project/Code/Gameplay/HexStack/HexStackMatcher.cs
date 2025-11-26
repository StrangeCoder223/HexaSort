using System.Linq;

namespace _Project.Code.Gameplay
{
    public class HexStackMatcher
    {
        private const int DestroyThreshold = 10;
        
        private readonly HexGridNavigator _gridNavigator;

        public HexStackMatcher(HexGridNavigator gridNavigator)
        {
            _gridNavigator = gridNavigator;
        }

        public Cell FindTransferTarget(Cell sourceCell)
        {
            if (!sourceCell.IsOccupied)
                return null;

            return _gridNavigator.GetNeighborCells(sourceCell)
                .FirstOrDefault(neighbor => ShouldTransferTo(sourceCell, neighbor));
        }

        public bool ShouldDestroy(Cell cell)
        {
            if (!cell.IsOccupied)
                return false;

            return cell.HexStack.GetTopColorCount() >= DestroyThreshold;
        }

        private bool ShouldTransferTo(Cell from, Cell to)
        {
            if (!from.IsOccupied || !to.IsOccupied)
                return false;

            if (from.HexStack.TopHex.Color != to.HexStack.TopHex.Color)
                return false;

            int fromCount = from.HexStack.HexCount;
            int toCount = to.HexStack.HexCount;

            if (toCount != fromCount)
                return toCount > fromCount;

            // При равном количестве выбираем по координатам (приоритет слева сверху)
            return to.Position.x < from.Position.x || (to.Position.x == from.Position.x && to.Position.y < from.Position.y);
        }
    }
}

