using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class HexStackTransfer : MonoBehaviour
    {
        private const int DestroyThreshold = 10;
        
        private Queue<Cell> _cellsToCheck;
        private bool _isProcessing;
        private Dictionary<(int, int), Cell> _cellsCache;
        private LevelGenerator _generator;

        [Inject]
        private void Construct(LevelGenerator generator)
        {
            _generator = generator;
        }
        
        public void Initialize()
        {
            _cellsToCheck = new Queue<Cell>();
            Cell.Occupied += TryTransfer;
            CacheCells();
        }

        private void OnDestroy()
        {
            Cell.Occupied -= TryTransfer;
        }

        private void CacheCells()
        {
            _cellsCache = new Dictionary<(int, int), Cell>();
            foreach (Cell cell in _generator.GeneratedCells)
            {
                _cellsCache[(cell.X, cell.Y)] = cell;
            }
        }

        private void TryTransfer(Cell cell)
        {
            _cellsToCheck.Enqueue(cell);
            
            if (!_isProcessing)
                ProcessQueue().Forget();
        }

        private async UniTask ProcessQueue()
        {
            _isProcessing = true;
            
            while (_cellsToCheck.Count > 0)
            {
                Cell cell = _cellsToCheck.Dequeue();
                await TryTransferAsync(cell);
            }
            
            // После обработки очереди проверяем всё поле на пропущенные слияния
            await RecheckAllCells();
            
            _isProcessing = false;
        }
        
        private async UniTask RecheckAllCells()
        {
            List<Cell> occupiedCells = new List<Cell>();
            foreach (var cell in _cellsCache.Values)
            {
                if (cell.IsOccupied)
                    occupiedCells.Add(cell);
            }
            
           
            bool foundTransfer = true;
            int maxIterations = 10;
            int iteration = 0;
            
            while (foundTransfer && iteration < maxIterations)
            {
                foundTransfer = false;
                iteration++;
                
                foreach (Cell cell in occupiedCells)
                {
                    if (!cell.IsOccupied)
                        continue;
                    
                    Cell targetNeighbor = FindTransferTarget(cell);
                    if (targetNeighbor != null)
                    {
                        await TransferStack(cell, targetNeighbor);
                        foundTransfer = true;
                        break;
                    }
                }
            }
        }
        
        private Cell FindTransferTarget(Cell cell)
        {
            if (!cell.IsOccupied)
                return null;

            foreach (Cell neighbor in GetNeighborCells(cell))
            {
                if (ShouldTransferTo(cell, neighbor))
                {
                    return neighbor;
                }
            }
            
            return null;
        }

        private async UniTask TransferStack(Cell fromCell, Cell toCell)
        {
            List<Hex> hexesToTransfer = await fromCell.HexStack.RemoveUpperAnimated(
                toCell.HexStack.transform.position,
                toCell.HexStack.HexCount
            );
            
            if (hexesToTransfer.Count == 0)
                return;
            
            toCell.HexStack.Fill(hexesToTransfer);
            
            fromCell.UpdateStackColors();
            toCell.UpdateStackColors();
            
            fromCell.TryClear();
            await CheckAndDestroy(toCell);
            
            if (fromCell.IsOccupied)
                await TryTransferFromCell(fromCell);
            
            await TryTransferFromCell(toCell);
        }

        private async UniTask TryTransferAsync(Cell cell)
        {
            if (!cell.IsOccupied)
                return;

            await TryTransferFromCell(cell);
            
            List<Cell> neighborCells = GetNeighborCells(cell);
            foreach (Cell neighbor in neighborCells)
            {
                if (neighbor.IsOccupied)
                {
                    await TryTransferFromCell(neighbor);
                }
            }
        }

        private async UniTask TryTransferFromCell(Cell cell)
        {
            if (!cell.IsOccupied)
                return;

            foreach (Cell neighbor in GetNeighborCells(cell))
            {
                if (ShouldTransferTo(cell, neighbor))
                {
                    await TransferStack(cell, neighbor);
                    break;
                }
            }
        }

        private bool ShouldTransferTo(Cell from, Cell to)
        {
            if (!to.IsOccupied)
                return false;

            if (from.HexStack.TopHex.Color != to.HexStack.TopHex.Color)
                return false;

            int fromCount = from.HexStack.HexCount;
            int toCount = to.HexStack.HexCount;

            if (toCount > fromCount)
                return true;

            if (toCount == fromCount)
                return to.X < from.X || (to.X == from.X && to.Y < from.Y);

            return false;
        }

        private async UniTask CheckAndDestroy(Cell cell)
        {
            if (!cell.IsOccupied)
                return;

            int topColorCount = cell.HexStack.GetTopColorCount();

            if (topColorCount >= DestroyThreshold)
            {
                await cell.HexStack.DestroyTopHexes(topColorCount);
                cell.UpdateStackColors();
                cell.TryClear();
            }
        }

        private List<Cell> GetNeighborCells(Cell cell)
        {
            List<Cell> neighbors = new List<Cell>();
            (int, int)[] offsets = GetHexNeighborOffsets(cell.X);

            foreach ((int dx, int dy) in offsets)
            {
                if (_cellsCache.TryGetValue((cell.X + dx, cell.Y + dy), out Cell neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private (int, int)[] GetHexNeighborOffsets(int x)
        {
            // For a hexagonal grid, neighbors depend on the parity of the column
            if (x % 2 == 0)
            {
                return new[] { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (1, -1) };
            }
            else
            {
                return new[] { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, 1), (1, 1) };
            }
        }
    }
}

