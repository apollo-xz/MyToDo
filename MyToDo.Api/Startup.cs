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
            // Ҫ���� .NET Core ��ʹ�ã���Ҫ�� MyToDoContext ע�뵽 ServiceCollection ������
            // �����optionʵ������DbContextOptionsBuilder����
            services.AddDbContext<MyToDoContext>(option =>
            {
                // ��ȡ��appsettings.json�е�TodoConnection�ֶ��ַ�����Data Source=to.db
                var connectionString = Configuration.GetConnectionString("ToDoConnection");
                // ����MyToDoContext����һ�����ݿ�Ϊ�洢������������UseSqlite����˾���Sqlite��connectionString���������ݿ���ַ���
                option.UseSqlite(connectionString);
            }).AddUnitOfWork<MyToDoContext>()  // ���ｫMyToDoContext��ʹ�ã�ͳһ��UnitOfWork�С����ĺô��ǣ����û��UnitOfWork
                                               // ��ĳ�������漰���ڵ�ĳһ��ʵ��Entity����DML����ʱ��������һ��ʵ��Ҳ����DML������
                                               // ����ǰ�����ε�DML���Ⱥ�ʹ�õ�������ͬ��MyToDoContextʵ�����������ĳ��DMLʧ�ܣ�
                                               // ��ʱ���ѱ�������һ���ԡ�
                                               // ��MyToDoContext��ʹ�ã�ͳһ��UnitOfWork�еĺô�֮һ�����ܹ�������������⡣
                                               // ��MyToDoContext�У���ʵ�弯DbSet��������MyToDoContext����ġ�����DbSet����⣬����
                                               // public DbSet<ToDo> ToDo { get; set; }������ζ��ͨ��MyToDoContext�ܹ������ݿ���ToDo��
                                               // �е�����ӳ�䵽�����е�ToDo���󣬿��ǵ����ܻ᷵�ض��ToDo���������ѯDML�������������
                                               // DbSet����ô����ĳ��ʵ�壬����CRUD�����ﶨ�����ȷ�����ء������������Repository���࣬ͨ��
                                               // Ϊÿһ�����ݱ�ʵ�彨��һ��Repository��Repositoryͨ������MyToDoContext�����в���������
                                               // myToDoContext.ToDo.AddAsync(new ToDo(){.....})�����CRUD�Ķ������ȷ
                                               // ��������UnitOfWork��������Ҫ��UnitOfWork��֪����ЩRepository����˾�����������
                                               // �������Ǿ���ͨ��UnitOfWork������Ҫ�����Ӧ��Repository������
                                               // var toDoService = unitOfWork.getRepository<ToDo>();
                                               // toDoService.GetAll();
                                               // ���⣬AddUnitOfWork<MyToDoContext>() ��ʹ�������й���һ��UnitOfWorkʵ����ʹ�ÿ���
                                               // ͨ������ע��ķ�ʽ���ʵ��������ToDoService�Ĺ��������÷�
            .AddCustomRepository<ToDo, ToDoRepository>()
            .AddCustomRepository<Memo, MemoRepository>()
            .AddCustomRepository<User, UserRepository>();

            // ��������������automapper������������ʵ����������֮���ת����
            // ����Ŀ�У������ݿ��з��ص����ݣ���Ӧ����ToDo��Memo֮�ࡣ������������е��ֶ�ֻ�������ݿ��е��У�����̫����
            // �������Ŀ�У����Ƕ����ٶ�����һЩDto�࣬��ЩDto������Entity���񣬵����ж���Ĺ��ܡ�ʹ����ЩDto�ܹ����ú�WPF�໥��ϡ�
            // ����Dto�����ֶ����� OnPropertyChanged()��֪ͨ���¹��ܡ���������WPF��ʹ��Dto����ͨ�� UnitOfWork����ȴ��Entity��
            // Ϊ��ʹ��Dto�ܴ�����ݿ⣬��ô��Ҫ��Dtoת����Entity��ͬʱ�������ݿⷵ�ص�Entity��Ҫת��Dto�������WPF��ϡ�

            // �����Ƕ����Mapӳ��ע�ᵽMapperConfiguration
            var automapperConfog = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProFile());
            });

            // ��MapperConfigurationע�ᵽ�����У�ע���ǵ���
            services.AddSingleton(automapperConfog.CreateMapper());

            // AddTransient�� ÿ��service�����ǻ�ò�ͬ��ʵ��
            // ��������ע��ToDoServiceʹ��ToDoController����װ��ToDoServiceʵ��
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
