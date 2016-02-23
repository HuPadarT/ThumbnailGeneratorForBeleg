using System;
using System.Collections.Generic;
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
        public static List<SourceFile> FilesList;

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

        public void StartProcess()
        {
            errorList = new List<string>();
            FileCnt = 0;
            DirCnt = 0;
            FilesList = new List<SourceFile>();
            string[] dirs = Directory.GetDirectories(DirPath);
            DirCnt = Directory.GetFiles(DirPath, "*.doc", SearchOption.AllDirectories).Count();
            int FilesCount = 0;

            ParallelOptions po = new ParallelOptions();
            //po.CancellationToken = cts.Token;
            po.MaxDegreeOfParallelism = 4;
            ProgStat = "Processing files, plase wait... if you want to stop processing then press Stop button.";
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                using (StreamWriter writerError = new StreamWriter("Error_log.txt"))
                {
                    try
                    {
                        Parallel.ForEach(dirs, po, dir =>
                        {
                            if (!UserCancel)
                            {
                                string[] files = Directory.GetFiles(dir, "*.doc", SearchOption.AllDirectories);
                                foreach (string l in files)
                                {
                                    SourceFile item = new SourceFile();
                                    item.FileNev = Path.GetFileNameWithoutExtension(l);
                                    item.Path = Path.GetFullPath(l).Replace(Path.GetFileName(l), string.Empty);
                                    item.State = State.Init;
                                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                                    {
                                        AddFileList(item);
                                    }));
                                }
                                foreach (string f in files)
                                {
                                    if (!UserCancel)
                                    {
                                        cp.PreviewToFile(f, this, writerError);
                                        System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                                        {
                                            ++FilesCount;
                                            FileCnt = FilesCount;
                                        }));
                                    }
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                        {
                            AddErrorList(ex.Message);
                            writerError.WriteLine(ex.Message);
                            writerError.Flush();
                        }));
                    }
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
