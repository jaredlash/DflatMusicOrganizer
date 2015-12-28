using System;

namespace Application.Presentation
{
    public interface IOrganizerView
    {
        Action<object, EventArgs> AddMusicSource { get; set; }
    }
}