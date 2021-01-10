using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Opifex.Rsvp
{
    public static class AllowedFileExtensions
    {
        public const string Gif = ".gif";
        public const string Jfif = ".jfif";
        public const string Jpe = ".jpe";
        public const string Jpeg = ".jpeg";
        public const string Jpg = ".jpg";
        public const string Png = ".png";
        public const string Pdf = ".pdf";

        private static readonly string[] _all = new[]
        {
            Gif, Jfif, Jpe, Jpeg, Jpg, Png, Pdf
        };

        private static readonly Dictionary<string, List<byte[]>> _fileSignature =
            new Dictionary<string, List<byte[]>>
        {
            { Gif, new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
            { Jfif, new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } } },
            { Jpe, new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } } },
            { Jpeg, new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                }
            },
            { Jpg, new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                }
            },
            { Png, new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            { Pdf, new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } }
        };

        public static void ValidateFile(IFormFile file)
        {
            var ext = GetNormalizedExtension(file);
            if (!_all.Contains(ext))
            {
                throw new Exception("This file extension isn't allowed.");
            }

            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                var signatures = _fileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                if (!signatures.Any(signature =>
                     headerBytes.Take(signature.Length).SequenceEqual(signature)))
                {
                    throw new Exception("The file signature doesn't match the extension.");
                }
            }
        }

        public static string GetNormalizedExtension(IFormFile file) => Path.GetExtension(file.FileName).ToLowerInvariant();

        public static string AcceptAttribute => string.Join(',', _all);
    }
}
