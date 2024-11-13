using account_service.models;
using Microsoft.EntityFrameworkCore;

namespace account_service.data_contexts;

public class UserDataContext : DbContext{

    public UserDataContext (DbContextOptions<UserDataContext> options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
    }

    public DbSet<UserModel> Users {get; set;}

}