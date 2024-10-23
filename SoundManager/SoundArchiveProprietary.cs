using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using Ionic.Zip;

namespace SoundManager
{
    /// <summary>
    /// Sound archive conversion utility for a popular proprietary file format
    /// </summary>
    /// <remarks>
    /// NOTE ABOUT FAIR USE: This class implements a decryption routine, mandatory for interoperability with this file format.
    /// We use the same approach as with the RAR file format, only allowing to unpack proprietary files supplied by the user.
    /// Generating proprietary files is not implemented here, requiring to purchase the original proprietary sotfware.
    /// </remarks>
    class SoundArchiveProprietary
    {
        public static readonly string FileIconPath = Path.Combine(RuntimeConfig.AppFolder, "SoundSchemeProprietary.ico");
        public const string FileExtension = "soundpack";
        private const string FileExtAction = "import";
        private const string MetadataFile = "soundpackage.data";

        // Keys used to encrypt the Zip file, this is an AES layer applied over the Zip file, not an encrypted Zip using Zip's own encryption feature
        private static byte[] zipAesKey = new byte[] { 0x43, 0x6f, 0x70, 0x79, 0x72, 0x69, 0x67, 0x68, 0x74, 0x3f, 0x53, 0x74, 0x61, 0x72, 0x64, 0x6f };
        private static byte[] zipAesIv = new byte[] { 0x7d, 0x2a, 0x7e, 0x61, 0x70, 0x3f, 0x3f, 0x3f, 0x53, 0x74, 0x61, 0x72, 0x64, 0x6f, 0x63, 0x6b };

        // Keys used to encrypt scheme metadata file inside the Zip file, this is a base64 3DES layer applied over an XML file
        private static byte[] xml3DesKey = new byte[] { 0x3f, 0x43, 0x6f, 0x70, 0x79, 0x72, 0x69, 0x67, 0x68, 0x74, 0x53, 0X74, 0x61, 0x72, 0x64, 0x6f, 0x63, 0x6b, 0x32, 0x30, 0x30, 0x38, 0x3f, 0x3f };
        private static byte[] xml3DesIv = new byte[] { 0x7d, 0x6c, 0x60, 0x3f, 0x2a, 0x7e, 0x61, 0x70 };

        /// <summary>
        /// Check if the provided file is a proprietary archive
        /// </summary>
        /// <param name="file">Path to file</param>
        /// <returns>TRUE if this file is a proprietary file</returns>
        public static bool IsProprietary(string file)
        {
            return file.ToLowerInvariant().EndsWith("." + FileExtension);
        }

        /// <summary>
        /// Convert a proprietary sound archive to our usual archive format
        /// </summary>
        /// <param name="infile">Path to proprietary file</param>
        /// <param name="outfile">Path to output sound scheme file</param>
        /// <returns>Output converted file</returns>
        public static string ConvertArchive(string infile, string outfile)
        {
            string tempZipFile = Path.GetTempFileName();

            // Decrypt Zip archive
            try
            {
                // Already decrypted?
                ZipFile.CheckZip(infile);
            }
            catch (ZipException)
            {
                // Decrypt Zip archive
                using (FileStream encryptedZipStream = new FileStream(infile, FileMode.Open))
                {
                    using (AesStream decryptingZipStream = new AesStream(encryptedZipStream, zipAesKey, zipAesIv))
                    {

                        using (FileStream outputZipStream = new FileStream(tempZipFile, FileMode.Create))
                        {
                            decryptingZipStream.CopyTo(outputZipStream);
                            infile = tempZipFile;
                        }
                    }
                }
            }

            using (ZipFile inZip = ZipFile.Read(infile))
            {
                if (inZip.ContainsEntry(MetadataFile))
                {
                    // Decrypt XML metadata file
                    string metadataXmlString;
                    using (MemoryStream encMetadataFileStream = new MemoryStream())
                    {
                        inZip.First(entry => entry.FileName.ToLowerInvariant() == MetadataFile).Extract(encMetadataFileStream);
                        encMetadataFileStream.Seek(0, 0);
                        byte[] metadataEncrypted = Convert.FromBase64String(new StreamReader(encMetadataFileStream).ReadToEnd().Replace(' ', '+'));
                        using (MemoryStream encryptedXmlStream = new MemoryStream(metadataEncrypted))
                        {
                            using (TripleDesStream decryptingXmlStream = new TripleDesStream(encryptedXmlStream, xml3DesKey, xml3DesIv))
                            {
                                metadataXmlString = new StreamReader(decryptingXmlStream).ReadToEnd();
                            }
                        }
                    }

                    // Load XML metadata file
                    XmlDocument metadata = new XmlDocument();
                    metadata.XmlResolver = null; // Do not process external entities
                    try
                    {
                        metadata.LoadXml(metadataXmlString);
                    }
                    catch (XmlException)
                    {
                        // Retry attempting to escape invalid entities in XML metadata file
                        metadata.LoadXml(metadataXmlString.Replace("&", "&amp;"));
                    }

                    // Process XML metadata file to determine thumbnail, author, and sound event file names
                    XmlElement rootNode = metadata.DocumentElement;
                    string schemeName = XmlGetValue(rootNode, "name") ?? "";
                    string schemeAuthor = XmlGetValue(rootNode, "author") ?? "";
                    string schemeComment = XmlGetValue(rootNode, "notes") ?? XmlGetValue(rootNode, "website") ?? XmlGetValue(rootNode, "email") ?? XmlGetValue(rootNode, "copyright");
                    string thumbnail = XmlGetValue(rootNode, new[] { "preview", "icon" });

                    // Destination file name <-> Source file name
                    var filesToCopy = new Dictionary<string, string>();
                    var filesCopied = new HashSet<string>();

                    if (!String.IsNullOrEmpty(thumbnail))
                    {
                        filesToCopy[SchemeMeta.SchemeImageFileName] = thumbnail;
                        filesCopied.Add(thumbnail);
                    }

                    // Sound events are defined in XML by their location in registry
                    XmlNode soundGroups = rootNode["groups"];
                    if (soundGroups != null)
                    {
                        foreach (SoundEvent soundEvent in SoundEvent.GetAll())
                        {
                            foreach (string regPath in soundEvent.RegistryKeys)
                            {
                                string[] path = regPath.Split('\\');
                                string fileName = XmlGetValue(soundGroups, path, "name");
                                if (!String.IsNullOrEmpty(fileName))
                                {
                                    filesToCopy[soundEvent.FileName] = fileName;
                                    filesCopied.Add(fileName);
                                    break;
                                }
                            }
                        }
                    }

                    // Keep unused source files just in case
                    foreach (ZipEntry entry in inZip.Entries)
                        if (!filesCopied.Contains(entry.FileName))
                            filesToCopy["_unused_" + entry.FileName] = entry.FileName;

                    // Generate SoundManager sound archive file
                    using (ZipFile outZip = new ZipFile())
                    {
                        foreach (var filePair in filesToCopy)
                        {
                            MemoryStream fileInMemory = new MemoryStream();
                            ZipEntry sourceEntry = inZip.FirstOrDefault(entry => entry.FileName.ToLowerInvariant() == filePair.Value.ToLowerInvariant());
                            if (sourceEntry != null) // Metadata may reference files that do not exist in the source archive, skip them
                            {
                                sourceEntry.Extract(fileInMemory);
                                fileInMemory.Seek(0, 0);
                                outZip.AddEntry(filePair.Key, fileInMemory);
                            }
                        }
                        outZip.AddEntry(SchemeMeta.SchemeInfoFileName, SchemeMeta.SerializeSchemeInfo(schemeName, schemeAuthor, schemeComment));
                        outZip.AddEntry("_unused_" + MetadataFile + ".xml", Encoding.UTF8.GetBytes(metadataXmlString));
                        outZip.Save(outfile);
                    }
                }
                else
                {
                    throw new System.IO.FileNotFoundException(MetadataFile);
                }
            }

            if (File.Exists(tempZipFile))
                File.Delete(tempZipFile);

            return outfile;
        }

        /// <summary>
        /// Look for child node of specified tag and get inner text
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="tag">Child node tag</param>
        /// <returns>Child node inner text or null if not found</returns>
        private static string XmlGetValue(XmlNode node, string tag)
        {
            return XmlGetValue(node, new[] { tag });
        }

        /// <summary>
        /// Look for child node of specified tag and get inner text
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="path">Child node tag path as string array</param>
        /// <returns>Child node inner text or null if not found</returns>
        private static string XmlGetValue(XmlNode node, string[] path)
        {
            foreach (string pathElement in path)
            {
                node = node[pathElement];
                if (node == null)
                    return null;
            }
            return node.InnerText;
        }

        /// <summary>
        /// Look for child node of specified attribute and get inner text
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="path">Child node attribute path as string array</param>
        /// <param name="navAttribute">Attribute name to look for path elements</param>
        /// <returns>Child node inner text or null if not found</returns>
        private static string XmlGetValue(XmlNode node, string[] path, string navAttributeName)
        {
            foreach (string pathElement in path)
            {
                bool foundChild = false;
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    foreach (XmlAttribute attr in childNode.Attributes)
                    {
                        if (attr.Name == navAttributeName && attr.Value == pathElement)
                        {
                            foundChild = true;
                            node = childNode;
                            break;
                        }
                    }
                    if (foundChild)
                        break;
                }
                if (!foundChild)
                    return null;
            }
            return node.InnerText;
        }

        /// <summary>
        /// An encrypted stream using AES, used for encrypting or decrypting data on the fly using AES
        /// </summary>
        class AesStream : Stream
        {
            CryptoStream enc;
            CryptoStream dec;
            public AesStream(Stream stream, byte[] key, byte[] iv)
            {
                BaseStream = stream;
                enc = new CryptoStream(stream, GenerateAES(key, iv).CreateEncryptor(), CryptoStreamMode.Write);
                dec = new CryptoStream(stream, GenerateAES(key, iv).CreateDecryptor(), CryptoStreamMode.Read);
            }
            public System.IO.Stream BaseStream { get; set; }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
                BaseStream.Flush();
            }

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get
                {
                    throw new NotSupportedException();
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public override int ReadByte()
            {
                return dec.ReadByte();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return dec.Read(buffer, offset, count);
            }

            public override long Seek(long offset, System.IO.SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void WriteByte(byte b)
            {
                enc.WriteByte(b);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                enc.Write(buffer, offset, count);
            }

            private RijndaelManaged GenerateAES(byte[] key, byte[] iv)
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.Padding = PaddingMode.ISO10126;
                cipher.Mode = CipherMode.CBC;
                cipher.KeySize = 128;
                cipher.BlockSize = 128;
                cipher.Key = key;
                cipher.IV = iv;
                return cipher;
            }
        }

        /// <summary>
        /// An encrypted stream using Triple DES, used for encrypting or decrypting data on the fly using Triple DES (3DES)
        /// </summary>
        /// <remarks>
        /// The Triple DES encryption algorithm is no longer secure: https://en.wikipedia.org/wiki/Triple_DES#Security
        /// </remarks>
        class TripleDesStream : Stream
        {
            CryptoStream enc;
            CryptoStream dec;
            public TripleDesStream(Stream stream, byte[] key, byte[] iv)
            {
                BaseStream = stream;
                enc = new CryptoStream(stream, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Write);
                dec = new CryptoStream(stream, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read);
            }
            public System.IO.Stream BaseStream { get; set; }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
                BaseStream.Flush();
            }

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get
                {
                    throw new NotSupportedException();
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public override int ReadByte()
            {
                return dec.ReadByte();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return dec.Read(buffer, offset, count);
            }

            public override long Seek(long offset, System.IO.SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void WriteByte(byte b)
            {
                enc.WriteByte(b);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                enc.Write(buffer, offset, count);
            }
        }
    }
}
