using System.Text;
using static System.String;


namespace Core.Infrastructure.Base
{
    public static class QueryGenerator
    {
        public static string Generate(object obj, bool ignoreNullProperties = false)
        {
            var stringBuilder = new StringBuilder();
            if (obj == null)
            {
                return Empty;
            }

            var properties = obj?.GetType()?.GetProperties();
            foreach (var property in properties)
            {
                if (obj.GetType().GetProperty(property.Name)?.GetValue(obj) != null)
                {
                    stringBuilder.Append($"{property.Name}={obj.GetType().GetProperty(property.Name)?.GetValue(obj)}&");
                }
                else
                {
                    stringBuilder.Append(ignoreNullProperties ? Empty : $"{property.Name}=&");
                }
            }

            var queryString = stringBuilder.Length > default(int)
                ? stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString()
                : Empty;
            return queryString;
        }
    }
}