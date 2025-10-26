namespace VoiceRecognition.Abstractions {

    public interface ISpeechService
    {
        bool IsListening { get; }

        event Action<string>? PartialResult;
        event Action<string>? FinalResult;

        // Новое событие для анимации волн
        event Action<float>? VolumeChanged;

        Task StartListeningAsync(CancellationToken ct = default);
        Task StopListeningAsync();
        Task<bool> RequestPermissionsAsync();
    }

}