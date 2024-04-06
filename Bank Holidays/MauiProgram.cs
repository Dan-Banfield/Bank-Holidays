using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace Bank_Holidays
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureLifecycleEvents(events => 
                {
#if ANDROID
                    events.AddAndroid(android => android.OnCreate((activity, bundle) => SetSystemBarTransparency(activity)));

                    static void SetSystemBarTransparency(Android.App.Activity activity)
                    {
                        activity.Window.AddFlags(Android.Views.WindowManagerFlags.LayoutNoLimits);
                        activity.Window.AddFlags(Android.Views.WindowManagerFlags.TranslucentNavigation);
                        activity.Window.AddFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
                        activity.Window.DecorView.SystemUiFlags = Android.Views.SystemUiFlags.Immersive;
                    }
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("GoogleSans-Regular.ttf", "GoogleSansRegular");
                    fonts.AddFont("GoogleSans-Bold.ttf", "GoogleSansBold");
                    fonts.AddFont("GoogleSans-BoldItalic.ttf", "GoogleSansBoldItalic");
                    fonts.AddFont("GoogleSans-Italic.ttf", "GoogleSansItalic");
                    fonts.AddFont("GoogleSans-Semibold.ttf", "GoogleSansSemibold");
                    fonts.AddFont("GoogleSans-Medium.ttf", "GoogleSansMedium");
                    fonts.AddFont("GoogleSans-MediumItalic.ttf", "GoogleSansMediumItalic");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}