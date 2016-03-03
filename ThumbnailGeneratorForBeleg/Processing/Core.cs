using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ThumbnailGeneratorForBeleg.Enums;
using ThumbnailGeneratorForBeleg.Model;
using ThumbnailGeneratorForBeleg.Processing;

namespace ThumbnailGeneratorForBeleg.Processing
{
    public class Core : PropertyChangeBase
    {
        private CreatePreview cp = new CreatePreview();

        public Core()
        {
            FileCnt = 0;
            DirCnt = 0;
            MarKesz = 0;
            ErrorCnt = 0;
            filesList = new ObservableCollection<SourceFile>();
            errorList = new ObservableCollection<string>();
        }

        #region main program members
        private static ObservableCollection<SourceFile> filesList;
        public static ObservableCollection<SourceFile> FilesList
        {
            get { return filesList; }
        }

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

        private int errorCnt;
        public int ErrorCnt
        {
            get { return this.errorCnt; }
            set
            {
                this.errorCnt = value;
                OnPropertyChanged("ErrorCnt");
            }
        }

        private int marKesz;
        public int MarKesz
        {
            get { return this.marKesz; }
            set
            {
                this.marKesz = value;
                OnPropertyChanged("MarKesz");
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

        private static ObservableCollection<string> errorList;
        public static ObservableCollection<string> Errorlist
        {
            get { return errorList; }
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

        public void StartProcess()
        {
            filesList.Clear();
            errorList.Clear();
            string[] files = Directory.GetFiles(DirPath, "*.doc", SearchOption.AllDirectories);
            foreach (string l in files)
            {
                SourceFile item = new SourceFile();
                item.FileNev = Path.GetFileNameWithoutExtension(l);
                item.Path = Path.GetFullPath(l).Replace(Path.GetFileName(l), string.Empty);
                item.FPath = Path.GetFullPath(l);
                item.State = State.Init;
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    AddFileList(item);
                }));
            }
            DirCnt = files.Count();
            int FilesCount = 0;

            ParallelOptions po = new ParallelOptions();
            //po.CancellationToken = cts.Token;
            po.MaxDegreeOfParallelism = 1;
            ProgStat = "Processing files, plase wait... if you want to stop processing then press Stop button.";
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                using (StreamWriter writerError = new StreamWriter("Error_log.txt"))
                {
                    Parallel.ForEach(FilesList, po, f =>
                    {
                        if (!UserCancel)
                        {
                            cp.PreviewToFile(f, this, writerError);
                            if (f.Hiba == string.Empty || f.Hiba == null)
                                f.State = State.Finised;
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                            {
                                ++FilesCount;
                                FileCnt = FilesCount;
                            }));
                        }
                    });
                }
            }).ContinueWith((a) =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        if (!UserCancel) ProgStat = "Files processing done.";
                        else
                        {
                            string txt = "Files processing stoped by User.";
                            ProgStat = txt;
                            AddErrorList(txt);
                        }
                        UserStart = false;
                        MessageBox.Show("Finished!");
                    }));
            });

        }
    }
}
