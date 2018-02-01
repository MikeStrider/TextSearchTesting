using IFilterTextReader;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TextSearchTesting
{
    public partial class WebForm1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)  
            {
                int count = 0;
                List<ListItem> files = new List<ListItem>();
                List<ListItem> files2 = DirSearch(new DirectoryInfo(Server.MapPath("~/docFolder/")), files);
                foreach(ListItem x in files2)
                {
                    if (x.Value != "")
                    {
                        count = count + 1;
                    }
                }
                lblfilecount.Text = "File Count = " + count.ToString();
                GridView1.DataSource = files2;          // load files into right side and count them
                GridView1.DataBind();

            }
        }

        public static List<ListItem> DirSearch(DirectoryInfo sDir, List<ListItem> files)
        {
            foreach (DirectoryInfo d in sDir.GetDirectories("*.*", SearchOption.TopDirectoryOnly))  
            {
                files.Add(new ListItem(d.Name, ""));
                foreach (FileInfo f in d.GetFiles())
                {
                    files.Add(new ListItem(f.Name, (f.DirectoryName + "/" + f.Name)));
                }
                DirSearch(d, files);     // recursively get files and folders
            }
            return files;
        }

        protected void DownloadFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ListItem drv = (ListItem)e.Row.DataItem;
                if (drv.Value =="")
                {
                    System.Web.UI.WebControls.Image Image3 = (System.Web.UI.WebControls.Image)e.Row.FindControl("Image3");
                    LinkButton HyperLink1 = (LinkButton)e.Row.FindControl("lnkDownload");
                    HyperLink1.Visible = false;
                    Image3.Visible = true;
                }
            }
        }

        protected void btnIndex_Click(object sender, EventArgs e)
        {
            int count = 0;
            lblResults.Text = "";
            lblResults.Text = lblResults.Text + " Indexing Started ... <br>";
            Lucene.Net.Store.Directory indexDir = FSDirectory.Open(new DirectoryInfo(Server.MapPath("~/indexingFolder/")));
            Analyzer indexAnalyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            IndexWriter indexWriter = new IndexWriter(indexDir, indexAnalyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/docFolder/"));

            foreach (DirectoryInfo folder in dir.GetDirectories("*.*", SearchOption.AllDirectories))
            {
                foreach (FileInfo files in folder.GetFiles())
                {
                    TextReader reader = new FilterReader(files.FullName);
                    using (reader)
                    {
                        int Start = files.FullName.IndexOf("docFolder", StringComparison.CurrentCultureIgnoreCase);         //build the path that works
                        string startPath = files.FullName;                                                       
                        string startPath2 = startPath.Substring(Start, (startPath.Length - Start));
                        Document doc = new Document();
                        doc.Add(new Field("text", reader.ReadToEnd(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
                        doc.Add(new Field("filename", files.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
                        doc.Add(new Field("filepath", startPath2, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
                        indexWriter.AddDocument(doc);
                        count = count + 1;
                    }
                    lblResults.Text = lblResults.Text + "Directory: " + folder.ToString() + " :: " + files.Name + "<br>";
                }
            }
            indexWriter.Optimize();
            indexWriter.Dispose();
            lblResults.Text = lblResults.Text + " ... Indexing Complete :: Count = " + count + "<br>";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == "" || txtSearch.Text == "  " || txtSearch.Text == "   " || txtSearch.Text == " ")
            {
                lblError.Text = "Please enter something, then click search.";
            } else 
            {
                try
                {
                    lblError.Text = "";
                    lblResults.Text = "";
                    Lucene.Net.Store.Directory indexDir = FSDirectory.Open(new DirectoryInfo(Server.MapPath("~/indexingFolder/")));
                    Analyzer indexAnalyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                    IndexReader indexReader = IndexReader.Open(indexDir, true);
                    Searcher searcher = new IndexSearcher(indexReader);
                    QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "text", indexAnalyzer);
                    Query qry = parser.Parse(txtSearch.Text);
                    TopDocs resultDocs = searcher.Search(qry, indexReader.MaxDoc);
                    var hits = resultDocs.ScoreDocs;
                    foreach (var hit in hits)
                    {
                        var documentFromSearch = searcher.Doc(hit.Doc);
                        string textResults0 = documentFromSearch.Get("text");
                        string data = getBetween(textResults0, txtSearch.Text);
                        lblResults.Text = lblResults.Text + "" + "<div style='background-color:LightGray; margin-top: 7px;'>" + documentFromSearch.Get("filename") + 
                            " . . . <a href='" + documentFromSearch.Get("filepath") + "'> download</a> . . . Score: " + hit.Score + "</div>" + "..." + data + "...";
                    }
                }
                catch
                {
                    lblError.Text = "Please enter valid characters and try again.";
                }
            }
        }

        public static string getBetween(string strSource, string strSearch)
        {
            string x1;
            int Start;
            Start = strSource.IndexOf(strSearch, StringComparison.CurrentCultureIgnoreCase);
            if (Start < 0) Start = 0;
            string storedKeyword = strSource.Substring(Start, strSearch.Length);
            Start = Start - 200;
            if ((Start + 400) > strSource.Length) Start = strSource.Length - 400;
            if (Start < 0)
                Start = 0;
            if (strSource.Length < 400)
                x1 = strSource.Substring(Start, strSource.Length);
            else
                x1 = strSource.Substring(Start, 400);
            string x2 = "<font style='background-color:#ffd480;'>" + storedKeyword + "</font>";
            return Regex.Replace(x1, strSearch, x2, RegexOptions.IgnoreCase);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            lblResults.Text = "<h2> Help and About </h2> <div> Languages used: C# / ASP.NET / CSS / HTML / JavaScript </div><br><div> Library used: C# - Lucene.NET - <u>https://lucenenet.apache.org</u> (creates index and searches)<br><br> Library used: C# - IFilterTextReader - <u>https://github.com/Sicos1977/IFilterTextReader</u> (converts docs into raw text) <br><br> Library used: Javascript - JQuery - http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js (allows for loading animations)</div> ";
        }
    }
}