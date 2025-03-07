﻿using ImageMagick;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace photography_gallery.Services
{
    public class ImageConversionService
    {
        private readonly int[] ImageSizes = { 400, 800, 1600 };
        private readonly string DirectorySeparator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";

        private readonly string InputDirectory;
        private readonly string OutputDirectory;
        private readonly IDatabase RedisDatabase;

        public ImageConversionService()
        {
        }

        public ImageConversionService(string inputDirectory, string outputDirectory, IDatabase redisDatabase)
        {
            InputDirectory = inputDirectory;
            OutputDirectory = outputDirectory;
            RedisDatabase = redisDatabase;
        }

        public void ProcessImages()
        {
            string[] fileList = Directory.GetFiles(InputDirectory, "*.jpg", SearchOption.AllDirectories);
            if (fileList.Length == 0)
            {
                Console.WriteLine("No .jpg files present in input directory (" + InputDirectory + ")");
                Environment.Exit(-1);
            }
            var opts = new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 1.0)) };
            Parallel.ForEach(fileList, opts, (imagePath) =>
            {
                string uploadedImageFileName = imagePath.Split(".").First().Split(DirectorySeparator).Last();
                string uploadedImageExtension = imagePath.Split(".").Last();
                string targetDirectory = OutputDirectory + GetRelativeImageDirectory(InputDirectory, imagePath);
                string potentialExistingImage = OutputDirectory + imagePath.Split(InputDirectory)[1];

                ExtractImageMetadata(imagePath, uploadedImageFileName, uploadedImageExtension);

                if (File.Exists(potentialExistingImage))
                {
                    if (ImagesAreDifferent(imagePath, potentialExistingImage))
                    {
                        foreach (int size in ImageSizes)
                        {
                            Console.WriteLine("Converting image " + imagePath + " to " + size + "px");
                            ResizeImage(imagePath, size, uploadedImageFileName, targetDirectory);
                        }
                        // And copy over original image
                        File.Copy(imagePath, targetDirectory + DirectorySeparator + uploadedImageFileName + "." + uploadedImageExtension, true);
                    } 
                }
                else
                {
                    foreach (int size in ImageSizes)
                    {
                        Console.WriteLine("Converting image " + imagePath + " to " + size + "px");
                        ResizeImage(imagePath, size, uploadedImageFileName, targetDirectory);
                    }
                    // And copy over original image
                    File.Copy(imagePath, targetDirectory + DirectorySeparator + uploadedImageFileName + "." + uploadedImageExtension, true);
                }

            });
        }

        public bool ImagesAreDifferent(string imagePath, string potentialExistingImage)
        {
            FileInfo inputImage = new FileInfo(imagePath);
            FileInfo existingImage = new FileInfo(potentialExistingImage);

            if (inputImage.Length != existingImage.Length)
            {
                return true;
            }
            else if (!File.ReadAllBytes(inputImage.FullName).SequenceEqual(File.ReadAllBytes(existingImage.FullName)))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Unchanged image found: " + imagePath);
                return false;
            }

        }

        private void ExtractImageMetadata(string imagePath, string uploadedImageFileName, string uploadedImageExtension)
        {
            using (MagickImage image = new MagickImage(imagePath))
            {
                // IExifProfile metadata = image.GetExifProfile();
                string redisReference = uploadedImageFileName + "." + uploadedImageExtension;
                RedisDatabase.HashSet(redisReference, new HashEntry[] {
                    // new HashEntry("Model",metadata.GetValue(ExifTag.Model).ToString()),
                    // new HashEntry("LensModel",metadata.GetValue(ExifTag.LensModel).ToString()),
                    // new HashEntry("FNumber",FixFNumber(metadata.GetValue(ExifTag.FNumber).ToString())),
                    // new HashEntry("FocalLength",metadata.GetValue(ExifTag.FocalLength).ToString()),
                    // new HashEntry("ExposureTime",metadata.GetValue(ExifTag.ExposureTime).ToString()),
                    new HashEntry("Height",image.Height),
                    new HashEntry("Width",image.Width),
                    new HashEntry("Dimensions",image.Width.ToString() + "," + image.Height.ToString()),
                    new HashEntry("AspectRatio",GetImageRatio(image.Width, image.Height))
                });
            }
        }

        private void ResizeImage(string imagePath, int newWidth, string uploadedImageFileName, string uploadedImageDirectory)
        {
            Console.WriteLine("Resizing " + uploadedImageFileName + " to " + newWidth + " pixels wide");
            Directory.CreateDirectory(uploadedImageDirectory);

            using (MagickImage image = new MagickImage(imagePath))
            {
                image.Resize(newWidth, Convert.ToInt32(newWidth * GetImageRatio(image.Width, image.Height)));
                image.Strip();
                int imageQuality;
                if (newWidth < 500)
                {
                    imageQuality = 75;
                }
                else
                {
                    imageQuality = 65;
                }
                image.Quality = imageQuality;
                image.Format = MagickFormat.Pjpeg;

                image.Write(uploadedImageDirectory + DirectorySeparator + uploadedImageFileName + "_" + newWidth.ToString() + ".jpg");
            }
        }

        public string FixFNumber(string input)
        {
            string[] splitInput = input.Split("/");
            if (splitInput.Length == 1)
            {
                return input;
            }
            else
            {
                return (float.Parse(splitInput[0]) / 10).ToString();
            }
        }

        private string GetRelativeImageDirectory(string inputDirectory, string imagePath)
        {
            string[] splitImagePath = imagePath.Split(inputDirectory).Last().Split(DirectorySeparator);
            Array.Resize(ref splitImagePath, splitImagePath.Length - 1);
            return string.Join(DirectorySeparator, splitImagePath);
        }

        public float GetImageRatio(int width, int height)
        {
            float w = width;
            float h = height;
            return h / w;
        }
    }
}