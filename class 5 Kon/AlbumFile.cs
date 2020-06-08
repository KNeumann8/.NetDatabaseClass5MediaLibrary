using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace class_5_Kon
{
    public class AlbumFile
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // public property
        public string filePath { get; set; }
        public List<Media> Albums { get; set; }

        // constructor is a special method that is invoked
        // when an instance of a class is created
        public AlbumFile(string path)
        {
            Albums = new List<Media>();
            filePath = path;
            // to populate the list with data, read from the data file
            try
            {
                StreamReader sr = new StreamReader(filePath);
                // first line contains column headers
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    // create instance of Album class
                    Album album = new Album();
                    string line = sr.ReadLine();
                    // first look for quote(") in string
                    // this indicates a comma(,) in movie title
                    int idx = line.IndexOf('"');
                    if (idx == -1)
                    {
                        // no quote = no comma in album title
                        // album details are separated with comma(,)
                        string[] albumDetails = line.Split(',');
                        album.mediaId = UInt64.Parse(albumDetails[0]);
                        album.title = albumDetails[1];
                        album.genres = albumDetails[2].Split('|').ToList();
                        //Album specific properties:
                        album.artist = albumDetails[3];
                        album.recordLabel = albumDetails[4];
                    }
                    else
                    {
                        // quote = comma in movie title
                        // extract the movieId
                        album.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                        // remove movieId and first quote from string
                        line = line.Substring(idx + 1);
                        // find the next quote
                        idx = line.IndexOf('"');
                        // extract the movieTitle
                        album.title = line.Substring(0, idx);
                        // remove title and last comma from the string
                        line = line.Substring(idx + 2);
                        //now that the commas and quotes are out of the way we should be able to do this normally
                        string[] albumDetails = line.Split(',');
                        album.genres = albumDetails[0].Split('|').ToList();
                        //Album specific properties:
                        album.artist = albumDetails[1];
                        album.recordLabel = albumDetails[2];

                    }
                    Albums.Add(album);
                }
                // close file when done
                sr.Close();
                logger.Info("Albums in file {Count}", Albums.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        // public method
        public bool isUniqueTitle(string title)
        {
            if (Albums.ConvertAll(m => m.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate album title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddAlbum(Album album)
        {
            try
            {
                // first generate movie id
                album.mediaId = Albums.Max(m => m.mediaId) + 1;
                // if title contains a comma, wrap it in quotes
                string title = album.title.IndexOf(',') != -1 ? $"\"{album.title}\"" : album.title;
                StreamWriter sw = new StreamWriter(filePath, true);
                sw.WriteLine($"{album.mediaId},{title},{string.Join("|", album.genres)},{album.artist},{album.recordLabel}");
                sw.Close();
                // add movie details to Lists
                Albums.Add(album);
                // log transaction
                logger.Info("Album id {Id} added", album.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }


    }
}
