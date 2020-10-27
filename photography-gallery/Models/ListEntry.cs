using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace photography_gallery.Models
{
    public class ListEntry
    {
        private string fullPath;
        private string relativePath;
        private string routablePath;
        private string smallImagePath;
        private string mediumImagePath;
        private string largeImagePath;
        private string displayName;
        private string imageWidth;
        private string imageHeight;
        private string type;

        public ListEntry(string fullPath, string relativePath, string routablePath, string smallImagePath, string mediumImagePath, string largeImagePath, string displayName, string imageWidth, string imageHeight, string type)
        {
            this.fullPath = fullPath;
            this.relativePath = relativePath;
            this.routablePath = System.Net.WebUtility.UrlEncode(routablePath);
            this.smallImagePath = smallImagePath.Replace(" ", "%20");
            this.mediumImagePath = mediumImagePath.Replace(" ", "%20");
            this.largeImagePath = largeImagePath.Replace(" ", "%20");
            this.displayName = displayName;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            this.type = type;
        }

        public string FullPath
        {
            get { return fullPath; }
            set { fullPath = value; }
        }

        public string RelativePath
        {
            get { return relativePath; }
            set { relativePath = value; }
        }
        public string RoutablePath
        {
            get { return routablePath; }
            set { routablePath = value; }
        }
        public string SmallImagePath
        {
            get { return smallImagePath; }
            set { smallImagePath = value; }
        }
        public string MediumImagePath
        {
            get { return mediumImagePath; }
            set { mediumImagePath = value; }
        }
        public string LargeImagePath
        {
            get { return largeImagePath; }
            set { largeImagePath = value; }
        }
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        public string ImageWidth
        {
            get { return imageWidth; }
            set { imageWidth = value; }
        }
        public string ImageHeight
        {
            get { return imageHeight; }
            set { imageHeight = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
