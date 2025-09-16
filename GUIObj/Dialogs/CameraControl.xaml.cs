using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GUIObj.Structs;
using HelpUtility;


namespace GUIObj.Dialogs
{
    /// <summary>
    /// Interaktionslogik für CameraControl.xaml
    /// </summary>
    public partial class CameraControl
    {

        private VirtualCamera _currentCam;

        private readonly List<VirtualCamera> _cameras;
        private bool _cameraControlChange;
        private bool _cameraPositionMove;
        private bool _cameraViewPointMove;
        private Point _cameraRefPoint;

        public delegate void CameraMove(VirtualCamera cam);

        public event CameraMove OnCameraMove;

        public CameraControl(IEnumerable<VirtualCamera> cams)
        {
            _cameraPositionMove = true;
            InitializeComponent();
            _cameraPositionMove = false;
            _cameras=new List<VirtualCamera>();
            UpdateCameraSettingsControlsPos(true);
            
            foreach (VirtualCamera tvlCamera in cams)
            {
                _cameras.Add(tvlCamera);
                lb_settings_camera.Items.Add(new ListBoxItem() { Content = tvlCamera.Description });
            }
        }

        public List<VirtualCamera> Cameras
        {
            get { return _cameras; }
        }

        private void lb_settings_camera_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int sel = lb_settings_camera.SelectedIndex;
            if (sel < 0)
                return;
            _currentCam = Cameras[sel];
            CameraHlList(_currentCam.Description);

            _cameraControlChange = true;
            tb_camera_description.Text = _currentCam.Description;
            dud_camera_z.Value = _currentCam.PosZ;
            dud_camera_x.Value = _currentCam.PosX;
            dud_camera_pan.Value = _currentCam.Pan;
            iud_camera_h.Value = _currentCam.Height;
            iud_camera_zoom.Value = _currentCam.Zoom;
            dud_camera_tilt.Value = _currentCam.Tilt;
            _cameraControlChange = false;

            UpdateCameraSettingsControlsPos(true);
            iud_camera_time.Value = _currentCam.Time;
            cb_camera_startcam.IsChecked = _currentCam.IsStartCam;
        }

        private void img_camera_viewpoint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Canvas c = (Canvas)sender;
                if (c == null)
                    return;
                _cameraViewPointMove = true;
                c.CaptureMouse();
                _cameraRefPoint = e.GetPosition(ca_camera_setting);
            }
        }

        private void img_camera_viewpoint_MouseMove(object sender, MouseEventArgs e)
        {
            Point x = e.GetPosition(ca_camera_setting);
            if (e.LeftButton == MouseButtonState.Pressed && _cameraViewPointMove)
            {
                Canvas c = (Canvas)sender;
                if (c == null)
                    return;

                Canvas.SetLeft(c, Canvas.GetLeft(c) + (x.X - _cameraRefPoint.X));
                Canvas.SetTop(c, Canvas.GetTop(c) + (x.Y - _cameraRefPoint.Y));
                UpdateCameraSettingsField();
            }
            _cameraRefPoint = x;
        }

        private void img_camera_viewpoint_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Canvas c = (Canvas)sender;
            if (c == null)
                return;
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    _cameraViewPointMove = false;
                    c.ReleaseMouseCapture();
                    UpdateCameraSettingsField();
                    break;

            }
        }

        private void img_camera_position_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Canvas c = (Canvas)sender;
                if (c == null)
                    return;
                _cameraPositionMove = true;
                c.CaptureMouse();
                _cameraRefPoint = e.GetPosition(ca_camera_setting);
            }
        }

        private void img_camera_position_MouseMove(object sender, MouseEventArgs e)
        {
            Point x = e.GetPosition(ca_camera_setting);
            if (e.LeftButton == MouseButtonState.Pressed && _cameraPositionMove)
            {
                Canvas c = (Canvas)sender;
                if (c == null)
                    return;

                Canvas.SetLeft(c, Canvas.GetLeft(c) + (x.X - _cameraRefPoint.X));
                Canvas.SetTop(c, Canvas.GetTop(c) + (x.Y - _cameraRefPoint.Y));
                UpdateCameraSettingsField();
            }
            _cameraRefPoint = x;
        }

        private void img_camera_position_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Canvas c = (Canvas)sender;
            if (c == null)
                return;
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    _cameraPositionMove = false;
                    c.ReleaseMouseCapture();
                    UpdateCameraSettingsField();
                    break;

            }
        }

        private void ca_camera_setting_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double s = (double)ca_camera_setting.RenderTransform.GetValue(ScaleTransform.ScaleXProperty);
            if (s > 0.5)
            {
                VisualStateManager.GoToElementState(gr_maingrid,
                                                e.Delta > 0 ? "CameraBig" : "CameraSmall", false);
            }
            else if (s < 0.5)
            {
                VisualStateManager.GoToElementState(gr_maingrid,
                                                e.Delta > 0 ? "CameraSmall" : "CameraSmallest", false);
            }
            else
            {
                VisualStateManager.GoToElementState(gr_maingrid,
                                                e.Delta > 0 ? "CameraBig" : "CameraSmallest", false);
            }
        }

        private void btn_savecamera_Click(object sender, RoutedEventArgs e)
        {
            if (tb_camera_description.Text.Contains("(readonly)"))
                return;
            VirtualCamera cam = Cameras.FirstOrDefault(camera => camera.Description == tb_camera_description.Text);
            if (cam == null)
            {
                cam = new VirtualCamera { Description = tb_camera_description.Text };
                Cameras.Add(cam);
                ListBoxItem lbic = new ListBoxItem { Content = cam.Description };
                lb_settings_camera.Items.Add(lbic);
  
                lb_settings_camera.SelectedIndex = lb_settings_camera.Items.Count - 1;
                CameraHlList(cam.Description);
                _currentCam = cam;
            }
            cam.PosZ = dud_camera_z.Value;
            cam.PosX = dud_camera_x.Value;
            cam.Pan = dud_camera_pan.Value;
            cam.Tilt = dud_camera_tilt.Value;
            cam.Height = iud_camera_h.Value;
            cam.Zoom = iud_camera_zoom.Value;
            cam.Time = iud_camera_time.Value;
            if (cb_camera_startcam.IsChecked == true)
            {
                foreach (VirtualCamera camera in Cameras)
                {
                    camera.IsStartCam = false;
                }
            }
            cam.IsStartCam = cb_camera_startcam.IsChecked == true;
        }

        private void position_L2V(double lx, double ly, out double vx, out double vy)
        {
            vy = -((((ly - 150) / 1300) * 10480.0) - 5240);
            vx = ((((lx - 285) / 840) * 6760.0) - 3380);
        }

        private void position_V2L(double vx, double vy, out double lx, out double ly)
        {
            ly = (((10480 - (vy + 5240)) / 10480.0) * 1300) + 150;
            lx = (((vx + 3380) / 6760.0) * 840) + 285;
        }

        private void CameraHlList(string selected)
        {
            foreach (ListBoxItem lbi in lb_settings_camera.Items)
            {
                lbi.Background = (string)lbi.Content == selected ? new SolidColorBrush(Colors.Yellow) : new SolidColorBrush(Colors.White);
            }
        }

        private void UpdateCameraSettingsControlsPos(bool forcelock)
        {
            if (_cameraPositionMove || _cameraViewPointMove)
                return;
            try
            {
                double cx = dud_camera_z.Value;
                double cy = dud_camera_x.Value;

                double vx = Canvas.GetLeft(img_camera_viewpoint) + 50;
                double vy = Canvas.GetTop(img_camera_viewpoint) + 50;
                double vvx;
                double vvy;
                position_L2V(vx, vy, out vvx, out vvy);
                double pan = dud_camera_pan.Value;
                int h = iud_camera_h.Value;
                double tilt = dud_camera_tilt.Value;
                if (tgl_camera_lock_pantilt.IsChecked == false && forcelock == false)
                {
                    pan = Utility.GetPan(cx, cy, vvx, vvy);
                    tilt = Utility.GetTilt(cx, cy, vvx, vvy, h);
                    dud_camera_pan.Value = Math.Round(pan, 2);
                    dud_camera_tilt.Value = Math.Round(tilt, 2);
                }
                else
                {
                    double pan2 = (360 - pan) - 90;
                    double dist = Utility.GetTiltDist(tilt, h);
                    double dx = dist * Math.Sin((pan2 * Math.PI) / 180.0);
                    double dy = dist * Math.Sin(((90 - pan2) * Math.PI) / 180.0);
                    double lvx;
                    double lvy;
                    position_V2L(cx + dx, cy + dy, out lvx, out lvy);

                    Canvas.SetLeft(img_camera_viewpoint, lvx - 50);
                    Canvas.SetTop(img_camera_viewpoint, lvy - 50);
                }

                //viz->feld
                double lcx;
                double lcy;
                position_V2L(cx, cy, out lcx, out lcy);
                Canvas.SetLeft(img_camera_position, lcx - 35);
                Canvas.SetTop(img_camera_position, lcy - 80);

                RotateTransform rotatepan = new RotateTransform((int)(-pan) + 270);
                img_camera_position.RenderTransform = rotatepan;

                if(OnCameraMove!=null)
                    OnCameraMove(new VirtualCamera("",cx,cy,h,pan,tilt,iud_camera_zoom.Value));
                //_vizCommands.CameraSet(cx, cy, h, pan, tilt, iud_camera_zoom.Value.GetValueOrDefault(45));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void UpdateCameraSettingsControlsPanTilt()
        {
            if (_cameraPositionMove || _cameraViewPointMove)
                return;
            try
            {
                double cx = dud_camera_z.Value;
                double cy = dud_camera_x.Value;

                double pan = dud_camera_pan.Value;
                double tilt = dud_camera_tilt.Value;
                int h = iud_camera_h.Value;
                double pan2 = (360 - pan) - 90;
                double dist = Utility.GetTiltDist(tilt, h);
                double dx = dist * Math.Sin((pan2 * Math.PI) / 180.0);
                double dy = dist * Math.Sin(((90 - pan2) * Math.PI) / 180.0);

                //viz->feld

                double lvx;
                double lvy;
                position_V2L(cx + dx, cy + dy, out lvx, out lvy);

                Canvas.SetLeft(img_camera_viewpoint, lvx - 50);
                Canvas.SetTop(img_camera_viewpoint, lvy - 50);

                RotateTransform rotatepan = new RotateTransform((int)(-pan) + 270);
                img_camera_position.RenderTransform = rotatepan;

                //_vizCommands.CameraSet(cx, cy, h, pan, tilt, iud_camera_zoom.Value.GetValueOrDefault(45));

                if (OnCameraMove != null)
                    OnCameraMove(new VirtualCamera("", cx, cy, h, pan, tilt, iud_camera_zoom.Value));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void UpdateCameraSettingsField()
        {
            double vcx, vcy, vvx, vvy;
            double cx = Canvas.GetLeft(img_camera_position) + 35;
            double cy = Canvas.GetTop(img_camera_position) + 80;
            double vx = Canvas.GetLeft(img_camera_viewpoint) + 50;
            double vy = Canvas.GetTop(img_camera_viewpoint) + 50;
            //feld ->viz
            position_L2V(cx, cy, out vcx, out vcy);
            position_L2V(vx, vy, out vvx, out vvy);

            dud_camera_z.Value = Math.Round(vcx, 1);
            dud_camera_x.Value = Math.Round(vcy, 1);
            double pan = Utility.GetPan(vcx, vcy, vvx, vvy);
            int h = iud_camera_h.Value;
            dud_camera_pan.Value = Math.Round(pan, 2);
            double tilt = Utility.GetTilt(vcx, vcy, vvx, vvy, h);
            dud_camera_tilt.Value = Math.Round(tilt, 2);
            RotateTransform rotatepan = new RotateTransform((int)(-pan) + 270);
            img_camera_position.RenderTransform = rotatepan;

            //_vizCommands.CameraSet(vcx, vcy, h, pan, tilt, iud_camera_zoom.Value.GetValueOrDefault(45));

            if (OnCameraMove != null)
                OnCameraMove(new VirtualCamera("", vcx, vcy, h, pan, tilt, iud_camera_zoom.Value));

        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void dud_camera_z_OnValueChanged(double value)
        {
            if (_cameraControlChange)
                return;
            _cameraControlChange = true;
            UpdateCameraSettingsControlsPos(false);
            _cameraControlChange = false;
        }

        private void iud_camera_h_OnValueChanged(int value)
        {
            if(_cameraControlChange)
                return;
            _cameraControlChange = true;
            UpdateCameraSettingsControlsPos(false);
            _cameraControlChange = false;
        }

        private void dud_camera_pan_OnValueChanged(double value)
        {
            if (_cameraControlChange)
                return;
            UpdateCameraSettingsControlsPanTilt();
        }

        private void iud_camera_time_OnValueChanged(int value)
        {
            UpdateCameraSettingsControlsPos(true);
        }
    }
}
