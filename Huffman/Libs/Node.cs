using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Node parentNode;

    public char label;
    public int value;

    public Node() {
        value = 0;
    }
    public Node(int value)
    {
        this.value = value;
    }
    public Node(char label, int value)
    {
        this.label = label;
        this.value = value;
    }
    public Node(char label, int value, Node leftNode, Node rightNode, Node parentNode)
    {
        this.label = label;
        this.value = value;

        this.leftNode = leftNode;
        this.rightNode = rightNode;
    }

    public Node getParentNode()
    {
        return this.parentNode;
    }
    public void setParentNode(Node parentNode)
    {
        this.parentNode = parentNode;
    }

    public Node getLeftNode()
    {
        return this.leftNode;
    }
    public Node getRightNode()
    {
        return this.rightNode;
    }

    public void setLeftNode(Node leftNode)
    {
        this.leftNode = leftNode;
    }
    public void setRightNode(Node rightNode)
    {
        this.rightNode = rightNode;
    }

    public char getLabel()
    {
        return this.label;
    }
    public int getValue()
    {
        return this.value;
    }

    public void setLabel(char label)
    {
        this.label = label;
    }
    public void setValue(int value)
    {
        this.value = value;
    }

    public VisualNode createVisualNodeTree(VisualNode parent = null)
    {
        VisualNode node = new VisualNode(this.parentNode == null ? -1 : (this.parentNode.getLeftNode() == this ? 0 : 1));
        node.setLeftNode(leftNode != null ? leftNode.createVisualNodeTree(parent: node) : null);
        node.setRightNode(rightNode != null ? rightNode.createVisualNodeTree(parent: node) : null);
        node.setParentNode(parent);
        node.setLabel(label);
        node.setValue(value);

        return node;
    }
    public Node[] getLowLevelNodes()
    {
        List<Node> lowLevelNodes = new List<Node>();

        if (leftNode == null)
        {
            lowLevelNodes.Add(this);
        } else
        {
            foreach (Node item in leftNode.getLowLevelNodes())
                lowLevelNodes.Add(item);
            foreach (Node item in rightNode.getLowLevelNodes())
                lowLevelNodes.Add(item);
        }

        return lowLevelNodes.ToArray();
    }
}

public class VisualNode : Node
{
    public PointF location;

    public new VisualNode leftNode;
    public new VisualNode rightNode;
    private new VisualNode parentNode;

    public int index = -1;

    public VisualNode(int index)
    {
        this.index = index;
    }

    public int[] getIndexTree()
    {
        List<int> ints = new List<int>();
        if (parentNode != null)
        {
            int[] _ints = parentNode.getIndexTree();

            foreach (int _index in _ints)
                ints.Add(_index);

            ints.Add(index);

            return ints.ToArray();
        } else
            return new int[] {};
    }

    public new VisualNode[] getLowLevelNodes()
    {
        List<VisualNode> lowLevelNodes = new List<VisualNode>();

        if (leftNode == null)
        {
            lowLevelNodes.Add(this);
        }
        else
        {
            foreach (VisualNode item in leftNode.getLowLevelNodes())
                lowLevelNodes.Add(item);
            foreach (VisualNode item in rightNode.getLowLevelNodes())
                lowLevelNodes.Add(item);
        }

        return lowLevelNodes.ToArray();
    }

    public new VisualNode getParentNode()
    {
        return parentNode;
    }
    public void setParentNode(VisualNode parentNode)
    {
        this.parentNode = parentNode;
    }

    public new VisualNode getLeftNode()
    {
        return leftNode;
    }
    public new VisualNode getRightNode()
    {
        return rightNode;
    }

    public void setLeftNode(VisualNode leftNode)
    {
        this.leftNode = leftNode;
    }
    public void setRightNode(VisualNode rightNode)
    {
        this.rightNode = rightNode;
    }

    public void setLocation(PointF location)
    {
        this.location = location;
    }

    public PointF getLocation()
    {
        return location;
    }
}
