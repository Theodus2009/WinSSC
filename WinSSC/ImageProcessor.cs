using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace WinSSC
{
    /// <summary>
    /// The ImageProcessor is responsible for resizing images to fit a variety of sizes, allowing various content
    /// pages in the site to reference images of an appropriate size.
    /// </summary>
    public class ImageProcessor
    {
        private const string PATHS_FILE = "ImagePaths.txt";
        private const string SIZES_FILE = "ImageSizes.txt";
        private static string[] EXTENSIONS = { ".jpg", ".jpeg", ".bmp", ".gif", ".png" };
        private static byte JPEG_QUALITY = 95;
        private string sourceDirectory = null, outputDirectory = null;
        private IList<imageSizeInfo> imageSizes;

        private bool configValid = false;

        public ImageProcessor(string configDirectory)
        {
            imageSizes = new List<imageSizeInfo>();
            string pathsFilePath = configDirectory + PATHS_FILE;
            if(File.Exists(pathsFilePath))
            {
                string[] fileLines = File.ReadAllLines(pathsFilePath);
                loadPathsFromConfig(fileLines);
                if(sourceDirectory == null || outputDirectory == null)
                {
                    Logger.LogError("Either source or destination directories were not specified in image path file (" + PATHS_FILE + "). Image processing will be skipped.");
                    return;
                }
            }
            else
            {
                Logger.LogError("Could not find " + pathsFilePath + ". Image processing will be skipped.");
            }

            string sizesFilePath = configDirectory + SIZES_FILE;
            if(File.Exists(pathsFilePath))
            {
                string[] fileLines = File.ReadAllLines(sizesFilePath);
                loadSizesFromFile(fileLines);
                if(sourceDirectory == null || outputDirectory == null)
                {
                    Logger.LogError("Either source or destination directories were not specified in image path file (" + SIZES_FILE + "). Image processing will be skipped.");
                    return;
                }
            }
            else
            {
                Logger.LogError("Could not find " + sizesFilePath + ". Image processing will be skipped.");
            }
            configValid = true;
        }

        private void loadPathsFromConfig(string[] fileLines)
        {
            foreach (string l in fileLines)
            {
                //Ignore whitspace lines
                if (l.Trim().Length == 0) continue;
                //Ignore comment lines
                if (l.Trim().StartsWith("#")) continue;
                string[] parts = l.Trim().Split('=');
                if (parts.Length < 2)
                {
                    Logger.LogWarning("Ignoring " + l + " in " + PATHS_FILE);
                    continue;
                }
                if (!parts[1].EndsWith("\\")) parts[1] += '\\';
                switch (parts[0].ToLower())
                {
                    case "source":
                        if(Directory.Exists(parts[1]))
                        {
                            sourceDirectory = parts[1];
                        }
                        else
                        {
                            Logger.LogError("Image paths file specified non-existent directory " + parts[1]);
                        }
                        break;
                    case "destination":
                        if(!Directory.Exists(parts[1]))
                        {
                            try 
	                        {	        
		                        Directory.CreateDirectory(parts[1]);
	                        }
	                        catch (System.IO.IOException ex)
	                        {
                                Logger.LogError("Could not create image output directory " + parts[1]);
		                        break;
	                        }
                        }
                        outputDirectory = parts[1];
                        break;
                }
            }
        }

        private void loadSizesFromFile(string[] fileLines)
        {
            foreach (string l in fileLines)
            {
                //Ignore whitspace lines
                if (l.Trim().Length == 0) continue;
                //Ignore comment lines
                if (l.Trim().StartsWith("#")) continue;
                string[] parts = l.Trim().Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 1)
                {
                    Logger.LogWarning("Ignoring " + l + " in " + SIZES_FILE);
                    continue;
                }
                if (parts.Length == 0) continue;

                Size imageSize;
                ScaleOperation operation = ScaleOperation.Scale;
                string suffix = null;
                byte imageQuality = JPEG_QUALITY;

                string[] sizeOperationParts = parts[0].Split(':');

                if (sizeOperationParts.Length > 1)
                {
                    if(sizeOperationParts[1].ToLower() == "crop")
                    {
                        operation = ScaleOperation.Crop;
                    }
                    else
                    {
                        Logger.LogWarning("Unknown image processing option " + sizeOperationParts[0] + " in " + SIZES_FILE);
                    }
                }

                string[] dimensions = sizeOperationParts[0].Split('x');
                if(dimensions.Length == 2)
                {
                    int width, height;
                    if(!int.TryParse(dimensions[0], out width) || !int.TryParse(dimensions[1], out height))
                    {
                        Logger.LogWarning("The value " + parts[0] + " in " + SIZES_FILE + " could not recognised as an image size. Expected <integer>x<integer>");
                        continue;
                    }
                    if(width < 1 || height < 1)
                    {
                        Logger.LogWarning("Both width and height must be positive integers - " + parts[0] + " in " + SIZES_FILE);
                        continue;
                    }
                    imageSize = new Size() { Width = width, Height = height };
                }
                else
                {
                    Logger.LogWarning("The value " + parts[0] + " in " + SIZES_FILE + " could not recognised as an image size. Expected <integer>x<integer>");
                    continue;
                }

                if(parts.Length > 1)
                {
                    suffix = parts[1];
                }
                else
                {
                    suffix = "";
                }

                if(parts.Length > 2)
                {
                    if(parts[2].EndsWith("%"))
                    {
                        byte quality;
                        if(byte.TryParse(parts[2].Substring(0, parts[2].Length - 1), out quality))
                        {
                            if(quality > 1 && quality <= 100) imageQuality = quality;
                        }
                    }
                    else
                    {
                        Logger.LogMessage("Third argument on " + l + " in " + SIZES_FILE + " was not a percentage. Expected 'NN%'");
                    }
                }

                imageSizeInfo isi = new imageSizeInfo() { NameSuffix = suffix, Size = imageSize , QpalityPercent = imageQuality, Operation = operation};
                imageSizes.Add(isi);
            }
        }


        public void ConvertImages()
        {
            if (!configValid) return;

            string[] filePaths = Directory.GetFiles(sourceDirectory);
            foreach(string file in filePaths)
            {
                string ext = Path.GetExtension(file).ToLower();
                if(EXTENSIONS.Any(x => x == ext))
                {
                    foreach (imageSizeInfo sizeInfo in imageSizes)
                    {
                        try
                        {
                            convertImage(file, sizeInfo);
                        }
                        catch(IOException ex)
                        {
                            Logger.LogError("File error occurred while processing " + file + " - " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Unknown error occurred while processing " + file + " - " + ex.Message);
                        }
                    }
                }
            }
        }

        //Based on http://stackoverflow.com/questions/8357173/which-free-image-resizing-library-can-i-use-for-resizing-and-probably-serving-im
        private void convertImage(string inFile, imageSizeInfo sizeInfo)
        {
            string outFile = outputDirectory + Path.GetFileNameWithoutExtension(inFile) + sizeInfo.NameSuffix + Path.GetExtension(inFile);

            if(File.Exists(outFile))
            {
                //Output already exists, no need to re-encode 
                return;
            }

            ImageCodecInfo jpgEncoder = null;

            //
            // Load via stream rather than Image.FromFile to release the file
            // handle immediately
            //
            using (Stream stream = new FileStream(inFile, FileMode.Open))
            {
                using (Image inImage = Image.FromStream(stream))
                {

                    float width, height;
                    float srcLeft, srcTop, srcWidth, srcHeight;

                    float widthRatio = ((float)inImage.Width) / sizeInfo.Size.Width;
                    float heightRatio = ((float)inImage.Height) / sizeInfo.Size.Height;

                    if(widthRatio < 0.001)
                    {
                        throw new Exception("Image size not reasonable");
                    }

                    switch(sizeInfo.Operation)
                    { 
                        default:
                            if(widthRatio <= 1.0 && heightRatio <= 1.0)
                            {
                                //Just copy the original file
                                try
                                {
                                    File.Copy(inFile, outFile, true);
                                }
                                catch(IOException ex)
                                {
                                    Logger.LogError("Could not copy " + inFile + " due to IOException - " + ex.Message);
                                }
                                return;
                            }
                            else if (widthRatio > heightRatio)
                            {
                                width = inImage.Width / widthRatio;
                                height = inImage.Height / widthRatio;
                            }
                            else
                            {
                                width = inImage.Width / heightRatio;
                                height = inImage.Height / heightRatio;
                            }
                            srcLeft = 0;
                            srcTop = 0;
                            srcWidth = inImage.Width;
                            srcHeight = inImage.Height;
                        break;
                        case ScaleOperation.Crop:
                            width = sizeInfo.Size.Width;
                            height = sizeInfo.Size.Height;

                            if(widthRatio > heightRatio)
                            {
                                srcTop = 0;
                                srcHeight = inImage.Height;
                                srcWidth = width * heightRatio;
                                srcLeft = (inImage.Width - srcWidth) / 2f;
                            }
                            else
                            {
                                srcLeft = 0;
                                srcWidth = inImage.Width;
                                srcHeight = height * widthRatio;
                                srcTop = (inImage.Height - srcHeight) / 2f;
                            }

                        break;
                    
                    }
                    using (Bitmap bitmap = new Bitmap((int)width, (int)height))
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.DrawImage(inImage, new Rectangle(0, 0, (int)width, (int)height), srcLeft, srcTop, srcWidth, srcHeight, GraphicsUnit.Pixel);
                            if (inImage.RawFormat.Guid == ImageFormat.Jpeg.Guid)
                            {
                                if (jpgEncoder == null)
                                {
                                    ImageCodecInfo[] ici = ImageCodecInfo.GetImageDecoders();
                                    foreach (ImageCodecInfo info in ici)
                                    {
                                        if (info.FormatID == ImageFormat.Jpeg.Guid)
                                        {
                                            jpgEncoder = info;
                                            break;
                                        }
                                    }
                                }
                                if (jpgEncoder != null)
                                {
                                    EncoderParameters ep = new EncoderParameters(1);
                                    ep.Param[0] = new EncoderParameter(Encoder.Quality, (long)sizeInfo.QpalityPercent);
                                    bitmap.Save(outFile, jpgEncoder, ep);
                                }
                                else
                                    bitmap.Save(outFile, inImage.RawFormat);
                            }
                            else
                            {
                                // Fill with white for transparent GIFs
                                //graphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
                                bitmap.Save(outFile, inImage.RawFormat);
                            }
                        }
                    }
                }
            }
        }

        private struct imageSizeInfo
        {
            public string NameSuffix;
            public Size Size;
            public string FileType;
            public byte QpalityPercent;
            public ScaleOperation Operation;
        }

        private enum ScaleOperation
        {
            Scale, //Maintains original aspect ratio
            Crop //Cuts the "oversized" dimension after resizing to fit dimensions exactly
        }
    }
}
