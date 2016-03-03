using Microsoft.Office.Interop.Word;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public partial class MainWindow : System.Windows.Window
    {
        private Core core;

        public MainWindow()
        {
            InitializeComponent();
            core = new Core();
            this.DataContext = core;

            core.ProgStat = "Please select source directory!";

            string version = System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion();
                //Assembly
                //     .GetExecutingAssembly()
                //     .GetReferencedAssemblies()
                //     .Where(x => x.Name == "System.Core").First().Version.ToString();
            MessageBox.Show("verzió:" + version);
        }

        #region buttons click event
        private void btnSource_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if(result.ToString().ToLower() == "ok")
            {
                core.DirPath = dialog.FileName;
            }
            if (core.TargetPath == string.Empty || core.TargetPath == null)
                core.ProgStat = "Please select Target directory!";
        }

        private void btnTaget_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result.ToString().ToLower() == "ok")
            {
                core.TargetPath = dialog.FileName;
            }
            if (core.DirPath == string.Empty || core.DirPath == null)
                core.ProgStat = "Please select Source directory!";
            else
                core.ProgStat = "Please click Start button!";
        }

        private void btnStop_Click(object sender, RoutedEventArgs e) // strop all thread
        {
            core.UserCancel = true;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (core.DirPath == string.Empty || core.DirPath == null)
            {
                core.ProgStat = "Please select Source directory!";
                return;
            }
            else if (core.TargetPath == string.Empty || core.TargetPath == null)
            {
                core.ProgStat = "Please select Target directory!";
                return;
            }
            core.UserStart = true;
            core.ProgStat = "I reading direrctories and files (this take long time), please wait!!";
            core.StartProcess();
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
    }
}
