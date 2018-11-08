using AutoMapper;
using Fonour.Application.MenuApp.Dtos;
using Fonour.Domain.Entities;
using Fonour.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fonour.Application.MenuApp
{
    public class MenuAppService : IMenuAppService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        public MenuAppService(IMenuRepository menuRepository, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public List<MenuDto> GetAllList()
        {
            var menus = _menuRepository.GetAllList().OrderBy(it => it.SerialNumber);
            //使用AutoMapper进行实体转换
            return Mapper.Map<List<MenuDto>>(menus);
        }

        public List<MenuDto> GetMenusByParent(Guid parentId, int startPage, int pageSize, out int rowCount)
        {
            var menus = _menuRepository.LoadPageList(startPage, pageSize, out rowCount, it => it.ParentId == parentId, it => it.SerialNumber);
            return Mapper.Map<List<MenuDto>>(menus);
        }

        public bool InsertOrUpdate(MenuDto dto)
        {
            var menu = _menuRepository.InsertOrUpdate(Mapper.Map<Menu>(dto));
            return menu == null ? false : true;
        }

        public void DeleteBatch(List<Guid> ids)
        {
            _menuRepository.Delete(it => ids.Contains(it.Id));
        }

        public void Delete(Guid id)
        {
            _menuRepository.Delete(id);
        }

        public MenuDto Get(Guid id)
        {
            return Mapper.Map<MenuDto>(_menuRepository.Get(id));
        }
        /// <summary>
        /// 根据用户获取功能菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public List<MenuDto> GetMenusByUser(Guid userId)
        {
            List<MenuDto> result = new List<MenuDto>();
            MenuDto m = new MenuDto();
            IOrderedEnumerable<Menu> allMenus = null;
            var userData = _userRepository.Get(userId);
            if (userId == Guid.Empty) //超级管理员
            {
                allMenus = _menuRepository.GetAllList(it => it.Type == 0).OrderBy(it => it.SerialNumber);
                return Data(allMenus);
            }
            var user = _userRepository.GetWithRoles(userId);
            if (user == null)
                return result;
            var userRoles = user.UserRoles;
            List<Guid> menuIds = new List<Guid>();
            foreach (var role in userRoles)
            {
                menuIds = menuIds.Union(_roleRepository.GetAllMenuListByRole(role.RoleId)).ToList();
            }
            allMenus = _menuRepository.GetAllList(it => it.Type == 0).OrderBy(it => it.Id);

            allMenus = _menuRepository.GetAllList2(it => it.Type == 0, userId.ToString()).OrderBy(it => it.Id);

            allMenus = allMenus.Where(it => menuIds.Contains(it.Id)).OrderBy(it => it.SerialNumber);
            return Data(allMenus);
        }

        public List<MenuDto> Data(IOrderedEnumerable<Menu> allMenus)
        {
            var ListHBData = new List<MenuDto>();
            var ParentList = new List<Menu>(); //定义一个ParentList列表
            var ParentLists= new List<Menu>();
            var ChildList = new List<Menu>(); //定义一个ChildList列表
            #region 循环分出父子级放到不同的列表中
            foreach (var item in allMenus)
            {
                //如果ParentId 不等于00000000-0000-0000-0000-000000000000
                if (item.ParentId.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    ChildList.Add(item);
                }
                else
                {
                    ParentList.Add(item);
                }
            }
            #endregion
            if (ParentList.Count<=0)
            {
                var parendId = "";
                bool p = false;
                foreach (var item in allMenus)
                {
                    var ff = _menuRepository.Get((Guid)item.ParentId);
                    if (p == false)
                    {
                        parendId = item.ParentId.ToString();
                        p = true;
                        ParentLists.Add(ff);
                    }
                    if (parendId == item.ParentId.ToString()) { }
                    else {
                        ParentLists.Add(ff);
                    }
                }
                foreach (var item in ParentLists)
                {
                    if (item.ParentId.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        MenuDto m = new MenuDto();
                        List<MenuDto> mm = new List<MenuDto>();
                        m.Id = item.Id;
                        m.Name = item.Name;
                        m.ParentId = item.ParentId;
                        m.Remarks = item.Remarks;
                        m.SerialNumber = item.SerialNumber;
                        m.Type = item.Type;
                        m.Url = item.Url;
                        m.Code = item.Code;
                        m.Icon = item.Icon;
                        foreach (var item1 in ChildList)
                        {
                            if (item.Id == item1.ParentId)
                            {
                                mm.Add(Mapper.Map<MenuDto>(item1));
                                m.menuData = mm;
                            }
                        }
                        ListHBData.Add(m);
                    }
                }
            }
            else
            {
                foreach (var item in ParentList)
                {
                    if (item.ParentId.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        MenuDto m = new MenuDto();
                        List<MenuDto> mm = new List<MenuDto>();
                        m.Id = item.Id;
                        m.Name = item.Name;
                        m.ParentId = item.ParentId;
                        m.Remarks = item.Remarks;
                        m.SerialNumber = item.SerialNumber;
                        m.Type = item.Type;
                        m.Url = item.Url;
                        m.Code = item.Code;
                        m.Icon = item.Icon;
                        foreach (var item1 in ChildList)
                        {
                            if (item.Id == item1.ParentId)
                            {
                                mm.Add(Mapper.Map<MenuDto>(item1));
                                m.menuData = mm;
                            }
                        }
                        ListHBData.Add(m);
                    }
                }
            }
            return Mapper.Map<List<MenuDto>>(ListHBData);
        }
    }
}
