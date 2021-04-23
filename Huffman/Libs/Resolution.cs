using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman.Libs
{
    public class Resolution
    {
        private bool isEmpty = true;
        public int Width
        {
            get;
        }
        public int Height
        {
            get;
        }
        public double aspectRatio;

        public Resolution()
        { }
        public Resolution(int side)
        {
            isEmpty = false;

            Width = side;
            Height = side;
        }
        public Resolution(int width, int height)
        {
            isEmpty = false;

            this.Width = width;
            this.Height = height;
        }
    }
}