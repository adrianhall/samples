using System.Text.Json.Serialization;

namespace Samples.Identity.Models.Manage;

/// <summary>
/// The response for a datatables.net server side rendering.
/// </summary>
/// <see href="https://datatables.net/manual/server-side"/>
/// <typeparam name="TEntity">The type of entity being sent.</typeparam>
public class BootstrapTableViewModel<TEntity> where TEntity : class
{
    /// <summary>
    /// The draw counter that this object is a response to - from the 
    /// <c>draw</c> parameter sent as part of the data request.
    /// </summary>
    [JsonPropertyName("draw")]
    public int RequestId { get; set; } = 0;

    /// <summary>
    /// Total records, before filtering (i.e. the total number of records in the database)
    /// </summary>
    [JsonPropertyName("recordsTotal")]
    public int TotalCount { get; set; } = 0;

    /// <summary>
    /// Total records, after filtering (i.e. the total number of records after filtering
    /// has been applied - not just the number of records being returned for this page of data).
    /// </summary>
    [JsonPropertyName("recordsFiltered")]
    public int FilteredCount { get; set; } = 0;

    /// <summary>
    /// The data to be displayed in the table. This is an array of data source objects, one for 
    /// each row, which will be used by DataTables. Note that this parameter's name can be 
    /// changed using the ajax option's dataSrc property.
    /// </summary>
        [JsonPropertyName("data")]
    public IEnumerable<TEntity> Items { get; set; } = [];

    /// <summary>
    /// Optional: If an error occurs during the running of the server-side processing script, you 
    /// can inform the user of this error by passing back the error message to be displayed using 
    /// this parameter. Do not include if there is no error.
    /// </summary>
    [JsonPropertyName("error")]
    public string? ErrorMessage { get; set; }
}
