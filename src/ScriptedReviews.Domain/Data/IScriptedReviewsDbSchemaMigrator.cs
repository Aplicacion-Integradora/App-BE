using System.Threading.Tasks;

namespace ScriptedReviews.Data;

public interface IScriptedReviewsDbSchemaMigrator
{
    Task MigrateAsync();
}
