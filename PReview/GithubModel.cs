using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;

namespace PReview
{
    public class GithubModel
    {
        private static readonly string FOR_REVIEW = "for review";

        private readonly GitHubClient github;
        private readonly Config config;

        public GithubModel()
        {
            config = Config.fromFile(Config.CONFIG_PATH);

            github = new GitHubClient(new ProductHeaderValue("PReview"), new Uri(config.GithubUrl))
            {
                Credentials = new Credentials(config.ApiToken)
            };
        }

        public Task<IReadOnlyList<PullRequest>> PullRequests
        {
            get { return github.PullRequest.GetAllForRepository(config.Organization, config.Repository); }
        }

        public async Task<PrStatuses> PrStatuses()
        {
            var pullRequests = await PullRequests;
            var prIssues = (await pullRequests.Select(pr => github.Issue.Get(config.Organization, config.Repository, pr.Number)).WhenAll()).ToDictionary(i => i.Number);

            var reviewablePrs = pullRequests.Where(pr => prIssues[pr.Number].Labels.Any(label => label.Name == FOR_REVIEW));

            var prsNeedingReview = (await reviewablePrs
                .Where(pr => pr.State == ItemState.Open)
                .Select(async pr =>
                {
                    var lastModification = (await github.Repository.Commit.Get(config.Organization, config.Repository, pr.Head.Ref)).Commit.Author.Date;
                    var issue = prIssues[pr.Number];
                    var comments = await github.Issue.Comment.GetAllForIssue(config.Organization, config.Repository, issue.Number);

                    var thumbComments = comments
                        .Where(c => c.CreatedAt > lastModification)
                        .Where(c => c.Body.Contains(":+1:"))
                        .ToList();

                    var okCount = thumbComments.Count;

                    var isCommentedByYou = thumbComments.Any(c => config.UserName == c.User.Login);

                    return !isCommentedByYou && okCount < 2 ? pr : null;
                })
                .WhenAll())
                .Where(pr => pr != null);

            return new PrStatuses(reviewablePrs.ToList(), prsNeedingReview.ToList());
        }
    }
}
