using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Day_8
{
    public class SIFImage
    {
        public int layers;

        private List<byte[]> _layerData = new List<byte[]>();
        private byte[] _rawData;
        private bool[] _pixelsWritten;
        private int _w;
        private int _h;

        private static readonly Dictionary<int, ConsoleColor> colorMap = new Dictionary<int, ConsoleColor>
        {
            { 0, ConsoleColor.White },
            { 1, ConsoleColor.Black },
        };

        public SIFImage(int width, int height, FileStream stream)
        {
            StreamReader sr = new StreamReader(stream);
            _w = width;
            _h = height;
            layers = (int)stream.Length / (_w * _h);
            _pixelsWritten = new bool[_w * _h];

            char[] temp = new char[stream.Length];
            sr.Read(temp, 0, temp.Length);
            _rawData = temp.Select(c => (byte)(c - 48)).ToArray();

            sr.Close();
            stream.Close();

            SeparateLayers();
        }

        private void SeparateLayers()
        {
            for (int i = 0; i < layers; i++)
            {
                _layerData.Add(_rawData.Skip(_w * _h * i).Take(_w * _h).ToArray());
            }
        }

        public void RenderImage(int startX, int startY)
        {
            int x;
            int y;

            foreach (byte[] layer in _layerData)
            {
                x = startX;
                y = startY;

                Console.SetCursorPosition(x, y);

                for (int i = 0; i < layer.Length; i++)
                {
                    Console.SetCursorPosition(x, y);

                    if (layer[i] != 2 && !_pixelsWritten[x + y * _w])
                    {
                        Console.BackgroundColor = colorMap[layer[i]];
                        Console.ForegroundColor = colorMap[layer[i]];
                        Console.WriteLine(" ");

                        //Thread.Sleep(35);

                        _pixelsWritten[x + y * _w] = true;
                    }

                    if (x >= _w)
                    {
                        x = startX;
                        y += 1;
                        Console.SetCursorPosition(x, y);
                    }

                    x += 1;
                }
            }
        }

        public byte[] GetRawData()
        {
            return _rawData;
        }
    }
}
