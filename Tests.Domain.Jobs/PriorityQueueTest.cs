using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Jobs;

using NUnit.Framework;

namespace Tests.Domain.Jobs
{
	[TestFixture]
	public class PriorityQueueTest
	{
		private List<Tuple<int, String>> data;

		public PriorityQueueTest ()
		{
		}

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			data = new List<Tuple<int, String>> () {
				new Tuple<int, string>(1, "one"),
				new Tuple<int, string>(2, "two"),
				new Tuple<int, string>(3, "three"),
				new Tuple<int, string>(4, "four"),
				new Tuple<int, string>(5, "five"),
				new Tuple<int, string>(6, "six"),
				new Tuple<int, string>(7, "seven"),
				new Tuple<int, string>(8, "eight"),
				new Tuple<int, string>(9, "nine"),
				new Tuple<int, string>(10, "ten")
			};
		}

		[Test]
		public void TestNewEmptyQueue()
		{
			var q = new PriorityQueue<int, string> ();
			Assert.AreEqual (0, q.Count);
		}

		[Test]
		public void TestAddOne()
		{
			var q = new PriorityQueue<int, string> ();

			q.Add (data[0].Item1, data[0].Item2);

			Assert.AreEqual (1, q.Count);
		}

		[Test]
		public void TestAddFour()
		{
			var q = new PriorityQueue<int, string> ();

			for (int i = 0; i < 4; i++)
				q.Add (data [i].Item1, data[i].Item2);

			Assert.AreEqual (4, q.Count);
		}

		[Test]
		public void TestAddFourAndEmpty()
		{
			var q = new PriorityQueue<int, string> ();

			for (int i = 0; i < 4; i++)
				q.Add (data [i].Item1, data[i].Item2);

			q.Clear ();

			Assert.AreEqual (0, q.Count);
		}

		[Test]
		public void TestRemoveFromEmptyThrowsInvalidOperationException()
		{
			var q = new PriorityQueue<int, String> ();

			Assert.That(() => q.Remove (), Throws.InvalidOperationException);
		}

		[Test]
		public void TestTryRemoveFromEmpty()
		{
			var q = new PriorityQueue<int, String> ();

			var item = q.TryRemove ();

			Assert.IsNull (item);
		}

		[Test]
		public void TestAddTenRemoveTen()
		{
			var q = new PriorityQueue<int, String> ();
			var expected = (from t in data select t.Item2).ToList ().ToList ();
			expected.Reverse ();
			var actual = new List<String> ();

			// Populate the queue in order (1, 2, 3, ...)
			foreach (var t in data) {
				q.Add (t.Item1, t.Item2);
			}

			// Items should be received in reverse order (10, 9, 8, ...)
			while (q.Count > 0)
				actual.Add (q.Remove ());

			
			Assert.AreEqual (actual.Count, expected.Count);
			for (int i = 0; i < actual.Count; i++)
			{
				Assert.AreEqual (expected [i], actual [i]);
			}
		}

		[Test]
		public void TestAddTwentyRemoveTwentyWithDups()
		{
			var q = new PriorityQueue<int, String> ();
			var expected = new List<String> ();
			foreach (var t in data)
			{
				expected.Add (t.Item2);
				expected.Add (t.Item2);
			}
			expected.Reverse ();
			var actual = new List<String> ();

			// Populate the queue in order (1, 2, 3, ..., 10, 1, 2, 3, ..., 10)
			foreach (var t in data) {
				q.Add (t.Item1, t.Item2);
			}
			foreach (var t in data) {
				q.Add (t.Item1, t.Item2);
			}


			// Items should be received in reverse, sorted order (10, 10, 9, 9, 8, 8, ...)
			while (q.Count > 0)
				actual.Add (q.Remove ());


			Assert.AreEqual (actual.Count, expected.Count);
			for (int i = 0; i < actual.Count; i++)
			{
				Assert.AreEqual (expected [i], actual [i]);
			}
		}
	}
}

