using Microsoft.EntityFrameworkCore;

namespace JobsViewer_API.Models
{
    public class JobsViewerContext : DbContext
    {
        public JobsViewerContext(DbContextOptions<JobsViewerContext> options)
        : base(options)
        {
        }

        public DbSet<SysJob> SysJobs { get; set; } = null!;
        public DbSet<SysJobHistory> SysJobHistories { get; set; } = null!;
    }
}
