using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace OAuthImplementation.Shared.ModelBinders;

public class FlexibleCaseModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext is null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelType = bindingContext.ModelType;
        object? model = null;

        

        if (modelType == typeof(string) || modelType.IsPrimitive || modelType.IsValueType)
        {
            var modelName = string.IsNullOrEmpty(bindingContext.ModelName) ? bindingContext.FieldName : bindingContext.ModelName;
            if (!string.IsNullOrEmpty(modelName))
            {
                var possibleKeys = new[]
                {
                    modelName,
                    ToSnakeCase(modelName)
                };

                foreach (var key in possibleKeys)
                {
                    var valueProviderResult = bindingContext.ValueProvider.GetValue(key);
                    if (valueProviderResult != ValueProviderResult.None)
                    {
                        var value = valueProviderResult.FirstValue;
                        if (value != null)
                        {
                            model = ConvertValue(value, modelType);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            model = Activator.CreateInstance(modelType);
            var modelProperties = modelType.GetProperties();

            foreach (var property in modelProperties)
            {
                var possibleKeys = new[]
                {
                    property.Name,
                    ToCamelCase(property.Name),
                    ToSnakeCase(property.Name) 
                };

                foreach (var key in possibleKeys)
                {
                    var valueProviderResult = bindingContext.ValueProvider.GetValue(key);

                    if (valueProviderResult != ValueProviderResult.None)
                    {
                        var value = valueProviderResult.FirstValue;
                        if (value != null)
                        {
                            var convertedValue = ConvertValue(value, property.PropertyType);
                            property.SetValue(model, convertedValue);
                        }
                        break;
                    }
                }
            }
        }

        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }

    private object ConvertValue(string value, Type targetType)
    {
        return targetType switch
        {
            Type t when t == typeof(string) => value,
            Type t when t.IsEnum => Enum.Parse(targetType, value, true),
            Type t when t == typeof(bool) => bool.Parse(value),
            Type t when t == typeof(int) => int.Parse(value, CultureInfo.InvariantCulture),
            Type t when t == typeof(double) => double.Parse(value, CultureInfo.InvariantCulture),
            Type t when t == typeof(DateTime) => DateTime.Parse(value, CultureInfo.InvariantCulture),
            Type t when t == typeof(Guid) => Guid.Parse(value, CultureInfo.InvariantCulture),
            _ => Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture)
        };
    }
    private string ToSnakeCase(string input)
    {
        return string.Concat(input.Select((x, i) =>
            i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }

    private string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input) || !char.IsUpper(input[0]))
            return input;

        var camelCase = char.ToLower(input[0], CultureInfo.InvariantCulture).ToString();
        if (input.Length > 1)
            camelCase += input.Substring(1);

        return camelCase;
    }
}