using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfControl.Models
{
    public sealed class ImageViewerModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Stack<object> _contextStack = new Stack<object>();
        int ImageCount;
        private int _currentIndex;
        private int _prevIndex;
        private int _nextIndex;
        Dictionary<int, byte[]> imageFiles;

        public ImageViewerModel(Dictionary<int, byte[]> imageFiles, int index)
        {
            this.imageFiles = imageFiles;
            ImageCount = imageFiles.Count;
            _prevIndex = index - 1;
            CurrentIndex = index;
            _nextIndex = index + 1;

            PrevContext = _prevIndex < 0 ? null : CreateContext(_prevIndex);
            CurrentContext = CreateContext(_currentIndex);
            NextContext = _nextIndex >= ImageCount ? null : CreateContext(_nextIndex);

            _contextStack.Push(PrevContext);

            PanStartedCommand = new Command(() =>
            {
                if (_contextStack.Any() && PrevContext == null)
                {
                    PrevContext = _contextStack.Peek();
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
                if (isNext)
                {
                    CurrentIndex += 1;
                    _prevIndex++;
                    _nextIndex++;
                    _contextStack.Push(CurrentContext);
                    PrevContext = CurrentContext;
                    CurrentContext = NextContext;
                    NextContext = _nextIndex >= ImageCount ? null : CreateContext(_nextIndex);
                }
                else
                {
                    if (!_contextStack.Any())
                    {
                        return;
                    }

                    CurrentIndex -= 1;
                    --_prevIndex;
                    --_nextIndex;
                    NextContext = CurrentContext;
                    CurrentContext = PrevContext;
                    _contextStack.Pop();
                    if (!_contextStack.Any() && _prevIndex >= 0) _contextStack.Push(CreateContext(_prevIndex));
                    PrevContext = _contextStack.Any() ? _contextStack.Peek() : null;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextContext)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrevContext)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentContext)));
            });
        }

        public object CurrentContext { get; set; }
        public object NextContext { get; set; }
        public object PrevContext { get; set; }

        public ICommand PanStartedCommand { get; }
        public ICommand PanPositionChangedCommand { get; }
        public int CurrentIndex { get => _currentIndex;
            set
            {
                _currentIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentIndex)));
            }
        }

        private object CreateContext(int index)
        {
            return new { ByteSource = CreateSource(index) };
        }

        private ImageSource CreateSource(int index)
        {
            return ImageSource.FromStream(() => new MemoryStream(imageFiles.ElementAt(index).Value));
        }
    }
}
