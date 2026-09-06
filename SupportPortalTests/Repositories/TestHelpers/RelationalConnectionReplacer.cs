using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace SupportPortalTests.Repositories.TestHelpers
{
    public static class RelationalConnectionReplacer
    {
        public static void Replace(DbContext ctx, DbConnection newConn)
        {
            var infra = (IInfrastructure<IServiceProvider>)ctx;
            var sp = infra.Instance;
            var relational = sp.GetService(typeof(IRelationalConnection)) as IRelationalConnection;
            if (relational == null) return;

            var rcType = relational.GetType();

            // Try to find any private field of type DbConnection
            var dbConnField = rcType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                    .FirstOrDefault(f => typeof(DbConnection).IsAssignableFrom(f.FieldType));
            if (dbConnField != null)
            {
                dbConnField.SetValue(relational, newConn);
                return;

            }

            // Try to find any private or internal property of type DbConnection that can be written
            var dbConnProp = rcType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                   .FirstOrDefault(p => p.PropertyType == typeof(DbConnection) && p.CanWrite);
            if (dbConnProp != null)
            {
                dbConnProp.SetValue(relational, newConn);
                return;

            }

            // As last resort, try to set any field/property whose name contains 'connection' and is of compatible type
            dbConnField = rcType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                .FirstOrDefault(f => f.Name.ToLower().Contains("connection") && typeof(DbConnection).IsAssignableFrom(f.FieldType));
            if (dbConnField != null)
            {
                dbConnField.SetValue(relational, newConn);
                return;

            }

            dbConnProp = rcType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                .FirstOrDefault(p => p.Name.ToLower().Contains("connection") && p.CanWrite && p.PropertyType == typeof(DbConnection));
            if (dbConnProp != null)
            {
                dbConnProp.SetValue(relational, newConn);
                return;

            }

        }

    }

}
