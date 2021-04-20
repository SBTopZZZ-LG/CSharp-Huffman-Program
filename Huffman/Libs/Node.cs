using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Node
{
    private Node leftNode;
    private Node rightNode;

    private char label;
    private int value;

    public Node(int value)
    {
        this.value = value;
    }
    public Node(char label, int value)
    {
        this.label = label;
        this.value = value;
    }
    public Node(char label, int value, Node leftNode, Node rightNode)
    {
        this.label = label;
        this.value = value;

        this.leftNode = leftNode;
        this.rightNode = rightNode;
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

    public void setValue(int value)
    {
        this.value = value;
    }
}
