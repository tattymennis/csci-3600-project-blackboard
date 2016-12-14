using System.Web;
using System.Web.Mvc;

namespace csci_3600_project_the_struggle
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
