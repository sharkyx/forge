﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Neon.Utilities {
    /// <summary>
    /// Caches type name to type lookups. Type lookups occur in all loaded assemblies.
    /// </summary>
    public static class TypeCache {
        /// <summary>
        /// Cache from fully qualified type name to type instances.
        /// </summary>
        private static Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Cache from Type to the respective TypeMetadata.
        /// </summary>
        private static Dictionary<Type, TypeMetadata> _cachedMetadata = new Dictionary<Type, TypeMetadata>();

        /// <summary>
        /// Finds the associated TypeMetadata for the given type.
        /// </summary>
        /// <param name="type">The type to find the type metadata for.</param>
        /// <returns>A TypeMetadata that models the given type.</returns>
        public static TypeMetadata FindTypeMetadata(Type type) {
            TypeMetadata metadata;
            if (_cachedMetadata.TryGetValue(type, out metadata) == false) {
                metadata = new TypeMetadata(type);
                _cachedMetadata[type] = metadata;
            }
            return metadata;
        }

        /// <summary>
        /// Find a type with the given name. An exception is thrown if no type with the given name
        /// can be found. This method searches all currently loaded assemblies for the given type.
        /// </summary>
        /// <param name="name">The fully qualified name of the type.</param>
        public static Type FindType(string name) {
            // see if the type is in the cache; if it is, then just return it
            {
                Type type;
                if (_cachedTypes.TryGetValue(name, out type)) {
                    return type;
                }
            }

            // cache lookup failed; search all loaded assemblies
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Type type = assembly.GetType(name);
                if (type != null) {
                    _cachedTypes[name] = type;
                    return type;
                }
            }

            // couldn't find the type; throw an exception
            throw new Exception(string.Format("Unable to find the type for {0}", name));
        }
    }
}