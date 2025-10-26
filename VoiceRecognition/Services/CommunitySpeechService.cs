using CommunityToolkit.Maui.Media;
using System.Globalization;
using VoiceRecognition.Abstractions;

namespace VoiceRecognition.Services

{

    public class CommunitySpeechService : ISpeechService
    {
        CancellationTokenSource? _cts;
        private bool _isStopping = false; // флаг остановки
        public bool IsListening { get; private set; }

        public event Action<string>? PartialResult;
        public event Action<string>? FinalResult;
        public event Action<float>? VolumeChanged;

        Random rnd = new();

        public async Task<bool> RequestPermissionsAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Microphone>();
            return status == PermissionStatus.Granted;
        }

        public async Task StartListeningAsync(CancellationToken ct = default)
        {
            if (IsListening || _isStopping) return; // защита от двойного старта

            if (!await RequestPermissionsAsync())
                throw new Exception("Microphone permission denied");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            IsListening = true;

            try
            {
                SpeechToText.Default.RecognitionResultUpdated += OnRecognitionUpdated;
                SpeechToText.Default.RecognitionResultCompleted += OnRecognitionCompleted;

                var options = new SpeechToTextOptions { Culture = new CultureInfo("ru-RU") };
                _ = SimulateVolumeAsync(_cts.Token);

                await SpeechToText.Default.StartListenAsync(options, _cts.Token);
            }
            catch
            {
                IsListening = false;
                throw;
            }
        }

        public async Task StopListeningAsync()
        {
            if (!IsListening || _isStopping) return;

            _isStopping = true;

            try
            {
                _cts?.Cancel();
                await SpeechToText.Default.StopListenAsync();

                SpeechToText.Default.RecognitionResultUpdated -= OnRecognitionUpdated;
                SpeechToText.Default.RecognitionResultCompleted -= OnRecognitionCompleted;
            }
            finally
            {
                IsListening = false;
                _isStopping = false;
            }
        }

        private void OnRecognitionUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs e)
        {
            PartialResult?.Invoke(e.RecognitionResult);
        }

        private void OnRecognitionCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs e)
        {
            FinalResult?.Invoke(e.RecognitionResult.Text ?? string.Empty);
            IsListening = false;
        }

        private async Task SimulateVolumeAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && IsListening)
            {
                float volume = (float)(rnd.NextDouble() * 10);
                VolumeChanged?.Invoke(volume);
                await Task.Delay(100);
            }
        }
    }


}