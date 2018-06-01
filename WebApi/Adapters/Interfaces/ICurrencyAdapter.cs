using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Database.Entities;

namespace WebApi.Adapters.Interfaces
{
    public interface ICurrencyAdapter
    {
        void GetAddress(int invoiceId, string xpub);
        void GetAddress(int invoiceId, User loggedUser);
        string GetVarSymbol();
        double GetExchangeRate(string fiatCurrencyCode);
    }
}
