using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyToDo.Api.Context
{
    //DbContext是实体类和数据库之间的桥梁
    //将数据库DML语句结果映射到这里的实体类
    public class MyToDoContext : DbContext
    {
        // 引入一个DbContextOptions用于配置DbContent。引入时需要指明一下这个Option是作用于哪一种DbContxtx，在这里就是我们自定义的MyToDoContext
        public MyToDoContext(DbContextOptions<MyToDoContext> options) : base(options)
        {

            
        }

        public DbSet<ToDo> ToDo { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Memo> Memo { get; set; }
    }
}
