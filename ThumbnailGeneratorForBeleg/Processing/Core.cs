using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    Parallel.ForEach(dirs, po, dir =>
                    {
                        if (!mn.UserCancel)
                        {
                            files = Directory.GetFiles(dir, "*.doc", SearchOption.AllDirectories);
                            List<string> fl = new List<string>();
                            foreach (string l in files)
                            {
                                fl.Add(l);
                            }
                            mn.FilesList = fl;
                            foreach (string f in files)
                            {
                                if (!mn.UserCancel)
                                {
                                    cp.PreviewToFile(f, mn);
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
                    List<string> hibak = new List<string>();
                    if (mn.Errorlist.Count != 0) hibak = mn.Errorlist;
                    hibak.Add(ex.Message);
                    mn.Errorlist = hibak;
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
    }
}
