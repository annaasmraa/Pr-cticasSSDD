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

    public async Task ConnectAsync()
    {
        // If using unencrypted HTTP/2 (http://) enable the switch (only for development) 
        if (_serverAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) 
            { 
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); 
            } 
        // Prepare channel options.  
        var channelOptions = new GrpcChannelOptions(); 
        //En el fichero proporcionado podéis comprobar que un apartado para conectar si usamos https (no requerido en esta práctica). 
        _channel = GrpcChannel.ForAddress(_serverAddress, channelOptions); 
        _client = new RestauranteService.RestauranteServiceClient(_channel); 
        Console.WriteLine($"Conectado al servidor en {_serverAddress}"); 
    }

    public async Task DisconnectAsync()
    {
        if (_channel !=null )
        {
            await _channel.ShutdownAsync();
            Console.WriteLine("Desconectado del servidor");
        }
        
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
    EnsureConnected();

    // Crear el objeto PedidoRequest
    var pedidoRequest = new PedidoRequest();

    // Añadir cada plato al pedido
    foreach (var (nombre, cantidad) in platos)
    {
        pedidoRequest.Platos.Add(new PlatoPedido { Nombre = nombre, Cantidad = cantidad });
    }

    // Asignar la distancia
    pedidoRequest.Distancia = distancia;

    try
    {
        // Enviar el pedido al servidor
        var response = await _client.HacerPedidoAsync(pedidoRequest);

        // Mostrar el identificador del pedido y el mensaje recibido
        Console.WriteLine($"ID del pedido: {response.IdPedido}");
        Console.WriteLine($"Mensaje: {response.Mensaje}");

        // Devolver la respuesta
        return response;
    }
    catch (Exception ex)
    {
        // Mostrar el mensaje de error
        Console.WriteLine($"Error al enviar el pedido: {ex.Message}");

        // Relanzar la excepción
        throw;
    }
}


 public async Task<EstadoResponse> ConsultarEstadoPedidoAsync(string idPedido)
{
    EnsureConnected();

    try
    {
        // Verificar si el cliente espera un string o un int
        // Intentamos usar el valor directamente como string (por si el cliente lo espera así)
        var consultaRequest = new ConsultaRequest
        {
            IdPedido = idPedido 
        };

        // Invocar el método en el servidor
        var response = await _client.ConsultarEstadoPedidoAsync(consultaRequest);

        // Mostrar el estado y el tiempo estimado del pedido
        Console.WriteLine($"Estado del pedido: {response.Estado}");
        Console.WriteLine($"Tiempo estimado: {response.TiempoEstimado} minutos");
        Console.WriteLine($"Mensaje: {response.Mensaje}");

        // Devolver la respuesta
        return response;
    }
    catch (Exception ex)
    {
        // Mostrar el mensaje de error
        Console.WriteLine($"Error al consultar el estado del pedido: {ex.Message}");

        // Relanzar la excepción
        throw;
    }
}




}
