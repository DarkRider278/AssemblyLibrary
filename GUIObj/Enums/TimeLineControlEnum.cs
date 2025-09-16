namespace GUIObj.Enums
{
    public enum TimeLineManipulationMode { Linked, Free }
    internal enum TimeLineAction { Move, StretchStart, StretchEnd }
    public enum TimeLineViewLevel { Minutes, Hours, Days, Weeks, Months, Years };

    public enum TimeLineItemState
    {
        NEUTRAL,
        PLAY,
        PREPARED,
        FINISH,
        ERROR,
        STOPPING
    }
}
