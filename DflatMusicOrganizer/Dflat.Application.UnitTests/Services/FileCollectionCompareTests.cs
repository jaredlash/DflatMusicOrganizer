using Dflat.Application.Models;
using Dflat.Application.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dflat.Application.UnitTests.Services;

[TestClass]
public class FileCollectionCompareTests
{
    private FileCollectionCompare comparer = new FileCollectionCompare();
    
    private List<File> group1 = [];   // 3 files
    private List<File> group2 = [];   // 5 files
    private List<File> group3 = [];   // 7 files
    private List<File> group1modified = []; // files from group1: first file different size, second file, different time, third file both different

    [TestInitialize]
    public void Initialize()
    {
        DateTime startTime = DateTime.Now.AddDays(-40);
        group1 = new List<File>()
        {
            new File
            {
                Filename = @"Z:\dir1\file1.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir1",
                Size = 123456,
                LastModifiedTime = startTime
            },
            new File
            {
                Filename = @"Z:\dir2\file2.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir2",
                Size = 234567,
                LastModifiedTime = startTime.AddDays(1)
            },
            new File
            {
                Filename = @"Z:\dir3\file3.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir3",
                Size = 345678,
                LastModifiedTime = startTime.AddDays(2)
            }
        };
        group2 = new List<File>()
        {
            new File
            {
                Filename = @"Z:\dir4\file4.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir4",
                Size = 456789,
                LastModifiedTime = startTime.AddDays(3)
            },
            new File
            {
                Filename = @"Z:\dir5\file5.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir5",
                Size = 567890,
                LastModifiedTime = startTime.AddDays(4)
            },
            new File
            {
                Filename = @"Z:\dir6\file6.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir6",
                Size = 678901,
                LastModifiedTime = startTime.AddDays(5)
            },
            new File
            {
                Filename = @"Z:\dir7\file7.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir7",
                Size = 789012,
                LastModifiedTime = startTime.AddDays(6)
            },
            new File
            {
                Filename = @"Z:\dir8\file8.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir8",
                Size = 890123,
                LastModifiedTime = startTime.AddDays(7)
            }
        };
        group3 = new List<File>()
        {
            new File
            {
                Filename = @"Z:\dir9\file9.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir9",
                Size = 901234,
                LastModifiedTime = startTime.AddDays(8)
            },
            new File
            {
                Filename = @"Z:\dir10\file10.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir10",
                Size = 1012345,
                LastModifiedTime = startTime.AddDays(9)
            },
            new File
            {
                Filename = @"Z:\dir11\file11.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir11",
                Size = 123456,
                LastModifiedTime = startTime.AddDays(10)
            },
             new File
            {
                Filename = @"Z:\dir12\file12.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir12",
                Size = 234567,
                LastModifiedTime = startTime.AddDays(11)
            },
            new File
            {
                Filename = @"Z:\dir13\file13.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir13",
                Size = 345678,
                LastModifiedTime = startTime.AddDays(12)
            },
            new File
            {
                Filename = @"Z:\dir14\file14.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir14",
                Size = 456789,
                LastModifiedTime = startTime.AddDays(13)
            },
            new File
            {
                Filename = @"Z:\dir15\file15.mp3",
                Extension = ".MP3",
                Directory = @"Z:\dir15",
                Size = 567890,
                LastModifiedTime = startTime.AddDays(14)
            }
        };
        // Contains copies of group1 files with different properties modified
        group1modified = new List<File>();
        group1modified.AddRange(group1.Select(x =>
            new File
            {
                Filename = x.Filename,
                Extension = x.Extension,
                Directory = x.Directory,
                Size = x.Size,
                LastModifiedTime = x.LastModifiedTime
            }).ToList());
        // first file different size
        group1modified[0].Size += 128;
        // second file, different time
        group1modified[1].LastModifiedTime = group1modified[1].LastModifiedTime.AddDays(3);
        //third file, both size and time are different
        group1modified[2].Size += 256;
        group1modified[2].LastModifiedTime = group1modified[2].LastModifiedTime.AddDays(5);
    }

    [TestMethod]
    public void Compare_ReturnsEmptyResult_WhenBeforeAndAfterAreEmpty()
    {
        var before = new List<File>();
        var after = new List<File>();

        var result = comparer.Compare(before, after);

        Assert.AreEqual(0, result.Added.Count);
        Assert.AreEqual(0, result.Removed.Count);
        Assert.AreEqual(0, result.Modified.Count);
    }

    [TestMethod]
    public void Compare_ReturnsEmptyResult_WhenBeforeAndAfterAreSame()
    {
        var before = group3;
        var after = group3;

        var result = comparer.Compare(before, after);

        Assert.AreEqual(0, result.Added.Count);
        Assert.AreEqual(0, result.Removed.Count);
        Assert.AreEqual(0, result.Modified.Count);
    }

    [TestMethod]
    public void Compare_ReturnsThreeAddedFiles_WhenBeforeEmptyAndAfterHasThree()
    {
        var before = new List<File>();
        var after = group1;

        var result = comparer.Compare(before, after);

        Assert.AreEqual(3, result.Added.Count);
        Assert.AreEqual(0, result.Removed.Count);
        Assert.AreEqual(0, result.Modified.Count);
    }

    [TestMethod]
    public void Compare_ReturnsFiveAddedFiles_WhenBeforeHasThreeDifferentFilesAfterHasBeforeAndFiveNew()
    {
        var before = group1;
        var after = new List<File>();
        after.AddRange(before);
        after.AddRange(group2);

        var result = comparer.Compare(before, after);

        Assert.AreEqual(5, result.Added.Count);
        Assert.AreEqual(0, result.Removed.Count);
        Assert.AreEqual(0, result.Modified.Count);
    }

    [TestMethod]
    public void Compare_ReturnsThreeModifiedFiles_WhenBeforeHasThreeAfterHasSameButModified()
    {
        var before = group1;
        var after = group1modified;

        var result = comparer.Compare(before, after);

        Assert.AreEqual(0, result.Added.Count);
        Assert.AreEqual(0, result.Removed.Count);
        Assert.AreEqual(3, result.Modified.Count);
    }

    [TestMethod]
    public void Compare_ReturnsThreeRemoved_WhenBeforeHasThreeAfterHasNone()
    {
        var before = group1;
        var after = new List<File>();

        var result = comparer.Compare(before, after);

        Assert.AreEqual(0, result.Added.Count);
        Assert.AreEqual(3, result.Removed.Count);
        Assert.AreEqual(0, result.Modified.Count);
    }

    [TestMethod]
    public void Compare_ReturnsFiveAddedThreeRemoved_WhenBeforeContainsGroup1AfterContainsGroup2()
    {
        var before = group1;
        var after = group2;

        var result = comparer.Compare(before, after);

        Assert.AreEqual(5, result.Added.Count);
        Assert.AreEqual(3, result.Removed.Count);
        Assert.AreEqual(0, result.Modified.Count);
    }

    [TestMethod]
    public void Compare_ReturnsFiveAddedThreeModified_WhenBeforeContainsGroup1AfterContainsGroup2AndGroup1Modified()
    {
        var before = group1;
        var after = new List<File>();
        after.AddRange(group2);
        after.AddRange(group1modified);

        var result = comparer.Compare(before, after);

        Assert.AreEqual(5, result.Added.Count);
        Assert.AreEqual(0, result.Removed.Count);
        Assert.AreEqual(3, result.Modified.Count);
    }

    [TestMethod]
    public void Compare_ReturnsFiveRemovedThreeModified_WhenBeforeContainsGroup1AndGroup2AfterContainsGroup1Modified()
    {
        var before = new List<File>();
        before.AddRange(group1);
        before.AddRange(group2);
        var after = group1modified;

        var result = comparer.Compare(before, after);

        Assert.AreEqual(0, result.Added.Count);
        Assert.AreEqual(5, result.Removed.Count);
        Assert.AreEqual(3, result.Modified.Count);
    }

    [TestMethod]
    public void Compare_Returns5Added7Removed3Modified_WhenBeforeContainsGroup1AndGroup3AfterContainsGroup2AndGroup1Modified()
    {
        var before = new List<File>();
        before.AddRange(group1);
        before.AddRange(group3);
        var after = new List<File>();
        after.AddRange(group2);
        after.AddRange(group1modified);

        var result = comparer.Compare(before, after);

        Assert.AreEqual(5, result.Added.Count);
        Assert.AreEqual(7, result.Removed.Count);
        Assert.AreEqual(3, result.Modified.Count);
    }
}
