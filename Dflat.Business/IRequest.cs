using System;
using System.Collections.Generic;

namespace Dflat.Business
{
    public interface IRequest
    {
        IResult Result { get; }
    }

	public interface IRequest<TResult> where TResult : class
	{
		TResult Result { get; }

		int Priority { get; }

		/* Add dependency */
        //ICollection<IRequest> ParentJobs { get; }

	}
}

