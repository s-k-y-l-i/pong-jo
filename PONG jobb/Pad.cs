using System.ComponentModel;
using System.Linq;
using System.Text;
//using PongGame.Annotations;


namespace PongGame
{
    class Pad : INotifyPropertyChanged
    {
        private int _yPosition;

        public double ActualHeight { get; internal set; }

        public int YPosition
        {
            get { return _yPosition; }
            set
            {
                _yPosition = value;
                OnPropertyChanged("YPosition");
            }
        }
        public int Height
        {
            get { return (int)ActualHeight; }
            set
            {
                ActualHeight = value;
                OnPropertyChanged("Height");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

   }
}
