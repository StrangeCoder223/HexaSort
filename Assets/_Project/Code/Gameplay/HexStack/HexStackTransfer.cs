using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Sirenix.Utilities;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class HexStackTransfer : MonoBehaviour
    {
        private const int MaxRecheckIterations = 10;
        
        private Queue<Cell> _cellsToCheck;
        private bool _isProcessing;
        private HexGridNavigator _gridNavigator;
        private HexStackMatcher _matcher;
        private HexTransferAnimator _animator;
        private LevelGenerator _generator;

        [Inject]
        private void Construct(LevelGenerator generator, HexTransferAnimator animator)
        {
            _generator = generator;
            _animator = animator;
        }
        
        public void Initialize()
        {
            _cellsToCheck = new Queue<Cell>();
            _gridNavigator = new HexGridNavigator(_generator.GeneratedCells);
            _matcher = new HexStackMatcher(_gridNavigator);
            
            _generator.GeneratedCells.ForEach(cell => cell.Occupied += EnqueueCell);
        }

        private void OnDestroy()
        {
            _generator.GeneratedCells.ForEach(cell => cell.Occupied -= EnqueueCell);
        }

        private void EnqueueCell(Cell cell)
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
                await ProcessCellWithNeighbors(cell);
            }
            
            await RecheckAllCells();
            
            _isProcessing = false;
        }
        
        private async UniTask RecheckAllCells()
        {
            List<Cell> occupiedCells = _gridNavigator.GetAllCells()
                .Where(c => c.IsOccupied)
                .ToList();
            
            for (int iteration = 0; iteration < MaxRecheckIterations; iteration++)
            {
                bool foundTransfer = false;
                
                foreach (Cell cell in occupiedCells.Where(c => c.IsOccupied))
                {
                    Cell target = _matcher.FindTransferTarget(cell);
                    if (target != null)
                    {
                        await TransferStack(cell, target);
                        foundTransfer = true;
                        break;
                    }
                }
                
                if (!foundTransfer)
                    break;
            }
        }

        private async UniTask TransferStack(Cell fromCell, Cell toCell)
        {
            List<Hex> hexesToTransfer = fromCell.HexStack.RemoveUpper();
            
            if (hexesToTransfer.Count == 0)
                return;
            
            await _animator.AnimateTransfer(hexesToTransfer, toCell.HexStack.TopPosition, toCell.HexStack.HexCount);
            
            toCell.HexStack.Fill(hexesToTransfer);
            
            UpdateCellState(fromCell);
            await UpdateAndCheckCell(toCell);
            
            if (fromCell.IsOccupied)
                await TryTransferFromCell(fromCell);
            
            await TryTransferFromCell(toCell);
        }
        
        private void UpdateCellState(Cell cell)
        {
            cell.UpdateStackColors();
            cell.TryClear();
        }
        
        private async UniTask UpdateAndCheckCell(Cell cell)
        {
            cell.UpdateStackColors();
            await CheckAndDestroy(cell);
            cell.TryClear();
        }

        private async UniTask ProcessCellWithNeighbors(Cell cell)
        {
            if (!cell.IsOccupied)
                return;

            await TryTransferFromCell(cell);
            
            foreach (Cell neighbor in _gridNavigator.GetNeighborCells(cell).Where(n => n.IsOccupied))
            {
                await TryTransferFromCell(neighbor);
            }
        }

        private async UniTask TryTransferFromCell(Cell cell)
        {
            if (!cell.IsOccupied)
                return;

            Cell target = _matcher.FindTransferTarget(cell);
            if (target != null)
            {
                await TransferStack(cell, target);
            }
        }

        private async UniTask CheckAndDestroy(Cell cell)
        {
            if (_matcher.ShouldDestroy(cell))
            {
                int topColorCount = cell.HexStack.GetTopColorCount();
                await cell.HexStack.DestroyTopHexes(topColorCount);
            }
        }
    }
}

