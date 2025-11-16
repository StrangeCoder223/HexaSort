using System.Collections.Generic;
using _Project.Code.Infrastructure.Factories;

namespace _Project.Code.Infrastructure.Services.WindowService
{
    public class WindowService : IWindowService
    {
        private readonly IUIFactory _uiFactory;
        private readonly Dictionary<WindowId, BaseWindow> _cachedWindows = new();
    
        public WindowService(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }
    
        public void OpenWindow(WindowId windowId)
        {
            if (_cachedWindows.ContainsKey(windowId))
            {
                _cachedWindows[windowId].Open();
                return;
            }

            switch (windowId)
            {
                case WindowId.Settings:
                    _uiFactory.CreateSettings();
                    break;
                case WindowId.Shop:
                    _uiFactory.CreateShop();
                    break;
            }
        }
    }

    public enum WindowId
    {
        Settings = 0,
        Shop = 1
    }
}