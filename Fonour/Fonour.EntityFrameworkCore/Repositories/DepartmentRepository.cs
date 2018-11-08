using Fonour.Domain.Entities;
using Fonour.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fonour.EntityFrameworkCore.Repositories
{
    public class DepartmentRepository : FonourRepositoryBase<Department>,IDepartmentRepository
    {
        public DepartmentRepository(FonourDBContext dbcontext) : base(dbcontext)
        {


        }
    }
}
