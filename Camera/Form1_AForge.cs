using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace camera
{
    public partial class Form1 : Form
    {
        FilterInfoCollection cameras;
        VideoCaptureDevice cam;
        public Form1()
        {
            InitializeComponent();
            cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo info in cameras)
            {
                comboBox1.Items.Add(info.Name);
            }
            comboBox1.SelectedIndex = 0;  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cam != null && cam.IsRunning) cam.Stop();
            cam = new VideoCaptureDevice(cameras[comboBox1.SelectedIndex].MonikerString);
            cam.NewFrame += Cam_NewFrame;
            cam.Start();
        }

        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap b = (Bitmap)eventArgs.Frame.Clone(); 
            pictureBox1.Image = b;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cam != null && cam.IsRunning) cam.Stop();
        }
    }
}
