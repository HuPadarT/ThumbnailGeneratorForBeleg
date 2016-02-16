using Microsoft.Office.Interop.Word;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace ThumbnailGeneratorForBeleg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            errorList = new List<string>();
            filesList = new List<string>();
            userCancel = new bool();
            userCancel = false;
            this.DataContext = this;
        }

        #region main program members
        private bool userCancel;
        public bool UserCancel
        {
            get { return this.userCancel; }
            set
            {
                this.userCancel = value;
                OnPropertyChanged("UserCancel");
            }
        }

        private string dirPath;
        public string DirPath
        {
            get { return this.dirPath; }
            set
            {
                this.dirPath = value;
                OnPropertyChanged("DirPath");
            }
        }

        private string targetPath;
        public string TargetPath
        {
            get { return this.targetPath; }
            set
            {
                this.targetPath = value;
                OnPropertyChanged("TargetPath");
            }
        }

        private int dirCnt;
        public int DirCnt
        {
            get { return this.dirCnt; }
            set
            {
                this.dirCnt = value;
                OnPropertyChanged("DirCnt");
            }
        }

        private int fileCnt;
        public int FileCnt
        {
            get { return this.fileCnt; }
            set
            {
                this.fileCnt = value;
                OnPropertyChanged("FileCnt");
            }
        }

        private List<string> filesList;
        public List<string> FilesList
        {
            get { return this.filesList; }
            set
            {
                this.filesList = value;
                OnPropertyChanged("FilesList");
            }
        }

        private List<string> errorList;
        public List<string> Errorlist
        {
            get { return this.errorList; }
            set
            {
                this.errorList = value;
                OnPropertyChanged("Errorlist");
            }
        }
        #endregion

        #region create jpeg from documents
        private void PreviewToFile(string sourcefile)
        {
            string filename1 = Path.GetFileName(sourcefile);
            string docPath = sourcefile.Replace(filename1, string.Empty);
            string targetpath = TargetPath + (docPath.Replace(DirPath, string.Empty));
            string target = targetpath + filename1.Split('.')[0] + ".png";
            string imgTarget = Path.ChangeExtension(target, "jpg");
            if (File.Exists(imgTarget)) return;

            //MessageFilter .Register();
            var app = new Microsoft.Office.Interop.Word.Application();

            app.Visible = false;

            var doc = app.Documents.Open(sourcefile);

            try
            {
                doc.ShowGrammaticalErrors = false;
                //doc.ShowRevisions = false;
                doc.ShowSpellingErrors = false;

                if (!Directory.Exists(targetpath))
                {
                    Directory.CreateDirectory(targetpath);
                }
                //Opens the word document and fetch each page and converts to image

                Microsoft.Office.Interop.Word.Window window = doc.Windows[1];
                Microsoft.Office.Interop.Word.Pane pane = window.Panes[1];
                var page = pane.Pages[1];
                var bits = page.EnhMetaFileBits;

                try
                {
                    using (var ms = new MemoryStream((byte[])(bits)))
                    {
                        System.Drawing.Image image2 = System.Drawing.Image.FromStream(ms);

                        using (var b = new Bitmap(image2.Width, image2.Height))
                        {
                            b.SetResolution(image2.HorizontalResolution, image2.VerticalResolution);

                            using (var g = Graphics.FromImage(b))
                            {
                                g.Clear(System.Drawing.Color.White);
                                g.DrawImageUnscaled(image2, 0, 0);
                            }
                            b.Save(imgTarget, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    List<string> hibak = new List<string>();
                    if (Errorlist.Count != 0) hibak = Errorlist;
                    hibak.Add(docPath + ": " + ex.Message);
                    Errorlist = hibak;
                    //MessageBox.Show(ex.Message);
                }
            }
            catch (System.Exception ex)
            {
                List<string> hibak = new List<string>();
                if (Errorlist.Count != 0) hibak = Errorlist;
                hibak.Add(docPath + ": " + ex.Message);
                Errorlist = hibak;
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                doc.Close(Type.Missing, Type.Missing, Type.Missing);
                app.Quit(Type.Missing, Type.Missing, Type.Missing);
                GC.Collect();
            }
            //MessageFilter.Revoke();
        }
        #endregion

        #region buttons click event
        private void btnSource_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if(result.ToString().ToLower() == "ok")
            {
                DirPath = dialog.FileName;
            }
        }

        private void btnTaget_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result.ToString().ToLower() == "ok")
            {
                TargetPath = dialog.FileName;
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e) // strop all thread
        {
            UserCancel = true;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            string[] dirs = Directory.GetDirectories(DirPath);
            DirCnt = Directory.GetFiles(DirPath, "*.doc", SearchOption.AllDirectories).Count();
            string[] files;
            int FilesCount = 0;
            ParallelOptions po = new ParallelOptions();
            //po.CancellationToken = cts.Token;
            po.MaxDegreeOfParallelism = 4;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    Parallel.ForEach(dirs, po, dir =>
                    {
                       // if (UserCancel) cts.Cancel();

                        files = Directory.GetFiles(dir, "*.doc", SearchOption.AllDirectories);
                        List<string> fl = new List<string>();
                        foreach (string l in files)
                        {
                            fl.Add(l);
                        }
                        FilesList = fl;
                        foreach (string f in files)
                        {
                            PreviewToFile(f);
                            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                            {
                                ++FilesCount;
                                FileCnt = FilesCount;
                            }));
                        }
                    });
                }
                catch(Exception ex)
                {
                    List<string> hibak = new List<string>();
                    if (Errorlist.Count != 0) hibak = Errorlist;
                    hibak.Add(ex.Message);
                    Errorlist = hibak;
                    //MessageBox.Show(ex.Message);
                }
            }).ContinueWith((a) =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        MessageBox.Show("Finished!");
                    }));
            });
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

    }
}
