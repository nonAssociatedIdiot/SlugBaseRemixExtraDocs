﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SlugBase
{
    /// <summary>
    /// Represents a collection of values with unique IDs that can be loaded from JSON.
    /// </summary>
    /// <typeparam name="TKey">The type of the <see cref="ExtEnum{T}"/> keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public class JsonRegistry<TKey, TValue>
        where TKey : ExtEnum<TKey>
    {
        private readonly Dictionary<TKey, Entry> _entries = new();
        private readonly Func<TKey, JsonObject, TValue> _fromJson;

        /// <summary>
        /// A collection of all keys used by this registry.
        /// </summary>
        public IEnumerable<TKey> Keys => _entries.Keys;

        /// <summary>
        /// A collection of all values registered.
        /// </summary>
        public IEnumerable<TValue> Values => _entries.Values.Select(entry => entry.Value);

        /// <summary>
        /// Create a new registry.
        /// </summary>
        /// <param name="fromJson">The factory that creates values from JSON.</param>
        public JsonRegistry(Func<TKey, JsonObject, TValue> fromJson)
        {
            _fromJson = fromJson;
        }

        /// <summary>
        /// Register all JSON files in a directory using <see cref="AddFromFile(string)"/>.
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        public void ScanDirectory(string directory)
        {
            var files = AssetManager.ListDirectory(directory, includeAll: true);

            foreach (var file in files.Where(file => file.EndsWith(".json")))
            {
                try
                {
                    // Unload the old entry loaded from this file path
                    foreach(var pair in _entries)
                    {
                        if (pair.Value.Path != null && pair.Value.Path.Equals(file, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Remove(pair.Key);
                            break;
                        }
                    }

                    // Create from JSON
                    AddFromFile(file);
                }
                catch (Exception e)
                {
                    if (e is JsonException jsonE)
                        SlugBasePlugin.Logger.LogError($"Failed to parse SlugBase object from {Path.GetFileName(file)}: {jsonE.Message}\nField: {jsonE.JsonPath ?? "unknown"}");
                    else
                        SlugBasePlugin.Logger.LogError($"Failed to load SlugBase object from {Path.GetFileName(file)}: {e.Message}");
                    SlugBasePlugin.Logger.LogError($"Full path: {file}");

                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// Parse a file as JSON and link it to a new <see cref="ExtEnum{T}"/> ID.
        /// </summary>
        /// <param name="path">The file path to the json.</param>
        /// <returns>The registered value.</returns>
        public TValue AddFromFile(string path)
        {
            return Add(path, JsonAny.Parse(File.ReadAllText(path)).AsObject());
        }

        /// <summary>
        /// Load a <typeparamref name="TValue"/> from JSON and link it to a new <see cref="ExtEnum{T}"/> ID.
        /// </summary>
        /// <param name="json">The json data for the new object.</param>
        /// <returns>The registered value.</returns>
        public TValue Add(JsonObject json) => Add(null, json);

        private TValue Add(string path, JsonObject json)
        {
            var key = ClaimID(json.GetString("id"));

            try
            {
                var entry = new Entry(path, _fromJson(key, json));
                _entries[key] = entry;
                return entry.Value;
            }
            catch
            {
                key.Unregister();
                throw;
            }
        }

        /// <summary>
        /// Unregister a value.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Remove(TKey id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (!_entries.ContainsKey(id))
                throw new ArgumentException($"ID \"{id}\" is not a value of {typeof(TKey).FullName}");

            if (!_entries.Remove(id))
                throw new ArgumentException($"ID \"{id}\" from {typeof(TKey).FullName} was not in the registry!");

            id.Unregister();
        }

        /// <summary>
        /// Get a registered value by key.
        /// </summary>
        /// <param name="id">The unique ID of the value.</param>
        /// <param name="value">The registered value with the unique ID.</param>
        /// <returns><c>true</c> if the value was found, <c>false</c> otherwise.</returns>
        public bool TryGet(TKey id, out TValue value)
        {
            if(id != null && _entries.TryGetValue(id, out var entry))
            {
                value = entry.Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Get a registered value by key.
        /// </summary>
        /// <param name="id">The unique ID of the value.</param>
        /// <returns>The registered value with the unique ID.</returns>
        public TValue GetOrDefault(TKey id)
        {
            return id != null && TryGet(id, out var value) ? value : default;
        }

        /// <summary>
        /// Gets the path to a registered value by key.
        /// </summary>
        /// <param name="id">The unique ID of the value.</param>
        /// <param name="path">The path to the value's JSON file.</param>
        /// <returns><c>true</c> if the value was found and has a path, <c>false</c> otherwise.</returns>
        public bool TryGetPath(TKey id, out string path)
        {
            if (id != null && _entries.TryGetValue(id, out var entry))
            {
                path = entry.Path;
                return true;
            }
            else
            {
                path = default;
                return false;
            }
        }

        private TKey ClaimID(string id)
        {
            if (ExtEnum<TKey>.values.entries.Contains(id))
                throw new ArgumentException($"ID \"{id}\" from {typeof(TKey).FullName} is already in use!");

            return (TKey)Activator.CreateInstance(typeof(TKey), id, true);
        }

        private readonly struct Entry
        {
            public readonly string Path;
            public readonly TValue Value;

            public Entry(string path, TValue value)
            {
                Path = path;
                Value = value;
            }
        }
    }
}