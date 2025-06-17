using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantWatch.Core.Models.Drivers;

namespace PlantWatch.Core.Services.Drivers
{

    public interface IPLCService
    {
        string Name { get; }
        bool IsConnected { get; }
        bool IsRunning { get; }

        IEnumerable<ITag> Tags { get; }

        Task StartAsync();
        Task StopAsync();
        Task<bool> WriteTagAsync(string tagName, object value);

        void ForceRemap();
    }

}
