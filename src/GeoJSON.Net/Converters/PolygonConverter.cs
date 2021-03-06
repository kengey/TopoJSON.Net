﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolygonConverter.cs" company="Joerg Battermann">
//   Copyright © Joerg Battermann 2014
// </copyright>
// <summary>
//   Defines the PolygonConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GeoJSON.Net.Converters
{
    using System;

    using Geometry;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Converter to read and write the <see cref="MultiPolygon" /> type.
    /// </summary>
    public class PolygonConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var coordinateElements = value as List<LineString>;
            if (coordinateElements != null && coordinateElements.Count > 0)
            {
                if (coordinateElements[0].Coordinates[0] is GeographicPosition)
                {
                    var converter = new LineStringConverter();
                    writer.WriteStartArray();
                    foreach (var subPolygon in coordinateElements)
                    {
                        converter.WriteJson(writer, subPolygon.Coordinates, serializer);
                    }
                    writer.WriteEndArray();
                }
                else
                    // ToDo: implement
                    throw new NotImplementedException();
            }
            else
                serializer.Serialize(writer, value);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param><param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var ringArray = serializer.Deserialize(reader) as JArray;
            var converter = new LineStringConverter();
            var rings = ringArray.Select(ring => (LineString)converter.ReadJson(reader, typeof(LineString), ring, serializer)).ToList();
            return rings;
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
            return objectType == typeof(Polygon);
        }
    }
}
