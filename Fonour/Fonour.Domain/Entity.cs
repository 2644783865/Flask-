using System;
using System.Collections.Generic;
using System.Text;

namespace Fonour.Domain
{
    /// <summary>
    /// 泛型实体基类
    /// </summary>

    public abstract class Entity<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }
    }

    /// <summary>
    /// 定义默认主键类型为Guid的尸体基类
    /// </summary>
    public abstract class Entity : Entity<Guid>
    {

    }
}
