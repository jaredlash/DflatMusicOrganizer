using Moq;
using Dflat.Business;
using NUnit.Framework;

namespace Dflat.ViewModels.Tests
{
    [TestFixture]
    public class FileSourceManagerViewModelTests
    {
        [Test]
        public void FileSourceManagerViewModelTest_CloseCommandDisposesUnitOfWork()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var fsmvm = new FileSourceManagerViewModel(mockUnitOfWork.Object);

            fsmvm.CloseCommand.Execute(null);

            mockUnitOfWork.Verify(m => m.Dispose(), Times.Once());
        }
    }
}