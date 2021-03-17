using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Varord.amParseQuestionsToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            //GetImages();
            GetHtmlAsync();
            Console.ReadLine();
        }

        private static async Task<List<List<Question>>> GetHtmlAsync()
        {
            var httpClient = new HttpClient();
            List<List<Question>> groups = new List<List<Question>>(10);

            for (int i = 0; i < 10; i++)
            {
                groups.Add(new List<Question>());
            }

            for (int i = 0; i < 63; i++)
            {
                int page = i + 1;
                var url = "https://varord.am/%D5%BF%D5%A5%D5%BD%D5%A1%D5%AF%D5%A1%D5%B6%D5%B6%D5%A5%D6%80/%D6%84%D5%B6%D5%B6%D5%A1%D5%AF%D5%A1%D5%B6-%D5%A9%D5%A5%D5%BD%D5%BF%D5%A5%D6%80/%D5%A9%D5%A5%D5%BD%D5%BF/" + page;

                var html = await httpClient.GetStringAsync(url);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var testQuestionsList = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("quizQest")).ToList();

                for (int j = 0; j < testQuestionsList.Count; j++)
                {
                    // var group = new List<Question>();

                    var question = new Question();
                    if (i == 54 && j == 8)
                    {
                        var questionText = testQuestionsList[j].Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "").Equals("clr")).FirstOrDefault();
                        question.questionText = questionText.InnerText;
                    }
                    else
                    {
                        var questionText = testQuestionsList[j].Descendants("p")
                        .Where(node => node.GetAttributeValue("class", "").Equals("testHarc")).FirstOrDefault();
                        question.questionText = questionText.InnerText;
                    }

                    var answers = testQuestionsList[j].Descendants("label");

                    var rightAnswer = answers.Where(a => a.InnerHtml.Contains("class=\"ra\"")).FirstOrDefault().InnerText;
                    question.answers = new List<string>() { rightAnswer };

                    foreach (var item in answers)
                    {
                        if (item.InnerHtml.Contains("wa"))
                        {
                            question.answers.Add(item.InnerText);
                        }
                    }

                    var clrDiv = testQuestionsList[j].Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "").Equals("clr")).FirstOrDefault();

                    if (clrDiv.FirstChild.Name == "img")
                    {
                        question.hasImage = true;
                        var imageUrl = "https://varord.am" + clrDiv.FirstChild.GetAttributeValue("src", "");
                        using (var webClient = new WebClient())
                        {
                            byte[] bytes = webClient.DownloadData(imageUrl);
                            string imageName = $"{j}_{groups[j].Count}";
                            if (imageUrl.Contains("newTestImg"))
                            {
                                imageName += "_water";
                            }
                            File.WriteAllBytes($@"C:\Users\beant\Desktop\Images\{imageName}.png", bytes);
                        }
                    }
                    //var testQuestionsList = htmlDocument.DocumentNode.Descendants("div")
                    //.Where(node => node.GetAttributeValue("class", "").Equals("clr")).ToList();
                    //group.Add(question);

                    groups[j].Add(question);
                }
            }
            var json = JsonConvert.SerializeObject(groups);
            File.WriteAllText(@"C:\Users\beant\Desktop\path.json", json);
            return groups;
        }

        public static async void GetImages()
        {
            var httpClient = new HttpClient();

            var url = "https://varord.am/%D5%BF%D5%A5%D5%BD%D5%A1%D5%AF%D5%A1%D5%B6%D5%B6%D5%A5%D6%80/%D6%84%D5%B6%D5%B6%D5%A1%D5%AF%D5%A1%D5%B6-%D5%A9%D5%A5%D5%BD%D5%BF%D5%A5%D6%80/%D5%A9%D5%A5%D5%BD%D5%BF/1";

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var testQuestionsList = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("quizQest")).ToList();

            var imageInfo = testQuestionsList[3].ChildNodes.ToList();


            Console.WriteLine();
        }
    }
}
