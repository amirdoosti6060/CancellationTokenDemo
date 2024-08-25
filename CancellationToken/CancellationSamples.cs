namespace CancellationTokenDemo
{
    public class CancellationSamples
    {
        public async Task CreationAndUsage()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            // Cancel after a delay of 2 seconds
            cts.CancelAfter(TimeSpan.FromSeconds(2));

            Console.WriteLine("Process is started and will cancel after 2 seconds ...");

            // Checking for cancellation within a long-running task
            await Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Task was cancelled.");
                        return;
                    }

                    // Simulate work
                    Thread.Sleep(500);
                    Console.WriteLine($"Task iteration {i}");
                }
            }, token);
        }

        public async Task DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            using (var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken))
            {
                byte[] buffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        public async Task DownloadMountainImage()
        {
            var url = @"https://upload.wikimedia.org/wikipedia/commons/6/6b/Big_Four_Mountain.jpg";
            var destinationPath = Path.Combine(Directory.GetCurrentDirectory() + "mountain.jpg");
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            // Cancel after a delay of 2 seconds
            cts.CancelAfter(TimeSpan.FromMilliseconds(50));

            try
            {
                Console.WriteLine("Start downloading mountain image ...");
                await DownloadFileAsync(url, destinationPath, token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Download was canceled.");
            }

        }
    }
}
