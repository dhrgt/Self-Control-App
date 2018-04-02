using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using SelfControl.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfControl.Models
{
    class PracticeViewerModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        int ImageCount;
        private int _currentIndex;
        private int _nextIndex;
        Dictionary<int, byte[]> imageFiles;

        public PracticeViewerModel(Dictionary<int, byte[]> imageFiles, int index)
        {
            this.imageFiles = imageFiles;
            ImageCount = imageFiles.Count;
            CurrentIndex = index;
            _nextIndex = index + 1;
            PrevContext = _nextIndex >= ImageCount ? null : CreateContext(_nextIndex);
            CurrentContext = CreateContext(_currentIndex);
            NextContext = _nextIndex >= ImageCount ? null : CreateContext(_nextIndex);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentIndex)));

            PanStartedCommand = new Command(() =>
            {
                PanEnable = !PracticeFactory.isPlaying;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PanEnable)));
                if (!PanEnable) return; 
                if (_nextIndex < ImageCount && PrevContext == null)
                {
                    PrevContext = CreateContext(_nextIndex);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrevContext)));
                }
                if (_nextIndex < ImageCount && NextContext == null)
                {
                    NextContext = CreateContext(_nextIndex);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextContext)));
                }

            });

            PanPositionChangedCommand = new Command((p) =>
            {
                var isNext = (bool)p;
                PanEnable = !PracticeFactory.isPlaying;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PanEnable)));
                if (!PanEnable) return; 
                if (isNext)
                {
                    CurrentIndex += 1;
                    _nextIndex++;
                    PrevContext = _nextIndex >= ImageCount ? null : CreateContext(_nextIndex);
                    CurrentContext = NextContext;
                    NextContext = _nextIndex >= ImageCount ? null : CreateContext(_nextIndex);
                }
                else
                {
                    CurrentIndex += 1;
                    ++_nextIndex;
                    NextContext = _nextIndex >= ImageCount ? null : CreateContext(_nextIndex);
                    CurrentContext = PrevContext;
                    PrevContext = _nextIndex >= ImageCount ? null : CreateContext(_nextIndex);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextContext)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrevContext)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentContext)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentIndex)));
            });
        }

        public PracticeFactory CurrentContext { get; set; }
        public PracticeFactory NextContext { get; set; }
        public PracticeFactory PrevContext { get; set; }

        
        public bool PanEnable { get; set; }

        public ICommand PanStartedCommand { get; }
        public ICommand PanPositionChangedCommand { get; }
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
            }
        }

        private PracticeFactory CreateContext(int index)
        {
            return new PracticeFactory { ByteSource = CreateSource(index) };
        }

        private byte[] CreateSource(int index)
        {
            return imageFiles.ElementAt(index).Value;
        }
    }
}
