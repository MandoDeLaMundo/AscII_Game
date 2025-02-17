﻿using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscII_Game.Systems
{
    public class MessageLog
    {
        private static readonly int _maxLines = 9;

        private readonly Queue<string> _lines;

        public MessageLog()
        {
            _lines = new Queue<string>();
        }

        public void Add(string message)
        {
            //Add a new line to the message bar
            _lines.Enqueue(message);

            //When a new line is added and it exceeds the maximum, it removes the oldest line
            if( _lines.Count > _maxLines )
            {
                _lines.Dequeue();
            }
        }

        public void Draw(RLConsole console)
        {
            // console.Clear();
            string[] lines = _lines.ToArray();
            for(int i = 0; i < lines.Length; i++)
            {
                console.Print(1, i +1, lines[i], RLColor.White);
            }
        }
    }
}
