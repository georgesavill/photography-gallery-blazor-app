using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using photography_gallery.Models;

namespace photography_gallery.Services
{
    public class ImageListService
    {
        public ImageListService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public List<ListEntry> GetDirectoryList(string subDirectory)
        {
            List<ListEntry> returnedDirectoryList = new List<ListEntry>();
            string rootDirectory = Path.Combine(WebHostEnvironment.WebRootPath, "images") + subDirectory;
            string[] directoryList = Directory.GetDirectories(rootDirectory, "*.*", SearchOption.TopDirectoryOnly);
            Array.Sort(directoryList);
            foreach (string entry in directoryList)
            {
                returnedDirectoryList.Add(CreateListEntry(entry, "dir"));
            }
            return returnedDirectoryList;
        }
        public List<ListEntry> GetFileList(string subDirectory)
        {
            List<ListEntry> returnedFileList = new List<ListEntry>();
            string rootDirectory = Path.Combine(WebHostEnvironment.WebRootPath, "images") + subDirectory;
            string[] fileList = Directory.GetFiles(rootDirectory, "*.jpg", SearchOption.TopDirectoryOnly);
            Array.Sort(fileList);
            foreach (string entry in fileList)
            {
                if (entry.IndexOf("_preview.") == -1 && entry.IndexOf("_thumbnail.") == -1)
                {
                    returnedFileList.Add(CreateListEntry(entry, "file"));
                }
            }
            return returnedFileList;
        }

        static ListEntry CreateListEntry(string entry, string type)
        {
            string relativePath = entry.Split("wwwroot").Last();
            string fileName = relativePath.Split(".").First();
            string fileExtension = relativePath.Split(".").Last();
            string thumnailPath = fileName + "_thumbnail." + fileExtension;
            string previewPath = fileName + "_preview." + fileExtension;

            return new ListEntry(entry, relativePath, thumnailPath, previewPath, entry.Split("/").Last(), type);
        }
    }
}
