using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PieskiLib
{
    class Book
    {
        internal string title;
        internal string author;
        internal string publisher;
        internal string date;
        internal string link;
        internal List<BookResource> resource = new List<BookResource>();
        internal Dictionary<string, BookResource> lib_resource = new Dictionary<string, BookResource>();
        
        internal Book(string html)
        {
            string ori_link = Regex.Match(html, "href=\\\"[\\s\\S]*?\\\"").Value;
            string ori_tittle = Regex.Match(html, "title\\s=\\s\\\"[\\S\\s]*?\\\"").Value;
            string ori_author = Regex.Match(html, "作 者：[\\s\\S]*?<").Value;
            string ori_date = Regex.Match(html, "出版年：[\\s\\S]*?<").Value;
            string ori_publisher = Regex.Match(html, "出版者：[\\s\\S]*?<").Value;

            this.link = "https://www.szlib.org.cn/Search/" + ori_link.Split('"')[1];
            this.title = ori_tittle.Split('"')[1];
            this.author = ori_author.Replace("作 者：", "").Replace("<","");
            this.date = ori_date.Replace("出版年：","").Replace("<", "");
            this.publisher = ori_publisher.Replace("出版者：", "").Replace("<", "");
        }

        internal Book()
        {

        }

        internal void GetResources()
        {
            WebRequest request = new WebRequest(link);
            string html = request.Download();
            MatchCollection books = Regex.Matches(html, "<tr><td>[\\s\\S]*?</tr>");
            foreach (Match book in books)
            {
                BookResource res = new BookResource(book.Value);
                resource.Add(res);
            }
        }
    }

    class BookResource
    {
        internal string barcode;            //条码
        internal string callcode;           //索书码
        internal string location;           //所在图书馆
        internal string state;              //状态
        internal string publishNO;          //卷次
        internal string type;               //类型  
        internal string place;              //具体位置

        internal BookResource(string html)
        {
            MatchCollection tdinfos = Regex.Matches(html, "<td[\\s\\S]*?</td>");      //获取表格中储存的信息
            this.barcode = tdinfos[0].Value.Replace("<td>", "").Replace("</td>", "");
            this.callcode = tdinfos[1].Value.Replace("<td>","").Replace("<fond face='Courier'>", "").Replace("<font face='Courier'>", "").Replace("</font>", "").Replace("</td>","");
            this.location = tdinfos[2].Value.Replace("<td>", "").Replace("</td>", "");
            this.state = tdinfos[3].Value.Replace("<td>", "").Replace("</td>", "");
            if (state != "0")
            {
                this.publishNO = tdinfos[4].Value.Replace("<td>", "").Replace("</td>", "");
                this.type = tdinfos[5].Value.Replace("<td>", "").Replace("</td>", "");
                this.place = tdinfos[6].Value.Split('>')[1].Replace("</td", "").Replace("<script", "").Replace("&nbsp;", "");
            }
            else
            {
                this.state = "已借出，应还日期：" + tdinfos[5].Value.Split('>')[1].Replace("</td", "").Replace("<script", "").Replace("&nbsp;", "");
                this.publishNO = tdinfos[3].Value.Replace("<td>", "").Replace("</td>", "");
                this.type = tdinfos[4].Value.Replace("<td>", "").Replace("</td>", "");
            }
        }
    }
}
