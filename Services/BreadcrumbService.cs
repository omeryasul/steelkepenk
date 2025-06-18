using Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services
{
    public class BreadcrumbService : IBreadcrumbService
    {
        private readonly List<(string Title, string? Url)> _breadcrumbs = new();

        public void AddBreadcrumb(string title, string? url = null)
        {
            _breadcrumbs.Add((title, url));
        }

        public List<(string Title, string? Url)> GetBreadcrumbs()
        {
            return _breadcrumbs.ToList();
        }

        public void Clear()
        {
            _breadcrumbs.Clear();
        }
    }
}
