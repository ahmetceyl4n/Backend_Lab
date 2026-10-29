namespace DotnetBackendLab.ThreadsAsync;

/// <summary>Her konsol örneğinin uyguladığı ortak arayüz.</summary>
public interface IExample
{
    /// <summary>Menüdeki ve "dotnet run -- N" çağrısındaki numara.</summary>
    int Number { get; }

    /// <summary>Menüde görünen kısa başlık.</summary>
    string Title { get; }

    /// <summary>Örneği çalıştırır.</summary>
    Task RunAsync();
}
