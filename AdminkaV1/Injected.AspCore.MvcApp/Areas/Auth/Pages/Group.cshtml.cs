using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupModel : PageModel
    {
        readonly static GroupMeta meta = Meta.GroupMeta;

        public Group Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Group, int> Crud;

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, () => $"{nameof(Group)}?id={Entity.GroupId}", "Groups", true);
            return Crud.HandleDetailsAsync(
                e => Entity = e,
                userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                meta.DetailsIncludes,
                meta.KeyConverter,
                meta.FindPredicate
            );
        }
    }
}