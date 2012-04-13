// -----------------------------------------------------------------------
// <copyright file="StringEnumConverter.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public sealed class StringEnumConverter : JsonConverter
    {
        private readonly Dictionary<Type, BidirectionalDictionary<string, string>> _enumMemberNamesPerType = new Dictionary<Type, BidirectionalDictionary<string, string>>();

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;

            string enumName = e.ToString("G");

            if (char.IsNumber(enumName[0]) || enumName[0] == '-')
            {
                writer.WriteValue(value);
            }
            else
            {
                BidirectionalDictionary<string, string> map = GetEnumNameMap(e.GetType());

                string resolvedEnumName;
                map.TryGetByFirst(enumName, out resolvedEnumName);
                resolvedEnumName = resolvedEnumName ?? enumName;
                writer.WriteValue(resolvedEnumName);
            }
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type t = objectType.IsNullable() ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!objectType.IsNullable())
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "Cannot convert null value to {0}.", objectType));

                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                var map = GetEnumNameMap(t);
                string resolvedEnumName;
                map.TryGetBySecond(reader.Value.ToString(), out resolvedEnumName);
                resolvedEnumName = resolvedEnumName ?? reader.Value.ToString();

                return Enum.Parse(t, resolvedEnumName, true);
            }

            if (reader.TokenType == JsonToken.Integer)
                return Enum.ToObject(t, reader.Value);

            throw new Exception(string.Format(CultureInfo.InvariantCulture, "Unexpected token when parsing enum. Expected String or Integer, got {0}." , reader.TokenType));
        }

        /// <summary>
        /// A cached representation of the Enum string representation to respect per Enum field name.
        /// </summary>
        /// <param name="t">The type of the Enum.</param>
        /// <returns>A map of enum field name to either the field name, or the configured enum member name (<see cref="EnumMemberAttribute"/>).</returns>
        private BidirectionalDictionary<string, string> GetEnumNameMap(Type t)
        {
            BidirectionalDictionary<string, string> map;

            if (!_enumMemberNamesPerType.TryGetValue(t, out map))
            {
                lock (_enumMemberNamesPerType)
                {
                    if (_enumMemberNamesPerType.TryGetValue(t, out map))
                        return map;

                    map = new BidirectionalDictionary<string, string>(
                      StringComparer.OrdinalIgnoreCase,
                      StringComparer.OrdinalIgnoreCase);

                    foreach (FieldInfo f in t.GetFields())
                    {
                        string n1 = f.Name;
                        string n2;

#if !NET20
                        n2 = f.GetCustomAttributes(typeof(EnumMemberAttribute), true)
                                      .Cast<EnumMemberAttribute>()
                                      .Select(a => a.Value)
                                      .SingleOrDefault() ?? f.Name;
#else
            n2 = f.Name;
#endif

                        string s;
                        if (map.TryGetBySecond(n2, out s))
                        {
                            throw new Exception(string.Format(CultureInfo.InvariantCulture, "Enum name '{0}' already exists on enum '{1}'.", n2, t.Name));
                        }

                        map.Add(n1, n2);
                    }

                    _enumMemberNamesPerType[t] = map;
                }
            }

            return map;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            Type t = objectType.IsNullable() ? Nullable.GetUnderlyingType(objectType) : objectType;
            return t.IsEnum;
        }

        private class BidirectionalDictionary<TFirst, TSecond>
        {
            private readonly IDictionary<TFirst, TSecond> _firstToSecond;
            private readonly IDictionary<TSecond, TFirst> _secondToFirst;

            public BidirectionalDictionary()
                : this(EqualityComparer<TFirst>.Default, EqualityComparer<TSecond>.Default)
            {
            }

            public BidirectionalDictionary(IEqualityComparer<TFirst> firstEqualityComparer, IEqualityComparer<TSecond> secondEqualityComparer)
            {
                _firstToSecond = new Dictionary<TFirst, TSecond>(firstEqualityComparer);
                _secondToFirst = new Dictionary<TSecond, TFirst>(secondEqualityComparer);
            }

            public void Add(TFirst first, TSecond second)
            {
                if (_firstToSecond.ContainsKey(first) || _secondToFirst.ContainsKey(second))
                {
                    throw new ArgumentException("Duplicate first or second");
                }
                _firstToSecond.Add(first, second);
                _secondToFirst.Add(second, first);
            }

            public bool TryGetByFirst(TFirst first, out TSecond second)
            {
                return _firstToSecond.TryGetValue(first, out second);
            }

            public bool TryGetBySecond(TSecond second, out TFirst first)
            {
                return _secondToFirst.TryGetValue(second, out first);
            }
        }
    }
}
