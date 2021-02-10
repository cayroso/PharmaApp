using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Pharmacies
{
    public static class PharmacyExtension
    {
        public static void ThrowIfNull(this Pharmacy me)
        {
            if (me == null)
                throw new ApplicationException("Pharmacy not found.");
        }
        public static void ThrowIfNullOrAlreadyUpdated(this Pharmacy me, string currentToken, string newToken)
        {
            me.ThrowIfNull();

            if (string.IsNullOrWhiteSpace(newToken))
                throw new ApplicationException("Anti-forgery token not found.");

            if (me.ConcurrencyToken != currentToken)
                throw new ApplicationException("Already updated by another user.");

            me.ConcurrencyToken = newToken;
        }
    }
}
