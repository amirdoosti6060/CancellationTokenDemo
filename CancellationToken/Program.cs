using CancellationTokenDemo;

Console.WriteLine("CancellationToken demo");
CancellationSamples sample = new CancellationSamples();

await sample.CreationAndUsage();
await sample.DownloadMountainImage();
