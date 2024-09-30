namespace Aspire.Hosting;

internal static class JdbcExpression
{
    internal static ReferenceExpression Create(this IResourceBuilder<PostgresDatabaseResource> dbResourceBuilder)
    {
        PostgresDatabaseResource dbResource = dbResourceBuilder.Resource;
        PostgresServerResource dbServer = dbResource.Parent;
        EndpointReference endpoint = dbServer.PrimaryEndpoint;

        return ReferenceExpression.Create($"jdbc:postgresql://{endpoint.Property(EndpointProperty.Host)}:{endpoint.Property(EndpointProperty.Port)}/{dbResource.Name}");
    }
}
