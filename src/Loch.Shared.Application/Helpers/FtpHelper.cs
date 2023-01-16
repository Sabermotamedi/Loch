using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Loch.Shared.Application.Helpers;
public static class FtpHelper
{
    public static string CreateFileUploadName(this string fileName, IFormFile file)
    {
        if (file == null) return string.Empty;
        return fileName + "." + file.FileName.Split('.').Last();
    }

    public static string GenerateId()
    {
        var result = DateTime.UtcNow.Year + DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Day + DateTime.UtcNow.Hour + DateTime.UtcNow.Minute + DateTime.UtcNow.Second + DateTime.UtcNow.Millisecond;
        return long.Parse(result).ToString();
    }
}
