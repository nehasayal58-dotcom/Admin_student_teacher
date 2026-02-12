using Admin_Student_Teacher.Models;
using System.IO.Compression;

namespace Admin_Student_Teacher.Data.Services
{
    public class ZipService:IZipService
    {
        public byte[] GenerateUsersZip(
     List<AdminUserVM> users,
     byte[] pdfBytes,
     byte[] excelBytes,
     byte[] csvBytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // Add PDF
                    var pdfEntry = zip.CreateEntry("FilteredUsers.pdf");
                    using (var entryStream = pdfEntry.Open())
                    {
                        entryStream.Write(pdfBytes, 0, pdfBytes.Length);
                    }

                    // Add Excel
                    var excelEntry = zip.CreateEntry("FilteredUsers.xlsx");
                    using (var entryStream = excelEntry.Open())
                    {
                        entryStream.Write(excelBytes, 0, excelBytes.Length);
                    }

                    // Add CSV
                    var csvEntry = zip.CreateEntry("FilteredUsers.csv");
                    using (var entryStream = csvEntry.Open())
                    {
                        entryStream.Write(csvBytes, 0, csvBytes.Length);
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}
