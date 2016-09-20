﻿using System;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Digipolis.Web.Guidelines.JsonConverters
{
    internal class EmptyCollectionContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            Predicate<object> shouldSerialize = property.ShouldSerialize;
            property.ShouldSerialize =
                obj => (shouldSerialize == null || shouldSerialize(obj)) && !IsEmptyCollection(property, obj);
            return property;
        }

        private bool IsEmptyCollection(JsonProperty property, object target)
        {
            try
            {
                var value = property.ValueProvider.GetValue(target);
                var collection = value as ICollection;
                if (collection != null && collection.Count == 0)
                    return true;

                if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    return false;

                var countProp = property.PropertyType.GetProperty("Count");
                if (countProp == null)
                    return false;

                var count = (int)countProp.GetValue(value, null);
                return count == 0;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}