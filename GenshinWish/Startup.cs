using GenshinWish.Attribute;
using GenshinWish.Common;
using GenshinWish.Dao;
using GenshinWish.Helper;
using GenshinWish.Service;
using GenshinWish.Service.WishService;
using GenshinWish.Timer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SqlSugar.IOC;
using System;
using System.IO;
using System.Reflection;

namespace GenshinWish
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            LogHelper.ConfigureLog();//log4net
            LogHelper.Info($"日志配置完毕...");

            loadSiteConfig();
            LogHelper.Info($"读取配置文件...");

            LogHelper.Info($"初始化数据库...");
            services.AddSqlSugar(new IocConfig()//注入Sqlsuger
            {
                DbType = IocDbType.MySql,
                ConnectionString = ApiConfig.ConnectionString,
                IsAutoCloseConnection = true//自动释放
            });
            new DBClient().CreateDB();
            LogHelper.Info($"数据库初始化完毕...");

            //依赖注入
            services.AddScoped<WeaponService, WeaponService>();
            services.AddScoped<StandardService, StandardService>();
            services.AddScoped<CharacterService, CharacterService>();
            services.AddScoped<FullWeaponService, FullWeaponService>();
            services.AddScoped<FullCharacterService, FullCharacterService>();
            services.AddScoped<AuthorizeService, AuthorizeService>();
            services.AddScoped<GoodsService, GoodsService>();
            services.AddScoped<MemberGoodsService, MemberGoodsService>();
            services.AddScoped<MemberService, MemberService>();
            services.AddScoped<WishRecordService, WishRecordService>();
            services.AddScoped<ReceiveRecordService, ReceiveRecordService>();
            services.AddScoped<RequestRecordService, RequestRecordService>();
            services.AddScoped<GenerateService, GenerateService>();

            services.AddScoped<AuthorizeDao, AuthorizeDao>();
            services.AddScoped<GoodsDao, GoodsDao>();
            services.AddScoped<MemberDao, MemberDao>();
            services.AddScoped<MemberGoodsDao, MemberGoodsDao>();
            services.AddScoped<GoodsPoolDao, GoodsPoolDao>();
            services.AddScoped<ReceiveRecordDao, ReceiveRecordDao>();
            services.AddScoped<RequestRecordDao, RequestRecordDao>();
            services.AddScoped<WishRecordDao, WishRecordDao>();

            services.AddScoped<AuthorizeAttribute>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //controller
            services.AddControllers();

            //Quartz
            LogHelper.Info($"正在初始化定时任务...");
            TimerManager.StartAllJob();

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GenshinWish Api 文档", Description = "GenshinWish Api 文档", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));// 为 Swagger 设置xml文档注释路径
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GenshinWish Api v1");
                c.RoutePrefix = string.Empty;//设置根节点访问
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //加载默认蛋池数据到内存
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<GoodsService>().LoadGoodsPool();
            }
        }

        //将配置文件中的信息加载到内存
        private void loadSiteConfig()
        {
            ApiConfig.ConnectionString = Configuration.GetSection("ConnectionString").Value;
            ApiConfig.ImgSavePath = Configuration.GetSection("ImgSavePath").Value;
            ApiConfig.MaterialSavePath = Configuration.GetSection("MaterialSavePath").Value;
            ApiConfig.ImgHttpUrl = Configuration.GetSection("ImgHttpUrl").Value;
            ApiConfig.PublicAuthCode = Configuration.GetSection("PublicAuthCode").Value;
        }

    }
}
