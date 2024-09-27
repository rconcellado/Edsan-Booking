using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EdsanBooking.Utilities
{
    public static class IdCreator
    {
        public static string GenerateNextId<TEntity>(DbSet<TEntity> dbSet, string prefix, Expression<Func<TEntity, string>> idSelector) where TEntity : class
        {
            var lastId = dbSet.OrderByDescending(idSelector).Select(idSelector).FirstOrDefault();
            if (string.IsNullOrEmpty(lastId) || lastId.Length <= prefix.Length)
            {
                return prefix + "0000001"; // Starting ID if there is no last ID or an unexpected format
            }

            // Extract the numeric part from the last ID
            int nextIdNumber = int.Parse(lastId.Substring(prefix.Length)) + 1;

            // Generate the new ID
            return prefix + nextIdNumber.ToString("D7"); // Adjust the D5 to accommodate the expected ID format
        }
    }
}
