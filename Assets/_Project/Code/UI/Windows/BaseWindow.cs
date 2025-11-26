using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public abstract class BaseWindow : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    private Canvas _canvas;

    protected virtual void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _closeButton.onClick.AddListener(Close);
    }

    public virtual void Open()
    {
        _canvas.enabled = true;
    }

    protected virtual void Close()
    {
        _canvas.enabled = false;
    }
}