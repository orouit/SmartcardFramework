/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Smartcard
{
    public abstract class CardEventArgs : EventArgs
    {
        public string Reader
        {
            get;
            private set;
        }

        public CardEventArgs(string reader)
        {
            Reader = reader;
        }
    }

    public class CardInsertedArgs : CardEventArgs
    {
        public ICard Card
        {
            get;
            private set;
        }

        public CardInsertedArgs(string reader, ICard card)
            : base(reader)
        {
            Card = card;
        }
    }

    public class CardRemovedArgs : CardEventArgs
    {
        public CardRemovedArgs(string reader)
            : base(reader)
        {
        }
    }
}
