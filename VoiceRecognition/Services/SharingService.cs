namespace VoiceRecognition.Services
{
    public class SharingService
    {
        public async Task ShareTextAsync(string text, string title = "Поделиться")
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = text,
                    Title = title
                });
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                System.Diagnostics.Debug.WriteLine($"Ошибка sharing: {ex.Message}");
            }
        }
    }
}
