using Microsoft.AspNetCore.Builder;
using ScriptedReviews;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
await builder.RunAbpModuleAsync<ScriptedReviewsWebTestModule>();

public partial class Program
{
}
