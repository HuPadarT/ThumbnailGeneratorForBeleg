using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ThumbnailGeneratorForBeleg.Enums;

namespace ThumbnailGeneratorForBeleg.Model
{
    public sealed class SourceFile : INotifyPropertyChanged
    {
        public string _fileNev;
        public string FileNev
        {
            get { return this._fileNev; }
            set
            {
                this._fileNev = value;
                OnPropertyChanged("FileNev");
            }
        }

        private State _state;
        public State SState
        {
            get { return this._state; }
            set
            {
                this._state = value;
                OnPropertyChanged("SState");
            }
        }

        public string _path;
        public string Path
        {
            get { return this._path; }
            set
            {
                this._path = value;
                OnPropertyChanged("Path");
            }
        }

        public string _fPath;
        public string FPath
        {
            get { return this._fPath; }
            set
            {
                this._fPath = value;
                OnPropertyChanged("FPath");
            }
        }

        public string _hiba;
        public string Hiba
        {
            get { return this._hiba; }
            set
            {
                this._hiba = value;
                OnPropertyChanged("Hiba");
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
