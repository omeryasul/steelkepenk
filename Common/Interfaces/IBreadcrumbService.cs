using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IBreadcrumbService
    {
        void AddBreadcrumb(string title, string? url = null);
        List<(string Title, string? Url)> GetBreadcrumbs();
        void Clear();
    }

}
