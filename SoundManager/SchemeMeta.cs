using System;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SharpTools;

namespace SoundManager
{
    /// <summary>
    /// Holds metadata about the current sound scheme
    /// </summary>
    static class SchemeMeta
    {
        public const string SchemeImageFileName = "Scheme.png";
        public static readonly string SchemeImageFilePath = Path.Combine(SoundEvent.DataDirectory, SchemeImageFileName);

        public const string SchemeInfoFileName = "Scheme.ini";
        public static readonly string SchemeInfoFilePath = Path.Combine(SoundEvent.DataDirectory, SchemeInfoFileName);

        private static Image _thumbnail = null;
        private static string _name = null;
        private static string _author = null;
        private static string _about = null;

        /// <summary>
        /// Reset all metadata to their default values.
        /// </summary>
        public static void ResetAll()
        {
            Thumbnail = null;
            Name = Translations.Get("default_scheme_name");
            Author = Translations.Get("default_scheme_author");
            About = Translations.Get("default_scheme_about");
        }

        /// <summary>
        /// Reload metadata from disk.
        /// </summary>
        public static void ReloadFromDisk()
        {
            _name = null;
            _author = null;
            _about = null;

            try
            {
                using (Image diskImage = Image.FromFile(SchemeImageFilePath))
                {
                    _thumbnail = new Bitmap(diskImage);
                }
                Thumbnail = _thumbnail;
            }
            catch
            {
                _thumbnail = null;
            }

            if (File.Exists(SchemeInfoFilePath))
            {
                //Not using the INIFile class as legacy scheme info files have a non-standard format
                string[] fileLines = File.ReadAllLines(SchemeInfoFilePath, Encoding.UTF8);
                if (fileLines.Length == 1 && fileLines[0].Contains(";"))
                    fileLines = fileLines[0].Split(';');
                foreach (string lineRaw in fileLines)
                {
                    string line = lineRaw.Trim();
                    string fieldName = line.Split('=')[0];
                    if (line.Length > (fieldName.Length + 1))
                    {
                        string fieldValue = line.Substring(fieldName.Length + 1);
                        fieldName = fieldName.Trim(' ', '"');
                        fieldValue = fieldValue.Trim(' ', '"');
                        switch (fieldName.ToLower())
                        {
                            case "name":
                            case "nom":
                                _name = fieldValue;
                                break;
                            case "author":
                            case "auteur":
                                _author = fieldValue;
                                break;
                            case "about":
                            case "commentaire":
                                _about = fieldValue;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save updated scheme information to disk.
        /// </summary>
        private static void SaveSchemeInfo()
        {
            File.WriteAllBytes(SchemeInfoFilePath, SerializeSchemeInfo(_name, _author, _about));
        }

        /// <summary>
        /// Get serialized scheme information for storing to disk or archive.
        /// </summary>
        /// <returns>Scheme information ready to write to storage medium.</returns>
        public static byte[] SerializeSchemeInfo(string name, string author, string about)
        {
            return Encoding.UTF8.GetBytes(String.Join("\r\n", new[]{
                "[SchemeInfo]",
                "name=" + name,
                "author=" + author,
                "about=" + about,
                ""
            }));
        }

        /// <summary>
        /// Get or set the sound scheme thumbnail. Image is automatically resized and saved to disk when this field is set.
        /// </summary>
        public static Image Thumbnail
        {
            get
            {
                return _thumbnail;
            }
            set
            {
                Image image = value;

                if (image != null)
                {
                    //Resize image to 100x100, if not already resized
                    if (image.Width != 100 || image.Height != 100)
                    {
                        //stackoverflow.com/a/24199315
                        Rectangle destRect = new Rectangle(0, 0, 100, 100);
                        Bitmap destImage = new Bitmap(100, 100);

                        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                        using (var graphics = Graphics.FromImage(destImage))
                        {
                            graphics.CompositingMode = CompositingMode.SourceCopy;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            using (var wrapMode = new ImageAttributes())
                            {
                                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                            }
                        }

                        image = destImage;
                    }

                    //Convert to PNG, as saving directly to a PNG file won't work for some reason
                    using (var tempMemStream = new MemoryStream())
                    {
                        //stackoverflow.com/a/1053123
                        image.Save(tempMemStream, ImageFormat.Png);
                        image = Image.FromStream(tempMemStream);
                    }

                    //Actual file type is PNG, but saving fails using ".png" file extension, so save & move instead
                    image.Save(SchemeImageFilePath + ".bmp");
                }

                if (File.Exists(SchemeImageFilePath))
                    File.Delete(SchemeImageFilePath);

                if (File.Exists(SchemeImageFilePath + ".bmp"))
                    File.Move(SchemeImageFilePath + ".bmp", SchemeImageFilePath);
            }
        }

        /// <summary>
        /// Get or set the sound scheme name. Changes are automatically saved to disk.
        /// </summary>
        public static string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (String.IsNullOrWhiteSpace(_name))
                    _name = "";
                SaveSchemeInfo();
            }
        }

        /// <summary>
        /// Get or set the sound scheme author. Changes are automatically saved to disk.
        /// </summary>
        public static string Author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = value;
                if (String.IsNullOrWhiteSpace(_author))
                    _author = "";
                SaveSchemeInfo();
            }
        }

        /// <summary>
        /// Get or set the sound scheme extra info. Changes are automatically saved to disk.
        /// </summary>
        public static string About
        {
            get
            {
                return _about;
            }
            set
            {
                _about = value;
                if (String.IsNullOrWhiteSpace(_about))
                    _about = "";
                SaveSchemeInfo();
            }
        }

        /// <summary>
        /// Get a suitable sound event to play on scheme load: LoadScheme > Startup > Logon
        /// </summary>
        public static SoundEvent EventToPlayOnLoad
        {
            get
            {
                SoundEvent loadSchemeSound = SoundEvent.Get(SoundEvent.EventType.LoadScheme);
                SoundEvent startupSound = SoundEvent.Get(SoundEvent.EventType.Startup);
                SoundEvent logonSound = SoundEvent.Get(SoundEvent.EventType.Logon);

                // Use LoadScheme sound for schemes having it
                if (File.Exists(loadSchemeSound.FilePath) && !loadSchemeSound.Disabled)
                    return loadSchemeSound;

                // Fallback: For schemes lacking a startup sound, use logon sound
                if (!File.Exists(startupSound.FilePath) && !logonSound.Disabled)
                    return logonSound;

                // Fallback: For schemes having a startup sound, use it unless it is Vista/7 (too common)
                try
                {
                    using (var md5 = System.Security.Cryptography.MD5.Create())
                    {
                        using (var stream = File.OpenRead(startupSound.FilePath))
                        {
                            string startupHash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                            if (startupHash == "155f2a0f886570157416ea85f4b4c613") // Windows Vista/7 Startup.wav
                            {
                                return logonSound; // Startup sound is too common, use logon sound instead
                            }
                        }
                    }
                }
                catch { /* fall back to startup sound on error */ }
                return startupSound;
            }
        }
    }
}
