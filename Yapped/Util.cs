using System.Threading.Tasks;
using System.Windows.Forms;
using Semver;

namespace Yapped
{
    internal static class Util
    {
        public static readonly string UpdateUrl = "https://www.nexusmods.com/sekiro/mods/121?tab=files";

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static async Task<bool> CheckForUpdatesAsync()
        {
            Octokit.GitHubClient gitHubClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Yapped"));
            try
            {
                Octokit.Release release = await gitHubClient.Repository.Release.GetLatest("JKAnderson", "Yapped");
                if (SemVersion.Parse(release.TagName) > Application.ProductVersion)
                {
                    return true;
                }
            }
            // Oh well.
            catch { }
            return false;
        }
    }
}
