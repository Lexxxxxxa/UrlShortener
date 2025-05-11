namespace UrlShortener.Utils;

public static class Base62Converter
{
    private const string Base62Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Encode(int value)
    {
        if (value == 0) return "0";

        var result = "";
        while (value > 0)
        {
            result = Base62Chars[value % 62] + result;
            value /= 62;
        }

        return result;
    }
}
