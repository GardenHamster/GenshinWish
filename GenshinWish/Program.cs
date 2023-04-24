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
LogHelper.Info($"��־��ʼ�����...");

ConfigHelper.LoadAppSettings(builder);
LogHelper.Info($"�����ļ��������...");

LogHelper.Info($"��ʼ��ʼ�����ݿ�...");
builder.Services.AddSqlSugar(new IocConfig()
{
    DbType = IocDbType.MySql,
    ConnectionString = ApiConfig.ConnectionString,
    IsAutoCloseConnection = true
});
new DBClient().CreateDB();
LogHelper.Info($"���ݿ��ʼ�����...");

//����ע��
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
LogHelper.Info($"��ʱ�����ʼ�����...");

//Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GenshinWish Api �ĵ�", Description = "GenshinWish Api �ĵ�", Version = "v1" });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));// Ϊ Swagger ����xml�ĵ�ע��·��
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
    c.RoutePrefix = string.Empty;//���ø��ڵ����
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
    //��json�ļ���ͬ������
    var initData = ConfigHelper.LoadInitData();
    scope.ServiceProvider.GetRequiredService<GoodsService>().SyncGoods(initData);

    //����Ĭ�ϵ������ݵ��ڴ�
    scope.ServiceProvider.GetRequiredService<GoodsService>().LoadGoodsPool();
}

app.Run();