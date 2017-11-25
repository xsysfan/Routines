﻿using System.Linq;
using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler
using Microsoft.Extensions.Configuration;
using DashboardCode.AdminkaV1.AuthenticationDom; // entity
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
{
    public class GroupsController : RoutineController
    {
        const string BindedFields = nameof(Group.GroupId) + ", " + nameof(Group.GroupName) + ", " + nameof(Group.GroupAdName);
        Include<Group> indexIncludes;
        Include<Group> detailsIncludes;
        Include<Group> editIncludes;
        Include<Group> deleteIncludes;

        public GroupsController(IConfigurationRoot  configurationRoot):base(configurationRoot)
        {
            this.indexIncludes = includable =>
                                      includable.IncludeAll(y => y.GroupPrivilegeMap)
                                          .ThenInclude(y => y.Privilege)
                                          .IncludeAll(y => y.UserGroupMap)
                                          .ThenInclude(y => y.User)
                                          .IncludeAll(y => y.GroupRoleMap)
                                          .ThenInclude(y => y.Role);
            this.detailsIncludes = indexIncludes;
            this.editIncludes = includable =>
                                   includable.IncludeAll(y => y.GroupPrivilegeMap)
                                       .ThenInclude(y => y.Privilege)
                                       .IncludeAll(y => y.GroupRoleMap)
                                       .ThenInclude(y => y.Role);
            this.deleteIncludes = indexIncludes;
        }

        public async Task<IActionResult> Index()
        {
            var routine = new MvcRoutine(this, null);
            return await routine.HandleStorageAsync<IActionResult, Group>(
                (repository) =>
                {
                    var groups = repository.List(indexIncludes);
                    return View(groups);
                });
        }

        public async Task<IActionResult> Details(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Group>(repository =>
            {
                return this.MakeActionResultOnRequest(
                    () => id != null,
                    () => repository.Find(e => e.GroupId == id, detailsIncludes)
                );
            });
        }

        public async Task<IActionResult> Create()
        {
            var routine = new MvcRoutine(this, null);
            return await routine.HandleStorageAsync<IActionResult, Group>(repository =>
            {
                var privilegesNavigation = new MvcNavigationFacade<Group, Privilege, GroupPrivilege, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Clone<Privilege>().List()
                    );
                var rolesNavigation = new MvcNavigationFacade<Group, Role, GroupRole, int>(
                    this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                    repository.Clone<Role>().List()
                );
                privilegesNavigation.Reset();
                rolesNavigation.Reset();
                return View();
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(BindedFields)] Group entity)
        {
            var routine = new MvcRoutine(this, new { group = entity });
            return await routine.HandleStorageAsync<IActionResult, Group>((repository, storage, state) =>
           {
               if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                   return Unauthorized();

               var privilegesNavigation = new MvcNavigationFacade<Group, Privilege, GroupPrivilege, string>(
                                   this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                                   repository.Clone<Privilege>().List()
                               );
               privilegesNavigation.Parse(
                                   e => new GroupPrivilege() { GroupId = entity.GroupId, PrivilegeId = e.PrivilegeId },
                                   s => s);

               var rolesNavigation = new MvcNavigationFacade<Group, Role, GroupRole, int>(
                                   this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                                   repository.Clone<Role>().List()
                               );
               rolesNavigation.Parse(
                                   e => new GroupRole() { GroupId = entity.GroupId, RoleId = e.RoleId },
                                   s => int.Parse(s));

               return this.MakeActionResultOnSave(
                   ModelState.IsValid,
                   () => storage.Handle(
                       batch =>
                       {
                           batch.Add(entity);
                           batch.ModifyRelated(entity, e => e.GroupPrivilegeMap, privilegesNavigation.Selected, (e1, e2) => e1.GroupId == e2.GroupId);
                           batch.ModifyRelated(entity, e => e.GroupRoleMap, rolesNavigation.Selected, (e1, e2) => e1.GroupId == e2.GroupId);
                       }),
                   () =>
                   {
                       privilegesNavigation.Reset();
                       rolesNavigation.Reset();
                       return View(entity);
                   }
               );
           });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Group>(repository =>
            {
                var privilegesNavigation = new MvcNavigationFacade<Group, Privilege, GroupPrivilege, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Clone<Privilege>().List()
                    );

                var rolesNavigation = new MvcNavigationFacade<Group, Role, GroupRole, int>(
                    this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                    repository.Clone<Role>().List()
                );

                return this.MakeActionResultOnRequest(
                    () => id != null,
                    () => repository.Find(e => e.GroupId == id, editIncludes),
                    (entity) =>
                    {
                        privilegesNavigation.Reset(entity.GroupPrivilegeMap.Select(e => e.PrivilegeId));
                        rolesNavigation.Reset(entity.GroupRoleMap.Select(e => e.RoleId));
                    }
                );
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(BindedFields)] Group group)
        {
            var routine = new MvcRoutine(this, new { group = group });
            return await routine.HandleStorageAsync<IActionResult, Group>(
                (repository, storage, state) =>
            {
                if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                    return Unauthorized();

                var privilegesNavigation = new MvcNavigationFacade<Group, Privilege, GroupPrivilege, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Clone<Privilege>().List()
                );

                privilegesNavigation.Parse(
                    e => new GroupPrivilege() { GroupId = group.GroupId, PrivilegeId = e.PrivilegeId },
                    s => s);

                var rolesNavigation = new MvcNavigationFacade<Group, Role, GroupRole, int>(
                    this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                    repository.Clone<Role>().List()
                );

                rolesNavigation.Parse(
                     e => new GroupRole() { GroupId = group.GroupId, RoleId = e.RoleId },
                     s => int.Parse(s)
                );

                return this.MakeActionResultOnSave(
                    ModelState.IsValid,
                    () => storage.Handle(
                        batch =>
                        {
                            batch.Modify(group);
                            batch.ModifyRelated(group,
                                e => e.GroupRoleMap,
                                rolesNavigation.Selected,
                                (e1, e2) => e1.RoleId == e2.RoleId
                            );
                            batch.ModifyRelated(group,
                                e => e.GroupPrivilegeMap,
                                privilegesNavigation.Selected,
                                (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                            );
                        }),
                    () =>
                    {
                        privilegesNavigation.Reset();
                        rolesNavigation.Reset();
                        return View(group);
                    }
                );
            });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Group>(repository =>
            {
                return this.MakeActionResultOnRequest(
                        () => id != null,
                        () => repository.Find(e => e.GroupId == id, deleteIncludes)
                    );
            });
        }
        [HttpPost, ActionName(nameof(GroupsController.Delete)), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Group>((repository, storage, state) =>
            {
                if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                    return Unauthorized();
                var entity = repository.Find(e => e.GroupId == id);
                return this.MakeActionResultOnSave(
                        true,
                        () => storage.Handle(batch => batch.Remove(entity)),
                        () => View(nameof(GroupsController.Delete), entity)
                    );
            });
        }
    }
}