using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace class_5_Kon
{
    class Program
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            string movieFile = "moviesPlus.csv";
            string bookFile = "books.csv";
            string albumFile = "albums.csv";

            logger.Info("Program started");



            //Reference and create movie file object
            MovieFile theMovies = new MovieFile(movieFile);
            //Reference and create book and album file objects
            BookFile theBooks = new BookFile(bookFile);
            AlbumFile theAlbums = new AlbumFile(albumFile);
            
            //Prepare the menu options and input validation loop
            int menuChoice = 1;
            bool inputWrong;
            do
            {
                inputWrong = false;
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1: View Movies");
                Console.WriteLine("2: Add Movie");
                Console.WriteLine("3: View Books");
                Console.WriteLine("4: Add Book");
                Console.WriteLine("5: View Albums");
                Console.WriteLine("6: Add Album");

                string input = Console.ReadLine();
                try
                {
                   menuChoice = int.Parse(input);
                }
                catch (System.FormatException e)
                {
                    Console.WriteLine("You need to respond with the corresponding number to an exsisting menu option!");
                    inputWrong = true;
                }
            } while (inputWrong);

            //perform user's action
            switch (menuChoice)
            {
                case 1:
                    //Display all movies in list
                    foreach (Media m in theMovies.Movies)
                    {
                        Console.WriteLine(m.Display());
                    }
                    break;
                case 2:
                    Movie movie = new Movie();
                    Console.WriteLine("What is the title?");
                    string input;
                    input = Console.ReadLine();
                    movie.title = input;


                    List<string> genres = new List<string>();
                    bool endGenre = false;
                    bool endLoop = false;
                    do
                    {
                        Console.WriteLine("What is the genre?");
                        genres.Add(Console.ReadLine());

                        endLoop = false;
                        do
                        {
                            Console.WriteLine("Is there another genre? (Y/N)");
                            input = Console.ReadLine();
                            try
                            {
                                if (input.ToUpper().Equals("Y")) endGenre = false;
                                else if (input.ToUpper().Equals("N")) endGenre = true;
                                else
                                {
                                    IOException e = new IOException();
                                    throw e;
                                }
                                endLoop = true;
                            }
                            catch
                            {
                                logger.Error("You failed to provide proper input!  Input: " + input);
                            }
                        } while (!endLoop);
                    } while (!endGenre);

                    movie.genres = genres;
                    

                    Console.WriteLine("Who is the director?");
                    input = Console.ReadLine();
                    movie.director = input;

                    Console.WriteLine("How many hours long is the movie?");
                    try
                    {
                        string hours = int.Parse(Console.ReadLine()); //fix this, add input validation!!!
                    }
                    catch
                    {
                        logger.Error("You failed to provide proper input!  Input: " + hours);
                    }
                    

                    TimeSpan runtime = new TimeSpan();

                    break;
                case 3:
                    //Display all Books
                    foreach (Media b in theBooks.Books)
                    {
                        Console.WriteLine(b.Display());
                    }
                    break;
                case 4:

                    break;
                case 5:
                    //Display all Albums
                    foreach (Media a in theAlbums.Albums)
                    {
                        Console.WriteLine(a.Display());
                    }
                    break;
                case 6:

                    break;
            }


            logger.Info("Program ended");

           

        }



    }
}
