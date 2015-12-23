using System;

namespace Domain.Jobs
{
	public interface IPriorityQueue<TPriority, TItem> where TPriority : IComparable
		where TItem : class
	{
		void Add(TPriority priority, TItem item);

		TItem TryRemove();

		TItem Remove();

		int Count { get; }

		TItem Peek();

		void Clear();
	}
}

