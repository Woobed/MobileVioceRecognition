namespace VoiceRecognition.Abstractions
{
    public interface ISpeechService
    {
        Task<bool> RequestPermissionsAsync();
        Task StartListeningAsync(CancellationToken ct = default);
        Task StopListeningAsync();
        event Action<string>? PartialResult;
        event Action<string>? FinalResult;
        bool IsListening { get; }
    }
}
