namespace FTPClient.Client.Models;

/// <summary>
/// Record of directory element (file or subdirectory).
/// </summary>
/// <param name="ElementName">Name of element in directory.</param>
/// <param name="IsDirectory">true -- element is directory, else -- false.</param>
public record DirectoryElement(string ElementName, bool IsDirectory);