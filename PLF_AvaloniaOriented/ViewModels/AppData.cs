using Avalonia.Controls;

namespace PLF_AvaloniaOriented.ViewModels
{
    public class AppData
    {
        private static AppData _instance;

        private AppData() 
        {
            Wnd = null;
        }

        public static AppData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppData();
                }
                return _instance;
            }
        }

        public Window Wnd { get; set; }
    }
}