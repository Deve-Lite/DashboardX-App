using Microsoft.AspNetCore.Components;
using System.Web;

namespace Presentation.Utils;

public class UrlUtils
{
    public static string GetParamFromCurrentUrl(NavigationManager manager, string paramName)
    {
        var query = manager.ToAbsoluteUri(manager.Uri).Query;

        if (!string.IsNullOrEmpty(query))
        {
            var queryParams = HttpUtility.ParseQueryString(query);

            return queryParams[paramName] ?? string.Empty;
        }

        return string.Empty;
    }
}
