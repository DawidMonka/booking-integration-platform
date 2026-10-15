using SupplierIntegration.Worker;
using SupplierIntegration.Worker.Clients;
using SupplierIntegration.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IDeadLetterPublisher, KafkaDeadLetterPublisher>();
builder.Services.AddHostedService<Worker>();

var supplierApiBaseUrl = builder.Configuration.GetSection("SupplierApi:BaseUrl").Value ?? throw new InvalidOperationException("SupplierApi:BaseUrl configuration is missing");
builder.Services.AddHttpClient<ISupplierClient, SupplierClient>(client =>
{
    client.BaseAddress = new Uri(supplierApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var host = builder.Build();
host.Run();
