using System;

namespace Application.Presentation
{
	public interface IOrganizerView
	{
		// Action
		event EventHandler<EventArgs> AddMusicSource;
	}
}

