using System;

namespace Application.Presentation
{
	public class OrganizerPresenter
	{
		private readonly IOrganizerView organizerView;

		public OrganizerPresenter (IOrganizerView organizerView)
		{
			if (organizerView == null)
				throw new ArgumentNullException("organizerWindow");

			this.organizerView = organizerView;

			this.organizerView.AddMusicSource += OnAddMusicSource;
		}

		private void OnAddMusicSource(object sender, EventArgs e)
		{

		}
	}
}

