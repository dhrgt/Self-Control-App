using SelfControl.Helpers;
using SelfControl.Helpers.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfControl.Models
{
    public class WeeklyReviewViewerModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Stack<object> _contextStack = new Stack<object>();
        int ImageCount;
        private int _currentIndex;
        private int _nextIndex;
        private int _prevIndex;
        List<FoodItem> foodList;

        public WeeklyReviewViewerModel(List<FoodItem> list, WeeklyReviewViewer parent)
        {
            foodList = list;
            ImageCount = list.Count;
            _prevIndex = -1;
            CurrentIndex = 0;
            _nextIndex = 1;

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

            PanPositionChangedCommand = new Command(async (p) =>
            {
                var isNext = (bool)p;
                if (isNext)
                {
                    bool radsFilled = true;
                    foreach (var rads in parent.radioGroups)
                    {
                        if (rads.Value.SelectedIndex == -1)
                        {
                            radsFilled = false;
                            break;
                        }
                    }
                    if (!radsFilled)
                    {
                        await parent.DisplayAlert("Alert", "Please fill all the fields", "OK");
                        return;
                    }
                    Dictionary<int, int> dict = new Dictionary<int, int>(parent.radioGroups.Count);
                    foreach (var rads in parent.radioGroups)
                    {
                        dict[rads.Key] = rads.Value.SelectedIndex;
                    }
                    parent.responses[foodList[CurrentIndex].ID] = dict;
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
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentIndex)));
            }
        }

        private object CreateContext(int index)
        {
            return new WeeklyReviewFactory() { ByteSource = CreateImageSource(index), NameSource = foodList[index].NAME };
        }

        private ImageSource CreateImageSource(int index)
        {
            return ImageSource.FromStream(() => new MemoryStream(GlobalVariables.DeserializeStringToByteArray(foodList[index].IMGBYTES)));
        }
    }
}
