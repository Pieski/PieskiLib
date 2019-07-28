using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PieskiLib
{
    class Program
    {
        static string library = null;
        static Dictionary<string, string> librarylist = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            librarylist.Add("深圳图书馆", "044005");
            librarylist.Add("宝安图书馆", "044007");
            librarylist.Add("南山图书馆", "044006");
            librarylist.Add("福田图书馆", "044008");
            Console.Write("---------------------------------------\n" +
                          "|1）按书名|2）按作者|3）按ISBN|e）退出|\n" +
                          "---------------------------------------\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            string input = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            while (input != "e" && input != "E")
            {
                Console.Write("\t1）深圳图书馆\n" +
                              "\t2）宝安图书馆\n" +
                              "\t3）南山图书馆\n" +
                              "\t4）福田图书馆\n");
                Console.WriteLine("在指定编号的行政区搜索（留空搜索全市）：");
                Console.ForegroundColor = ConsoleColor.Blue;
                library = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                switch (library)
                {
                    case "1":
                        library = librarylist["深圳图书馆"];
                        break;
                    case "2":
                        library = librarylist["宝安图书馆"];
                        break;
                    case "3":
                        library = librarylist["南山图书馆"];
                        break;
                    case "4":
                        library = librarylist["福田图书馆"];
                        break;
                }
                switch (input)
                {
                    case "1":
                        Console.Write("输入书名：");
                        SearchByName(Console.ReadLine());
                        break;
                    case "2":
                        Console.Write("输入作者：");
                        SearchByAuthor(Console.ReadLine());
                        break;
                    case "3":
                        Console.Write("输入ISBN：");
                        SearchByISBN(Console.ReadLine());
                        break;
                }
                Console.Write("---------------------------------------\n" +
                              "|1）按书名|2）按作者|3）按ISBN|e）退出|\n" +
                              "---------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Blue;
                input = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void SearchByName(string name)
        {
            string path = "https://www.szlib.org.cn/Search/searchshow.jsp?" +
                "v_tablearray=bibliosm%2Cserbibm%2Capabibibm%2Cmmbibm%2C&" +
                "v_index=title&v_value="+name+"+&cirtype=&" +
                "v_startpubyear=&v_endpubyear=&v_publisher=&v_author=&" +
                "sortfield=ptitle&sorttype=desc&library="+library;
            WebRequest request = new WebRequest(path);
            string htmlinfo = request.Download();
            Match booklist = Regex.Match(htmlinfo, 
                "<ul class=\\\"booklist\\\">[\\S\\s]*?</ul>");
            MatchCollection books = Regex.Matches(booklist.Value,
                "<li>[\\s\\S]*?</li>");

            List<Book> result = new List<Book>();
            foreach(Match match in books)
            {
                Book book = new Book(match.Value);
                book.GetResources();
                result.Add(book);
            } 
            ShowResult(result);
        }

        static void SearchByAuthor(string author)
        {
            string path = "https://www.szlib.org.cn/Search/searchshow.jsp?" +
                "v_tablearray=bibliosm%2Cserbibm%2Capabibibm%2Cmmbibm%2C&" +
                "v_index=author&v_value="+author+"+&" +
                "cirtype=&v_startpubyear=&v_endpubyear=&v_publisher=&" +
                "v_author=&sortfield=ptitle&sorttype=desc&library="+library;
            WebRequest request = new WebRequest(path);
            string htmlinfo = request.Download();
            Match booklist = Regex.Match(htmlinfo,
                "<ul class=\\\"booklist\\\">[\\S\\s]*?</ul>");
            MatchCollection books = Regex.Matches(booklist.Value,
                "<li>[\\s\\S]*?</li>");

            List<Book> result = new List<Book>();
            foreach (Match match in books)
            {
                Book book = new Book(match.Value);
                book.GetResources();
                result.Add(book);
            }
            ShowResult(result);
        }

        static void SearchByISBN(string ISBN)
        {

        }

        static void ShowResult(List<Book> result)
        {
            foreach(Book book in result)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("*{0}\n\t作者：{1}\n\t出版商：{2}\n\t出版年份:{3}",
                    book.title,book.author,book.publisher,book.date);
                Console.ForegroundColor = ConsoleColor.White;
                foreach(BookResource res in book.resource)
                {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("\t\t条形码：\t" + res.barcode);
                    Console.WriteLine("\t\t索书码：\t" + res.callcode);
                    Console.WriteLine("\t\t所在地：\t" + res.location);
                    if (Regex.IsMatch(res.state, "已[\\S\\s]*"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\t\t状态：\t\t" + res.state);
                        Console.ForegroundColor = ConsoleColor.White;
                    }else
                        Console.WriteLine("\t\t状态：\t\t" + res.state);
                    Console.WriteLine("\t\t卷次：\t\t" + res.publishNO);
                    Console.WriteLine("\t\t类型：\t\t" + res.type);
                    Console.WriteLine("\t\t具体位置：\t" + res.place);
                }
            }
        }
    }
}
