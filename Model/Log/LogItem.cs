#region

using GalaSoft.MvvmLight;

#endregion

namespace LeagueSharp.Loader.Model.Log
{
    public class LogItem : ObservableObject
    {
        private LogLevel _level;
        private string _message;
        private string _source;

        public LogLevel Level
        {
            get { return _level; }
            set { Set(() => Level, ref _level, value); }
        }

        public string Source
        {
            get { return _source; }
            set { Set(() => Source, ref _source, value); }
        }

        public string Message
        {
            get { return _message; }
            set { Set(() => Message, ref _message, value); }
        }
    }
}