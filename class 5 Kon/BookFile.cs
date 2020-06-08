using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace class_5_Kon
{
    public class BookFile
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // public property
        public string filePath { get; set; }
        public List<Media> Books { get; set; }

        // constructor is a special method that is invoked
        // when an instance of a class is created
        public BookFile(string path)
        {
            Books = new List<Media>();
            filePath = path;
            // to populate the list with data, read from the data file
            try
            {
                StreamReader sr = new StreamReader(filePath);
                // first line contains column headers
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    // create instance of Book class
                    Book book = new Book();
                    string line = sr.ReadLine();
                    // first look for quote(") in string
                    // this indicates a comma(,) in movie title
                    int idx = line.IndexOf('"');
                    if (idx == -1)
                    {
                        // no quote = no comma in movie title
                        // book details are separated with comma(,)
                        string[] bookDetails = line.Split(','); 
                        book.mediaId = UInt64.Parse(bookDetails[0]);
                        book.title = bookDetails[1];
                        book.genres = bookDetails[2].Split('|').ToList();
                        //book specific properties:
                        book.author = bookDetails[3];
                        book.pageCount =  UInt16.Parse(bookDetails[4]);
                        book.publisher = bookDetails[5];
                    }
                    else
                    {
                        // quote = comma in movie title
                        // extract the movieId
                        book.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                        // remove movieId and first quote from string
                        line = line.Substring(idx + 1);
                        // find the next quote
                        idx = line.IndexOf('"');
                        // extract the movieTitle
                        book.title = line.Substring(0, idx);
                        // remove title and last comma from the string
                        line = line.Substring(idx + 2);
                        //now that the commas and quotes are out of the way we should be able to do this normally
                        string[] bookDetails = line.Split(',');
                        book.genres = bookDetails[0].Split('|').ToList();
                        //book specific properties:
                        book.author = bookDetails[1];
                        book.pageCount = UInt16.Parse(bookDetails[2]);
                        book.publisher = bookDetails[3];

                    }
                    Books.Add(book);
                }
                // close file when done
                sr.Close();
                logger.Info("Books in file {Count}", Books.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        // public method
        public bool isUniqueTitle(string title)
        {
            if (Books.ConvertAll(m => m.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate book title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddBook(Book book)
        {
            try
            {
                // first generate movie id
                book.mediaId = Books.Max(m => m.mediaId) + 1;
                // if title contains a comma, wrap it in quotes
                string title = book.title.IndexOf(',') != -1 ? $"\"{book.title}\"" : book.title;
                StreamWriter sw = new StreamWriter(filePath, true);
                sw.WriteLine($"{book.mediaId},{title},{string.Join("|", book.genres)},{book.author},{book.pageCount},{book.publisher}");
                sw.Close();
                // add movie details to Lists
                Books.Add(book);
                // log transaction
                logger.Info("Book id {Id} added", book.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }


    }
}
