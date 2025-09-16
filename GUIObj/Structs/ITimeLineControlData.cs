using System;
using System.Windows.Media;

namespace GUIObj.Structs
{
    public interface ITimeLineControlData
    {
        DateTime? StartTime { get; set; }
        DateTime? EndTime { get; set; }
        Boolean TimelineViewExpanded { get; set; }
        String DurationTime { get; set; }
        long RunId { get; }
        SolidColorBrush BGColor { get; set; }

        void InitRunID();
    }
}