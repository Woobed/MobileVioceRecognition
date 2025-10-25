using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using VoiceRecognition.Abstractions;
using VoiceRecognition.Services;

namespace VoiceRecognition
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .UseMauiCommunityToolkitCore(); ;

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<ISpeechService, CommunitySpeechService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
