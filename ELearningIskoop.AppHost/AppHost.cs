var builder = DistributedApplication.CreateBuilder(args);
// Redis Cache
var cache = builder.AddRedis("cache");

// Message Broker (RabbitMQ)
var messaging = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();

// PostgreSQL Database  
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var elearningDb = postgres.AddDatabase("ElearningIskoop");

// API projesine referanslarý ekleyin
builder.AddProject<Projects.ELearningIskoop_API>("elearningiskoop-api")
    .WithReference(elearningDb)     // Database baðlantýsý
    .WithReference(cache)           // Redis baðlantýsý  
    .WithReference(messaging);      // RabbitMQ baðlantýsý

builder.Build().Run();
