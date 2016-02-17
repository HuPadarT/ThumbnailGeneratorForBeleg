using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ThumbnailGeneratorForBeleg.Processing
{
    public class CreatePreview
    {
        public void PreviewToFile(string sourcefile, MainWindow maw)
        {
            string filename1 = Path.GetFileName(sourcefile);
            string docPath = sourcefile.Replace(filename1, string.Empty);
            string targetpath = maw.TargetPath + (docPath.Replace(maw.DirPath, string.Empty));
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
                            int resize = 1;
                            if (image2.Width > 700 || image2.Height > 950) resize = 10;
                            else if (image2.Width > 500 || image2.Height > 700) resize = 6;
                            else if (image2.Width > 350 || image2.Height > 500) resize = 4;
                            else if (image2.Width > 200 || image2.Height > 350) resize = 2;
                            b.SetResolution((image2.HorizontalResolution / resize), (image2.VerticalResolution) / resize);

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
                    if (maw.Errorlist.Count != 0) hibak = maw.Errorlist;
                    hibak.Add(docPath + ": " + ex.Message);
                    maw.Errorlist = hibak;
                    //MessageBox.Show(ex.Message);
                }
            }
            catch (System.Exception ex)
            {
                List<string> hibak = new List<string>();
                if (maw.Errorlist.Count != 0) hibak = maw.Errorlist;
                hibak.Add(docPath + ": " + ex.Message);
                maw.Errorlist = hibak;
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
    }
}
