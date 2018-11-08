using Fonour.Domain.Entities;
using Fonour.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fonour.EntityFrameworkCore.Repositories
{
    public class MenuRepository : FonourRepositoryBase<Menu>, IMenuRepository
    {
        public MenuRepository(FonourDBContext dbcontext) : base(dbcontext)
        {

        }
    }
}
