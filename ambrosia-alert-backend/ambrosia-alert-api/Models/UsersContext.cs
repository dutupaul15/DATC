using Microsoft.EntityFrameworkCore;

namespace ambrosia_alert_api.Models;

public class UsersContext(DbContextOptions<UsersContext> options) : DbContext(options)
{
    public DbSet<UserItem> UsersItems {get; set;}  = null!;
}