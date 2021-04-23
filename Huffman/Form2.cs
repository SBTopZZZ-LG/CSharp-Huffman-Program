using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Huffman.Libs;

namespace Huffman
{
    public partial class Form2 : Form
    {
        private Func<bool, bool, bool, Color, Resolution, Image> previewCall;

        private Action<Image> callback;

        private Image finalImage;

        public Form2(Func<bool, bool, bool, Color, Resolution, Image> previewCall,
            Action<Image> callback)
        {
            this.previewCall = previewCall;
            this.callback = callback;

            InitializeComponent();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = radioButton5.Checked;
            label2.Enabled = radioButton5.Checked;

            numericUpDown1.Enabled = radioButton5.Checked;
            numericUpDown2.Enabled = radioButton5.Checked;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            performPreviewCall();
        }

        private void performPreviewCall(bool updatePreviewImage = true)
        {
            finalImage = previewCall(checkBox1.Checked, checkBox2.Checked, checkBox3.Checked,
                radioButton2.Checked ? button3.BackColor : Color.Transparent, getSelectedResolution());

            if (updatePreviewImage)
                pictureBox1.Image = finalImage;
        }

        private Resolution getSelectedResolution()
        {
            int width = 450;
            int height = 450;

            if (radioButton4.Checked)
            {
                width = 800;
                height = 800;
            }

            if (radioButton5.Checked)
            {
                width = (int) numericUpDown1.Value;
                height = (int) numericUpDown2.Value;
            }

            return new Resolution(width, height);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            performPreviewCall();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            performPreviewCall();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            performPreviewCall();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Enabled = radioButton2.Checked;

            performPreviewCall();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            performPreviewCall();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                button3.BackColor = colorDialog1.Color;
            performPreviewCall();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            callback(finalImage);
            this.Close();
        }
    }
}
