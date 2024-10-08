using JobViewerWebApp.Models;

namespace JobViewerWebApp.Services
{
    public class JobsServerComparer : IEqualityComparer<SysJob>
    {
        public bool Equals(SysJob x, SysJob y)
        {
            if (x == null || y == null) return false;
            return x.Server == y.Server;
        }

        public int GetHashCode(SysJob obj)
        {
            return obj.Server.GetHashCode();
        }
    }
}
