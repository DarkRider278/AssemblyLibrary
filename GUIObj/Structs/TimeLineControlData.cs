using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using GUIObj.Enums;
using Color = System.Windows.Media.Color;

namespace GUIObj.Structs
{
    public class TimeLineControlData : ITimeLineControlData, INotifyPropertyChanged
    {
        public DateTime? StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }

        public DateTime? EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                _offTime = _endTime.Value.AddMilliseconds(Offtime);
                OnPropertyChanged(nameof(EndTime));
            }
        }

        public Boolean TimelineViewExpanded { get; set; }

        public String DurationTime
        {
            get { return _durationTime; }
            set
            {
                _durationTime = value;
                OnPropertyChanged(nameof(DurationTime));
            }
        }

        public String ItemName { get; set; }
        public String Item { get; set; }

        public SolidColorBrush BGColor { get; set; }
        private int _timeLine;

        private long _runid;
        private DateTime? _startTime;
        private DateTime? _endTime;
        private DateTime? _offTime;
        private string _durationTime;
        private int _offtime;

        private TimeLineItemState _state;

        public long RunId
        {
            get
            {
                return _runid;
            }
        }

        public int TimeLine
        {
            get { return _timeLine; }
            set { _timeLine = value; }
        }

        public TimeLineItemState State
        {
            get { return _state; }
            set
            {
                _state = value;
                SetItemStatus(_state);
            }
        }

        public DateTime? OffTime
        {
            get { return _offTime; }
            set { _offTime = value; }
        }

        public int Offtime
        {
            get { return _offtime; }
            set { _offtime = value; }
        }

        public int ListDuration
        {
            get { return (int)((DateTime)EndTime).Subtract((DateTime)StartTime).TotalMilliseconds;}
        }


        public TimeLineControlData()
        {
            BGColor = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255));
            _runid = 0;
            Offtime = 0;
        }

        public TimeLineControlData(string id, string name, int durationms) : this()
        {
            Item = id;
            ItemName = name == "" ? id : name;
            DateTime start=DateTime.Today;
            StartTime = start;
            DateTime end;
            if (durationms <0)
            {
                Offtime = durationms;
                durationms = 10000;
            }
            end =DateTime.Today.AddMilliseconds(durationms);
            EndTime = end;
            OffTime = end.AddMilliseconds(Offtime);
            DurationTime = end.Subtract(start).TotalSeconds.ToString("#### s");
            
        }

        public TimeLineControlData(string id, string name, DateTime start, DateTime end) : this()
        {
            Item = id;
            ItemName = name == "" ? id : name;
            StartTime = start;
            EndTime = end;
            DurationTime = end.Subtract(start).TotalSeconds.ToString("#### s");
        }

        public void InitRunID()
        {
            if (_runid == 0)
                _runid = DateTime.Now.Ticks;
        }

        private void SetItemStatus(TimeLineItemState state)
        {
            switch (state)
            {
                case TimeLineItemState.NEUTRAL:
                    BGColor = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255));
                    break;
                case TimeLineItemState.PLAY:
                    BGColor = new SolidColorBrush(Color.FromArgb(100, 0, 255, 0));
                    break;
                case TimeLineItemState.PREPARED:
                    BGColor = new SolidColorBrush(Color.FromArgb(100, 0, 0, 180));
                    break;
                case TimeLineItemState.FINISH:
                    BGColor = new SolidColorBrush(Color.FromArgb(100, 255, 128, 0));
                    break;
                case TimeLineItemState.ERROR:
                    BGColor = new SolidColorBrush(Color.FromArgb(100, 255, 10, 10));
                    break;
                case TimeLineItemState.STOPPING:
                    BGColor = new SolidColorBrush(Color.FromArgb(100, 128, 172, 0));
                    break;
            }

            OnPropertyChanged("BGColor");
        }

        public void MoveStartTime(DateTime newstart)
        {
            TimeSpan ts = newstart.Subtract((DateTime)StartTime);
            EndTime = ((DateTime)EndTime).Add(ts);
            OffTime= ((DateTime)EndTime).AddMilliseconds(Offtime);
            StartTime = newstart;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}