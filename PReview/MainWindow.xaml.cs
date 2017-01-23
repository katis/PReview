using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PReview
{

    public partial class MainWindow : Window
    {
        private readonly GithubVM github;

        public MainWindow()
        {
            InitializeComponent();

            github = new GithubVM();
            this.DataContext = github;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var pr = github.FindPrByUri(e.Uri);

            var match = Regex.Match(pr.Title, @"(?i)EVI(:?\W|_)([0-9]+)");
            if (match != null && match.Groups.Count >= 3)
            {
                var eviUri = $"https://atlas.elisa.fi/jira/browse/EVI-{match.Groups[2]}";
                Process.Start(new ProcessStartInfo(eviUri));
            }

            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
