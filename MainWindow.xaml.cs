using ApiHunterSnovio.Hunter;
using ApiHunterSnovio.Snovio;
using CefSharp;
using CefSharp.Wpf;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Xml;
using Telerik.Windows.Controls;

namespace eBayDEParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            CefSettings settings = new CefSettings();
            settings.IgnoreCertificateErrors = false;
            //CefSharpSettings.ShutdownOnExit = true;
            CefSharpSettings.ConcurrentTaskExecution = true;

            if (!Cef.IsInitialized)
            {
                Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
            }

            InitializeComponent();
            gridView.ItemsSource = database.database;
            cont.IsEnabled = false;
            pause.IsEnabled = false;
            taskMax.IsEnabled = false;

            var s = new BrowserSettings();
            s.ImageLoading = CefState.Disabled;
            s.LocalStorage = CefState.Disabled;
            s.Databases = CefState.Disabled;

            Browser.BrowserSettings = s;
        }

        bool isEnd = false;
        public object syncLink = new object();

        public MyViewModel database = new MyViewModel();
        public List<Company> countQ = new List<Company>();


        object readwritefile = new object();
        string dyskToSave = "";

        public string Path
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);

                return path = path.Substring(0, path.LastIndexOf('/') + 1);
            }
        }

        public object lockObject = new object();
        public void SaveLog(string msg)
        {
            lock (lockObject)
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                path = path.Substring(0, path.LastIndexOf('/') + 1);

                using (StreamWriter writer = new StreamWriter(string.Format("{0}{1}", path, "log.txt"), true))
                {
                    writer.WriteLine(msg);
                }
            }
        }

        object[] mSyncWeb = new object[] { };
        public void ProcessExceptions(WebException ex)
        {
            process.Dispatcher.Invoke(() =>
            {
                lock (mSyncWeb)
                {
                    process.Text = ex.Message;
                }
            });
            SaveLog(ex.Message);
            if (ex.InnerException != null) SaveLog(ex.Message);
            if (ex.Response != null) SaveLog(ex.Response.ResponseUri.AbsolutePath);
            if (ex.Response != null)
            {
                using (var steam = ex.Response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(steam))
                    {
                        string json = reader.ReadToEnd();
                        SaveLog(json);

                    }
                }
            }
        }

        public Dictionary<string, Company> dicChecked = new Dictionary<string, Company>();

        public void Save(string dysk)
        {
            lock (readwritefile)
            {
                var fileNameOry = dysk;
                var fileNameNew = dysk + ".tmp";
                var backup = dysk + ".backup";
                if (File.Exists(fileNameNew)) File.Delete(fileNameNew);

                using (FileStream file = File.Open(fileNameNew, FileMode.CreateNew, FileAccess.Write))
                {
                    using (StreamWriter filewrite = new StreamWriter(file, UTF8Encoding.UTF8))
                    {
                        foreach (var item in dicChecked.Keys)
                        {
                            var compPerson = dicChecked[item];
                            filewrite.WriteLine(compPerson.ToString());  
                        }
                    }
                }
                File.Replace(fileNameNew, fileNameOry, backup);
            }
        }

        public void Append(Company comp, string dysk)
        {
            lock (readwritefile)
            {
                using (FileStream file = File.Open(dysk, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter filewrite = new StreamWriter(file, UTF8Encoding.UTF8))
                    {
                        filewrite.WriteLine(comp.ToString());
                    }
                }
            }
        }

        public bool ExistCompany(Company comp)
        {
            return dicChecked.ContainsKey(comp.Id);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Csv Files (*.txt,*.csv)|*.txt;*.csv";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string filename = dlg.FileName;
                    textbox1.Text = filename;
                    string endOfFile = string.Empty;
                    lock (readwritefile)
                    {
                        dyskToSave = textbox1.Text;
                        using (FileStream file = File.Open(textbox1.Text, FileMode.Open, FileAccess.Read))
                        {
                            using (StreamReader fileread = new StreamReader(file, Encoding.UTF8))
                            {
                                while (!fileread.EndOfStream)
                                {
                                    string line = fileread.ReadLine();
                                    string[] emails = line.Split(new string[] { ";" }, StringSplitOptions.None);

                                    if (emails.Length >= 8)
                                    {
                                        Company comp = new Company();
                                        comp.Name = emails[1];
                                        comp.NameOrg = emails[2];
                                        comp.Register = emails[3];
                                        comp.CreateBy = emails[4];
                                        comp.Category = emails[5];
                                        comp.Country = emails[6];
                                        comp.Id = emails[0];

                                        AddCompany(comp, filename, false);
                                    }
                                    else if(emails.Length == 1)
                                    {
                                        Company comp = new Company();
                                        comp.Name = string.Empty;
                                        comp.NameOrg = string.Empty;
                                        comp.Register = string.Empty;
                                        comp.Category = string.Empty;
                                        comp.Country = string.Empty;
                                        comp.Id = emails[0];

                                        AddCompany(comp, filename, false);
                                    }

                                    if ((emails.Length < 8 && emails.Length != 1 ) || (emails.Length > 15 && emails.Length != 0))
                                    {
                                        SaveLog($"Rekord ma zbyt dużo kolumn: {string.Join(";", emails)}");
                                    }
                                }
                            }
                        }
                    }

                    Count.Dispatcher.Invoke(() =>
                    {
                        SetInfo();
                    });
                }
                catch (Exception ex)
                {
                    SaveLog(ex.Message);
                    if (ex.InnerException != null) SaveLog(ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void SetInfo()
        {
            Dispatcher.Invoke(() =>
            {
                infoMsg.Text = $"W bazie danych znajduje się {dicChecked.Count} filmów.";
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textbox1.Text))
                {
                    MessageBox.Show(messageBoxText: "Podaj plik do, którego mam wrzucać wyniki wyszukiwania");
                }
                else
                {
                    if (!File.Exists(textbox1.Text))
                    {
                        MessageBox.Show(messageBoxText: "Podany plik nie istnieje. Poszukaj lepiej.");
                    }
                    else
                    {
                        csv.IsEnabled = false;
                        GetSite(textbox1.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message);
                if (ex.InnerException != null) SaveLog(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        public void AddCompany(Company comp, string dysk, bool save = false, bool asCompany = false)
        {

            Uri myUri = new Uri(comp.Id.StartsWith("http") ? comp.Id : "http://" + comp.Id);
            string host = myUri.AbsoluteUri.Replace("www.", "");
            comp.Id = host;

            Company source = null;
            bool isNewCompany = false;
            if (dicChecked.ContainsKey(comp.Id))
            {
                source = dicChecked[comp.Id];
            }
            else
            {
                isNewCompany = true;
                source = new Company();
                source.Id = comp.Id;
                source.Name = comp.Name;
                source.NameOrg = comp.NameOrg;
                source.Register = comp.Register;
                source.CreateBy = comp.CreateBy;
                source.Country = comp.Country;
                source.Category = comp.Category;
                dicChecked.Add(source.Id, source);

                if (save && (string.IsNullOrEmpty(source.Name) || string.IsNullOrEmpty(source.NameOrg) || string.IsNullOrEmpty(source.Register) || string.IsNullOrEmpty(source.Category) || string.IsNullOrEmpty(source.Country) || string.IsNullOrEmpty(source.CreateBy)))
                {
                    AddToDownload(source);
                }
            }

            if (source != null)
            {
                var p = dicChecked[comp.Id];
                bool isChange = false;

                if (p.Trim(p.Category) != p.Trim(comp.Category) && !string.IsNullOrEmpty(comp.Category))
                {
                    if (string.IsNullOrEmpty(p.Category))
                    {
                        p.Category = comp.Trim(comp.Category);
                        isChange = true;
                    }
                }

                if (p.Trim(p.Name) != p.Trim(comp.Name) && !string.IsNullOrEmpty(comp.Name))
                {
                    if (string.IsNullOrEmpty(p.Name))
                    {
                        p.Name = comp.Trim(comp.Name);
                        isChange = true;
                    }
                }

                if (p.Trim(p.NameOrg) != p.Trim(comp.NameOrg) && !string.IsNullOrEmpty(comp.NameOrg))
                {
                    if (string.IsNullOrEmpty(p.NameOrg))
                    {
                        p.NameOrg = comp.Trim(comp.NameOrg);
                        isChange = true;
                    }
                }

                if (p.Trim(p.Register) != p.Trim(comp.Register) && !string.IsNullOrEmpty(comp.Register))
                {
                    if (string.IsNullOrEmpty(p.Register))
                    {
                        p.Register = comp.Trim(comp.Register);
                        isChange = true;
                    }
                }

                if (p.Trim(p.Country) != p.Trim(comp.Country) && !string.IsNullOrEmpty(comp.Country))
                {
                    if (string.IsNullOrEmpty(p.Country))
                    {
                        p.Country = comp.Trim(comp.Country);
                        isChange = true;
                    }
                }

                if (p.Trim(p.CreateBy) != p.Trim(comp.CreateBy) && !string.IsNullOrEmpty(comp.CreateBy))
                {
                    if (string.IsNullOrEmpty(p.CreateBy))
                    {
                        p.CreateBy = comp.Trim(comp.CreateBy);
                        isChange = true;
                    }
                }


                if (!string.IsNullOrEmpty(p.NameOrg) || !string.IsNullOrEmpty(p.Name) || !string.IsNullOrEmpty(p.Country) || !string.IsNullOrEmpty(p.Category) || !string.IsNullOrEmpty(p.CreateBy) || !string.IsNullOrEmpty(p.Register))
                {
                    p.IsChecked = true;
                }
                if (p.IsChecked == false)
                {
                    AddToDownload(p);
                }
                
                if (save && isChange) Save(dysk);
            }

            if (!this.database.database.Contains(source)) this.database.Add(source);
        }

        public void AddToDownload(Company company)
        {
            try
            {
                if (!this.ExistCompany(company) || company.IsChecked == false)
                {
                    countQ.Add(company);
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message);
                if (ex.InnerException != null) SaveLog(ex.Message);
            }
        }


        public object syncSave = new object();
        public void GetSite(string dysk)
        {
            new Thread((arg) => {
                try
                {
                    Count.Dispatcher.Invoke(() =>
                    {
                        Count.Content = "Start";
                        startGet.IsEnabled = false;
                        play.IsEnabled = false;
                        pause.IsEnabled = true;
                        taskMax.IsEnabled = false;
                    });

                    var list = countQ.ToArray();
                    foreach (var item in list)
                    {
                        try
                        {
                            Queue_dequeueHandler(item);

                            countQ.Remove(item);
                        }
                        catch (Exception ex)
                        {
                            listbox.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                listbox.Items.Add(new ListBoxItem() { Content = "Error: " + ex.Message, Background = Brushes.Red });
                                Count.Content = "Koniec";
                                startGet.IsEnabled = true;
                                play.IsEnabled = true;
                                cont.IsEnabled = false;
                                isPause = false;
                                pause.IsEnabled = false;
                                taskMax.IsEnabled = true;
                            }));

                            SaveLog(ex.Message);
                            if (ex.InnerException != null) SaveLog(ex.Message);
                        }
                    }

                    listbox.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Count.Content = "Koniec";
                        startGet.IsEnabled = true;
                        play.IsEnabled = true;
                        cont.IsEnabled = false;
                        isPause = false;
                        pause.IsEnabled = false;
                        taskMax.IsEnabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    listbox.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        listbox.Items.Add(new ListBoxItem() { Content = ex.Message, Background = Brushes.Red });
                        Count.Content = "Koniec";
                        startGet.IsEnabled = true;
                        play.IsEnabled = true;
                        cont.IsEnabled = false;
                        isPause = false;
                        pause.IsEnabled = false;
                        taskMax.IsEnabled = true;
                    }));
                    SaveLog(ex.Message);
                    if (ex.InnerException != null) SaveLog(ex.Message);
                }
            }).Start();
        }

        private void Queue_dequeueHandler(Company comp)
        {
            Waiting();
            process.Dispatcher.Invoke(() =>
            {
                lock (mSyncWeb)
                {
                    process.Text = $"{comp.Id}";
                }
            });
            //Pobranie z API i zapis do
            try
            {
                Company baseCompany = new Company();
                baseCompany.Id = comp.Id;
                baseCompany.CreateBy = comp.CreateBy;
                baseCompany.Category = comp.Category;
                baseCompany.Register = comp.Register;
                baseCompany.Country = comp.Country;
                baseCompany.Name = comp.Name;
                baseCompany.NameOrg = comp.NameOrg;

                Uri myUri = new Uri(comp.Id.StartsWith("http") ? comp.Id : "http://" + comp.Id);
                string host = myUri.AbsoluteUri;

                CancellationToken cancel = new CancellationToken();
                var t = this.StartSearching(this.Browser, cancel, baseCompany.Id);
                t.Wait(60000, cancel);
                var value = t.Result;

                var s = value;
                string php = s.Replace(oldValue: "&amp;", newValue: "&");
                HtmlDocument item = new HtmlDocument();
                item.LoadHtml(php);

                var title = item.DocumentNode.SelectNodes("//h1[contains(@class, 'filmCoverSection__title')]");
                if(title != null && title.Count > 0)
                {
                    baseCompany.Name = title[0].InnerText;
                }
                else
                {

                }

                var orgTitle = item.DocumentNode.SelectNodes("//div[contains(@class, 'filmCoverSection__originalTitle')]");
                if (orgTitle != null && orgTitle.Count > 0)
                { 
                    baseCompany.NameOrg = orgTitle[0].InnerText;
                }
                else
                {
                    baseCompany.NameOrg = baseCompany.Name;
                }

                var year = item.DocumentNode.SelectNodes("//div[contains(@class, 'filmCoverSection__year')]");
                if (year != null && year.Count > 0)
                {
                    baseCompany.Register = year[0].InnerText;
                }
                else
                {

                }

                var director = item.DocumentNode.SelectNodes("//a[@itemprop='director']");
                if (director != null && director.Count > 0)
                {
                    baseCompany.CreateBy = director[0].InnerText;
                }
                else
                {

                }

                var genre = item.DocumentNode.SelectNodes("//div[@itemprop='genre']");
                if (genre != null && genre.Count > 0)
                {
                    baseCompany.Category = genre[0].InnerText;
                }
                else
                {

                }

                var country = item.DocumentNode.SelectNodes("//div[contains(@class, 'filmInfo__info--productionCountry')]");
                if (country != null && country.Count > 0)
                {
                    baseCompany.Country = country[0].InnerText;
                }
                else
                {

                }
                comp.IsChecked = true;
                AddCompany(baseCompany, dyskToSave, true, true); 
                SetInfo();
            }
            catch (WebException ex)
            {
                ProcessExceptions(ex);
            }
            catch (Exception ex)
            {
                process.Dispatcher.Invoke(() =>
                {
                    lock (mSyncWeb)
                    {
                        process.Text = ex.Message;
                    }
                });
                SaveLog(ex.Message);
                if (ex.InnerException != null) SaveLog(ex.Message);
                Thread.Sleep(5000);
            }
        }

        private void Waiting()
        {
            bool wait = false;
            lock (objectPause)
            {
                wait = isPause;
            }
            if (wait)
            {
                Thread.Sleep(millisecondsTimeout: 500);
                Waiting();
            }
            else return;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string kopiuj = "";
            foreach (ListBoxItem item in listbox.Items)
            {
                kopiuj = kopiuj + "\r\n" + item.Content.ToString();
            }
            Clipboard.SetText(kopiuj);
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string link = (sender as TextBlock).Text;
            if (!string.IsNullOrEmpty(link))
            {
                try
                {
                    new Uri(link);
                    System.Diagnostics.Process.Start(link);
                }
                catch (Exception ex)
                {
                    SaveLog(ex.Message);
                    if (ex.InnerException != null) SaveLog(ex.Message);
                    return;
                }
            }
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            Button_Click_2(startGet, new RoutedEventArgs());
        }

        bool isPause = false;
        object objectPause = new object();
        private void pause_Click(object sender, RoutedEventArgs e)
        {
            lock (objectPause)
            {
                isPause = true;
                pause.IsEnabled = false;
                cont.IsEnabled = true;
                taskMax.IsEnabled = false;
            }
        }

        private void cont_Click(object sender, RoutedEventArgs e)
        {
            lock (objectPause)
            {
                pause.IsEnabled = true;
                isPause = false;
                cont.IsEnabled = false;
                taskMax.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textbox1.Text))
            {
                if (MessageBox.Show(messageBoxText: "Czy wyczyścić bazę ??", caption: "Czyszczenie", button: MessageBoxButton.OKCancel, icon: MessageBoxImage.Question, defaultResult: MessageBoxResult.Cancel) == MessageBoxResult.OK)
                {
                    try
                    {
                        lock (readwritefile)
                        {
                            using (FileStream file = File.Open(textbox1.Text, FileMode.Truncate, FileAccess.Write))
                            {
                                using (StreamWriter filewrite = new StreamWriter(file))
                                {
                                    filewrite.Flush();
                                    this.database.database.Clear();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SaveLog(ex.Message);
                        if (ex.InnerException != null) SaveLog(ex.Message);
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {

                }
            }
            else
            {
                MessageBox.Show(messageBoxText: "Żaden plik z bazą nie jest otwarty");
            }
        }

        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Clipboard.SetText(((sender as ListBox).SelectedItem as ListBoxItem).Content.ToString());
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            string extension = "xlsx";

            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = extension,
                Filter = String.Format("{1} files (.{0})|.{0}|All files (.)|.", extension, "Excel"),
                FilterIndex = 1
            };

            if (dialog.ShowDialog() == true)
            {
                using (Stream stream = dialog.OpenFile())
                {
                    gridView.ExportToXlsx(stream,
                        new GridViewDocumentExportOptions()
                        {
                            ShowColumnFooters = false,
                            ShowColumnHeaders = true,
                            ShowGroupFooters = false,
                            AutoFitColumnsWidth = true,
                            IgnoreCollapsedGroups = false,
                            ShowGroupRows = true,
                            ShowGroupHeaderRowAggregates = false
                        });
                }
            }
        }


        public async Task<string> StartSearching(
    IChromiumWebBrowserBase browser,
    CancellationToken cancel,
    string site = "")
        {
            try
            {
                this.SetInfoAdresUrl(site);
                LoadUrlAsyncResponse response = null;
                try
                {
                    response = await browser.LoadUrlAsync(site);
                }
                catch (Exception ex)
                {
                    this.SetError("", ex);
                    if (ex.StackTrace != null && ex.StackTrace.Contains("CefSharp.WebBrowserExtensions.LoadUrlAsync"))
                    {
                        CreateNewIstance(browser);
                        return null;
                    }
                }

                //var response = await browser.LoadUrlAsync(site);
                if (response != null && response.Success)
                {
                    if (cancel.IsCancellationRequested) throw new Exception($"Koniec czasu pobierania dla {site}");
                    string tHtml = await browser.GetSourceAsync();
                    return tHtml;
                }
                else
                {
                    this.SetError($"Nie udało się pobrać strony {site}");
                    return "";
                }

            }
            catch (Exception ex)
            {
                this.SetError("", ex);
                if (ex.StackTrace != null && ex.StackTrace.Contains("CefSharp.WebBrowserExtensions.LoadUrlAsync"))
                {
                    CreateNewIstance(browser);
                }
                return "";
            }
        }

        private void SetError(string error, Exception ex = null)
        {
            listbox.Dispatcher.BeginInvoke((Action)(() =>
            {
                listbox.Items.Add(new ListBoxItem() { Content = "Error: " + error + " " + ex.Message, Background = Brushes.Red });
            }));
        }

        public void SetInfoAdresUrl(string msg)
        {
            this.process.Dispatcher.Invoke(() =>
            {
                this.process.Text = msg;
            });
        }

        bool watingBrowser = false;
        public void CreateNewIstance(IChromiumWebBrowserBase browser)
        {
            this.Dispatcher.Invoke(() => {
                CefSettings settings = new CefSettings();
                settings.IgnoreCertificateErrors = false;

                if (!Cef.IsInitialized)
                {
                    Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
                }
                var v = new ChromiumWebBrowser();

                var s = new BrowserSettings();
                s.ImageLoading = CefState.Disabled;
                v.BrowserSettings = s;
                v.Margin = new Thickness(10, 10, 30, 10);
                v.Visibility = Visibility.Visible;
                v.Address = "";
                v.MinWidth = 500;
                v.Width = 500;
                v.MinHeight = 500;
                v.Height = 500;

                if (isValidName(Browser, browser))
                {
                    browserPanel.Children.Remove(Browser);
                    browser = null;
                    this.Browser = null;

                    v.Name = "Browser";
                    this.Browser = v;
                    browserPanel.Children.Add(Browser);
                    this.watingBrowser = false;
                }
            });
        }

        private bool isValidName(IChromiumWebBrowserBase b1, IChromiumWebBrowserBase b2)
        {
            bool isvalid = false;
            this.Dispatcher.Invoke(() =>
            {
                isvalid = (b1 as ChromiumWebBrowser).Name == (b2 as ChromiumWebBrowser).Name;
            });
            return isvalid;
        }


    }
}
