using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace camera
{
    public partial class Form2 : Form
    {
        private Capture cap;
        public Form2()
        {
            InitializeComponent();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Image<Bgr, byte> nextFrame = cap.QueryFrame();
            Image<Gray, byte> grayframe = nextFrame.Convert<Gray, byte>();
            p1.Image = nextFrame.ToBitmap();    //hien anh mau  
            p2.Image = grayframe.ToBitmap();    //hien anh trang den
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            cap = new Capture(0); //set up camera
            timer1.Enabled = true;

        }
    }
}
