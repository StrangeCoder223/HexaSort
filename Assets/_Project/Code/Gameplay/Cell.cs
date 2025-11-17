using _Project.Code.Infrastructure.Factories;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class Cell : MonoBehaviour
    {
        public bool IsOcuppied => _hexStack != null;
        
        private HexStack _hexStack;
        
        public void Occupy(HexStack hexStack)
        {
            if (_hexStack != null)
                return;
            
            _hexStack = hexStack;
            _hexStack.transform.SetParent(transform);
            _hexStack.transform.localPosition = Vector3.zero;
        }
    }
}