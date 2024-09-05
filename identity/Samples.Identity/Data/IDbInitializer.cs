namespace Samples.Identity.Data;

/// <summary>
/// Interface describing how to initialize a database (for dependency injection).
/// </summary>
public interface IDbInitializer
{
    /// <summary>
    /// Initialize the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A task that resolves when complete.</returns>
    Task InitializeDatabaseAsync(CancellationToken cancellationToken = default);
}
