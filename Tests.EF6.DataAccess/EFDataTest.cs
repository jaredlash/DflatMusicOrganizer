using Dflat.EF6.DataAccess;
using NUnit.Framework;

namespace Tests.EF6.DataAccess
{
    [TestFixture]
    public class EFDataTest
    {
        [SetUp]
        public virtual void TestInitialize()
        {
            using (var db = new DataContext())
            {
                if (db.Database.Exists())
                    db.Database.Delete();

                db.Database.Create();
            }

        }

        [TearDown]
        public virtual void TestCleanup()
        {
            using (var db = new DataContext())
                if (db.Database.Exists())
                    db.Database.Delete();
        }
    }
}
