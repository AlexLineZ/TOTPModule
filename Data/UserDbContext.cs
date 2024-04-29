using Microsoft.EntityFrameworkCore;
using OTPModule.Entities;
using System;

namespace OTPModule.Data
{
    public class UserDbContext:DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }

        public DbSet<UserEntity> userEntities { get; set; }
        public DbSet<SecretKeyEntity> secretKeyEntities { get; set; }

    }
}
