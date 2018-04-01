using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using Google.Cloud.Vision.V1;
using Tesseract;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
namespace GoogleSearchConsole
{
    class Program
    {
        private static void Main(string[] args)
        {
            while(true)
            {
                Console.ReadKey();
                var watch = System.Diagnostics.Stopwatch.StartNew();

                //As soon as we create the class, constructor clicks the image of the question
                QuestionImage questionImage = new QuestionImage();

                //gets result of the OCR
                OcrResult questionImgResult = MyFunction(@"E:\foo\screencap1.png").GetAwaiter().GetResult();
                Console.WriteLine(questionImgResult.Text);

                //For Answer
                AnswerImage answerImage = new AnswerImage();

                //gets result of the answer OCR
                OcrResult answerImgResult = MyFunction(@"E:\foo\screencap2.png").GetAwaiter().GetResult();
                Console.WriteLine(answerImgResult.Text);

                //string builders to add the words
                var questionMaker = new System.Text.StringBuilder();

                foreach (var line in questionImgResult.Lines) //For each line this will execute
                {
                    foreach (OcrWord word in line.Words) //now every word in that line
                    {
                        questionMaker.AppendFormat("{0} ", word.Text);
                    }
                }
                string query = questionMaker.ToString(); //this is our query

                var answerOneMaker = new System.Text.StringBuilder();
                var answerTwoMaker = new System.Text.StringBuilder();
                var answerThreeMaker = new System.Text.StringBuilder();
                var answerFourMaker = new System.Text.StringBuilder();

                //bools that helps to parse the questions and answers from OcrResult
                bool answerOnefound = false;
                bool answerTwoFound = false;
                bool answerThreeFound = false;
                bool answerFourFound = false;

                //number of words in an answer
                int answerOneWords = 0;
                int answerTwoWords = 0;
                int answerThreeWords = 0;
                int answerFourWords = 0;

                foreach (var line in answerImgResult.Lines)
                {
                    if (answerOnefound != true) // if first question is not found
                    {
                        foreach (OcrWord word in line.Words) //adds every word to the first answerMaker
                            answerOneMaker.AppendFormat("{0} ", word.Text);
                        answerOnefound = true; //makes first question found true
                    }

                    else if (answerOnefound == true && answerTwoFound != true) //will only run when question marks is found, answer one is found and answer two isn't found yet
                    {
                        foreach (OcrWord word in line.Words)
                            answerTwoMaker.AppendFormat("{0} ", word.Text);
                        answerTwoFound = true; // question two is found
                    }
                    else if (answerTwoFound == true && answerThreeFound != true) //will only run when question marks is found, answer one is found and answer two isn't found yet
                    {
                        foreach (OcrWord word in line.Words)
                            answerThreeMaker.AppendFormat("{0} ", word.Text);
                        answerThreeFound = true;
                    }
                    else if (answerThreeFound == true && answerFourFound != true)
                    {
                        foreach (OcrWord word in line.Words)
                            answerFourMaker.AppendFormat("{0} ", word.Text);
                        answerFourFound = true;
                    }
                }
                string answer1 = answerOneMaker.ToString().Trim().ToLower(); //removes any spaces before and after, lowers as well
                string answer2 = answerTwoMaker.ToString().Trim().ToLower();
                string answer3 = answerThreeMaker.ToString().Trim().ToLower();
                string answer4 = answerFourMaker.ToString().Trim().ToLower();

                Console.WriteLine(answer1);
                Console.WriteLine(answer2);
                Console.WriteLine(answer3);
                Console.WriteLine(answer4);


                const string apiKey = "AIzaSyBPz2h15DtLjvDLdOQogoODIZvEf9Egysg";
                const string searchEngineId = "006514251126006113086:nqq3qo7cn1u";

                int countAnswer1 = 0;
                int countAnswer2 = 0;
                int countAnswer3 = 0;
                int countAnswer4 = 0;
                var customSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = apiKey });
                var listRequest = customSearchService.Cse.List(query);
                listRequest.Cx = searchEngineId;

                Console.WriteLine("Start...");
                IList<Result> paging = new List<Result>();
                var count = 0;
                while (count < 2)
                {
                    Console.WriteLine($"Page {count}");
                    listRequest.Start = count * 10 + 1;
                    paging = listRequest.Execute().Items;
                    if (paging != null)
                        foreach (var item in paging)
                        {
                            foreach (var text in item.Snippet.Split(' '))
                            {
                                string foundWords = text.Trim().ToLower();
                                foundWords = foundWords.Replace(",", "").Replace("_", "");

                                foreach (string possibleAnswer in answer1.Split(' '))
                                {
                                    if (possibleAnswer.Equals(foundWords))
                                        countAnswer1++;
                                }

                                foreach (var possibleAnswer in answer2.Split(' '))
                                {
                                    if (possibleAnswer.Equals(foundWords))
                                        countAnswer2++;
                                }

                                foreach (var possibleAnswer in answer3.Split(' '))
                                {
                                    if (possibleAnswer.Equals(foundWords))
                                        countAnswer3++;
                                }

                                foreach (var possibleAnswer in answer4.Split(' '))
                                {
                                    if (possibleAnswer.Equals(foundWords))
                                        countAnswer4++;
                                }
                                //if (answer1.Contains(foundWords))
                                //    countAnswer1++;
                                //if (answer2.Contains(foundWords))
                                //    countAnswer2++;
                                //if (answer3.Contains(foundWords))
                                //    countAnswer3++;
                                //if (answer4.Contains(foundWords))
                                //    countAnswer4++;
                            }
                        }
                    count++;
                }
                Console.WriteLine("Done.");
                Console.WriteLine("Answer 1: " + answer1 + " " + countAnswer1);
                Console.WriteLine("Answer 2: " + answer2 + " " + countAnswer2);
                Console.WriteLine("Answer 3: " + answer3 + " " + countAnswer3);
                Console.WriteLine("Answer 4: " + answer4 + " " + countAnswer4);

                int answerMaxCount = Math.Max(Math.Max(Math.Max(countAnswer1, countAnswer2), countAnswer3), countAnswer4);
                string answer = "";
                if (answerMaxCount == countAnswer1)
                    answer = answer1;
                if (answerMaxCount == countAnswer2)
                    answer = answer2;
                if (answerMaxCount == countAnswer3)
                    answer = answer3;
                if (answerMaxCount == countAnswer4)
                    answer = answer4;
                Console.WriteLine(" \t Answer is : {0}", answer);

                //opens browser
                //Process.Start("https://www.google.com/search?q=" + Uri.EscapeDataString(query));

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("Time it took to execute the whole thing : {0}", elapsedMs.ToString());
            }
        }

        //OCR Function
        public static async Task<OcrResult> MyFunction(string fileURL)
        {
            var ocrEngine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en-us"));
            if (ocrEngine == null)
            {
                throw new InvalidOperationException("engine was null");
            }
            FileStream imageStream = File.Open(fileURL, FileMode.Open);
            var decoder = await BitmapDecoder.CreateAsync(imageStream.AsRandomAccessStream());
            var softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            var ocrResult = await ocrEngine.RecognizeAsync(softwareBitmap);
            imageStream.Dispose();
            return ocrResult;
        }
    }
}
