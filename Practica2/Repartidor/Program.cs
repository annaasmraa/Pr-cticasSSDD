using System.Threading.Channels;
using Grpc.Net.Client;
using Repartidor.Protos;

namespace Repartidor;

class Program
{
    static async Task Main(string[] args)
    {
       //To be completed
       // Reemplaza las dos siguientes líneas con la implementación propuesta en el guión, ya que
       // solo se han creados estas dos variables para proporcionar el proyecto base libre de errores
       var channel = GrpcChannel.ForAddress("");
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
       throw new Exception(); // Eliminar y sustituir por el código que implementa el pseudocódigo
    }
}
