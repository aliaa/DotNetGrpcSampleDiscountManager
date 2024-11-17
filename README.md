This is a sample project for demonstrating gRPC usage in .Net 8
It consists of 4 projects:

## Projects
1. **DiscountManager.Server**: An asp.net core project as a host to gRPC calls
2. **DiscountManager.BlazorClient**: A simple Blazor web-assembly client to consume gRPC-web messages.
3. **DiscountManager.ConsoleClient**: A simple Console client to consume gRPC messages.
3. **DiscountManager.ProtoDefinitions**: A common project referenced on all other projects containing proto files.

## How to run
1. Have an Sql Server instance running.
2. Configure connection string in appsettings.Development.json
3. Run migrations
4. Run server first
5. Check the port of server and if required update the server url in the clients
6. Run any of both clients