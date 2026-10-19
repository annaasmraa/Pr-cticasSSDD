using Grpc.Net.Client;
using Restaurante.Protos;

namespace Cliente.Services;


public class RestauranteClientService
{
    private readonly string _serverAddress;
    private GrpcChannel? _channel;
    private RestauranteService.RestauranteServiceClient? _client;

    public RestauranteClientService(string serverAddress = "http://localhost:5000")
    {
        _serverAddress = serverAddress;
    }

    public Task ConnectAsync()
    {
        _channel = GrpcChannel.ForAddress(_serverAddress);
        _client = new RestauranteService.RestauranteServiceClient(_channel);
        return Task.CompletedTask;
    }

    public Task DisconnectAsync()
    {
        _channel?.Dispose();
        _channel = null;
        _client = null;
        return Task.CompletedTask;
    }

    private void EnsureConnected()
    {
        if (_client == null)
        {
            throw new InvalidOperationException("No está conectado al servidor. Llame a ConnectAsync() primero.");
        }
    }

    public async Task<PedidoResponse> HacerPedidoAsync(List<(string nombre, int cantidad)> platos, int distancia)
    {
        EnsureConnected(); // No eliminar

        throw new Exception(); // Eliminar y sustituir por el código que implementa el pseudocódigo
    }

    public async Task<EstadoResponse> ConsultarEstadoPedidoAsync(string idPedido)
    {
        EnsureConnected();

        throw new Exception(); // Eliminar y sustituir por el código que implementa el pseudocódigo
    }
}
