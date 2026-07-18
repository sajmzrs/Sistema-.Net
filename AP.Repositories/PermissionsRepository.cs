using AP.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AP.Repositories
{
    public interface IPermissionsRepository
    {
        bool HasPermission(string userEmail, string actionName);
    }

    public class PermissionsRepository : IPermissionsRepository
    {
        private readonly ProductDBEntities _context;

        public PermissionsRepository()
        {
            _context = new ProductDBEntities();
        }

        public bool HasPermission(string userEmail, string actionName)
        {
            const string query = @"
                SELECT CAST(CASE WHEN EXISTS
                (
                    SELECT 1
                    FROM dbo.Users AS U
                    INNER JOIN dbo.UserRoles AS UR ON UR.UserID = U.UserID
                    INNER JOIN dbo.Permissions AS P ON P.UserRole = UR.ID
                    INNER JOIN dbo.UserActions AS UA ON UA.ID = P.UserAction
                    WHERE U.Email = @UserEmail
                      AND UA.Name = @ActionName
                ) THEN 1 ELSE 0 END AS bit)";

            SqlParameter emailParameter = new SqlParameter("@UserEmail", userEmail);
            SqlParameter actionParameter = new SqlParameter("@ActionName", actionName);

            return _context.Database
                .SqlQuery<bool>(query, emailParameter, actionParameter)
                .Single();
        }
    }
}
