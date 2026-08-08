using System.Web.Optimization;

namespace BusTravelManagement.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // All scripts and styles are loaded from CDN in _Layout.cshtml.
            // Bundle-based references have been migrated to CDN.
            BundleTable.EnableOptimizations = false;
        }
    }
}
