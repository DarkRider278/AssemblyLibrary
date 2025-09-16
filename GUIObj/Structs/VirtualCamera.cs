namespace GUIObj.Structs
{
    public class VirtualCamera
    {
        private string _description;
        private double _posZ; //-3380 - 3380
        private double _posX; //-5240 - 5240
        private double _pan;
        private double _tilt;
        private int _height;
        private int _zoom;
        private int _time;
        private bool _isStartCam;

        public VirtualCamera()
        {
            Description = "New Camera";
            PosZ = 0;
            PosX = 6500;
            Pan = 0;
            Tilt = -21;
            Height = 2500;
            Zoom = 45;
            Time = 1000;
            IsStartCam = false;
        }
        public VirtualCamera(VirtualCamera cam)
        {
            Description = cam.Description;
            Copyfrom(cam);
            IsStartCam = false;
        }
        public VirtualCamera(string description, double x, double z, int height, double pan, double tilt, int zoom)
        {
            Description = description;
            PosZ = z;
            PosX = x;
            Pan = pan;
            Tilt = tilt;
            Height = height;
            Zoom = zoom;
            Time = 1000;
            IsStartCam = false;
        }

        public void Copyfrom(VirtualCamera cam)
        {
            PosZ = cam.PosZ;
            PosX = cam.PosX;
            Pan = cam.Pan;
            Tilt = cam.Tilt;
            Height = cam.Height;
            Zoom = cam.Zoom;
            Time = cam.Time;
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public double PosZ
        {
            get { return _posZ; }
            set { _posZ = value; }
        }

        public double PosX
        {
            get { return _posX; }
            set { _posX = value; }
        }

        public double Pan
        {
            get { return _pan; }
            set { _pan = value; }
        }

        public double Tilt
        {
            get { return _tilt; }
            set { _tilt = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        public int Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public bool IsStartCam
        {
            get { return _isStartCam; }
            set { _isStartCam = value; }
        }
    }
}
