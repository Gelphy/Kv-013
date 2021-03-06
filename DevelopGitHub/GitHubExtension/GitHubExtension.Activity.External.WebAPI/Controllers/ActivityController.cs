using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

using GitHubExtension.Activity.External.WebAPI.Models;
using GitHubExtension.Activity.External.WebAPI.Queries;
using GitHubExtension.Infrastructure.Constants;
using GitHubExtension.Infrastructure.Extensions.Identity;

using Microsoft.AspNet.Identity;

namespace GitHubExtension.Activity.External.WebAPI.Controllers
{
    public class ActivityController : ApiController
    {
        public const string YouHaveNoRepositorySelected = "You have no repository selected";

        private readonly IGitHubEventsQuery _eventsQuery;

        public ActivityController(IGitHubEventsQuery eventsQuery)
        {
            _eventsQuery = eventsQuery;
        }

        [Route(ExternalActivityRoutes.GetGitHubActivityRoute)]
        public async Task<IHttpActionResult> GetGitHubActivity([FromUri] int page)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            string fullRepositoryName = claimsIdentity.FindFirstValue(ClaimConstants.CurrentProjectName);
            if (fullRepositoryName == null)
            {
                return BadRequest(YouHaveNoRepositorySelected);
            }

            var token = User.GetExternalAccessToken();

            EventsPaginationModel model =
                await _eventsQuery.GetGitHubEventsAsync(fullRepositoryName, token, page);

            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }
    }
}