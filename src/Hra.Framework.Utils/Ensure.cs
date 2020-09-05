using System;
using System.Collections.Generic;
using System.Linq;

namespace Hra.Framework.Utils
{
    public static class Ensure
    {
        public static void NotNull(object @object, string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName)) throw new ArgumentNullException(nameof(parameterName));

            if (@object is null) throw new ArgumentNullException(nameof(parameterName));
        }

        public static void NotNull<T>(T @object, string parameterName) where T : class
        {
            if (string.IsNullOrEmpty(parameterName)) throw new ArgumentNullException(nameof(parameterName));

            if (@object is null) throw new ArgumentNullException(nameof(T), parameterName);
        }

        public static void NotNullOrDefault<T>(T @object, string parameterName)
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            NotNull(@object, typeof(T).Name);

            if (@object.IsDefaultValue()) throw new ArgumentException($"{parameterName} should not be equal to it's default value {default(T)}");
        }

        public static bool IsDefaultValue<T>(this T @object) => !typeof(T).IsEnum && Equals(@object, default(T));

        public static void NotNullOrEmpty<T>(IEnumerable<T> collection, string parameterName)
        {
            NotNull(collection, parameterName);

            if (!collection.Any()) throw new ArgumentException($"The element {parameterName} is empty");
        }

        public static void IsEnum<T>(string parameterName)
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            if (!typeof(T).IsEnum) throw new ArgumentException($"{typeof(T).Name} is not an enum", parameterName);
        }

        public static void IsInterface<T>(string parameterName)
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            if (!typeof(T).IsInterface) throw new ArgumentException($"{typeof(T).Name} is not an interface");
        }

        public static void ArgumentIsTypeOf<T>(object @object, string parameterName)
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            NotNull(@object, typeof(T).Name);

            if (!(@object is T)) throw new ArgumentException($"{@object.GetType().Name} is not of type {typeof(T).Name}", parameterName);
        }
    }
}
