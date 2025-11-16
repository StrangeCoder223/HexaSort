using _Project.Code.Infrastructure.Services.WindowService;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenWindowButton : MonoBehaviour
{
    [SerializeField] private WindowId _windowId;
    
    private Button _button;
    private IWindowService _windowService;
    
    [Inject]
    private void Construct(IWindowService windowService)
    {
        _windowService = windowService;
    }

    private void Awake()
    {
        _button.onClick.AddListener(() => _windowService.OpenWindow(_windowId));
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}
