using System.Threading.Channels;
using Google.Protobuf.WellKnownTypes; // Añade esta línea al principio del archivo
using Grpc.Net.Client;
using Repartidor.Protos;


namespace Repartidor;


class Program
{
    static async Task Main(string[] args)
    {
       //To be completed
       // Configurar para permitir HTTP/2 sin TLS 
        var httpHandler = new HttpClientHandler(); 
        httpHandler.ServerCertificateCustomValidationCallback =  
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; 
        var channel = GrpcChannel.ForAddress("http://localhost:5000", 
        new GrpcChannelOptions 
        { 
            HttpHandler = httpHandler 
        }); 
        var client = new RestauranteService.RestauranteServiceClient(channel); 


        Console.WriteLine("Cliente gRPC del Repartidor");
        Console.WriteLine("1. Tomar pedido para repartir");
        Console.WriteLine("2. Salir");

        while (true)
        {
            Console.Write("\nElige una opción (1-2): ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await TomarPedidoParaRepartir(client);
                    break;
                case "2":
                    return;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }
        }
    }

    static async Task TomarPedidoParaRepartir(RestauranteService.RestauranteServiceClient client)
    {
        try
        {
            var emptyRequest = new Empty();
            // Llamar al método remoto para tomar un pedido para repartir
            var response = client.TomarPedidoParaRepartir(emptyRequest);

            // Mostrar en pantalla el ID del pedido recibido
            Console.WriteLine($"ID del pedido recibido: {response.IdPedido}");

        }
        catch (Exception ex)
        {
            // Mostrar en pantalla el mensaje de error
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    }

