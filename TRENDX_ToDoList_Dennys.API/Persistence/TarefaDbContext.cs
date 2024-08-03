using Microsoft.EntityFrameworkCore;
using TRENDX_ToDoList_Dennys.API.Entities;

namespace TRENDX_ToDoList_Dennys.API.Persistence
{
    public class TarefaDbContext : DbContext
    {
        public TarefaDbContext(DbContextOptions<TarefaDbContext> options) : base(options)
        {
        }

        public DbSet<Tarefa> Tarefas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tarefa>(e =>
            {
                e.HasKey(t => t.Id);
                e.Property(t => t.Title)
                .HasMaxLength(50)
                .HasColumnType("varchar(50)");
                e.Property(t => t.Description)
                .HasMaxLength(250)
                .HasColumnType("varchar(250)");
            });
        }
    }
}