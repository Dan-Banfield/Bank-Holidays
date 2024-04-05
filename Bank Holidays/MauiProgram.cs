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
                    events.AddAndroid(android => android.OnCreate((activity, bundle) => SetSystemBarColour(activity)));

                    static void SetSystemBarColour(Android.App.Activity activity)
                    {
                        var navigationBarColour = new Android.Graphics.Color(
                            (byte)28,
                            (byte)28,
                            (byte)28,
                            (byte)255
                        );

                        activity.Window.SetNavigationBarColor(navigationBarColour);
                        activity.Window.SetStatusBarColor(navigationBarColour);
                    }
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}