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

        public MainWindow()
        {
            InitializeComponent();
            errorList = new List<string>();
            filesList = new List<string>();
            this.DataContext = this;
            core = new Core();
            main = this;
        }

        #region main program members
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
            //if (!UserCancel) UserCancel = true;
            //else UserCancel = false;
            UserCancel = true;

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            UserStart = true;
            core.StartProcess(main);
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
