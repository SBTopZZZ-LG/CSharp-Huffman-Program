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
            char[] chars = textBox1.Text.ToCharArray();

            FrequencyItem[] freqs = getCharsFrequencyTable(chars, sort: true);

            TreeNode final = computeFrequencyTreeAsTreeNode(freqs);

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(final);
            treeView1.ExpandAll();
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
