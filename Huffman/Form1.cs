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
using Newtonsoft.Json;

namespace Huffman
{
    public partial class Form1 : Form
    {
        bool draw = false;

        bool isDark = false;

        Node rootNode;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            draw = true;
            initiate(textBox1.Text);
        }

        private void initiate(String inputText)
        {
            char[] chars = inputText.ToCharArray();

            FrequencyItem[] freqs = getCharsFrequencyTable(chars, sort: true);

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(computeFrequencyTreeAsTreeNode(freqs));
            treeView1.ExpandAll();

            Node final = computeFrequencyTree(freqs);

            rootNode = final;

            textBox2.Text = encodeToText(final, inputText);

            beginDraw(final);
        }

        private String encodeToText(Node root, String text)
        {
            List<VisualNode> lowLevelNodes = root.createVisualNodeTree().getLowLevelNodes().ToList();

            String encoded = "";

            foreach (char chr in text.ToCharArray())
            {
                VisualNode _item = firstWhere(lowLevelNodes,
                    delegate (VisualNode item)
                    {
                        return item.getLabel().Equals(chr);
                    });

                foreach (int val in _item.getIndexTree())
                    encoded += val.ToString();
                encoded += "-";
            }
            encoded = encoded.Substring(0, encoded.Length - 1);

            return encoded;
        }

        private int getLocantOfNode(Node root)
        {
            int count = 1;
            Node currentNode = root;
            while(currentNode.getRightNode() != null)
            {
                currentNode = currentNode.getRightNode();
                count++;
            }
            return count;
        }

        private void beginDraw(Node root)
        {
            int height = getLocantOfNode(root);

            VisualNode visualRoot = root.createVisualNodeTree();

            generateVisualNodeLocations(ref visualRoot, height);

            const int padding = 20;
            Size bounds = new Size(pictureBox1.Size.Width - (2 * padding), pictureBox1.Size.Height - (2 * padding));

            List<VisualNode> currentLevelNodes = new List<VisualNode>();
            currentLevelNodes.Add(visualRoot);

            Graphics gfx = pictureBox1.CreateGraphics();
            gfx.Clear(BackColor);

            List<VisualNode> queue = new List<VisualNode>();
            queue.Add(visualRoot);

            // Styles
            int circleRadius = 30;

            Brush circleFill = isDark ? Brushes.Black : Brushes.Black;
            Pen circleOutline = isDark ? Pens.White : Pens.Black;

            Brush innerLabelColor = isDark ? Brushes.White : Brushes.White;

            Brush outerLabelColor = isDark ? Brushes.White : Brushes.Black;

            Pen lineColor = isDark ? Pens.White : Pens.Black;

            Brush branchNumberColor = isDark ? Brushes.White : Brushes.Black;
            //

            while (queue.Count > 0)
            {
                VisualNode item = queue[0];
                queue.RemoveAt(0);

                if (item.getLeftNode() != null)
                {
                    queue.Add(item.getLeftNode());
                    queue.Add(item.getRightNode());
                }

                Point rectPos = new Point((int) ( (float) ( bounds.Width + padding ) * item.getLocation().X ),
                    (int) ( ( bounds.Height + padding ) * item.getLocation().Y ));

                // Draw lines
                if (item.getLeftNode() != null)
                {
                    gfx.DrawLine(lineColor, new PointF(item.getLocation().X * (bounds.Width + padding), item.getLocation().Y * (bounds.Height + padding)),
                        new PointF(item.getLeftNode().getLocation().X * (bounds.Width + padding), item.getLeftNode().getLocation().Y * (bounds.Height + padding)));
                    gfx.DrawLine(lineColor, new PointF(item.getLocation().X * (bounds.Width + padding), item.getLocation().Y * (bounds.Height + padding )),
                        new PointF(item.getRightNode().getLocation().X * (bounds.Width + padding ), item.getRightNode().getLocation().Y * (bounds.Height + padding )));
                }
                //

                // Draw branch numbers
                if (item.getLeftNode() != null && checkBox1.Checked)
                {
                    Size _prefSize = gfx.MeasureString("0", Font).ToSize();
                    gfx.DrawString("0", Font, branchNumberColor, new PointF(
                        ( item.getLocation().X * ( bounds.Width + padding )
                        + item.getLeftNode().getLocation().X * ( bounds.Width + padding ) ) / 2 - (_prefSize.Width),
                        ( item.getLocation().Y * ( bounds.Height + padding )
                        + item.getLeftNode().getLocation().Y * ( bounds.Height + padding ) ) / 2 - (_prefSize.Height)));
                    _prefSize = gfx.MeasureString("1", Font).ToSize();
                    gfx.DrawString("1", Font, branchNumberColor, new PointF(
                        ( item.getLocation().X * ( bounds.Width + padding )
                        + item.getRightNode().getLocation().X * ( bounds.Width + padding ) ) / 2 + (_prefSize.Width),
                        ( item.getLocation().Y * ( bounds.Height + padding )
                        + item.getRightNode().getLocation().Y * ( bounds.Height + padding ) ) / 2 - (_prefSize.Height)));
                }
                //

                // Draw Circles
                gfx.FillEllipse(circleFill, new Rectangle(new Point(rectPos.X - circleRadius / 2, rectPos.Y - circleRadius / 2),
                    new Size(circleRadius, circleRadius)));
                gfx.DrawEllipse(circleOutline, new Rectangle(new Point(rectPos.X - circleRadius / 2, rectPos.Y - circleRadius / 2),
                    new Size(circleRadius, circleRadius)));
                //

                // Inner label
                Size prefSize = gfx.MeasureString(item.getValue().ToString(), Font).ToSize();
                gfx.DrawString(item.getValue().ToString(), Font, innerLabelColor, new Point(rectPos.X - (prefSize.Width / 2), rectPos.Y - ( prefSize.Height / 2 )));
                //

                // Outer label
                if (item.getLabel() != '\u0000' && checkBox2.Checked)
                {
                    prefSize = gfx.MeasureString(item.getLabel() == ' ' ? "' '" : item.getLabel().ToString(), Font).ToSize();
                    gfx.DrawString(item.getLabel() == ' ' ? "' '" : item.getLabel().ToString(), Font, outerLabelColor, new Point(rectPos.X - ( prefSize.Width / 2 ), rectPos.Y + circleRadius - ( prefSize.Height / 2 )));
                }
                //
            }
        }

        private bool generateVisualNodeLocations(ref VisualNode root, int height)
        {
            List<VisualNode> queue = new List<VisualNode>();
            queue.Add(root);

            int level = 1;
            int counter = queue.Count;
            while(queue.Count > 0)
            {
                if (counter == 0)
                { 
                    counter = queue.Count;
                    level++;
                }

                VisualNode item = queue[0];
                queue.RemoveAt(0);

                int[] indexTree = item.getIndexTree();

                PointF bias;

                if (indexTree.Length == 0)
                {
                    bias = new PointF(0.5f, 1 / ((float)height + 1));
                } else
                {
                    float LL = 0;
                    float UL = 1f;

                    foreach (int index in indexTree)
                    {
                        float MID = (UL + LL) / 2;
                        if (index == 0)
                            UL = MID;
                        else
                            LL = MID;
                    }

                    bias = new PointF((LL + UL) / 2, 1 / ((float) height + 1) * level);
                }

                item.setLocation(bias);

                counter--;

                if (item.getLeftNode() != null)
                {
                    queue.Add(item.getLeftNode());
                    queue.Add(item.getRightNode());
                }
            }

            return true;
        }

        private Node computeFrequencyTree(FrequencyItem[] items)
        {
            List<Node> copyItems = new List<Node>(items.Length);
            foreach (FrequencyItem item in items)
                copyItems.Add(new Node(item.getLabel(), item.getFrequency()));
                
            while (copyItems.Count > 1)
            {
                Node item1 = copyItems[0],
                    item2 = copyItems[1];

                Node newItem = new Node(item1.getValue() + item2.getValue());
                item1.setParentNode(newItem);
                item2.setParentNode(newItem);
                newItem.setLeftNode(item1);
                newItem.setRightNode(item2);

                copyItems.RemoveAt(0);
                copyItems.RemoveAt(0);

                copyItems.Add(newItem);
            }

            return copyItems[0];
        }

        private TreeNode computeFrequencyTreeAsTreeNode(FrequencyItem[] items)
        {
            List<TreeNode> copyItems = new List<TreeNode>(items.Length);
            List<int> values = new List<int>();
            foreach (FrequencyItem item in items)
            {
                TreeNode node = new TreeNode();

                node.Text = (item.getLabel() != '\u0000' ? item.getLabel() + " : " : "") + item.getFrequency().ToString();
                copyItems.Add(node);
                values.Add(item.getFrequency());
            }

            while (copyItems.Count > 1)
            {
                int value1 = values[0],
                    value2 = values[1];
                TreeNode item1 = copyItems[0],
                    item2 = copyItems[1];

                TreeNode newItem = new TreeNode();
                newItem.Text = (value1 + value2).ToString();
                newItem.Nodes.Add(item1);
                newItem.Nodes.Add(item2);

                copyItems.RemoveAt(0);
                copyItems.RemoveAt(0);
                values.RemoveAt(0);
                values.RemoveAt(0);

                copyItems.Add(newItem);
                values.Add(value1 + value2);
            }

            return copyItems[0];
        }

        private FrequencyItem[] getCharsFrequencyTable(char[] chars, bool sort = false)
        {
            List<FrequencyItem> items = new List<FrequencyItem>();

            foreach (char chr in chars)
            {
                FrequencyItem item = null;

                if (items.Count > 0)
                    item = firstWhere(items, 
                        delegate (FrequencyItem _item) {
                            if (_item == null)
                                return true;
                            if (_item.getLabel().Equals(chr))
                                return true;
                            return false;
                        });

                if (item == null)
                {
                    item = new FrequencyItem(chr, 1);
                    items.Add(item);
                }
                else
                    item.setFrequency(item.getFrequency() + 1);
            }

            if (sort)
                items.OrderBy(delegate (FrequencyItem item)
                {
                    return item.getFrequency();
                });

            return items.ToArray();
        }

        
        private T firstWhere<T>(List<T> items, Func<T, bool> action)
        {
            foreach (T item in items)
                if (action.Invoke(item))
                    return item;
            return default(T);
        }

        private T firstWhere<T>(T[] items, Func<T, bool> action)
        {
            foreach (T item in items)
                if (action.Invoke(item))
                    return item;
            return default(T);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = textBox1.TextLength > 0;
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.SelectAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            draw = false;
            textBox1.Text = "Huffman";
            textBox2.Text = "-------";
            treeView1.Nodes.Clear();
            pictureBox1.CreateGraphics().Clear(BackColor);
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (draw)
                button1.PerformClick();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.ForeColor = isDark ? SystemColors.WindowText : SystemColors.Control;
            this.BackColor = isDark ? SystemColors.Control : Color.FromArgb(255, 32, 32, 32);

            textBox1.ForeColor = ForeColor;
            textBox1.BackColor = isDark ? SystemColors.Window : BackColor;

            textBox2.ForeColor = ForeColor;
            textBox2.BackColor = isDark ? SystemColors.Window : BackColor;

            groupBox1.ForeColor = ForeColor;
            groupBox2.ForeColor = ForeColor;

            treeView1.ForeColor = ForeColor;
            treeView1.BackColor = isDark ? SystemColors.Window : BackColor;

            isDark = !isDark;

            Properties.Settings.Default.isDark = isDark;
            Properties.Settings.Default.Save();

            button2.PerformClick();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.isDark)
                button3.PerformClick();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Form2 frm2 = new Form2(generateImage, saveImage);
            frm2.Show();
        }

        // Callback functions

        private Image generateImage(bool showBranchNumbers, bool showLabels, bool showInformation,
            Color bgColor, Resolution canvasResolution)
        {
            Image img = new Bitmap(canvasResolution.Width, canvasResolution.Height);
            Graphics gfx = Graphics.FromImage(img);
            gfx.Clear(bgColor);

            int height = getLocantOfNode(rootNode);

            VisualNode visualRoot = rootNode.createVisualNodeTree();

            generateVisualNodeLocations(ref visualRoot, height);

            const int padding = 20;
            Size bounds = new Size(img.Size.Width - ( 2 * padding ), img.Size.Height - ( 2 * padding ));

            List<VisualNode> currentLevelNodes = new List<VisualNode>();
            currentLevelNodes.Add(visualRoot);

            List<VisualNode> queue = new List<VisualNode>();
            queue.Add(visualRoot);

            // Styles
            int circleRadius = 30;

            Brush circleFill = Brushes.Black;
            Pen circleOutline = Pens.Black;

            Brush innerLabelColor = Brushes.White;

            Brush outerLabelColor = Brushes.Black;

            Pen lineColor = Pens.Black;

            Brush branchNumberColor = Brushes.Black;
            //

            // Draw information
            if (showInformation)
                gfx.DrawString("Huffman encoding of '" + textBox1.Text + "'", Font, Brushes.Black, new Point(5, 5));
            //

            while (queue.Count > 0)
            {
                VisualNode item = queue[0];
                queue.RemoveAt(0);

                if (item.getLeftNode() != null)
                {
                    queue.Add(item.getLeftNode());
                    queue.Add(item.getRightNode());
                }

                Point rectPos = new Point((int) ( (float) ( bounds.Width + padding ) * item.getLocation().X ),
                    (int) ( ( bounds.Height + padding ) * item.getLocation().Y ));

                // Draw lines
                if (item.getLeftNode() != null)
                {
                    gfx.DrawLine(lineColor, new PointF(item.getLocation().X * ( bounds.Width + padding ), item.getLocation().Y * ( bounds.Height + padding )),
                        new PointF(item.getLeftNode().getLocation().X * ( bounds.Width + padding ), item.getLeftNode().getLocation().Y * ( bounds.Height + padding )));
                    gfx.DrawLine(lineColor, new PointF(item.getLocation().X * ( bounds.Width + padding ), item.getLocation().Y * ( bounds.Height + padding )),
                        new PointF(item.getRightNode().getLocation().X * ( bounds.Width + padding ), item.getRightNode().getLocation().Y * ( bounds.Height + padding )));
                }
                //

                // Draw branch numbers
                if (item.getLeftNode() != null && showBranchNumbers)
                {
                    Size _prefSize = gfx.MeasureString("0", Font).ToSize();
                    gfx.DrawString("0", Font, branchNumberColor, new PointF(
                        ( item.getLocation().X * ( bounds.Width + padding )
                        + item.getLeftNode().getLocation().X * ( bounds.Width + padding ) ) / 2 - ( _prefSize.Width ),
                        ( item.getLocation().Y * ( bounds.Height + padding )
                        + item.getLeftNode().getLocation().Y * ( bounds.Height + padding ) ) / 2 - ( _prefSize.Height )));
                    _prefSize = gfx.MeasureString("1", Font).ToSize();
                    gfx.DrawString("1", Font, branchNumberColor, new PointF(
                        ( item.getLocation().X * ( bounds.Width + padding )
                        + item.getRightNode().getLocation().X * ( bounds.Width + padding ) ) / 2 + ( _prefSize.Width ),
                        ( item.getLocation().Y * ( bounds.Height + padding )
                        + item.getRightNode().getLocation().Y * ( bounds.Height + padding ) ) / 2 - ( _prefSize.Height )));
                }
                //

                // Draw Circles
                gfx.FillEllipse(circleFill, new Rectangle(new Point(rectPos.X - circleRadius / 2, rectPos.Y - circleRadius / 2),
                    new Size(circleRadius, circleRadius)));
                gfx.DrawEllipse(circleOutline, new Rectangle(new Point(rectPos.X - circleRadius / 2, rectPos.Y - circleRadius / 2),
                    new Size(circleRadius, circleRadius)));
                //

                // Inner label
                Size prefSize = gfx.MeasureString(item.getValue().ToString(), Font).ToSize();
                gfx.DrawString(item.getValue().ToString(), Font, innerLabelColor, new Point(rectPos.X - ( prefSize.Width / 2 ), rectPos.Y - ( prefSize.Height / 2 )));
                //

                // Outer label
                if (item.getLabel() != '\u0000' && showLabels)
                {
                    prefSize = gfx.MeasureString(item.getLabel() == ' ' ? "' '" : item.getLabel().ToString(), Font).ToSize();
                    gfx.DrawString(item.getLabel() == ' ' ? "' '" : item.getLabel().ToString(), Font, outerLabelColor, new Point(rectPos.X - ( prefSize.Width / 2 ), rectPos.Y + circleRadius - ( prefSize.Height / 2 )));
                }
                //
            }

            return img;
        }

        private void saveImage(Image img)
        {
            saveFileDialog1.FileName = textBox1.Text;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                img.Save(saveFileDialog1.FileName);
        }

        //
    }
}
