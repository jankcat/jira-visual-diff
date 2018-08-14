using System;
using System.Collections.Generic;
using Jankcat.VisualCompare.Lib.Models;
using ImageMagick;

namespace Jankcat.VisualCompare.Lib.Utilities
{
    public static class ImageUtils
    {
        public static ComparisonResult Compare(IMagickImage original, IMagickImage updated)
        {
            var compareResult = new ComparisonResult();
            // https://github.com/dlemstra/Magick.NET/blob/9943055423b90ac35933dde602b3af9aab746a8a/Source/Magick.NET/Shared/Enums/ErrorMetric.cs
            // https://www.imagemagick.org/script/command-line-options.php#metric
            var compareSettings = new CompareSettings()
            {
                HighlightColor = MagickColors.White,//MagickColor.FromRgba(255, 20, 147, 255),
                LowlightColor = MagickColors.Black,
                Metric = ErrorMetric.Absolute
            };

            using (IMagickImage originalMImage = new MagickImage(original))
            {
                // Set Fuzzing Level
                originalMImage.ColorFuzz = new Percentage(8);
                compareResult.TotalPixels = originalMImage.Height * originalMImage.Width;
                using (IMagickImage diffMImage = new MagickImage())
                {
                    using (IMagickImage updatedMImage = new MagickImage(updated))
                    {
                       compareResult.ChangedPixels = originalMImage.Compare(updatedMImage, compareSettings, diffMImage);
                    }

                    // flatten the colors
                    diffMImage.AutoLevel();
                    // bold the diff, make the changes pop
                    diffMImage.Morphology(new MorphologySettings
                    {
                        Kernel = Kernel.Octagon,
                        KernelArguments = "5",
                        Method = MorphologyMethod.Dilate
                    });

                    // flip the colors
                    diffMImage.Opaque(MagickColors.White, MagickColors.Red);
                    diffMImage.Opaque(MagickColors.Black, MagickColors.Transparent);

                    using (var origFadeImg = new MagickImage(original))
                    {
                        origFadeImg.Colorize(MagickColors.White, new Percentage(75));
                        using (IMagickImageCollection collection = new MagickImageCollection(origFadeImg.ToByteArray()))
                        {
                            collection.Add(diffMImage);
                            compareResult.DiffImage = new MagickImage(collection.Merge()); 
                        }
                    }
                }
            }
            return compareResult;
        }

        public static IMagickImage Crop(IMagickImage image, CropDimension cropDimensions)
        {
            var mImage = new MagickImage(image);
            mImage.Crop(cropDimensions.X, cropDimensions.Y, cropDimensions.Width, cropDimensions.Height);
            mImage.RePage();
            return mImage;
        }

        public static IMagickImage MergeImageLists(Dictionary<int, Dictionary<int, IMagickImage>> images)
        {
            using (MagickImageCollection imgCollection = new MagickImageCollection())
            {
                foreach (var imageList in images)
                {
                    imgCollection.Add(new MagickImage(MergeImageRow(imageList.Value)));
                }
                return imgCollection.AppendVertically();
            }
        }

        public static IMagickImage MergeImageRow(Dictionary<int, IMagickImage> images) 
        {
            using (MagickImageCollection imgCollection = new MagickImageCollection())
            {
                foreach (var image in images) {
                    imgCollection.Add(new MagickImage(image.Value));
                }
                return imgCollection.AppendHorizontally();
            }
        }
    }
}
