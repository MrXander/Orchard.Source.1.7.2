namespace Alexan.PhotoAlbums.Extensions
{
    public static class StringExtensins
    {
        public static bool? ToBool(this string str)
        {
            bool value;
            return bool.TryParse(str, out value) ? (bool?)value : null;
        }
    }
}