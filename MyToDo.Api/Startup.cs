using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyToDo.Api.Context;
using MyToDo.Api.Context.Repository;
using MyToDo.Api.Extensions;
using MyToDo.Api.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyToDo.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 要想在 .NET Core 中使用，需要将 MyToDoContext 注入到 ServiceCollection 容器中
            // 这里的option实际上是DbContextOptionsBuilder类型
            services.AddDbContext<MyToDoContext>(option =>
            {
                // 获取到appsettings.json中的TodoConnection字段字符串：Data Source=to.db
                var connectionString = Configuration.GetConnectionString("ToDoConnection");
                // 设置MyToDoContext以哪一种数据库为存储。由于这里是UseSqlite，因此就是Sqlite，connectionString是连接数据库的字符串
                option.UseSqlite(connectionString);
            }).AddUnitOfWork<MyToDoContext>()  // 这里将MyToDoContext的使用，统一在UnitOfWork中。它的好处是，如果没有UnitOfWork
                                               // 当某个任务涉及到在当某一个实体Entity进行DML操作时，还对另一个实体也进行DML操作，
                                               // 这里前后两次的DML会先后使用到两个不同的MyToDoContext实例，如果其中某次DML失败，
                                               // 此时很难保持数据一致性。
                                               // 将MyToDoContext的使用，统一在UnitOfWork中的好处之一就是能够避免上面的问题。
                                               // 在MyToDoContext中，有实体集DbSet，这是在MyToDoContext层面的。对于DbSet的理解，例如
                                               // public DbSet<ToDo> ToDo { get; set; }，它意味着通过MyToDoContext能够将数据库中ToDo表
                                               // 中的数据映射到程序中的ToDo对象，考虑到可能会返回多个ToDo对象（例如查询DML），因此类型是
                                               // DbSet。那么对于某个实体，它的CRUD在哪里定义和明确动作呢。这里就引入了Repository的类，通过
                                               // 为每一个数据表实体建立一个Repository，Repository通过持有MyToDoContext来进行操作，例如
                                               // myToDoContext.ToDo.AddAsync(new ToDo(){.....})，完成CRUD的定义和明确
                                               // 现在有了UnitOfWork，我们需要向UnitOfWork告知有哪些Repository，因此就有下面的语句
                                               // 这样我们就能通过UnitOfWork根据需要获得相应的Repository，例如
                                               // var toDoService = unitOfWork.getRepository<ToDo>();
                                               // toDoService.GetAll();
                                               // 此外，AddUnitOfWork<MyToDoContext>() 还使得容器托管了一个UnitOfWork实例，使得可以
                                               // 通过依赖注入的方式获得实例，例如ToDoService的构造器的用法
            .AddCustomRepository<ToDo, ToDoRepository>()
            .AddCustomRepository<Memo, MemoRepository>()
            .AddCustomRepository<User, UserRepository>();

            // 在这里我们引入automapper。它的作用是实现两个对象之间的转换。
            // 在项目中，从数据库中返回的数据，对应的是ToDo，Memo之类。这类对象他们有的字段只能是数据库中的列，功能太局限
            // 因此在项目中，我们额外再定义了一些Dto类，这些Dto和数据Entity很像，但是有额外的功能。使用这些Dto能够更好和WPF相互配合。
            // 例如Dto中在字段上有 OnPropertyChanged()的通知更新功能。这样，在WPF中使用Dto，而通过 UnitOfWork返回却是Entity。
            // 为了使得Dto能存回数据库，那么需要将Dto转换成Entity，同时，从数据库返回的Entity需要转成Dto，方便和WPF配合。

            // 将我们定义的Map映射注册到MapperConfiguration
            var automapperConfog = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProFile());
            });

            // 将MapperConfiguration注册到容器中，注意是单例
            services.AddSingleton(automapperConfog.CreateMapper());

            // AddTransient： 每次service请求都是获得不同的实例
            // 向容器里注入ToDoService使得ToDoController可以装配ToDoService实例
            services.AddTransient<IToDoService, ToDoService>();
            services.AddTransient<IMemoService, MemoService>();
            services.AddTransient<ILoginService, LoginService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyToDo.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyToDo.Api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
