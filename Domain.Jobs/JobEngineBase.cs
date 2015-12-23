using System;
using System.Threading.Tasks;

namespace Domain.Jobs
{
	public class JobEngineBase
	{
		private TaskScheduler scheduler = null;
		int dummy_result;

		public JobEngineBase ()
		{
			scheduler = TaskScheduler.FromCurrentSynchronizationContext ();
			dummy_result = 0;
		}

		public void RunJob()
		{
			Task.Factory.StartNew<int> (() => dummy_result + 1)
				.ContinueWith ((t) => dummy_result = t.Result, this.scheduler);
		}
	}
}

