using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BDMT.Client.Clientside
{
    public static class GrpcExtensions
    {
        public static void AddGrpcChannel(this IServiceCollection services, string baseUri)
        {
            services.AddScoped(services =>
            {
                // Create a gRPC-Web channel pointing to the backend server
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
                return GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
            });
        }

        public static void AddGrpcCodeFirstService<TService>(this IServiceCollection services) 
            where TService : class
        {
            services.AddScoped(services =>
            {
                var channel = services.GetRequiredService<GrpcChannel>();
                return channel.CreateGrpcService<TService>();
            });
        }
    }
}
