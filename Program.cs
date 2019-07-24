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
        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Loading completed");
            Console.Write("---------------------------\n" +
                          "|1）Name|2）Author|3）ISBN|\n" +
                          "---------------------------\n");
            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Enter book name:");
                    SearchByName(Console.ReadLine());
                    break;
                case "2":
                    Console.Write("Enter author:");
                    SearchByAuthor(Console.ReadLine());
                    break;
                case "3":
                    Console.Write("Enter ISBN:");
                    SearchByISBN(Console.ReadLine());
                    break;
            }
#endif
            Console.ReadKey();
        }

        static void SearchByName(string name)
        {
            string path = "https://www.szlib.org.cn/Search/searchshow.jsp?" +
                "v_tablearray=bibliosm%2Cserbibm%2Capabibibm%2Cmmbibm%2C&" +
                "v_index=title&v_value="+name+"+&cirtype=&" +
                "v_startpubyear=&v_endpubyear=&v_publisher=&v_author=&" +
                "sortfield=ptitle&sorttype=desc";
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
        }

        static void SearchByISBN(string ISBN)
        {

        }

        static void ShowResult(List<Book> result)
        {
            foreach(Book book in result)
            {
                Console.WriteLine("*{0}\n\t作者：{1}\n\t出版商：{2}\n\t出版年份:{3}",
                    book.title,book.author,book.publisher,book.date);
                foreach(BookResource res in book.resource)
                {

                    Console.WriteLine("\t\t条形码：\t" + res.barcode);
                    Console.WriteLine("\t\t索书码：\t" + res.callcode);
                    Console.WriteLine("\t\t所在地：\t" + res.location);
                    Console.WriteLine("\t\t状态：\t\t" + res.state);
                    Console.WriteLine("\t\t卷次：\t\t" + res.publishNO);
                    Console.WriteLine("\t\t类型：\t\t" + res.type);
                    Console.WriteLine("\t\t具体位置：\t" + res.place);
                }
            }
        }
    }
}
