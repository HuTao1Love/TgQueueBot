using Application.Extensions;
using DataAccess.Extensions;
using Environment.Extensions;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection();

collection
    .AddApplication()
    .LoadEnvironment()
    .LoadDatabase();