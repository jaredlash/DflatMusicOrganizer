using System;

namespace Domain.Jobs
{
	public interface IRequest<TResult> where TResult : class
	{
		TResult Result { get; }

		int Priority { get; }

		/* Add dependency */

	}
}

