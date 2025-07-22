using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.PresentationLayer.Middleware
{
    public class TransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            // Sadece veri değiştiren istekler için (GET işlemleri hariç)
            if (context.Request.Method is "POST" or "PUT" or "DELETE" or "PATCH")
            {
                // Transaction başlatılır
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                try
                {
                    // Controller → Service → SaveChangesAsync çağrısı yapılır
                    await _next(context);

                    // Transaction commit edilir (SaveChanges zaten serviste çağrıldığı için tekrar etmiyoruz)
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                // GET veya HEAD gibi okuma işlemleri için direkt devam
                await _next(context);
            }
        }
    }
}