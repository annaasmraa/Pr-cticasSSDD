using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Restaurante.Protos;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Restaurante.Services;



public class RestauranteService : Restaurante.Protos.RestauranteService.RestauranteServiceBase, IDisposable
{
    private readonly ILogger<RestauranteService> _logger;
    private readonly ConcurrentQueue<Pedido> _ordersQueue;
    private const int MaxQueueCapacity = 10;
    private readonly SemaphoreSlim _readyOrdersSemaphore;
    private readonly object _takeLock;
    private int _orderID;

    public RestauranteService(ILogger<RestauranteService> logger)
    {
        _logger = logger;
        _ordersQueue = new ConcurrentQueue<Pedido>();
        _readyOrdersSemaphore = new SemaphoreSlim(0, MaxQueueCapacity);
        _takeLock = new object();
        _orderID = 0;
    }


       public override async Task<PedidoResponse> HacerPedido(PedidoRequest request, ServerCallContext context)
    {
        lock(_takeLock)
        {
            if (_ordersQueue.Count < MaxQueueCapacity)
            {
                int numeroPedidoActual = _orderID;
                Pedido nuevoPedido = new Pedido(
                    numeroPedidoActual,
                    request.Platos.ToList(),
                    (int)request.Distancia
                );
                _ordersQueue.Enqueue(nuevoPedido);
                _logger.LogInformation("Pedido recibido: {PedidoId} - {PlatosCount} platos - Distancia: {Distancia} km", nuevoPedido.Id, nuevoPedido.Platos.Count, nuevoPedido.TiempoEstimado);
                _orderID++;
                _readyOrdersSemaphore.Release();
                return new PedidoResponse
                {
                    IdPedido = numeroPedidoActual,
                    Mensaje = $"Pedido #{numeroPedidoActual} recibido correctamente. Tiempo estimado: {nuevoPedido.TiempoEstimado} minutos."
                };
            }
        }
        throw new RpcException(new Status(StatusCode.Internal, "Error al procesar el pedido porque la cola está llena"));
    }

 public override Task<EstadoResponse> ConsultarEstadoPedido(ConsultaRequest request, ServerCallContext context)
{
    // Buscar en la cola el primer pedido cuyo Id sea igual al IdPedido recibido en la petición
    var pedido = _ordersQueue.FirstOrDefault(p => p.Id == request.IdPedido.ToString());

    // Si se encuentra el pedido
    if (pedido != null)
    {
        // Devolver una respuesta con el estado y el tiempo estimado del pedido
        return Task.FromResult(new EstadoResponse
        {
            Estado = pedido.Estado,
            Mensaje = $"Estado del pedido {pedido.Id}",
            TiempoEstimado = pedido.TiempoEstimado
        });
    }

    // Si no se encuentra el pedido
    // Lanzar una excepción indicando que el pedido no existe
    throw new RpcException(new Status(StatusCode.NotFound, "El pedido no existe"));
}


 public override Task<PedidoResponse> ConsultarPlatosPedido(ConsultaRequest request, ServerCallContext context)
{
    // Buscar en la cola el primer pedido cuyo Id sea igual al IdPedido recibido en la petición
    var pedido = _ordersQueue.FirstOrDefault(p => p.Id == request.IdPedido.ToString());

    // Si se encuentra el pedido
    if (pedido != null)
    {
        // Construir una descripción de los platos
        var detallePlatos = string.Join(", ",
            pedido.Platos.Select(plato => $"{plato.Cantidad}x {plato.Nombre}"));

        // Devolver una respuesta con el mensaje que incluye los platos
        return Task.FromResult(new PedidoResponse
        {
            IdPedido = int.Parse(pedido.Id),
            Mensaje = $"Platos del pedido {pedido.Id}: {detallePlatos}"
        });
    }

    // Si no se encuentra el pedido
    throw new RpcException(new Status(StatusCode.NotFound, "El pedido no existe"));
}



    public override async Task<PedidoResponse> TomarPedidoParaRepartir(Empty request, ServerCallContext context)
    {
        // Esperar hasta que haya al menos un pedido disponible (señal del semáforo)
        await _readyOrdersSemaphore.WaitAsync();

        // Entrar en sección crítica (lock)
        lock (_takeLock)
        {
            // Intentar extraer un pedido de la cola
            if (_ordersQueue.TryDequeue(out var pedido))
            {
                // Marcar el pedido como "En reparto"
                pedido.ActualizarEstado("En reparto");

                // Devolver una respuesta con el ID y mensaje de reparto
                return new PedidoResponse
                {
                    IdPedido = int.Parse(pedido.Id),
                    Mensaje = $"Pedido {pedido.Id} tomado para repartir."
                };
            }
        }

        // Si no se ha podido extraer un pedido, lanzar una excepción
        throw new RpcException(new Status(StatusCode.Unavailable, "No hay pedidos disponibles"));
    }

   public void Dispose()
{
    // Liberar los recursos del semáforo
    _readyOrdersSemaphore?.Dispose();
}
}