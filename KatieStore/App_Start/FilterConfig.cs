using KatieStore.App_Start;
using System.Web;
using System.Web.Mvc;

namespace KatieStore
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new OutputCacheAttribute());    //Bad idea!
            filters.Add(new CartCalculatorAttribute()); //Better idea - cart calculator will now be run on every page!
        }
    }
}
