using System;
using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// Generic JSON converter that works with any class.
/// </summary>
public static class GenericJsonConverter
{
    /// <summary>
    /// Generic object to JSON (works with any object)
    /// </summary>
    /// <param name="obj">The object to convert to JSON</param>
    /// <returns>JSON string representation of the object</returns>
    public static string ObjectToJson(object obj)
    {
        string json = JsonUtility.ToJson(obj, true);
        
        // Fix Unity's JsonUtility behavior where null strings are serialized as empty strings
        // This regex replaces empty string values with null for string fields
        json = FixNullStringSerialization(json);
        
        Debug.Log($"ObjectToJson result: {json}");
        return json;
    }
    
    /// <summary>
    /// Fixes Unity's JsonUtility behavior where null strings are serialized as empty strings.
    /// This method converts empty string values back to null for proper JSON serialization.
    /// </summary>
    /// <param name="json">The JSON string to fix</param>
    /// <returns>Fixed JSON string with proper null values</returns>
    private static string FixNullStringSerialization(string json)
    {
        // Pattern to match empty string values in JSON: "fieldName": ""
        // This will be replaced with: "fieldName": null
        string pattern = @":\s*""""";
        string replacement = @": null";
        
        return Regex.Replace(json, pattern, replacement);
    }
    
    /// <summary>
    /// Converse operation: JSON to Object (works with any class)
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="json">The JSON string to deserialize</param>
    /// <returns>The deserialized object of type T</returns>
    /// <exception cref="InvalidOperationException">Thrown when deserialization fails</exception>
    public static T JsonToObject<T>(string json)
    {
        try
        {
            return JsonUtility.FromJson<T>(json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to deserialize JSON: {ex.Message}", ex);
        }
    }
} 