using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Restaurante.Protos;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Restaurante.Services;

public class RestauranteService : Protos.RestauranteService.RestauranteServiceBase, IDisposable
{
    //To be completed

    public RestauranteService(ILogger<RestauranteService> logger)
    {
       //To be completed

    }
    
    public override async Task<PedidoResponse> HacerPedido(PedidoRequest request, ServerCallContext context)
    {
       
        // To be completed
        throw new RpcException(new Status(StatusCode.Internal, "Error al procesar el pedido porque la cola está llena"));
    }

    public  override Task<EstadoResponse> ConsultarEstadoPedido(ConsultaRequest request, ServerCallContext context)
    {
        // To be completed
          throw new RpcException(new Status(StatusCode.NotFound, "Pedido no encontrado"));
        
    }

    public override Task<PedidoResponse> ConsultarPlatosPedido(ConsultaRequest request, ServerCallContext context)
    {
        // To be completed

        throw new RpcException(new Status(StatusCode.NotFound, "Pedido no encontrado"));
        
    }

    public override async Task<PedidoResponse> TomarPedidoParaRepartir(Empty request, ServerCallContext context)
    {
       
        // To be completed        
        throw new RpcException(new Status(StatusCode.Unavailable, "No hay pedidos disponibles"));
    }

    public void Dispose()
    { 

       // To be completed
    }
}