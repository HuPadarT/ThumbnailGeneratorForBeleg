using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ThumbnailGeneratorForBeleg.Model;

namespace ThumbnailGeneratorForBeleg.Processing
{
    public class CreatePreview
    {
        public void PreviewToFile(SourceFile sourcefile, Core maw, StreamWriter errwr)
        {
            string filename1 = Path.GetFileName(sourcefile.FPath);
            string docPath = sourcefile.Path;
            string targetpath = maw.TargetPath + (docPath.Replace(maw.DirPath, string.Empty));
            string target = targetpath + filename1.Split('.')[0] + ".png";
            string imgTarget = Path.ChangeExtension(target, "jpg");
            if (File.Exists(imgTarget))
            {
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    maw.MarKesz++;
                }));
                return;
            }
            sourcefile.State = Enums.State.Processing;
            //MessageFilter .Register();
            var app = new Microsoft.Office.Interop.Word.Application();
            ImageHandler imgh = new ImageHandler();
            app.Visible = false;

            var doc = app.Documents.Open(sourcefile, ReadOnly:true);

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
                            sourcefile.State = Enums.State.Processing;
                            imgh.SaveImage(b, 250, 400, 80, imgTarget);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        sourcefile.State = Enums.State.Error;
                        sourcefile.Hiba = ex.Message;
                        maw.AddErrorList(sourcefile.FileNev + ":" + ex.Message);
                        errwr.WriteLine(sourcefile.FileNev + ":" + ex.Message);
                        errwr.Flush();
                        maw.ErrorCnt++;
                    }));
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    sourcefile.State = Enums.State.Error;
                    sourcefile.Hiba = ex.Message;
                    maw.AddErrorList(sourcefile.FileNev + ":" + ex.Message);
                    errwr.WriteLine(sourcefile.FileNev + ":" + ex.Message);
                    errwr.Flush();
                    maw.ErrorCnt++;
                }));
            }
            finally
            {
                doc.Close(Type.Missing, Type.Missing, Type.Missing);
                app.Quit(Type.Missing, Type.Missing, Type.Missing);
                GC.Collect();
            }
            //MessageFilter.Revoke();
        }
    }
}
