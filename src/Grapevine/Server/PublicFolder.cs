﻿using System.IO;
using System.Reflection;
using Grapevine.Util;

namespace Grapevine.Server
{
    /// <summary>
    /// Provides methods for working with files and folder for static content
    /// </summary>
    public class PublicFolder
    {
        protected const string DefaultFolder = "public";
        protected const bool IsFilePath = true;
        private string _folderPath;

        public PublicFolder()
        {
            var path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            if (path != null) _folderPath = Path.Combine(path, DefaultFolder);
            if (!FolderExists(_folderPath)) CreateFolder(_folderPath);
        }

        /// <summary>
        /// Gets or sets the default file to return when a directory is requested
        /// </summary>
        public string DefaultFileName { get; set; } = "index.html";

        /// <summary>
        /// Gets or sets the optional prefix for specifying when static content should be returned
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the folder to be scanned for static content requests
        /// </summary>
        public string FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = FolderExists(value) ? value : CreateFolder(value); }
        }

        /// <summary>
        /// If it exists, responds to the request with the requested file
        /// </summary>
        public IHttpContext SendPublicFile(IHttpContext context)
        {
            if ((context.Request.HttpMethod != HttpMethod.GET && context.Request.HttpMethod != HttpMethod.HEAD) || string.IsNullOrWhiteSpace(_folderPath)) return context;

            var path = string.IsNullOrWhiteSpace(Prefix) ? context.Request.PathInfo : context.Request.PathInfo.Replace(Prefix, "");
            path = path.TrimStart('/', '\\');

            var filepath = GetFilePath(path);
            if (filepath != null) context.Response.SendResponse(filepath, IsFilePath);

            return context;
        }

        /// <summary>
        /// Returns true if the specified directory exists
        /// </summary>
        private static bool FolderExists (string folder)
        {
            return !string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder);
        }

        /// <summary>
        /// Returns the path the folder created
        /// </summary>
        private static string CreateFolder(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder)) return null;
            var info = Directory.CreateDirectory(folder);
            return info.FullName;
        }

        /// <summary>
        /// Returns the path to the specified file
        /// </summary>
        private string GetFilePath(string pathinfo)
        {
            if (string.IsNullOrWhiteSpace(_folderPath)) return null;
            var path = pathinfo.Replace("/", Path.DirectorySeparatorChar.ToString());
            path = Path.Combine(_folderPath, path);

            if (File.Exists(path)) return path;
            if (!Directory.Exists(path)) return null;

            path = Path.Combine(path, DefaultFileName);
            return File.Exists(path) ? path : null;
        }
    }
}
