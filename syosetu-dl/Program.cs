﻿using HtmlAgilityPack;
using System.Text;

namespace syosetu_dl {
    internal class Program {
        public static void WriteText(ref string filePath, ref string content) {
            try {
                if (!File.Exists(filePath)) {
                    string? directoryPath = Path.GetDirectoryName(filePath);
                    if (directoryPath == null) throw new Exception("GetDirectoryName error");
                    if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                    using (FileStream fs = File.Create(filePath)) {
                        byte[] contents = new UTF8Encoding(true).GetBytes(content);
                        fs.Write(contents, 0, contents.Length);
                    }
                    Console.WriteLine($"{filePath}:已写入.");
                } else Console.WriteLine("文件已存在.");
            } catch (Exception ex) {
                Console.WriteLine("发生错误: " + ex.Message);
            }
        }
        static async Task Main(string[] args) {
            if (args.Length == 0) return;
            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                client.DefaultRequestHeaders.Add("Cookie", "over18=yes");
                string base_url = args[0];
                int from = int.Parse(args[1]);
                int to = int.Parse(args[2]);
                for (int i = from; i <= to; ++i) {
                    HttpResponseMessage response = await client.GetAsync($"{base_url}{i}/");
                    string htmlContent = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlContent);
                    var pTags = doc.DocumentNode.SelectNodes("//p[starts-with(@id, 'L')]");
                    StringBuilder str = new StringBuilder();
                    foreach (var p in pTags) {
                        str.Append(p.InnerText);
                        str.Append('\n');
                    }
                    string result = str.ToString();
                    string file = $"{args[3]}\\Chapter-{i}.txt";
                    WriteText(ref file, ref result);
                }
            }
        }
    }
}
