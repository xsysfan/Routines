using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class GroupDeleteModel : PageModel, IGroupPartialModel
    {
        readonly static GroupMeta meta = Meta.GroupMeta;

        public Group Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Group, int> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, () => $"{nameof(Group)}?id={Entity.GroupId}", "Groups", true);
            return Crud.HandleDeleteAsync(
                e => Entity = e,
                authorize: null,
                meta.DeleteIncludes, 
                meta.KeyConverter,
                meta.FindPredicate
            );
        }

        public Task<IActionResult> OnPostAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, () => $"{nameof(Group)}?id={Entity.GroupId}", "Groups", true);
            return Crud.HandleDeleteConfirmedAsync(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                nameof(Entity), 
                meta.Constructor,
                meta.HiddenFormFields
            );
        }
    }
}