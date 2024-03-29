﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SharpTools;
using System.IO;

namespace DownloadSchemes
{
    /// <summary>
    /// Minimal class for interacting with GitHub API
    /// By ORelio - (c) 2023 - Available under the CDDL-1.0 license
    /// </summary>
    static class GitHubApi
    {
        private static readonly string ApiBase = "https://api.github.com/";
        private static readonly string ApiListContents = ApiBase + "repos/{0}/{1}/contents/{2}";

        /// <summary>
        /// Retrieve a list of files in repostory
        /// </summary>
        /// <param name="user">Username or Organization name</param>
        /// <param name="repository">Repository Name</param>
        /// <param name="folder">Folder name relative to repository's root folder</param>
        /// <param name="recursive">Also process subfolders</param>
        /// <returns>List of file URLs</returns>
        public static List<string> ListFilesInRepo(string user, string repository, string folder = "/", bool recursive = false)
        {
            if (String.IsNullOrEmpty(user))
                throw new ArgumentException("user cannot be null or empty", "user");
            if (String.IsNullOrEmpty(repository))
                throw new ArgumentException("repository cannot be null or empty", "repository");
            if (String.IsNullOrEmpty(folder))
                throw new ArgumentException("folder cannot be null or empty", "folder");
            if (folder.StartsWith("/"))
                folder = folder.TrimStart('/');

            string url = String.Format(ApiListContents, user, repository, folder);
            WebClient apiClient = new WebClient();
            apiClient.Headers["User-Agent"] = typeof(GitHubApi).Namespace + "/1.0";
            apiClient.Headers["Accept"] = "application/vnd.github+json";
            apiClient.Headers["X-GitHub-Api-Version"] = "2022-11-28";
            string result;

            try
            {
                result = apiClient.DownloadString(url);
            }
            catch (WebException e)
            {
                if (e.Response.Headers.AllKeys.Contains("X-RateLimit-Remaining") && e.Response.Headers["X-RateLimit-Remaining"] == "0")
                {
                    long timestamp;
                    string message = "GitHub API Rate limit exceeded, please retry after ";
                    if (e.Response.Headers.AllKeys.Contains("X-RateLimit-Reset") && long.TryParse(e.Response.Headers["X-RateLimit-Reset"], out timestamp))
                    {
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
                        message += dateTime.ToString();
                    }
                    else message += "a few minutes";
                    throw new WebException(message + '.', e);
                }
                else
                {
                    using (StreamReader reader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        string message = reader.ReadToEnd();
                        if (!String.IsNullOrEmpty(message))
                            throw new WebException(message, e);
                    }
                }
                throw;
            }

            Json.JSONData data = Json.ParseJson(result);
            List<string> files = new List<string>();

            if (data.Type == Json.JSONData.DataType.Array)
            {
                foreach (Json.JSONData item in data.DataArray)
                {
                    if (item.Properties.ContainsKey("type"))
                    {
                        if (item.Properties["type"].StringValue == "file")
                        {
                            if (item.Properties.ContainsKey("download_url") && !String.IsNullOrEmpty(item.Properties["download_url"].StringValue))
                            {
                                files.Add(item.Properties["download_url"].StringValue);
                            }
                        }
                        else if (recursive && item.Properties["type"].StringValue == "dir" && item.Properties.ContainsKey("path"))
                        {
                            files.AddRange(ListFilesInRepo(user, repository, item.Properties["path"].StringValue, recursive));
                        }
                    }
                }
            }

            return files;
        }
    }
}
