using FITCarpoolWebApp.Components;
using FITCarpoolWebApp.Components.Account;
using FITCarpoolWebApp.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Microsoft.AspNetCore.Identity.UI.Services;
using MudBlazor.Services;
using DataAccessLibrary.Data.API;
using AspNetMonsters.Blazor.Geolocation;
using DataAccessLibrary;
using Serilog;
using DataAccessLibrary.Data.Database;


var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration));
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<Radzen.DialogService>(); // Register the DialogService.
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
// Services for component libraries 
builder.Services.AddRadzenComponents();
builder.Services.AddMudServices();
builder.Services.AddScoped<Radzen.NotificationService>();

// Data Access Services 
builder.Services.AddTransient<IGMapsAPI, GMapsAPI>();

// Database Connection services
builder.Services.AddTransient<IGroupScheduleData, GroupScheduleData>();
builder.Services.AddTransient<IGroupRecommendationData, GroupRecomendationData>();
builder.Services.AddTransient<ICarpoolGroupsData, CarpoolGroupsData>();
builder.Services.AddTransient<IFriendsData, FriendsData>();
builder.Services.AddTransient<IGroupMemberLocationsData, GroupMemberLocationsData>();
builder.Services.AddTransient<IGroupMembersData, GroupMembersData>();
builder.Services.AddTransient<IMessagesData, MessagesData>();
builder.Services.AddTransient<IPreferencesData, PreferencesData>();
builder.Services.AddTransient<ISchedulesData, SchedulesData>();
builder.Services.AddTransient<IRolesData, RolesData>();
builder.Services.AddTransient<ITripStatisticsData, TripStatisticsData>();
builder.Services.AddTransient<IUsersData, UsersData>();
builder.Services.AddTransient<ILocationData, LocationData>();
builder.Services.AddTransient<IReportsData, ReportsData>();
builder.Services.AddTransient<IReviewsData, ReviewsData>();
builder.Services.AddTransient<IRatingsData, RatingsData>();
// Geolocation Services
builder.Services.AddHttpClient();
builder.Services.AddTransient<LocationService>();
// services to allow database connection 
builder.Services.AddTransient<ISQLDataAccess, SQLDataAccess>();
// Services to allow groups to former 

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();
// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
