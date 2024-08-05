using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Represents a container for a list of sequences.
/// </summary>
public class SequencesContainer
{
    #pragma warning disable // Disable all warnings

    /// <summary>
    /// Gets or sets the list of sequences.
    /// </summary>
    public List<List<int>> Sequences { get; set; }
}

/// <summary>
/// Reads JSON files containing sequences from a specified folder.
/// </summary>
public class JsonFolderReader
{

    #pragma warning disable // Disable all warnings

    /// <summary>
    /// Gets the list of all sequences read from JSON files.
    /// </summary>
    public List<SequencesContainer> AllSequences { get; private set; }

    /// <summary>
    /// Initializes a new instance of the JsonFolderReader class with the specified folder path.
    /// </summary>
    /// <param name="folderPath">The path to the folder containing JSON files.</param>
    public JsonFolderReader(string folderPath)
    {
        // Initialize the list of all sequences
        AllSequences = new List<SequencesContainer>();

        // Get all JSON files in the specified folder
        string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");

        // Iterate through each JSON file
        foreach (var filePath in jsonFiles)
        {
            // Read the contents of the JSON file
            string jsonContent = File.ReadAllText(filePath);

            // Deserialize the JSON content into a SequencesContainer object
            SequencesContainer sequencesContainer = JsonConvert.DeserializeObject<SequencesContainer>(jsonContent);

            // Add the deserialized SequencesContainer object to the list of all sequences
            AllSequences.Add(sequencesContainer);
        }
    }
}
