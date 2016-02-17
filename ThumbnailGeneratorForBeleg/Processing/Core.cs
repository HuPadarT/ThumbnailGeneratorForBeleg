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
    public class Core
    {
        private CreatePreview cp = new CreatePreview();

        public void StartProcess(MainWindow mn)
        {
            string[] dirs = Directory.GetDirectories(mn.DirPath);
            mn.DirCnt = Directory.GetFiles(mn.DirPath, "*.doc", SearchOption.AllDirectories).Count();
            string[] files;
            int FilesCount = 0;

            ParallelOptions po = new ParallelOptions();
            //po.CancellationToken = cts.Token;
            po.MaxDegreeOfParallelism = 4;
            mn.ProgStat = "Processing files, plase wait... if you want to stop processing then press Stop button.";
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                using (StreamWriter writerError = new StreamWriter("Error_log.txt"))
                {
                    try
                    {
                        Parallel.ForEach(dirs, po, dir =>
                        {
                            if (!mn.UserCancel)
                            {
                                files = Directory.GetFiles(dir, "*.doc", SearchOption.AllDirectories);
                                foreach (string l in files)
                                {
                                    SourceFile item = new SourceFile();
                                    item.FileNev = Path.GetFileNameWithoutExtension(l);
                                    item.Path = Path.GetFullPath(l).Replace(Path.GetFileName(l), string.Empty);
                                    item.State = State.Init;
                                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                                    {
                                        mn.AddFileList(item);
                                    }));
                                }
                                foreach (string f in files)
                                {
                                    if (!mn.UserCancel)
                                    {
                                        cp.PreviewToFile(f, mn, writerError);
                                        System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                                        {
                                            ++FilesCount;
                                            mn.FileCnt = FilesCount;
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
                            mn.AddErrorList(ex.Message);
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
                        if (!mn.UserCancel) mn.ProgStat = "Files processing done.";
                        else
                        {
                            string txt = "Files processing stoped by User.";
                            mn.ProgStat = txt;
                            mn.AddErrorList(txt);
                        }
                        mn.UserStart = false;
                        MessageBox.Show("Finished!");
                    }));
            });

        }
    }
}
