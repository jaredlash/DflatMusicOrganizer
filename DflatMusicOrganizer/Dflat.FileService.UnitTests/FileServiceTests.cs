using FileService;
using FileService.Infrastructure;
using FileService.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.FileService.UnitTests;

internal class FileServiceTests
{
    private Mock<IFileSystem> _mockFileSystem;
    private DirectoryService _directoryService;
    private const string BaseDirectory = "C:\\BaseDirectory";

    [SetUp]
    public void SetUp()
    {
        _mockFileSystem = new Mock<IFileSystem>();
        _directoryService = new DirectoryService(BaseDirectory, _mockFileSystem.Object);
    }

    [Test]
    public void ListDirectoryContents_WithNullRelativePath_ReturnsContentsOfBaseDirectory()
    {
        // Arrange
        var fileSystemEntries = new List<string> { "file1.txt", "subdir" };
        _mockFileSystem.Setup(fs => fs.DirectoryExists(BaseDirectory)).Returns(true);
        _mockFileSystem.Setup(fs => fs.DirectoryExists(Path.Combine(BaseDirectory, "subdir"))).Returns(true);
        _mockFileSystem.Setup(fs => fs.EnumerateFileSystemEntries(BaseDirectory)).Returns(fileSystemEntries);

        // Act
        var result = _directoryService.ListDirectoryContents(null);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Count(), Is.EqualTo(2));
        Assert.That(result.Value.Any(entry => entry.Name == "file1.txt" && entry.Type == "File"), Is.True);
        Assert.That(result.Value.Any(entry => entry.Name == "subdir" && entry.Type == "Directory"), Is.True);
    }

    [Test]
    public void ListDirectoryContents_WithInvalidPath_ReturnsFailureResult()
    {
        // Arrange
        var relativePath = new RelativePath(["..", "/"]);

        // Act
        var result = _directoryService.ListDirectoryContents(relativePath);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Invalid path."));
    }

    [Test]
    public void ListDirectoryContents_WithNonExistentDirectory_ReturnsFailureResult()
    {
        // Arrange
        var relativePath = new RelativePath(["nonexistent"]);
        var targetDirectory = Path.Combine(BaseDirectory, "nonexistent");
        _mockFileSystem.Setup(fs => fs.DirectoryExists(targetDirectory)).Returns(false);

        // Act
        var result = _directoryService.ListDirectoryContents(relativePath);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Directory not found."));
    }

    [Test]
    public void ListDirectoryContents_WithValidRelativePath_ReturnsContentsOfTargetDirectory()
    {
        // Arrange
        var relativePath = new RelativePath(["subdir"]);
        var targetDirectory = Path.Combine(BaseDirectory, "subdir");
        var fileSystemEntries = new List<string> { "file2.txt", "subsubdir" };
        _mockFileSystem.Setup(fs => fs.DirectoryExists(targetDirectory)).Returns(true);
        _mockFileSystem.Setup(fs => fs.DirectoryExists(Path.Combine(targetDirectory, "subsubdir"))).Returns(true);
        _mockFileSystem.Setup(fs => fs.EnumerateFileSystemEntries(targetDirectory)).Returns(fileSystemEntries);

        // Act
        var result = _directoryService.ListDirectoryContents(relativePath);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Count(), Is.EqualTo(2));
        Assert.That(result.Value.Any(entry => entry.Name == "file2.txt" && entry.Type == "File"), Is.True);
        Assert.That(result.Value.Any(entry => entry.Name == "subsubdir" && entry.Type == "Directory"), Is.True);
    }
}
