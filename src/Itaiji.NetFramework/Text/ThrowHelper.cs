using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itaiji.Text;

internal static class ThrowHelper
{
    public static void ThrowArgumentOutOfRangeException(string paramName)
    {
        throw new ArgumentOutOfRangeException(paramName);
    }

    public static void ThrowArgumentNullException(string paramName)
    {
        throw new ArgumentNullException(paramName);
    }

    internal static void ThrowArgumentException_DestinationTooShort()
    {
        //TODO: localize message
        throw new ArgumentException("", "destination");
    }

    internal static void ThrowArgumentOutOfRange_IndexMustBeLessException()
    {
        //TODO
        throw new ArgumentOutOfRangeException("", "destination");
    }

    internal static void ThrowArgumentException_CannotExtractScalar(string paramName)
    {
        //TODO
        throw new ArgumentException("", "destination");

    }


}

internal static class ExceptionArgument
{
    public static string ch => "ch";
    public static string value => "value";
    public static string index => "index";
    public static string input => "input";
    public static string culture => "culture";
}