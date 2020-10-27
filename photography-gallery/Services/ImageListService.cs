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

        public bool DirectoryExists(string requestedDirectory)
        {
            string pathToCheck = Path.Combine(WebHostEnvironment.WebRootPath, "images/") + System.Net.WebUtility.UrlDecode(requestedDirectory);

            if (Directory.Exists(pathToCheck))
            {
                return true;
            } else
            {
                return false;
            }
        }
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
                if (entry.IndexOf("_400.") == -1 && entry.IndexOf("_800.") == -1 && entry.IndexOf("_1600.") == -1) // TODO: Make this nicer.
                {
                    returnedFileList.Add(CreateListEntry(entry, "file"));
                }
            }
            return returnedFileList;
        }

        static ListEntry CreateListEntry(string entry, string type)
        {
            string relativePath = entry.Split("wwwroot").Last();
            string routablePath = relativePath.Split("images/").Last();
            string fileName = relativePath.Split(".").First();
            string fileExtension = relativePath.Split(".").Last();
            string smallImagePath = fileName + "_400." + fileExtension;
            string mediumPath = fileName + "_800." + fileExtension;
            string largePath = fileName + "_1600." + fileExtension;
            string displayName = entry.Split("/").Last();

            string imageDimensions = @RedisDatabaseClass.RedisDatabase.HashGet(displayName, "Dimensions").ToString();
            string imageWidth = imageDimensions.Split(",")[0];
            string imageHeight = imageDimensions.Split(",")[1];

            return new ListEntry(entry, relativePath, routablePath, smallImagePath, mediumPath, largePath, displayName, imageWidth, imageHeight, type);
        }
    }
}
