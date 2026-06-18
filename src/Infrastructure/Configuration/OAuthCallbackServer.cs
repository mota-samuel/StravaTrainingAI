using System.Net;

namespace Infrastructure.Configuration;
public sealed class OAuthCallbackServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly TaskCompletionSource<string> _tcs = new();

    public OAuthCallbackServer(string url = "http://localhost:5000/callback/")
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(url);
    }

    public void Start() => _listener.Start();

    public async Task<string> WaitForCodeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.Register(() => _tcs.TrySetCanceled());

        _ = Task.Run(async () =>
        {
            var context = await _listener.GetContextAsync();
            var code = context.Request.QueryString["code"];

            if (string.IsNullOrWhiteSpace(code))
            {
                _tcs.TrySetException(
                    new InvalidOperationException("Código OAuth não recebido!"));
                return;
            }

            var html = """
                            <html><body style='font-family:sans-serif;text-align:center;padding:50px'>
                <h2>✅ Autenticação concluída!</h2>
                <p>Pode fechar esta aba e voltar ao Visual Studio.</p>
                </body></html>
                <html><body style='font-family:sans-serif;text-align:center;padding:50px'>
                <h2>✅ Autenticação concluída!</h2>
                <p>Pode fechar esta aba e voltar ao Visual Studio.</p>
                </body></html>

            """;
            var buf = System.Text.Encoding.UTF8.GetBytes(html);
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = buf.Length;
            await context.Response.OutputStream.WriteAsync(buf);
            context.Response.OutputStream.Close();

            _tcs.TrySetResult(code);
        }, cancellationToken);
        return await _tcs.Task;
    }


    public void Dispose() => _listener.Stop();
}
