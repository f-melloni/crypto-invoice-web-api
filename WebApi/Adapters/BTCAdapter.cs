using WebApi.Database.Entities;

namespace WebApi.Adapters
{
    public class BTCAdapter : BTCBasedAdapter
    {
        public BTCAdapter()
        {
            CurrencyCode = "BTC";
        }

        public override void GetAddress(int invoiceId, User loggedUser)
        {
            GetAddress(invoiceId, loggedUser.BTCXPUB);
        }
    }
}
