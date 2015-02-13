using System;
using System.Net;
using Domain.Servicos;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;

namespace projectionstest {
    internal class Program {
        private static void Main(string[] args) {
            var projManager = new ProjectionsManager(new ConsoleLogger(), new IPEndPoint(IPAddress.Loopback, 2125), TimeSpan.FromSeconds(60));


            try {
                
                var t1 = new ServicoDuplicacaoNif(projManager).NifDuplicado("123456789", Guid.NewGuid()).Result;
                var t = projManager.GetStateAsync("NifsDirecoes").Result;
                Console.WriteLine(t);
            }
            catch (Exception ex) {
                Console.Write(ex.ToString());
            }
        }
    }
}