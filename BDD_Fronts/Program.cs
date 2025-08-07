using BDD_Fronts.Middlewares;

namespace BDD_Fronts;
/// <summary>
/// エントリーポイント
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        /*** 依存性定義 ***/
        Configs.SetupAppDependency.SettingDependencyInjection(
            builder.Configuration, builder.Services);
        /*** Http Sessioに関する設定 ***/
        builder.Services.AddControllersWithViews();
        // セッションの設定
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // セッションの有効期間を設定
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true; // GDPRに準拠するために必要な場合
        });



        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // カスタムミドルウェアを登録する
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseSession(); // Sessionの利用

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Index}/{action=Index}");

        app.Run();
    }
}
