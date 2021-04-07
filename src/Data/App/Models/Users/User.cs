using Cayent.Core.Data.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Data.App.Models.Users
{
    public class User: UserBase
    {        
        public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();

    }

    public static class UserExtension
    {

        public static void ThrowIfNull(this User me)
        {
            if (me == null)
                throw new ApplicationException("User not found.");
        }

        public static void ThrowIfNullOrAlreadyUpdated(this User me, string currentToken, string newToken)
        {
            me.ThrowIfNull();

            if (string.IsNullOrWhiteSpace(newToken))
                throw new ApplicationException("Anti-forgery token not found.");

            if (me.ConcurrencyToken != currentToken)
                throw new ApplicationException("User already updated by another user.");

            me.ConcurrencyToken = newToken;
        }
    }

    public class UserConfiguration : UserConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            this.ConfigureEntity(builder);
        }

        private void ConfigureEntity(EntityTypeBuilder<User> builder)
        {
        }
    }
}
