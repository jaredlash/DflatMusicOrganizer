using System.Collections.Generic;
using Dflat.Business;
using Moq;
using Dflat.Business.Models;
using Dflat.Business.Repositories;
using NUnit.Framework;

namespace Tests.Business.Integration_Tests
{
    [TestFixture]
	public class FileSourceFolderTest
	{
        List<IFileSourceFolder> dummyRepo;
        IFileSourceFolderRepository fileSourceFolderRepository;
        IUnitOfWork unitOfWork;

        [SetUp]
        public void TestInitialize()
        {
            dummyRepo = new List<IFileSourceFolder>();

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            

            var mockFileSourceFolderRepository = new Mock<IFileSourceFolderRepository>();
            mockFileSourceFolderRepository.Setup(m => m.Create()).Returns(new FileSourceFolder());
            
            fileSourceFolderRepository = mockFileSourceFolderRepository.Object;

            mockUnitOfWork.SetupGet(m => m.IFileSourceFolderRepository).Returns(fileSourceFolderRepository);

            unitOfWork = mockUnitOfWork.Object;

        }

		public FileSourceFolderTest ()
		{
		}


		[Test]
		public void TestCreateFromRepository()
		{
            IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
            

			Assert.IsNotNull(fileSourceFolder);
		}


        /*
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
			var q = new PriorityQueue<int, string> ();

			Assert.That(() => q.Remove (), Throws.InvalidOperationException);
		}

		[Test]
		public void TestTryRemoveFromEmpty()
		{
			var q = new PriorityQueue<int, string> ();

			var item = q.TryRemove ();

			Assert.IsNull (item);
		}

		[Test]
		public void TestAddTenRemoveTen()
		{
			var q = new PriorityQueue<int, string> ();
			var expected = (from t in data select t.Item2).ToList ().ToList ();
			expected.Reverse ();
			var actual = new List<string> ();

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
			var q = new PriorityQueue<int, string> ();
			var expected = new List<string> ();
			foreach (var t in data)
			{
				expected.Add (t.Item2);
				expected.Add (t.Item2);
			}
			expected.Reverse ();
			var actual = new List<string> ();

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
		}*/
	}
}

