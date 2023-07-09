using Newtonsoft.Json.Linq;

namespace JackCraftLauncher.Class.Utils;

public class JsonUtils
{
    /// <summary>
    ///     合并与添加一串Json到另外一串Json
    /// </summary>
    /// <param name="source">要与目标合并的Json JObject</param>
    /// <param name="target">需要合并的目标Json JObject</param>
    /// <returns>已合并与添加后的JObject</returns>
    public static JObject MergedJson(JObject source, JObject target)
    {
        JObject obj1 = target;
        JObject obj2 = source;
        // 将第二个 JObject 对象中的属性合并到第一个 JObject 对象中
        foreach (var property in obj2.Properties())
        {
            if (obj1[property.Name] != null && obj1[property.Name].Type == JTokenType.Array && property.Value.Type == JTokenType.Array)
            {
                // 如果属性值是数组，则将第二个 JObject 对象中的数组元素添加到第一个 JObject 对象中的数组元素的后面
                JArray array1 = (JArray)obj1[property.Name];
                JArray array2 = (JArray)property.Value;
                foreach (var item in array2)
                {
                    array1.Add(item);
                }
            }
            else
            {
                // 否则，将第二个 JObject 对象中的属性值添加到第一个 JObject 对象中的属性值的后面
                obj1[property.Name] = property.Value;
            }
        }

        return obj1;
        /*var result = new JObject();
        foreach (var property in source.Properties())
        {
            result.Add(property.Name, property.Value);
        }

        foreach (var property in target.Properties())
        {
            if (result.ContainsKey(property.Name))
            {
                if (property.Value is JObject sourceValue && result[property.Name] is JObject targetValue)
                {
                    result[property.Name] = MergedJson(sourceValue, targetValue);
                }
                else
                {
                    result[property.Name] = property.Value;
                }
            }
            else
            {
                result.Add(property.Name, property.Value);
            }
        }

        return result.ToString(Formatting.None);*/
    }
}