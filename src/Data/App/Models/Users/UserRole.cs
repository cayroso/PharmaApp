using Cayent.Core.Data.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.App.Models.Users
{
    public class UserRole:UserRoleBase
    {

    }

    public class UserRoleConfiguration : UserRoleConfiguration<UserRole>
    {
        public override void Configure(EntityTypeBuilder<UserRole> builder)
        {
            base.Configure(builder);
            this.ConfigureEntity(builder);
        }

        private void ConfigureEntity(EntityTypeBuilder<UserRole> builder)
        {
        }
    }
}
