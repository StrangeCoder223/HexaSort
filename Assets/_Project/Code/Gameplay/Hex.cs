using _Project.Code.Infrastructure.Configs;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    public class Hex : MonoBehaviour
    {
        public HexColor Color => _colorConfig.HexColor;
        
        [SerializeField] private MeshRenderer _meshRenderer;
        private HexConfig _colorConfig;
        
        public void Initialize(HexConfig colorConfig)
        {
            _meshRenderer.material.color = colorConfig.MeshColor;
            _colorConfig = colorConfig;
        }
    }
}