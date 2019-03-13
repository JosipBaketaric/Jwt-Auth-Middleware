using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using System.Threading.Tasks;
using System.Collections.Concurrent;

/// <summary>
/// Description of UrlPathPaterns.
/// </summary>
internal class UrlPathPaterns
{
    public static ControllerActionDescriptor MatchingPathPatern(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, string pathToMatch, string method)
    {

        if (pathToMatch.StartsWith("/"))
        {
            pathToMatch = pathToMatch.Substring(1);
        }

        int n = NumOfSegments(pathToMatch);
        // usporedba po broju segmenata 
        // ako nema putanja sa istim brojem segmenata, onda nema podudaranja
        ConcurrentBag<string> lstEqualNumOfSegments = new ConcurrentBag<string>();

        Parallel.ForEach(actionDescriptorCollectionProvider.ActionDescriptors.Items, (p) =>
        {
            if (p?.AttributeRouteInfo?.Template != null && NumOfSegments(p.AttributeRouteInfo.Template) == n)
            {
                // ako postoji patern putanje sa istim brojem segmenata onda ga dodaj u listu
                lstEqualNumOfSegments.Add(p.AttributeRouteInfo.Template);
            }
        });

        if (lstEqualNumOfSegments.Count == 0)
        {
            return null;
        }

        // PROVJERA SEGMENATA
        string[] segmentsM = GetSegments(pathToMatch);
        string choosenPatern = "";

        Parallel.ForEach(lstEqualNumOfSegments, (p, state) => {
            string[] paternSegments = GetSegments(p);
            int rez = 0;
            for (int i = 0; i < n; i++)
            {
                bool isToken = false;
                if (paternSegments[i].IndexOf("{") > -1) { isToken = true; }
                if (isToken) { rez++; }
                else
                {
                    if (segmentsM[i] == paternSegments[i]) { rez++; }
                }
            }

            if (rez == n)
            {
                choosenPatern = p;
                state.Break();
            }
        });

        if (String.IsNullOrEmpty(choosenPatern))
        {
            return null;
        }

        List<ControllerActionDescriptor> retval = actionDescriptorCollectionProvider.ActionDescriptors.Items
            .Where(y => y?.AttributeRouteInfo?.Template == choosenPatern)
            .Select(x=> x as ControllerActionDescriptor)
            .ToList();

        if(retval == null)
        {
            return null;
        }

        //filter by method
        retval = retval.Where(x => CheckIfMethodMatch(x, method) == true)
            .ToList();

        if (retval == null || retval.Count == 0)
        {
            return null;
        }

        //if retval.Count > 0 ?? - take first

        return retval[0];
    }


    private static bool CheckIfMethodMatch(ControllerActionDescriptor route, string method)
    {
        ConcurrentBag<bool> result = new ConcurrentBag<bool>();

        Parallel.ForEach(route.ActionConstraints, (actionConstraint, state) => {
            if ((actionConstraint as HttpMethodActionConstraint).HttpMethods.Contains(method))
            {
                result.Add(true);
                state.Break();
            }
        });

        if (result.Contains(true))
        {
            return true;
        }

        return false;
    }

    private static int NumOfSegments(string path)
    {
        return path.Split("/".ToCharArray()).Length;
    }

    private static string[] GetSegments(string path)
    {
        return path.Split("/".ToCharArray());
    }

}