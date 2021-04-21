using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace Huffman
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

            //MessageBox.Show(getLocantOfNode(final).ToString());

            beginDraw(final, getLocantOfNode(final));
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

        private void beginDraw(Node root, int height)
        {
            VisualNode visualRoot = root.createVisualNodeTree();

            generateVisualNodeLocations(ref visualRoot, height);

            const int padding = 20;
            Size bounds = new Size(pictureBox1.Size.Width - 2 * padding, pictureBox1.Size.Height - 2 * padding);

            List<VisualNode> currentLevelNodes = new List<VisualNode>();
            currentLevelNodes.Add(visualRoot);

            Graphics gfx = pictureBox1.CreateGraphics();
            gfx.Clear(SystemColors.Control);

            List<VisualNode> queue = new List<VisualNode>();
            queue.Add(visualRoot);

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
                    (int) ( (float) ( bounds.Height + padding ) * item.getLocation().Y ));

                gfx.DrawRectangle(Pens.Black, 
                    new Rectangle(new Point(rectPos.X - 10, rectPos.Y - 10), 
                    new Size(20, 20)));
                Size prefSize = gfx.MeasureString(item.getValue().ToString(), Font).ToSize();
                gfx.DrawString(item.getValue().ToString(), Font, Brushes.Red, new Point(rectPos.X - (prefSize.Width / 2), rectPos.Y - ( prefSize.Height / 2 )));
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
            foreach (FrequencyItem item in items)
            {
                TreeNode node = new TreeNode();

                node.Text = item.getFrequency().ToString();
                copyItems.Add(node);
            }

            while (copyItems.Count > 1)
            {
                TreeNode item1 = copyItems[0],
                    item2 = copyItems[1];

                TreeNode newItem = new TreeNode();
                newItem.Text = (int.Parse(item1.Text) + int.Parse(item2.Text)).ToString();
                newItem.Nodes.Add(item1);
                newItem.Nodes.Add(item2);

                copyItems.RemoveAt(0);
                copyItems.RemoveAt(0);

                copyItems.Add(newItem);
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
                    item = map(items, 
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
        private FrequencyItem map(List<FrequencyItem> items, Func<FrequencyItem, bool> action)
        {
            foreach (FrequencyItem item in items)
                if (action.Invoke(item))
                    return item;
            return null;
        }
    }
}
