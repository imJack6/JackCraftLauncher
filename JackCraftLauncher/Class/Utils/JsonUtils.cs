using System.Linq;
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
        // 将第二个 JObject 对象中的属性合并到第一个 JObject 对象中
        foreach (var property in source.Properties())
        {
            if (target[property.Name] != null && target[property.Name].Type == JTokenType.Array && property.Value.Type == JTokenType.Array)
            {
                // 如果属性值是数组，则将第二个 JObject 对象中的数组元素添加到第一个 JObject 对象中的数组元素的后面
                JArray array1 = (JArray)target[property.Name];
                JArray array2 = (JArray)property.Value;
                foreach (var item in array2)
                {
                    array1.Add(item);
                }
            }
            else if (target[property.Name] != null && target[property.Name].Type == JTokenType.Object && property.Value.Type == JTokenType.Object)
            {
                // 如果属性值是 JObject 对象，则递归调用 MergedJson() 方法
                MergedJson((JObject)property.Value, (JObject)target[property.Name]);
            }
            else
            {
                // 否则，将第二个 JObject 对象中的属性值添加到第一个 JObject 对象中的属性值的后面
                target[property.Name] = property.Value;
            }
        }

        return target;
    }
    public static JObject RemoveNullProperties(JObject obj)
    {
        // 获取所有属性
        var properties = obj.Properties().ToList();
        foreach (var property in properties)
        {
            // 如果属性值为 null，则删除该属性
            if (property.Value.Type == JTokenType.Null)
            {
                property.Remove();
            }
            // 如果属性值为 JObject 对象，则递归调用 RemoveNullProperties() 方法
            else if (property.Value.Type == JTokenType.Object)
            {
                RemoveNullProperties((JObject)property.Value);
            }
            // 如果属性值为 JArray 对象，则遍历数组中的元素，如果元素为 JObject 对象，则递归调用 RemoveNullProperties() 方法
            else if (property.Value.Type == JTokenType.Array)
            {
                var array = (JArray)property.Value;
                for (int i = array.Count - 1; i >= 0; i--)
                {
                    if (array[i].Type == JTokenType.Object)
                    {
                        RemoveNullProperties((JObject)array[i]);
                    }
                }
            }
        }

        return obj;
    }
    public static JObject RemoveEmptyProperties(JObject obj)
    {
        // 获取所有属性
        var properties = obj.Properties().ToList();
        foreach (var property in properties)
        {
            // 如果属性值为 null 或空的 JObject 对象，则删除该属性
            if (property.Value.Type == JTokenType.Null || (property.Value.Type == JTokenType.Object && !property.Value.HasValues))
            {
                property.Remove();
            }
            // 如果属性值为 JObject 对象，则递归调用 RemoveEmptyProperties() 方法
            else if (property.Value.Type == JTokenType.Object)
            {
                RemoveEmptyProperties((JObject)property.Value);
            }
            // 如果属性值为 JArray 对象，则遍历数组中的元素，如果元素为 JObject 对象，则递归调用 RemoveEmptyProperties() 方法
            else if (property.Value.Type == JTokenType.Array)
            {
                var array = (JArray)property.Value;
                for (int i = array.Count - 1; i >= 0; i--)
                {
                    if (array[i].Type == JTokenType.Object)
                    {
                        RemoveEmptyProperties((JObject)array[i]);
                    }
                }
            }
        }

        return obj;
    }
}