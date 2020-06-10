using System;

namespace Bathymetry.Data.Providers
{
    public interface IReadingProvider
    {
        bool IsStarted { get; }

        event EventHandler<string> OnReadingRecieved;

        void Start();
        void Stop();
    }
}