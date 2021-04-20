using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FrequencyItem
{
    private char label;
    private int frequency;

    public FrequencyItem(int frequency)
    {
        this.frequency = frequency;
    }
    public FrequencyItem(char label, int frequency)
    {
        this.label = label;
        this.frequency = frequency;
    }

    public char getLabel()
    {
        return this.label;
    }

    public void setFrequency(int frequency)
    {
        this.frequency = frequency;
    }
    public int getFrequency()
    {
        return this.frequency;
    }
}
