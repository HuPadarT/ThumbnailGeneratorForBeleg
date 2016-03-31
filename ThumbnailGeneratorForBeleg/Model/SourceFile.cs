using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ThumbnailGeneratorForBeleg.Enums;

namespace ThumbnailGeneratorForBeleg.Model
{
    public class SourceFile : INotifyPropertyChanged
    {
        public string path;
        public string Path
        {
            get { return this.path; }
            set
            {
                this.path = value;
                OnPropertyChanged("Path");
            }
        }

        public string fileNev;
        public string FileNev
        {
            get { return this.fileNev; }
            set
            {
                this.fileNev = value;
                OnPropertyChanged("FileNev");
            }
        }

        public string fPath;
        public string FPath
        {
            get { return this.fPath; }
            set
            {
                this.fPath = value;
                OnPropertyChanged("FPath");
            }
        }

        public string hiba;
        public string Hiba
        {
            get { return this.hiba; }
            set
            {
                this.hiba = value;
                OnPropertyChanged("Hiba");
            }
        }

        private State state;
        public State State
        {
            get { return this.state; }
            set
            {
                this.state = value;
                OnPropertyChanged("State");
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
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
