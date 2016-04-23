using System;

namespace DflatApplication.Presentation
{
    public interface IOrganizerView
    {
        event EventHandler<EventArgs> AddMusicSource;
    }
}