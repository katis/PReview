using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;

namespace PReview
{
    public class GithubVM : NotifyExt
    {
        private readonly GithubModel github = new GithubModel();
        private readonly CancellationTokenSource canceller = new CancellationTokenSource();

        public GithubVM()
        {
            JumpList.GetJumpList(Application.Current)?.JumpItems?.Clear();
            Task.Run(() => updatePrStatusesPeriodically(canceller.Token));
        }

        private async Task updatePrStatusesPeriodically(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var statuses = await github.PrStatuses();
                    RunInUI(() => PrStatuses = statuses);
                } catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                await Task.Delay(120000, token);
            }
        }

        private string error;
        public string Error
        {
            get { return error; }
            set { Set(ref error, value); }
        }

        private PrStatuses prStatuses;

        public PrStatuses PrStatuses {
            get { return prStatuses; }
            set {
                Set(ref prStatuses, value);

                Total = PrStatuses.Total;
                var n = PrStatuses.PrsNeedingReview.Count;
                ProgressState =
                    n <= 1 ? TaskbarItemProgressState.Normal :
                    n == 2 ? TaskbarItemProgressState.Paused :
                    TaskbarItemProgressState.Error;
                updateJumpList();
            }
        }

        private TaskbarItemProgressState progressState = TaskbarItemProgressState.Normal;
        public TaskbarItemProgressState ProgressState
        {
            get { return progressState; }
            set { Set(ref progressState, value); }
        }

        private double total;
        public double Total
        {
            get { return total; }
            set { Set(ref total, value); }
        }

        private ImageSource overlayImage;
        public ImageSource OverlayImage
        {
            get { return overlayImage; }
            set { Set(ref overlayImage, value); }
        }

        private void updateJumpList()
        {
            var jumpTasks = PrStatuses.PrsNeedingReview.Select(jumpTask);

            var jumpList = createJumpList();
            foreach (var task in jumpTasks)
            {
                jumpList.JumpItems.Add(task);
            }

            JumpList.SetJumpList(Application.Current, jumpList);
        }

        private JumpList createJumpList()
        {
            var list = new JumpList();
            list.ShowFrequentCategory = false;
            list.ShowRecentCategory = false;
            return list;
        }

        private JumpTask jumpTask(Octokit.PullRequest pr)
        {
            var eviNum = eviNumber(pr);
            return new JumpTask
            {
                Title = pr.Title,
                Arguments = $"{eviNum};{pr.HtmlUrl.AbsoluteUri}",
                Description = eviNum == "" ? $"Open PR page of \"{pr.Title}\"" : $"Open JIRA & PR page for EVI-{eviNum}",
                CustomCategory = "Pull Requests",
                IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
                ApplicationPath = Assembly.GetEntryAssembly().CodeBase
            };
        }

        private string eviNumber(Octokit.PullRequest pr)
        {
            var match = Regex.Match(pr.Title, @"(?i)EVI(:?\W|_)([0-9]+)");
            if (match != null && match.Groups.Count >= 3)
            {
                return match.Groups[2].Value;
            }
            return "";
        }

        public Octokit.PullRequest FindPrByUri(Uri uri)
        {
            return PrStatuses.PrsForReview.Find(pr => pr.HtmlUrl == uri);
        }
    }
}
