using Amazon.DynamoDBv2;
using MemoryDbAspnet.Model;
using MemoryDbAspnet.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


// Add basic demo configuration
var demoConfig = new MemoryDbDemoConfig();
builder.Configuration.Bind("MemoryDbDemoConfig", demoConfig);
builder.Services.AddSingleton(demoConfig);

// Add AWS Configuration
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();


// Add the service that goes out to DynamoDB
builder.Services.AddTransient<IOrderService, OrderService>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
