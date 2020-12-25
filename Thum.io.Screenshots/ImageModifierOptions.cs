using System;
using System.Reflection;
using System.Collections.Generic;

namespace Thum.io.Screenshots
{
    /// <summary>
    /// The API image modifier options
    /// https://www.thum.io/documentation/api/url
    /// </summary>
    public class ImageModifierOptions
    {
        /// <summary>
        /// Thumbnail width in pixels
        /// (Default: 600 (1/2 of the viewportWidth)).
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of original screen shot in pixels
        /// (Default: 1200)
        /// </summary>
        public int Crop { get; set; }

        /// <summary>
        /// Refresh the thumbnail if the cached image
        /// is older than this amount, in hours
        /// </summary>
        public int MaxAge { get; set; }

        /// <summary>
        /// Return a JPG instead of PNG format when possible.
        /// JPG images are smaller but are not as high quality.
        /// This is the default for images with a width of 320px or less.
        /// (Recommended for thumbnails and smaller, particularly when
        /// a large number of screenshots are embedded on a single page)
        /// </summary>
        public bool AllowJpg { get; set; }

        /// <summary>
        /// Return a PNG format regardless of the resolution.
        /// PNG images are larger but have degradation due to compression.
        /// </summary>
        public bool Png { get; set; }

        /// <summary>
        /// Don't animate the resulting image, just return the final png.
        /// (Good for batch jobs that download images) 
        /// </summary>
        public bool NoAnimate { get; set; }

        /// <summary>
        /// Return an image containing the full page, not just the visible area.
        /// These requests will not display a loading spinner but instead will
        /// block until the final png is ready.
        /// (Only available to "better" plan users)
        /// </summary>
        public bool FullPage { get; set; }

        /// <summary>
        /// Wait for the specified number of seconds after the webpage has
        /// loaded before taking a screen shot. This is useful for pages
        /// that have animations or other ajax based components that load asynchronously.
        /// (Only available to "better" plan users)
        /// </summary>
        public int Wait { get; set; }

        /// <summary>
        /// Set the viewportWidth of the browser. Maximum value is 1200
        /// (Default: 1200)
        /// (Only available to "better" plan users)
        /// </summary>
        public int ViewportWidth { get; set; }

        /// <summary>
        /// Emulate an iPhone 5
        /// (Only available to "better" plan users)
        /// </summary>
        public bool Iphone5 { get; set; }

        /// <summary>
        /// Emulate an iPhone 6
        /// (Only available to "better" plan users)
        /// </summary>
        public bool Iphone6 { get; set; }

        /// <summary>
        /// Emulate an iPhone 6 Plus
        /// (Only available to "better" plan users)
        /// </summary>
        public bool Iphone6Plus { get; set; }

        /// <summary>
        /// /// Emulate an iPhone X
        /// (Only available to "better" plan users)
        /// </summary>
        public bool IphoneX { get; set; }

        /// <summary>
        /// Emulate a Galaxy S5
        /// (Only available to "better" plan users)
        /// </summary>
        public bool GalaxyS5 { get; set; }

        public override string ToString()
        {
            var propertiesWithValues = new List<string>();
            var properties = GetType()
                .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in properties)
            {
                var value = p.GetValue(this);
                var includeProperty = value != null;
                var includeValue =  value != null;

                switch (value)
                {
                    case bool _ :
                        includeProperty = Convert.ToBoolean(value);
                        includeValue = false;
                        break;
                    case int _ when Convert.ToInt32(value) == 0:
                        includeProperty = false;
                        break;
                }

                if (includeProperty)
                {
                    propertiesWithValues.Add(includeValue ? $"{p.Name}/{value}" : $"{p.Name}");
                }
            }

            return string.Join("/", propertiesWithValues);
        }
    }
}