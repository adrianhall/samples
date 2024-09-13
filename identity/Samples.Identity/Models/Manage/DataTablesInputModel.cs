using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Samples.Identity.Models.Manage;

public class DataTablesInputModel
{
    /// <summary>
    /// The draw counter that this object is a response to - from the 
    /// <c>draw</c> parameter sent as part of the data request.
    /// </summary>
    [FromQuery(Name = "draw")]
    public int RequestId { get; set; } = 0;

    [FromQuery(Name = "start")]
    public int StartIndex { get; set; } = 0;

    [FromQuery(Name = "length")]
    public int Length { get; set; } = 0;

    // TODO: search[value] = string
    // TODO: search[regex] = boolean
    // TODO: order[i][column] = integer
    // TODO: order[i][dir] = string - asc/desc
    // TODO: columns[i][data]
    // TODO: columns[i][name]
    // TODO: columns[i][searchable]
    // TODO: columns[i][orderable]
    // TODO: columns[i][search][value]
    // TODO: columns[i][search][regex]
}
