/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard
{
    public class CardHelper
    {
        protected APDUPlayer apduPlayer;
        protected APDUResponse lastApduResponse = null;

        protected CardHelper(ICard card)
        {
            Card = card;
            apduPlayer = new APDUPlayer(card);
        }

        public APDUResponse LastResponse
        {
            get { return lastApduResponse; }
        }

        public ushort StatusCode
        {
            get
            {
                ushort code = 0xFFFF;
                if (lastApduResponse != null)
                {
                    code = lastApduResponse.Status;
                }

                return code;
            }
        }

        public ICard Card
        {
            get;
            private set;
        }
    }
}
