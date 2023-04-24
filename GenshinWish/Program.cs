using GenshinWish.Attribute;
using GenshinWish.Common;
using GenshinWish.Dao;
using GenshinWish.Helper;
using GenshinWish.Models.DTO;
using GenshinWish.Service;
using GenshinWish.Service.WishService;
using GenshinWish.Timer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SqlSugar.IOC;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

LogHelper.ConfigureLog();
LogHelper.Info($"日志初始化完毕...");

ConfigHelper.LoadAppSettings(builder);
LogHelper.Info($"配置文件加载完毕...");

LogHelper.Info($"开始初始化数据库...");
builder.Services.AddSqlSugar(new IocConfig()
{
    DbType = IocDbType.MySql,
    ConnectionString = ApiConfig.ConnectionString,
    IsAutoCloseConnection = true
});
new DBClient().CreateDB();
LogHelper.Info($"数据库初始化完毕...");

//依赖注入
builder.Services.AddScoped<WeaponService, WeaponService>();
builder.Services.AddScoped<StandardService, StandardService>();
builder.Services.AddScoped<CharacterService, CharacterService>();
builder.Services.AddScoped<FullWeaponService, FullWeaponService>();
builder.Services.AddScoped<FullCharacterService, FullCharacterService>();
builder.Services.AddScoped<AuthorizeService, AuthorizeService>();
builder.Services.AddScoped<GoodsService, GoodsService>();
builder.Services.AddScoped<MemberGoodsService, MemberGoodsService>();
builder.Services.AddScoped<MemberService, MemberService>();
builder.Services.AddScoped<WishRecordService, WishRecordService>();
builder.Services.AddScoped<ReceiveRecordService, ReceiveRecordService>();
builder.Services.AddScoped<RequestRecordService, RequestRecordService>();
builder.Services.AddScoped<GenerateService, GenerateService>();

builder.Services.AddScoped<AuthorizeDao, AuthorizeDao>();
builder.Services.AddScoped<GoodsDao, GoodsDao>();
builder.Services.AddScoped<MemberDao, MemberDao>();
builder.Services.AddScoped<MemberGoodsDao, MemberGoodsDao>();
builder.Services.AddScoped<GoodsPoolDao, GoodsPoolDao>();
builder.Services.AddScoped<ReceiveRecordDao, ReceiveRecordDao>();
builder.Services.AddScoped<RequestRecordDao, RequestRecordDao>();
builder.Services.AddScoped<WishRecordDao, WishRecordDao>();

builder.Services.AddScoped<AuthorizeAttribute>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllers();

TimerManager.StartAllJob();
LogHelper.Info($"定时任务初始化完毕...");

//Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GenshinWish Api 文档", Description = "GenshinWish Api 文档", Version = "v1" });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));// 为 Swagger 设置xml文档注释路径
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
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

using (var scope = app.Services.CreateScope())
{
    //从json文件中同步数据
    var initData = ConfigHelper.LoadInitData();
    scope.ServiceProvider.GetRequiredService<GoodsService>().SyncGoods(initData);

    //加载默认蛋池数据到内存
    scope.ServiceProvider.GetRequiredService<GoodsService>().LoadGoodsPool();
}

app.Run();