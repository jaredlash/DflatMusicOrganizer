using System;

namespace Dflat.Application.Models;

public record FileResult(string Filename, string Directory, string Extension, long Size, DateTime LastModifiedTime);