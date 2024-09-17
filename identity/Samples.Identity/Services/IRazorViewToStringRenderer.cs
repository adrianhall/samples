namespace Samples.Identity.Services;

/// <summary>
/// Definition of the mechanism to render a Razor View as a string.
/// </summary>
public interface IRazorViewToStringRenderer
{
    /// <summary>
    /// Renders an email template without a model.
    /// </summary>
    /// <param name="viewName">The name of the view.</param>
    /// <returns>The rendered HTML email template.</returns>
    Task<string> RenderViewToStringAsync(string viewName);

    /// <summary>
    /// Renders an email template with a model.
    /// </summary>
    /// <typeparam name="TModel">The type of the view model.</typeparam>
    /// <param name="viewName">The name of the view.</param>
    /// <param name="model">The view model.</param>
    /// <returns>The rendered HTML email template.</returns>
    Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
}
