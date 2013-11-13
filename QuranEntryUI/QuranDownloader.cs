using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;
using System.Threading;

namespace QuranEntryUI
{
    public class QuranDownloader
    {
        private static string[] Quran_Translations = new[] {
            //"Deutsch",
	        "Eng-Abdul Daryabadi",
	        "Eng-Dr. Mohsin",
	        "Eng-Mufti Taqi Usmani",
	        "Eng-Pickthal-Audio",
	        "Eng-Yusuf Ali",
	        "French",
	        "Indonesian",
	        "Malaysian",
	        "Spanish",
	        "Turkish",
	        "Urdu-Ahmed Ali",
	        "Urdu-Ashraf Thanwi",
	        "Urdu-Jalandhry-Audio",
        };

        private static string[] Quran_Scripts = new string[] {
	        "Usmani",
	        "IndoPak"
        };

        private static string GetLanguage(string translation)
        {
            string result = "Arabic";
            switch (translation)
            {
                case "Deutsch":
                    result = "Deutsch";
                    break;
                case "Eng-Abdul Daryabadi":
                case "Eng-Dr. Mohsin":
                case "Eng-Mufti Taqi Usmani":
                case "Eng-Pickthal-Audio":
                case "Eng-Yusuf Ali":
                    result = "English";
                    break;
                case "French":
                    result = "French";
                    break;
                case "Indonesian":
                    result = "Indonesian";
                    break;
                case "Malaysian":
                    result = "Malaysian";
                    break;
                case "Spanish":
                    result = "Spanish";
                    break;
                case "Turkish":
                    result = "Turkish";
                    break;
                case "Urdu-Ahmed Ali":
                case "Urdu-Ashraf Thanwi":
                case "Urdu-Jalandhry-Audio":
                    result = "Urdu";
                    break;
                default:
                    result = "Arabic";
                    break;
            }

            return result;
        }

        public static void StartDownloadingQuran(string destination)
        {
            //string outputFile = @"E:\Personal\WinData\Documents\QEQuran_Fatiha.html";
            //string url = @"http://quranexplorer.com/quran/Print/default.aspx?Sura=1&FromVerse=1&ToVerse=0&Script=Usmani&Translation=Urdu-Ahmed Ali&TajweedRules=0&Zoom=100";
            //url = @"http://quranexplorer.com/quran/Print/default.aspx?Sura=1&FromVerse=1&ToVerse=300&Script=Usmani&Translation=Urdu-Ahmed Ali";
            //url = @"http://quranexplorer.com/quran";

            string xmlDestination = Path.Combine(destination, DateTime.Now.ToString("dd-MM-HH-mm-ss"));

            int fromVerse = 1;
            int toVerse = 286;
            string url = "http://quranexplorer.com/quran/Print/default.aspx?Sura={0}&FromVerse={1}&ToVerse={2}&Script={3}&Translation={4}";

            Debug.WriteLine(string.Format("Download quran data started."));

            foreach (string script in Quran_Scripts)
            {
                Debug.WriteLine(string.Format("Downloading '{0}' quran data.", script));
                
                string selTranslation = "";
                using (StringWriter StrWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(StrWriter))
                    {
                        foreach (string translation in Quran_Translations)
                        {
                            selTranslation = translation;
                            string language = GetLanguage(translation);
                            Debug.WriteLine(string.Format("Download quran for language {0} and translation {1}", language, translation));

                            xmlWriter.WriteStartElement("Quran");
                            xmlWriter.WriteAttributeString("script", script);
                            xmlWriter.WriteAttributeString("language", language);
                            xmlWriter.WriteAttributeString("translation", translation);

                            xmlWriter.WriteStartElement("Chapters");

                            for (int sura = 1; sura <= 114; sura++)
                            {
                                //for sura Fatiha take care of Basmallah as added first ayah
                                if (sura == 1)
                                    fromVerse = 2;
                                else
                                    fromVerse = 1;

                                string nreUrl = string.Format(url, sura, fromVerse, toVerse, script, translation);
                                Debug.WriteLine(string.Format("Download chapter {0} from {1}", sura.ToString(), nreUrl));
                                string htmlQuran = DownloadPage(nreUrl);

                                Thread.Sleep(20);
                                

                                if (!ExtractText(htmlQuran, sura.ToString(), xmlWriter, language))
                                {
                                    Debug.WriteLine(string.Format("############## Error: failed to download chapter {0} for language {1} and translation {2} #####################", sura.ToString(), language, translation));
                                }
                            }
                            xmlWriter.WriteEndElement(); //End Chapters Element

                            xmlWriter.WriteEndElement(); //End Quran Element
                        }
                        xmlWriter.Flush();
                    }
                    StrWriter.Flush();
                    File.WriteAllText(Path.Combine(xmlDestination, string.Format("{0}_{1}_Quran.xml", selTranslation, script)), StrWriter.ToString());
                }
            }

            Debug.WriteLine(string.Format("Download quran data finished."));
        }

        private static bool ExtractText(string htmlQuran, string chapterId, XmlWriter xmlWriter, string language)
        {
            bool result = false;
            try
            {
                HtmlDocument xDoc = new HtmlDocument();
                xDoc.LoadHtml(htmlQuran);
                var rows = from elem in xDoc.DocumentNode.Descendants(@"td") select elem;

                xmlWriter.WriteStartElement("Chapter");

                foreach (var row in rows)
                {
                    var crows = from celem in row.Descendants("font")
                                where string.IsNullOrEmpty(celem.InnerHtml) == false && string.Equals(celem.Attributes["class"].Value, language)
                                select celem.InnerHtml;

                    if (crows.Count() > 0)
                    {
                        int total_Ayahs = 0;
                        StringBuilder ayahs = new StringBuilder();
                        foreach (var crow in crows)
                        {
                            /*if (((language.Equals("Arabic") && crow.Contains("ِسۡمِ ٱللهِ ٱلرَّحۡمَـٰنِ ٱلرَّحِيمِ"))
                                || (!language.Equals("Arabic") && crow.Contains("In the name of Allah, the Beneficent, the Merciful (1)"))) 
                                && total_Ayahs == 0)
                            {
                                continue;
                            }*/
                            string tmp = crow.Replace("BR", "br");
                            tmp = tmp.Replace("<br></span>", "</br></span>");
                            XDocument xd = XDocument.Parse(tmp);
                            ayahs.Append(xd.Root.FirstNode.ToString());
                            total_Ayahs++;                            
                        }

                        //we are done
                        if (total_Ayahs > 0)
                        {
                            xmlWriter.WriteAttributeString("name", "");
                            xmlWriter.WriteAttributeString("id", chapterId);
                            xmlWriter.WriteAttributeString("total_ayahs", total_Ayahs.ToString());
                            xmlWriter.WriteString(ayahs.ToString());
                            result = true;
                            break;
                        }
                    }
                }

                xmlWriter.WriteEndElement(); //End Chapter Element
            }
            catch //(System.Exception ex)
            {

            }
            return result;
        }

        private static string DownloadPage(string webURL, string strFilePath)
        {
            WebRequest myWebRequest = WebRequest.Create(webURL);
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream ReceiveStream = myWebResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(ReceiveStream, encode);

            string strResponse = readStream.ReadToEnd();
            StreamWriter oSw = new StreamWriter(strFilePath);
            oSw.WriteLine(strResponse);
            oSw.Close();
            readStream.Close();
            myWebResponse.Close();
            return strResponse;
        }

        private static string DownloadPage(string webURL)
        {
            WebRequest myWebRequest = WebRequest.Create(webURL);
            WebResponse myWebResponse = myWebRequest.GetResponse();
            //Stream ReceiveStream = myWebResponse.GetResponseStream();
            //Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            //StreamReader readStream = new StreamReader(ReceiveStream, encode);
            //return readStream.ReadToEnd();

            long len = myWebResponse.ContentLength;
            byte[] barr = new byte[len];
            myWebResponse.GetResponseStream().Read(barr, 0, (int)len);
            myWebResponse.Close();
            return Encoding.UTF8.GetString(barr);
        }
    }
}
