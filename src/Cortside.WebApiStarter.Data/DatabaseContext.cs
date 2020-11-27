using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cortside.WebApiStarter.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Cortside.WebApiStarter.Data {

    public partial class DatabaseContext : DbContext, IDatabaseContext {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IHttpContextAccessor httpContextAccessor) : base(options) {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Domain.WebApiStarter> WebApiStarter { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            SetAuditableEntityValues();
            return (await base.SaveChangesAsync(true, cancellationToken));
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken)) {
            SetAuditableEntityValues();
            return (await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
        }

        public override int SaveChanges() {
            SetAuditableEntityValues();

            return base.SaveChanges();
        }

        private void SetAuditableEntityValues() {

            // check for subject in subjects set and either create or get to attach to AudibleEntity
            var updatingSubject = GetCurrentSubject();
            ChangeTracker.DetectChanges();
            var modified = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added);
            var added = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);

            foreach (var item in modified) {
                if (item.Entity is AuditableEntity entity) {
                    ((AuditableEntity)(item.Entity)).LastModifiedSubject = updatingSubject;
                    ((AuditableEntity)(item.Entity)).LastModifiedDate = DateTime.Now.ToUniversalTime();
                }
            }

            foreach (var item in added) {
                if (item.Entity is AuditableEntity entity) {
                    ((AuditableEntity)(item.Entity)).CreatedSubject = updatingSubject;
                    ((AuditableEntity)(item.Entity)).CreatedDate = DateTime.Now.ToUniversalTime();
                }
            }
        }

        /// <summary>
        /// Gets or creates the current subject record.
        /// </summary>
        /// <returns></returns>
        private Subject GetCurrentSubject() {

            var currentUser = GetCurrentUser();
            Guid subjectId = currentUser != null ? Guid.Parse(currentUser) : Guid.Empty;

            var subject = Subjects.Local.FirstOrDefault(s => s.SubjectId == subjectId);
            if (subject == null) {
                subject = Subjects.FirstOrDefault(s => s.SubjectId == subjectId);
            }
            if (subject == null) {
                subject = new Subject {
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    SubjectId = subjectId
                };
                if (currentUser != null) {
                    if (_httpContextAccessor != null) {
                        var httpContext = _httpContextAccessor.HttpContext;
                        if (httpContext != null) {
                            subject.GivenName = httpContext.User.FindFirst("given_name")?.Value;
                            subject.Name = httpContext.User.FindFirst("name")?.Value;
                            subject.FamilyName = httpContext.User.FindFirst("family_name")?.Value;
                            subject.UserPrincipalName = httpContext.User.FindFirst("upn")?.Value ?? httpContext.User.FindFirst("client_id")?.Value;
                        }
                    }
                }
                Subjects.Add(subject);
            }
            return subject;

        }

        private string GetCurrentUser() {
            // IHttpContextAccessor will have be be setup in IoC and injected into context constructor
            if (_httpContextAccessor != null) {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null) {
                    // If it returns null, even when the user was authenticated, you may try to get the value of a specific claim 
                    var authenticatedUserId = httpContext.User.FindFirst("sub")?.Value;
                    if (authenticatedUserId != null) {
                        return authenticatedUserId;
                    }
                }
            }
            //If not authenticated return null.
            return null;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Domain.WebApiStarter>()
                .HasOne(p => p.CreatedSubject);
            modelBuilder.Entity<Domain.WebApiStarter>()
                .HasOne(p => p.LastModifiedSubject);
            modelBuilder.HasDefaultSchema("dbo");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public Task<int> SaveChangesAsync() {
            return base.SaveChangesAsync(default(System.Threading.CancellationToken));
        }
    }
}
