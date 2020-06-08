using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace class_5_Kon
{
    public class MovieFile
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // public property
        public string filePath { get; set; }
        public List<Media> Movies { get; set; }

        // constructor is a special method that is invoked
        // when an instance of a class is created
        public MovieFile(string path)
        {
            Movies = new List<Media>();
            filePath = path;
            // to populate the list with data, read from the data file
            try
            {
                StreamReader sr = new StreamReader(filePath);
                // first line contains column headers
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    // create instance of Movie class
                    Movie movie = new Movie();
                    string line = sr.ReadLine();
                    // first look for quote(") in string
                    // this indicates a comma(,) in movie title
                    int idx = line.IndexOf('"');
                    if (idx == -1)
                    {
                        // no quote = no comma in movie title
                        // movie details are separated with comma(,)
                        string[] movieDetails = line.Split(',');
                        movie.mediaId = UInt64.Parse(movieDetails[0]);
                        movie.title = movieDetails[1];
                        movie.genres = movieDetails[2].Split('|').ToList();
                        //Movie Specific Properties:
                        movie.runningTime = TimeSpan.Parse(movieDetails[4]);
                        movie.director = movieDetails[3];
                    }
                    else
                    {
                        // quote = comma in movie title
                        // extract the movieId
                        movie.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                        // remove movieId and first quote from string
                        line = line.Substring(idx + 1);
                        // find the next quote
                        idx = line.IndexOf('"');
                        // extract the movieTitle
                        movie.title = line.Substring(0, idx);
                        // remove title and last comma from the string
                        line = line.Substring(idx + 2);
                        //with the commas and quotes out of the way this can be done normally
                        string[] movieDetails = line.Split(',');
                        movie.genres = movieDetails[0].Split('|').ToList();
                        //Movie Specific Properties:
                        movie.runningTime = TimeSpan.Parse(movieDetails[2]);
                        movie.director = movieDetails[1];

                    }
                    Movies.Add(movie);
                }
                // close file when done
                sr.Close();
                logger.Info("Movies in file {Count}", Movies.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        // public method
        public bool isUniqueTitle(string title)
        {
            if (Movies.ConvertAll(m => m.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate movie title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddMovie(Movie movie)
        {
            try
            {
                // first generate movie id
                movie.mediaId = Movies.Max(m => m.mediaId) + 1;
                // if title contains a comma, wrap it in quotes
                string title = movie.title.IndexOf(',') != -1 ? $"\"{movie.title}\"" : movie.title;
                StreamWriter sw = new StreamWriter(filePath, true);
                sw.WriteLine($"{movie.mediaId},{title},{string.Join("|", movie.genres)},{movie.director},{movie.runningTime}");
                sw.Close();
                // add movie details to Lists
                Movies.Add(movie);
                // log transaction
                logger.Info("Movie id {Id} added", movie.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }


    }
}
