using Microsoft.EntityFrameworkCore;

namespace MovejobtoWms.Models
{
    public class MoveJobContext : DbContext
    {
        public MoveJobContext(DbContextOptions<MoveJobContext> options) : base(options)
        {
        }

        public DbSet<FileDetail> FileDetails { get; set; }

        public DbSet<Recorddetail> Recorddetails { get; set; }

        public DbSet<Recorddetail37> Recorddetails37 { get; set; }

        public DbSet<ActivityDetail> ActivityDetails { get; set; }

        public DbSet<BookDetail> BookDetails { get; set; }

        public DbSet<trnFilePropertyDetail> trnFilePropertyDetails { get; set; }
        public DbSet<StageDetail> StageDetails { get; set; }

        public DbSet<SP_BMJ_JobDetails> SP_BMJ_JobDetails { get; set; }
        public DbSet<BMJ_MstCustomerStageDetails> BMJ_MstCustomerStageDetails { get; set; }
        public DbSet<SP_BMJ_AssignedJobDetails> SP_BMJ_AssignedJobDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recorddetail>().HasKey(k => new { k.fileID, k.activityid });
            modelBuilder.Entity<Recorddetail37>().HasKey(k => new { k.fileID, k.activityid });
            modelBuilder
             .Entity<SP_BMJ_JobDetails>(eb =>
              {
                  eb.HasNoKey();
              }).Entity<BMJ_MstCustomerStageDetails>(eb =>
              {
                  eb.HasNoKey();
              }).Entity<SP_BMJ_AssignedJobDetails>(eb =>
              {
                  eb.HasNoKey();
              });
        }


    }
}