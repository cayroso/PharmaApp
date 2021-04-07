using Cayent.Core.Data.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.App.Models.Users
{
    public class Role : RoleBase
    {
    }

    public class RoleConfiguration : RoleConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            base.Configure(builder);
            this.ConfigureEntity(builder);
        }

        private void ConfigureEntity(EntityTypeBuilder<Role> builder)
        {
        }
    }
}
