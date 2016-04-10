using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    enum Command { Hit}

    struct Message
    {
        public double value;
        public Command command;

        public Message(int val, Command mess)
        {
            value = val;
            command = mess;
        }
    }
    struct MessageStack
    {
        public Stack<Message> mess;

        public MessageStack(bool IsThisEmpty)
        {
            mess = new Stack<Message>();
        }
        public int Count
        {
            get { return mess.Count; }
        }
        public void AddMessage(Command message, int val = 0)
        {
            mess.Push(new Message(val, message));
        }

        public Message Pop()
        {
            return mess.Pop();
        }
    }
}
