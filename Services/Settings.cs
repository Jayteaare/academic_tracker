using Microsoft.Maui.Storage;

namespace C868_WGU_Tallis_Jordan.Services
{
    public static class Settings
    {
        public static bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
        }

        public static void ClearSettings() => Preferences.Clear();
    }
}