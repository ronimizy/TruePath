// SPDX-FileCopyrightText: 2024 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace TruePath.Tests;

public class AbsolutePathTests
{
    [Fact]
    public void PathIsNormalizedOnCreation()
    {
        if (!OperatingSystem.IsWindows()) return;

        const string path = @"C:/Users/John Doe\Documents";
        var absolutePath = new AbsolutePath(path);
        Assert.Equal(@"C:\Users\John Doe\Documents", absolutePath.Value);
    }

    [Fact]
    public void ConstructorThrowsOnNonRootedPath()
    {
        const string path = "uprooted";
        const string expectedMessage = $"Path \"{path}\" is not absolute.";
        var ex = Assert.Throws<ArgumentException>(() => new AbsolutePath(path));
        Assert.Equal(expectedMessage, ex.Message);

        var localPath = new LocalPath("uprooted");
        ex = Assert.Throws<ArgumentException>(() => new AbsolutePath(localPath));
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Theory]
    [InlineData("/etc/bin", "/usr/bin", "../../usr/bin")]
    [InlineData("/usr/bin/log", "/usr/bin", "..")]
    [InlineData("/usr/bin", "/usr/bin/log", "log")]
    public void RelativeToReturnsCorrectRelativePath(string from, string to, string expected)
    {
        if (OperatingSystem.IsWindows()) return;

        var fromPath = new AbsolutePath(from);
        var toPath = new AbsolutePath(to);

        LocalPath relativePath = toPath.RelativeTo(fromPath);

        Assert.Equal(expected, relativePath.Value);
    }

    [Theory]
    [InlineData(@"C:\bin", @"D:\bin", @"D:\bin")]
    [InlineData(@"C:\bin\debug", @"C:\bin", "..")]
    [InlineData(@"C:\bin", @"C:\bin\log", "log")]
    public void RelativeToReturnsCorrectRelativePathForWindows(string from, string to, string expected)
    {
        if (OperatingSystem.IsWindows() is false) return;

        var fromPath = new AbsolutePath(from);
        var toPath = new AbsolutePath(to);

        LocalPath relativePath = toPath.RelativeTo(fromPath);

        Assert.Equal(expected, relativePath.Value);
    }
}
