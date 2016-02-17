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
using ThumbnailGeneratorForBeleg.Model;
using ThumbnailGeneratorForBeleg.Processing;

namespace ThumbnailGeneratorForBeleg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        private Core core;
        MainWindow main;
        private static PropertyChangeBase pcb;

        public static List<SourceFile> FilesList = new List<SourceFile>();

        public MainWindow()
        {
            InitializeComponent();
            core = new Core();
            main = this;
            errorList = new List<string>();
            DirPath = string.Empty;
            TargetPath = string.Empty;
            this.DataContext = this;

            ProgStat = "Please select source directory!";
        }

        #region main program members
        public bool AddFileList(SourceFile item)
        {
            if (item != null)
            {
                FilesList.Add(item);
                OnPropertyChanged("FilesList");
                return true;
            }
            else return false;
        }

        private bool userCancel;
        public bool UserCancel
        {
            get { return this.userCancel; }
            set
            {
                this.userCancel = value;
                if (userCancel) UserStart = false;
                OnPropertyChanged("UserCancel");
            }
        }

        private bool userStart;
        public bool UserStart
        {
            get { return this.userStart; }
            set
            {
                this.userStart = value;
                if (userStart) UserCancel = false;
                OnPropertyChanged("UserStart");
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

        private string progStat;
        public string ProgStat
        {
            get { return this.progStat; }
            set
            {
                this.progStat = value;
                OnPropertyChanged("ProgStat");
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

        private List<string> errorList;
        public List<string> Errorlist
        {
            get { return this.errorList; }
        }

        public bool AddErrorList(string item)
        {
            if (item != null)
            {
                errorList.Add(item);
                OnPropertyChanged("errorList");
                return true;
            }
            else return false;
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
            if(TargetPath == string.Empty || TargetPath == null)
                ProgStat = "Please select Target directory!";
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
            if (DirPath == string.Empty || DirPath == null)
                ProgStat = "Please select Source directory!";
            else
                ProgStat = "Please click Start button!";
        }

        private void btnStop_Click(object sender, RoutedEventArgs e) // strop all thread
        {
            UserCancel = true;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Errorlist.Clear();
            FilesList.Clear();
            FileCnt = 0;
            DirCnt = 0;
            if (DirPath == string.Empty || DirPath == null)
            {
                ProgStat = "Please select Source directory!";
                return;
            }
            else if (TargetPath == string.Empty || TargetPath == null)
            {
                ProgStat = "Please select Target directory!";
                return;
            }
            UserStart = true;
            ProgStat = "I reading direrctories and files (this take long time), please wait!!";
            core.StartProcess(main);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void execTransparencyMenuItem(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuItem).IsChecked)
                this.Opacity = 0.7;
            else
                this.Opacity = 1;
        }

        private void execClose(object sender, RoutedEventArgs e)
        {
            this.Close();
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
