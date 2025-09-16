using System;

namespace HelpUtility
{
    /// <summary>
    /// This class includes static methods often used with counting with frames.
    /// </summary>
    public sealed class VideoUtils
    {
        private int _framesPerHour;
        private int _framesPerMinute;
        private int _framesPerSecond;

        internal VideoUtils(int framesPerSecond)
        {
            this._framesPerSecond = framesPerSecond;
            _framesPerMinute = framesPerSecond*60;
            _framesPerHour = _framesPerMinute*60;
        }

        /// <summary>
        /// Instance of FrameUtils for 25 frames per second.
        /// </summary>
        public static readonly VideoUtils VideoUtils25 = new VideoUtils(25);

        /// <summary>
        /// Instance of FrameUtils for 40 frames per second.
        /// </summary>
        public static readonly VideoUtils VideoUtils40 = new VideoUtils(40);

        /// <summary>
        /// Instance of FrameUtils for 50 frames per second.
        /// </summary>
        public static readonly VideoUtils VideoUtils50 = new VideoUtils(50);

        /// <summary>
        /// Instance of FrameUtils for 60 frames per second.
        /// </summary>
        public static readonly VideoUtils VideoUtils60 = new VideoUtils(60);

        /// <summary>
        /// Decode fields to hours, minutes, seconds and fields.
        /// </summary>
        /// <param name="framesToDecode">Frames to decode.</param>
        /// <param name="hour">Returned hours.</param>
        /// <param name="minute">Returned minutes.</param>
        /// <param name="second">Returned seconds.</param>
        /// <param name="frame">Returned fields.</param>
        public void DecodeFrames(int framesToDecode, out int hour, out int minute, out int second, out int frame)
        {
            hour = framesToDecode / _framesPerHour;
            framesToDecode %= _framesPerHour;
            minute = framesToDecode / _framesPerMinute;
            framesToDecode %= _framesPerMinute;
            second = framesToDecode / _framesPerSecond;
            framesToDecode %= _framesPerSecond;
            frame = framesToDecode;
        }

        /// <summary>
        /// Encode frames from hours, minutes, seconds and fields.
        /// </summary>
        /// <param name="hour">Hours to encode.</param>
        /// <param name="minute">Minutes to encode.</param>
        /// <param name="second">Seconds to encode.</param>
        /// <param name="frame">Returned frames.</param>
        /// <returns></returns>
        public int EncodeFrames(int hour, int minute, int second, int frame)
        {
            return frame + second*_framesPerSecond + minute*_framesPerMinute + hour*_framesPerHour;
        }

        /// <summary>
        /// Converts frames to time code data.
        /// </summary>
        /// <param name="frames">frames number to convert.</param>
        /// <returns>TimeCode data.</returns>
        public byte[] FramesToTimeCode(int frames)
        {
            int h, m, s, f;
            byte f_bcd;
            bool field_flag;

            DecodeFrames(frames, out h, out m, out s, out f);
            field_flag = (f%2) == 1;

            if (_framesPerSecond > 30)
            {
                f = f/2;
            }

            f_bcd = DecToBCD(f);

            if (field_flag && (_framesPerSecond > 30))
            {
                f_bcd = (byte) (f_bcd | 0x80);
            }

            return new byte[] {f_bcd, DecToBCD(s), DecToBCD(m), DecToBCD(h)};
        }

        /// <summary>
        /// Converts time code data to frames.
        /// </summary>
        /// <param name="timeCodeData">Time code data to convert.</param>
        /// <returns>Number of frames</returns>
        /// <exception cref="System.ArgumentException">Is thrown if the length of the timeCodeData is not 4.</exception>
        public int TimeCodeToFrames(byte[] timeCodeData)
        {
            if (timeCodeData.Length != 4)
            {
                //**throw new ArgumentException("Time code array has to be 4 bytes long.");
                throw new ArgumentException("Time code array has to be 4 bytes long.");
            }

            byte f = BCDToDec(timeCodeData[0]);
            // we mask seconds and also hours because it could be possible both
            byte s = BCDToDec((byte) (timeCodeData[1] & 0x7F)); // mask out 0x80 (= odd field identificator)
            byte m = BCDToDec(timeCodeData[2]);
            byte h = BCDToDec((byte) (timeCodeData[3] & 0x7F)); // mask out 0x80 (= odd field identificator)
            return EncodeFrames(h, m, s, f);
        }

        /// <summary>
        /// Convert decimal value to the BCD code.
        /// </summary>
        /// <param name="dec">Decimal value to conver.</param>
        /// <returns>BCD code.</returns>
        public static byte DecToBCD(int dec)
        {
            return (byte) ((dec%10) | ((dec/10) << 4));
        }

        /// <summary>
        /// Convert BCD code value to the decimal value.
        /// </summary>
        /// <param name="bcd">BCD code value.</param>
        /// <returns>Decimal value.</returns>
        public static byte BCDToDec(byte bcd)
        {
            return (byte) ((bcd & 0xF) + (bcd >> 4)*10);
        }

        public int FramesPerHour
        {
            get { return _framesPerHour; }
        }

        public int FramesPerMinute
        {
            get { return _framesPerMinute; }
        }

        public int FramesPerSecond
        {
            get { return _framesPerSecond; }
        }

        public string FramesToStrTimeCode(int frames)
        {
            int h, m, s, f;
            DecodeFrames(frames, out h, out m, out s, out f);
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", h, m, s, f);
        }

        public int StrTimeCodeToFrames(string StringTimeCode)
        {
            string[] tCArray = StringTimeCode.Split(new char[] {':', '.'});
            if (tCArray.Length == 4)
            {
                try
                {
                    return
                        EncodeFrames(
                            int.Parse(tCArray[0]), int.Parse(tCArray[1]), int.Parse(tCArray[2]), int.Parse(tCArray[3]));
                }
                catch
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
    }
}