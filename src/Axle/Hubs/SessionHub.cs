﻿namespace Axle.Hubs
{
    using System;
    using System.Threading.Tasks;
    using Axle.Persistence;
    using Microsoft.AspNetCore.SignalR;
    using Serilog;

    public class SessionHub : Hub
    {
        private readonly SessionHubMethods<SessionHub> hubMethods;
        private readonly IRepository<string, HubConnectionContext> connectionRepository;

        public SessionHub(SessionHubMethods<SessionHub> hubMethods, IRepository<string, HubConnectionContext> connectionRepository)
        {
            this.hubMethods = hubMethods;
            this.connectionRepository = connectionRepository;
        }

        public static string Name => "session";

        public void TerminateSession()
        {
            this.hubMethods.TerminateSession(this.Context.ConnectionId);
        }

        public void StartSession(string userId)
        {
            this.hubMethods.StartSession(this.Context.ConnectionId, userId);
        }

        public override Task OnConnectedAsync()
        {
            Log.Information($"New connection established (ID: {this.Context.ConnectionId}).");
            this.connectionRepository.Add(this.Context.ConnectionId, this.Context.Connection);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Log.Information($"Disconnected: {this.Context.ConnectionId}).");
            this.connectionRepository.Remove(this.Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
