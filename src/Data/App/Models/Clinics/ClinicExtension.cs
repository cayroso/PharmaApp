using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Clinics
{
    public static class ClinicExtension
    {
        public static void ThrowIfNull(this Clinic me)
        {
            if (me == null)
                throw new ApplicationException("Clinic not found.");
        }
        public static void ThrowIfNullOrAlreadyUpdated(this Clinic me, string currentToken, string newToken)
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
