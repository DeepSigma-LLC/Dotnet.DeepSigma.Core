using DeepSigma.Core.Extensions;
using System.IO.Compression;
using System.Text;

namespace DeepSigma.Core.Utilities;

/// <summary>
/// Provides utility methods for compressing and decompressing files and directories using ZIP and GZip format.
/// </summary>
/// <remarks>
/// Zip is a popular format that can contain multiple files and directories, while GZip is typically used for compressing single files or data streams.
/// GZip is often used in combination with the TAR format (resulting in .tar.gz files) for archiving multiple files into a single compressed file.
/// GZip is useful for compressing data streams since you can compress data on-the-fly as it is being transmitted or received, which can help reduce latency and improve performance in networked applications.
/// GZip is also commonly used for compressing web content, such as HTML, CSS, and JavaScript files, to reduce bandwidth usage and improve page load times.
/// </remarks>
public static class CompressionUtilities
{

    /// <summary>
    /// Unzips a ZIP archive from a stream to the specified extraction path.
    /// </summary>
    /// <param name="zipStream"></param>
    /// <param name="extractPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="cancellationToken"></param>
    public static async Task UnzipAsync(Stream zipStream, string extractPath, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            string destinationPath = Path.Combine(extractPath, entry.FullName);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

            if (!entry.Name.IsNullOrEmpty())
            {
                await  entry.ExtractToFileAsync(destinationPath, overwrite: overwrite, cancellationToken: cancellationToken);
            }
        }
    }

    /// <summary>
    /// Unzips a ZIP archive from a file path to the specified extraction path.
    /// </summary>
    /// <param name="zipPath"></param>
    /// <param name="extractPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task UnzipDecompressAsync(string zipPath, string extractPath, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(zipPath))
            throw new FileNotFoundException("Zip file not found.", zipPath);

        Directory.CreateDirectory(extractPath);
        await System.IO.Compression.ZipFile.ExtractToDirectoryAsync(zipPath, extractPath, overwriteFiles: overwrite, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Zips the contents of a directory into a ZIP archive at the specified path.
    /// </summary>
    /// <param name="sourceDirectory"></param>
    /// <param name="zipPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    public static async Task ZipDirectoryCompressAsync(string sourceDirectory, string zipPath, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");

        if (File.Exists(zipPath) && !overwrite)
            throw new IOException($"Zip file already exists: {zipPath}");

        await System.IO.Compression.ZipFile.CreateFromDirectoryAsync(sourceDirectory, zipPath, CompressionLevel.Optimal, includeBaseDirectory: false);
    }

    /// <summary>
    /// Zips multiple files into a ZIP archive at the specified path.
    /// </summary>
    /// <param name="filePaths"></param>
    /// <param name="zipPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="IOException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task ZipFilesCompressAsync(IEnumerable<string> filePaths, string zipPath, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        if (File.Exists(zipPath) && !overwrite)
            throw new IOException($"Zip file already exists: {zipPath}");

        using FileStream zipToOpen = new(zipPath, FileMode.Create);
        using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Create);

        foreach (var filePath in filePaths)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            await archive.CreateEntryFromFileAsync(filePath, Path.GetFileName(filePath), CompressionLevel.Optimal, cancellationToken);
        }
    }

    /// <summary>
    /// Zips a single file into a ZIP archive at the specified path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="zipPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    public static async Task ZipFileCompressAsync(string filePath, string zipPath, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");
        if (File.Exists(zipPath) && !overwrite)
            throw new IOException($"Zip file already exists: {zipPath}");


        using FileStream zipToOpen = new(zipPath, FileMode.Create);
        using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Create);

        await archive.CreateEntryFromFileAsync(filePath, Path.GetFileName(filePath), CompressionLevel.Optimal, cancellationToken);
    }

    /// <summary>
    /// Compresses a single file into a GZip file at the specified path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="gzipPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    public static async Task GZipFileCompressAsync(string filePath, string gzipPath, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");
        if (File.Exists(gzipPath) && !overwrite)
            throw new IOException($"GZip file already exists: {gzipPath}");

        using FileStream input = File.OpenRead(filePath);
        using FileStream output = File.Create(gzipPath);
        using GZipStream gzip = new(output, CompressionMode.Compress);

        await input.CopyToAsync(gzip, cancellationToken);
    }

    /// <summary>
    /// Decompresses a GZip file to the specified output path.
    /// </summary>
    /// <param name="gzipPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    public static async Task GZipFileDecompressAsync(string gzipPath, string outputPath, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(gzipPath))
            throw new FileNotFoundException($"GZip file not found: {gzipPath}");
        if (File.Exists(outputPath) && !overwrite)
            throw new IOException($"Output file already exists: {outputPath}");

        using FileStream input = File.OpenRead(gzipPath);
        using GZipStream gzip = new(input, CompressionMode.Decompress);
        using FileStream output = File.Create(outputPath);

        await gzip.CopyToAsync(output, cancellationToken);
    }

    /// <summary>
    /// Compresses a stream into a GZip stream asynchronously.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <param name="cancellationToken"></param>    
    /// <returns></returns>
    public static async Task GzipCompressStreamAsync(Stream input, Stream output, CancellationToken cancellationToken = default)
    {
        using var gzip = new GZipStream(
            output,
            CompressionLevel.Optimal,
            leaveOpen: true);

        await input.CopyToAsync(gzip, cancellationToken);
    }

    /// <summary>
    /// Decompresses a GZip stream into a regular stream asynchronously.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task GunzipDecompressStreamAsync(Stream input, Stream output, CancellationToken cancellationToken = default)
    {
        using var gzip = new GZipStream(
            input,
            CompressionMode.Decompress,
            leaveOpen: true);

        await gzip.CopyToAsync(output, cancellationToken);
    }

    /// <summary>
    /// Compresses a string into a GZip byte array.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static byte[] GZipStringCompress(string text)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(text);
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress, leaveOpen: true))
        {
            gzip.Write(inputBytes, 0, inputBytes.Length);
        }

        return output.ToArray();
    }

    /// <summary>
    /// Decompresses a GZip byte array back into a string.
    /// </summary>
    /// <param name="compressedData"></param>
    /// <returns></returns>
    public static string GZipStringDecompress(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return Encoding.UTF8.GetString(output.ToArray());
    }
}
