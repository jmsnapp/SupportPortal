using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SupportPortalAPI.Filters
{
    /// <summary>
    /// Turns constraint violations raised by SaveChanges into the status code the caller
    /// deserves. Without this every duplicate Name and every bad foreign key surfaces as a
    /// 500, which tells a client nothing and hides real server faults among routine input
    /// mistakes.
    /// <para>
    /// Only violations that are genuinely the caller's fault are translated. Anything else —
    /// including truncation, which in this solution means the EF model disagrees with the
    /// deployed column width rather than that the caller sent something too long — is left
    /// alone so it still reports as a 500 and stays visible.
    /// </para>
    /// </summary>
    public sealed class DbUpdateExceptionFilter : IExceptionFilter
    {
        // SQL Server error numbers.
        private const int UniqueConstraint = 2627;   // Violation of UNIQUE KEY constraint
        private const int DuplicateKeyRow  = 2601;   // Cannot insert duplicate key row in unique index
        private const int ConstraintCheck  = 547;    // FOREIGN KEY or CHECK constraint
        private const int NullNotAllowed   = 515;    // Cannot insert NULL into a non-nullable column

        public void OnException(ExceptionContext context)
        {
            ProblemDetails? problem = context.Exception switch
            {
                DbUpdateConcurrencyException => Problem(
                    StatusCodes.Status409Conflict,
                    "Concurrent modification",
                    "The record was changed by someone else after you loaded it. Re-read it and reapply your change."),

                DbUpdateException dbEx when dbEx.InnerException is SqlException sql => Translate(sql),

                _ => null
            };

            if (problem is null) return;

            context.Result = new ObjectResult(problem) { StatusCode = problem.Status };
            context.ExceptionHandled = true;

        }

        private static ProblemDetails? Translate(SqlException sql) => sql.Number switch
        {
            UniqueConstraint or DuplicateKeyRow => Problem(
                StatusCodes.Status409Conflict,
                "Duplicate value",
                $"A record with that value already exists. Violated constraint: {ConstraintName(sql)}."),

            ConstraintCheck => Problem(
                StatusCodes.Status400BadRequest,
                "Invalid reference",
                $"One of the referenced records does not exist. Violated constraint: {ConstraintName(sql)}."),

            NullNotAllowed => Problem(
                StatusCodes.Status400BadRequest,
                "Missing required value",
                "A required field was not supplied."),

            _ => null
        };

        private static ProblemDetails Problem(int status, string title, string detail) =>
            new ProblemDetails { Status = status, Title = title, Detail = detail };

        /// <summary>
        /// Pulls the constraint name out of the message — SQL Server quotes it, and it is the
        /// one piece of the message worth showing, since it says which rule was broken.
        /// </summary>
        private static string ConstraintName(SqlException sql)
        {
            string message = sql.Message;
            int open = message.IndexOf('"');
            if (open < 0) { open = message.IndexOf('\''); }
            if (open < 0) { return "unknown"; }

            char quote = message[open];
            int close = message.IndexOf(quote, open + 1);
            return close < 0 ? "unknown" : message.Substring(open + 1, close - open - 1);

        }

    }

}
