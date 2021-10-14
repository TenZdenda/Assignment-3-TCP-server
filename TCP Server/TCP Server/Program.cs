using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using BookLibrary;

namespace TCP_Server
{
    class Program
    {
        public static List<Book> Books = new List<Book>()
        {
            new Book("ALCHYMIE", "Rory Sutherland", 400, 978807111),
            new Book("Spotify", "Sven Carlsson", 256, 978807222),
            new Book("Konec stárnutí", "David Sinclair", 384, 978807333),
            new Book("Stát se investorem", "Mikuláš Splítek", 320, 978807444)
        };
        
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 4646);
            listener.Start();
            
            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();

                Task.Run(() => { HandleRequests(socket); });
            }
        }
        
        protected static void HandleRequests(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            String choice = reader.ReadLine();
            String value = reader.ReadLine();

            switch (choice)
            {
                case "GetAll":
                    string serializedListBooks = JsonSerializer.Serialize(Books);

                    writer.WriteLine(serializedListBooks);
                    break;
                case "Get":
                    Book book1 = Books.Find(book => book.ISBN13 == Int32.Parse(value));
                    string serializedBook = JsonSerializer.Serialize(book1);
                    
                    writer.WriteLine(serializedBook);
                    break;
                case "Save":
                    Book book2 = JsonSerializer.Deserialize<Book>(value);
                    Books.Add(book2);
                    
                    break;
                default:
                    writer.WriteLine("This is not supported");
                    break;
            }

            Console.WriteLine("Client says:" + choice + ", " + value);

            writer.Flush();
            socket.Close();

        }
    }
}