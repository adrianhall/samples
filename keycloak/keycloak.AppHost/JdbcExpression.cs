namespace Aspire.Hosting;

internal static class JdbcExpression
{
    internal static ReferenceExpression Create(IResourceBuilder<PostgresDatabaseResource> dbResourceBuilder)
    {
        PostgresDatabaseResource dbResource = dbResourceBuilder.Resource;
        PostgresServerResource dbServer = dbResource.Parent;
        EndpointReference endpoint = dbServer.PrimaryEndpoint;

        if (dbResourceBuilder.ApplicationBuilder.ExecutionContext.IsRunMode)
        {
            return ReferenceExpression.Create($"jdbc:postgresql://host.docker.internal:{endpoint.Property(EndpointProperty.Port)}/{dbResource.Name}");
        }
        else
        {
            return ReferenceExpression.Create($"jdbc:postgresql://{endpoint.Property(EndpointProperty.Host)}:{endpoint.Property(EndpointProperty.Port)}/{dbResource.Name}");
        }
    }
}
