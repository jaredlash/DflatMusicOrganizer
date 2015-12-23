using System;
using System.Collections.Generic;

namespace Domain.Jobs
{
	public class PriorityQueue<TPriority, TItem> : IPriorityQueue<TPriority, TItem>
		where TPriority : IComparable
		where TItem : class
	{
		private List<Tuple<TPriority, TItem>> heap = null;

		public PriorityQueue ()
		{
			heap = new List<Tuple<TPriority, TItem>> ();
		}

		public void Add(TPriority priority, TItem item)
		{
			int child = heap.Count;

			heap.Add (new Tuple<TPriority, TItem>(priority, item));
			while (child > 0)
			{
				int parent = (child - 1) >> 1;
				if (heap[parent].Item1.CompareTo(heap[child].Item1) < 0)
					_Swap (parent, child);

				child = parent;
			}
		}

		public TItem TryRemove()
		{
			TItem item = null;

			if (heap.Count == 0)
				return null;

			int parent = 0;
			item = heap [0].Item2;

			int last = heap.Count - 1;

			heap [0] = heap [last];
			heap.RemoveAt (last);
			last--;

			while (true)
			{
				int left = parent * 2 + 1;
				int right = left + 1;
				int child;

				// If the left child would be after the end of the queue
				// then the right child is too and we are done
				if (left > last)
					break;

				if (right > last) {
					child = left;
				} else {
					if (heap [left].Item1.CompareTo (heap [right].Item1) > 0)
						child = left;
					else
						child = right;
				}

				if (heap [child].Item1.CompareTo (heap [parent].Item1) > 0)
					_Swap (child, parent);
				else
					break;

				parent = child;

			}

			return item;
		}

		public TItem Remove()
		{
			var item = TryRemove ();

			if (item == null)
				throw new InvalidOperationException ("PriorityQueue is empty!");
			return item;
		}

		public int Count
		{
			get
			{
				return heap.Count;
			}
		}

		public TItem Peek()
		{
			if (heap.Count > 0)
				return heap [0].Item2;

			return null;
		}

		public override string ToString ()
		{
			return string.Format ("[PriorityQueue: Count={0}] {1}", Count, heap.ToString());
		}

		public void Clear()
		{
			heap.Clear ();
		}

		private void _Swap(int first, int second)
		{
			var temp = heap[first];
			heap[first] = heap[second];
			heap[second] = temp;
		}
	}
}

