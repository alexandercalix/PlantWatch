using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Models.Drivers;

namespace PlantWatch.Engine.Core.Services.Drivers
{

    public interface IPLCService
    {
        Guid Id { get; }
        string Name { get; }
        bool IsConnected { get; }
        bool IsRunning { get; }

        IEnumerable<ITag> Tags { get; }

        Task StartAsync();
        Task StopAsync();
        Task<bool> WriteTagAsync(Guid Id, object value);

        void ForceRemap();
    }

}
