using JobViewerWebApp.Models;

namespace JobViewerWebApp.Services
{
    public class JobHistoryServerComparer : IEqualityComparer<SysJobHistory>
    {
        public bool Equals(SysJobHistory x, SysJobHistory y)
        {
            if (x == null || y == null) return false;
            return x.Server == y.Server;
        }

        public int GetHashCode(SysJobHistory obj)
        {
            return obj.Server.GetHashCode();
        }
    }
}
