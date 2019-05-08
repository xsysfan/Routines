using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class PrivilegesModel : PageModel
    {
        readonly static PrivilegeMeta meta = Meta.PrivilegeMeta;

        public IEnumerable<Privilege> List { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Privilege, string> Crud;

        public Task<IActionResult> OnGet()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Privilege, string>(this, null, defaultUrl: null, true);
            return Crud.HandleIndexAsync(
                l => List = l,
                authorize: null,
                meta.IndexIncludes
            );
        }
    }
}