using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Papara_Final_Project.Models;
using Papara_Final_Project.Repositories;
using Papara_Final_Project.Services;
using Papara_Final_Project.Validations;
using Papara_Final_Project.UnitOfWorks;
using System.Text;
using FluentValidation;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Papara_Final_Project.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection")));

builder.Services.AddControllers().AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<UserRegisterValidator>();
    fv.RegisterValidatorsFromAssemblyContaining<UserUpdateValidator>();
    fv.RegisterValidatorsFromAssemblyContaining<CategoryValidator>();
    fv.RegisterValidatorsFromAssemblyContaining<ProductValidator>();
    fv.RegisterValidatorsFromAssemblyContaining<CouponValidator>();
    fv.RegisterValidatorsFromAssemblyContaining<OrderValidator>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    options.SerializerSettings.Culture = new CultureInfo("tr-TR");
});

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"]
        };
    });

// Swagger ayarlarýný ekliyoruz
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Papara-Final-Project", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IValidator<UserRegisterDTO>, UserRegisterValidator>();
builder.Services.AddScoped<IValidator<UserUpdateDTO>, UserUpdateValidator>();
builder.Services.AddScoped<IValidator<CategoryDTO>, CategoryValidator>();
builder.Services.AddScoped<IValidator<ProductDTO>, ProductValidator>();
builder.Services.AddScoped<IValidator<CouponDTO>, CouponValidator>();
builder.Services.AddScoped<IValidator<OrderDTO>, OrderValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Papara-Final-Project v1"));
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
