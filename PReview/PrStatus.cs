using Octokit;
using System.Collections.Generic;

namespace PReview
{
    public class PrStatuses
    {
        public List<PullRequest> PrsForReview { get; }
        public List<PullRequest> PrsNeedingReview { get; }

        public PrStatuses(List<PullRequest> prsForReview, List<PullRequest> prsNeedingReview)
        {
            PrsForReview = prsForReview;
            PrsNeedingReview = prsNeedingReview;
        }

        public double Total
        {
            get { return ((double)PrsNeedingReview.Count) / ((double)PrsForReview.Count); }
        }
    }
}
