using CommunityToolkit.Maui.Media;
using System.Text.RegularExpressions;
using VoiceRecognition.Abstractions;
using System.Globalization;

namespace VoiceRecognition.Services
{
    public class CommunitySpeechService : ISpeechService
    {
        CancellationTokenSource? _cts;
        public bool IsListening { get; private set; }

        public event Action<string>? PartialResult;
        public event Action<string>? FinalResult;

        public async Task<bool> RequestPermissionsAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Microphone>();
            return status == PermissionStatus.Granted;
        }
        public async Task StartListeningAsync(CancellationToken ct = default)
        {
            if (IsListening) return;
            if (!await RequestPermissionsAsync()) throw new Exception("Microphone permission denied");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            IsListening = true;

            // CommunityToolkit speech to text
            try
            {
                SpeechToText.Default.RecognitionResultUpdated += OnRecognitionUpdated;
                SpeechToText.Default.RecognitionResultCompleted += OnRecognitionCompleted;


                SpeechToTextOptions options = new() { Culture = new CultureInfo("ru-RU") };

                await SpeechToText.Default.StartListenAsync(options,_cts.Token);
            }
            finally
            {
                IsListening = false;
            }
        }

        public async Task StopListeningAsync()
        {
            if (IsListening)
            {
                await SpeechToText.Default.StopListenAsync();
                SpeechToText.Default.RecognitionResultUpdated -= OnRecognitionUpdated;
                SpeechToText.Default.RecognitionResultCompleted -= OnRecognitionCompleted;
                _cts?.Cancel();
                IsListening = false;
            }
        }

        private void OnRecognitionUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs e)
        {
            // Обновляется во время диктовки
            PartialResult?.Invoke(e.RecognitionResult);
        }

        private void OnRecognitionCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs e)
        {
            // Финальный результат
            FinalResult?.Invoke(e.RecognitionResult.Text ?? string.Empty);
            IsListening = false;
        }
    }
}
