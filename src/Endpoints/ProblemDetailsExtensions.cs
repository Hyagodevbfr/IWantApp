﻿namespace IWantApp.Endpoints;

public static class ProblemDetailsExtensions
{
    public static Dictionary<string,string[]> ConvertToProblemDetails (this IReadOnlyCollection<Notification> notifications)
    {
        return notifications.GroupBy(g => g.Key)
                .ToDictionary(g => g.Key,g => g.Select(notification => notification.Message)
                .ToArray( ));
    }

    public static Dictionary<string,string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> error )
    {
        var dictionary = new Dictionary<string,string[]>
        {
            { "Error",error.Select(e => e.Description).ToArray( ) }
        };
        
        return dictionary;
    }

}
