using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Quran.DAL.Implementations;
using Quran.Repository;
using Quran.Model;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.ComponentModel;
using System.Threading;
using QuranEntryUI.Interfaces;

namespace QuranEntryUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        #region Fields

        IQuran iQuran = null;
        BackgroundWorker bgWorker = new BackgroundWorker();

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /*private PetaPoco.Database DBContext
        {
            get
            {
                if (DBContext_ == null)
                {
                    DBContext_ = dbFactory.Get();
                }

                return DBContext_;
            }
        }

        private void LoadChaptersFromDB()
        {
            try
            {
                if (DBContext != null)
                {
                    ChapterRepository cr = new ChapterRepository(dbFactory);
                    foreach (var chapter in cr.GetChapters())
                    {
                        quranData.AddChapter(new ChapterData(chapter));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }

        private void LoadQuranFromXml(string xmlFile)
        {
            Commit.IsEnabled = false;
            selectQuranFile.IsEnabled = false;

            XDocument doc = XDocument.Load(xmlFile);
            var quran = doc.XPathSelectElement("//Quran");
            bool failed = false;

            if (quran != null)
            {
                
                quranData.language_ = Convert.ToInt32(quran.Attribute("language").Value);
                quranData.scriptId_ = (byte) (quran.Attribute("script").Value.Equals("Usmani") ? 1 : 2);
                quranData.translator_ = Convert.ToInt32(quran.Attribute("translator").Value);

                Debug.WriteLine(string.Format("Processing Quran data: Language: {0} Script: {1} Translator: {2}.", quranData.language_, quran.Attribute("script").Value, quranData.translator_));

                var chapters = from elem1 in quran.Descendants("Chapter")
                               where elem1 != null
                               select elem1;

                if (chapters != null)
                {
                    this.Cursor = Cursors.Wait;
                    try
                    {
                        foreach (var chapter in chapters)
                        {
                            string name = chapter.Attribute("name").Value;
                            byte id = Convert.ToByte(chapter.Attribute("id").Value);
                            int totalAyahs = Convert.ToInt32(chapter.Attribute("total_ayahs").Value);
                            bool revPlace = chapter.Attribute("revelation_place").Value.Equals("Makki")? false:true;
                            string text = chapter.Value;

                            //get the initial related chapter data from DB
                            ChapterData cd = quranData.chapterDict[id];

                            //update the chapter revelation place info
                            cd.chapter_.RevPlace = revPlace;

                            //in case of quran translation
                            if (quranData.language_ > 1)
                            {
                                cd.lsChapter.Id = id;
                                cd.lsChapter.Name = name;
                                cd.lsChapter.TransId = quranData.translator_;
                                cd.lsChapter.Description = string.Format("Auto added at '{0}'.", DateTime.Now.ToString()); //comments as description
                            }

                            //process the surah text by splitting it into verses
                            if (failed = !ProcessSurahText(cd, text))
                            {
                                Debug.WriteLine(string.Format("Failed to update chapter '{0}' data.", name));
                                break;
                            }

                            bgWorker.ReportProgress((int)((id/114)*100));

                            Thread.Sleep(10);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        failed = true;
                    }
                    finally
                    {
                        this.Cursor = Cursors.Arrow;
                    }
                }
            }

            if (failed)
            {
                Debug.WriteLine("########### Quran data processing aborted ################");
            }
            else
            {
                Debug.WriteLine("Quran data processed successfully.");
            }
        }

        private bool ProcessSurahText(ChapterData selChapter, string text)
        {
            if (!selChapter.isValid || selChapter.chapter_ == null)
            {
                Debug.WriteLine("Can't process empty chapter object.");
                return false;
            }

            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                Debug.WriteLine(string.Format("Can't process empty text for Surah '{0} - {1}'.", selChapter.chapter_.Id, selChapter.chapter_.Name));
                return false;
            }

            //trim spaces from name if any
            selChapter.chapter_.Name = selChapter.chapter_.Name.Trim().Trim(Convert.ToChar(32)).Trim();

            Debug.WriteLine(string.Format("Processing Surah '{0} - {1}' ayahs '{2}' which is '{3}'.", selChapter.chapter_.Id, selChapter.chapter_.Name, selChapter.chapter_.TotalAyahs, (selChapter.chapter_.RevPlace) ? "Madani" : "Makki"));

            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream(byteArray);
            // convert stream to string
            StreamReader reader = new StreamReader(stream);
            string surahText = reader.ReadToEnd();
            
            //sajda and ruku count
            int sajdaCount = surahText.Length - surahText.Replace("۩", "").Length;
            int rukuCount = surahText.Length - surahText.Replace("۞", "").Length;

            if (!string.IsNullOrEmpty(surahText))
            {
                string[] ayahs = Regex.Split(surahText, @"\p{Z}*\p{P}+\p{C}*\p{N}+\p{C}*\p{P}+\p{Z}*");
                //don't use the last item which is end of paragraph
                int ayahCount = (ayahs[ayahs.Length - 1].Length == 0 || ayahs[ayahs.Length - 1].Length == 1) ? ayahs.Length - 1 : ayahs.Length;

                if (selChapter.chapter_.TotalAyahs == ayahCount)
                {
                    for (int i = 0; i < ayahCount; i++)
                    {
                        string ayah = ayahs[i].Trim().Trim(Convert.ToChar(32)).Trim();

                        //check sajda
                        if (sajdaCount > 0 &&
                            (ayah.Length - ayah.Replace("۩", "").Length) > 0)
                        {
                            ayah = ayah.Replace("۩", "");
                            selChapter.sajdas_.Add(i + 1);
                        }

                        //Raku check
                        if (rukuCount > 0 &&
                            (ayah.Length - ayah.Replace("۞", "").Length) > 0)
                        {
                            ayah = ayah.Replace("۞", "");
                            selChapter.rukus_.Add(i + 1);
                        }

                        if (quranData.language_ == 1) //Arabic text only
                        {
                            if (selChapter.AddVerse((short)(i + 1), //verse Id
                                ayah, // ayah text
                                quranData.scriptId_, //Usmani script
                                string.Format("Auto added at '{0}'.", DateTime.Now.ToString()) //comments as description
                                ) == false)
                            {
                                Debug.WriteLine(string.Format("Failed to insert verse '{0}' text '{1}'.", (i + 1), ayah));
                                break;
                            }
                        }
                        else //Translations only
                        {
                            IVerseRepository verseRepo = new VerseRepository(dbFactory);
                            var dbVerse = verseRepo.Get(v => v.Ayah == (short)(i + 1) && v.Chapter == selChapter.chapter_.Id);

                            //if found the arabic quran verse then proceed with translation otherwise halt
                            if (dbVerse != null)
                            {
                                if (selChapter.AddLSVerse(dbVerse.Id, //verse Id
                                    quranData.translator_, //Translation id
                                    ayah, // ayah text
                                    string.Format("Auto added at '{0}'.", DateTime.Now.ToString()) //comments as description
                                    ) == false)
                                {
                                    Debug.WriteLine(string.Format("Failed to insert translated verse '{0}' text '{1}'.", (i + 1), ayah));
                                    break;
                                }
                            }
                            else
                            {
                                Debug.WriteLine(string.Format("Failed to find arabic verse id for translated verse '{0}'.", (i + 1)));
                                break;
                            }
                        }
                    }

                    if (!selChapter.IsValidVersesCount)
                    {
                        Debug.WriteLine(string.Format("Surah '{0}' ayahs count '{1}' is not valid.", selChapter.chapter_.Name, ayahCount));
                        selChapter.verses_.Clear();
                    }
                    else
                    {
                        selChapter.isValid = true;
                        return true;
                    }
                }
                else
                {
                    Debug.WriteLine(string.Format("Surah '{0}' ayahs count '{1}' is not valid.", selChapter.chapter_.Name, ayahCount));
                }
            }

            selChapter.isValid = false;
            return false;
        }*/

        #endregion

        #region Event Handlers

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Commit.IsEnabled = true;
            selectQuranFile.IsEnabled = true;
        }

        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progress.Value = e.ProgressPercentage;
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Action action = () => iQuran = QuranProcessor.ProcessXml(e.Argument.ToString());
            if (Thread.CurrentThread != Dispatcher.Thread)
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, action);
            }
            else
            {
                action();
            }
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            QuranDownloader.StartDownloadingQuran(@"E:\Projects\Khilafah\Data\Quran_XmlData");

            /*string xmlFilePath = quranXmlFilePath.Text;

            if (!string.IsNullOrEmpty(xmlFilePath))
            {
                bgWorker.DoWork +=
            new DoWorkEventHandler(bgWorker_DoWork);
                bgWorker.ProgressChanged +=
                    new ProgressChangedEventHandler(bgWorker_ProgressChanged);
                bgWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
                bgWorker.WorkerReportsProgress = true;

                progress.Value = 0;
                progress.Visibility = Visibility.Visible;

                bgWorker.RunWorkerAsync(quranXmlFilePath.Text);
            }
            else
            {
                MessageBox.Show("No xml file selected to process.");
            }*/
        }

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            iQuran.CommitChanges();
        }

        private void selectQuranFile_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Quran"; // Default file name 
            dlg.DefaultExt = ".xml"; // Default file extension 
            dlg.Filter = "Quran (.xml)|*.xml"; // Filter files by extension 

            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                quranXmlFilePath.Text = dlg.FileName;
            }
        }

        #endregion

        #region Commented Code

        /*private void ProcessSurahText()
        {
            string surahText = surahTextbox.Text;
            surahText = surahText.Replace("(", "");
            surahText = surahText.Replace(")", "");
            //surahText = surahText.Replace(")", "");
            int sajdaCount = surahText.Length - surahText.Replace("۩", "").Length;
            surahText = surahText.Replace("۩", "");

            int rukuCount = surahText.Length - surahText.Replace("۞", "").Length;
            surahText = Regex.Replace(surahText, "۞", "");

            if (!string.IsNullOrEmpty(surahText))
            {
                string[] ayahs = Regex.Split(surahText, @"\p{N}");

                //TODO: 
                //1- Remove leading and trailing spaces
                //2- Remove special character like '۞' {1758}
                //
                for (int i = 0; i < ayahs.Length; i++)
                {
                    if (string.IsNullOrEmpty(ayahs[i]))
                    {
                        
                    }
                    else if (ayahs[i].Length == 1)
                    {
                    }
                    else if (ayahs[i].Length == 0)
                    {
                    }

                    //AyahsListview.Items.Add(ayahs[i]);
                }
            }
        }*/

        /*private void LoadControllFromDB()
        {
            try
            {
                DBContext = dbFactory.Get();
                if (DBContext != null)
                {
                    ChapterRepository cr = new ChapterRepository(dbFactory);
                    foreach (var chapter in cr.GetChapters())
                    {
                        SurahsCombobox.Items.Add(chapter.Id.ToString() + "- " + chapter.Name);
                        quranData.AddChapter(new ChapterData(chapter));
                    }
                    SurahsCombobox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }*/

        /*private void WriteQuranTemplateXml()
        {
            SurahTypeCombobox.Items.Add("Makki");
            SurahTypeCombobox.Items.Add("Madani");

            try
            {
                using (TextWriter tw = new StringWriter())
                {
                    using (XmlTextWriter xtw = new XmlTextWriter(tw))
                    {
                        xtw.Formatting = Formatting.Indented;
                        xtw.WriteStartElement("Quran");
                        xtw.WriteAttributeString("language", "ar");
                        xtw.WriteAttributeString("script", "Usmani");

                        xtw.WriteStartElement("Chapters");

                        DBContext = dbFactory.Get();
                        if (DBContext != null)
                        {
                            ChapterRepository cr = new ChapterRepository(dbFactory);
                            foreach (var chapter in cr.GetChapters())
                            {
                                xtw.WriteStartElement("Chapter");
                                xtw.WriteAttributeString("name", chapter.Name);
                                xtw.WriteAttributeString("id", chapter.Id.ToString());
                                xtw.WriteAttributeString("revelation_place", "Makki");
                                xtw.WriteAttributeString("total_ayahs", chapter.TotalAyahs.ToString());

                                SurahsCombobox.Items.Add(chapter.Id.ToString() + "- " + chapter.Name);
                                quranData.AddChapter(new ChapterData(chapter));

                                xtw.WriteEndElement();
                            }

                            SurahsCombobox.SelectedIndex = 0;
                        }

                        xtw.WriteEndElement();
                        xtw.WriteEndElement();
                        xtw.Flush();
                    }

                    System.IO.File.WriteAllText(@"E:\Projects\Quran_Data\Quran-Arabic-Auto.xml", tw.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }*/

        #endregion
    }
}
