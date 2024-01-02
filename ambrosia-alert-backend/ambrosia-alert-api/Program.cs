using System.Configuration;
using Azure.Data.Tables;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod();

                      });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create an Azure Storage Table client  
string connectionString = "DefaultEndpointsProtocol=https;AccountName=datctable1;AccountKey=i2NdpOOkCX3MuBvbXrNB8Hqgo3CTk1zVQaVrDCHPj5LrkC+le3IwbPyXOHpmzOKxlPnNw5nC2+zv+AStc9Y4VQ==;EndpointSuffix=core.windows.net";
CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
string tableName = "AmbrosiaAppUsers";
TableServiceClient tableServiceClient = new(connectionString);
TableClient tableClient = tableServiceClient.GetTableClient(tableName);

// Register the Azure Storage Table client in the dependency injection container
builder.Services.AddSingleton(tableClient);

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
