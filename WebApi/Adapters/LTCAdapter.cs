using WebApi.Database.Entities;

namespace WebApi.Adapters
{
    public class LTCAdapter : BTCBasedAdapter
    {
        public LTCAdapter()
        {
            CurrencyCode = "LTC";
        }

        public override void GetAddress(int invoiceId, User loggedUser)
        {
            GetAddress(invoiceId, loggedUser.LTCXPUB);
        }
    }
}
