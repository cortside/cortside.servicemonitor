using System.Threading.Tasks;
using Cortside.WebApiStarter.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cortside.WebApiStarter.Data {
    public interface IDatabaseContext {
        DbSet<Domain.WebApiStarter> WebApiStarter { get; set; }
        DbSet<Subject> Subjects { get; set; }
        Task<int> SaveChangesAsync();
    }
}
