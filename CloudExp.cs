using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace anomalydetectionapp
{
    /// <summary>
    /// Class containing methods to read sequences from JSON files, trim input sequences, train a model, and detect anomalies.
    /// </summary>
    class CloudExp
    {
        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="tValue">Threshold value for anomaly detection.</param>
        /// <param name="trainingfolderPath">Path to the folder containing training data.</param>
        /// <param name="predictingfolderPath">Path to the folder containing predicting data.</param>
        public void Run(double tValue = 0.1, string trainingfolderPath = "training", string predictingfolderPath = "predicting")
        {
            #pragma warning disable // Disable all warnings

            // Set the input folder paths for training and predicting data
            CloudMetrics.InputTrainingFolderPath = trainingfolderPath;
            CloudMetrics.InputPredictingFolderPath = predictingfolderPath;

            // Using stopwatch to calculate the total training time
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Generate a timestamp for the output folder name
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputFolder = $"anomaly_experiment_{timestamp}";

            // Set the output folder path
            //string outputFolderPath = Path.Combine("", outputFolder);
            string outputFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputFolder);

            // Ensure the output directory exists
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            // Set the full file paths
            string outputFilePath = Path.Combine(outputFolderPath, $"anomaly_experiment_output_{timestamp}.txt");
            string imageFilePath = Path.Combine(outputFolderPath, $"anomaly_plot_{timestamp}.html");

            CloudMetrics.ExperimentOutputPath = outputFilePath;

            List<List<String>> completeOutputTextString = new List<List<String>>();
            List<String> welcomeList = new List<String>();

            // Add strings to the welcome list in one line
            welcomeList.AddRange(new List<string>
            {
                "",
                "*********************************",
                "",
                "Hello! Beginning our anomaly detection experiment.",
                "",
                "*********************************"
            });

            completeOutputTextString.Add(welcomeList);

            // Create a dictionary to store sequences
            Dictionary<string, List<double>> mysequences = new Dictionary<string, List<double>>();

            // Read sequences from training and predicting folders
            var trainingjsonReader = new JsonFolderReader(trainingfolderPath);
            var predictingjsonreader = new JsonFolderReader(predictingfolderPath);

            var trainingSequenceData = trainingjsonReader.AllSequences;
            var predictingSequencesData = predictingjsonreader.AllSequences;

            int sequenceIndex = 1;

            // Process sequences from the training folder
            foreach (var sequencesData in trainingSequenceData)
            {
                var sequences = sequencesData.Sequences;

                foreach (var sequence in sequences)
                {
                    List<double> convertedSequence = sequence.Select(x => (double)x).ToList();

                    string sequenceKey = "S" + sequenceIndex;
                    mysequences.Add(sequenceKey, convertedSequence);
                    sequenceIndex++;
                }
            }

            // Train the model using MultiSequenceLearning
            MultiSequenceLearning myexperiment = new MultiSequenceLearning();
            var predictor = myexperiment.Run(mysequences);
            predictor.Reset();

            // Create lists to store all data and anomaly indices
            List<double[]> allData = new List<double[]>();
            List<List<int>> allAnomalyIndices = new List<List<int>>();

            // Detect anomalies in sequences from the predicting folder
            foreach (var sequencesData in predictingSequencesData)
            {
                var sequences = sequencesData.Sequences;

                foreach (var sequence in sequences)
                {
                    // Convert the sequence to a list of double values
                    List<double> inputlist = sequence.Select(x => (double)x).ToList();
                    double[] inputArray = inputlist.ToArray();

                    // Trim some values randomly in the beginning of the sequence
                    Random random = new Random();
                    int trimCount = random.Next(1, 4);
                    double[] inputTestArray = inputArray.Skip(trimCount).ToArray();

                    // Get the anomaly detection result from the AnomalyDetection class
                    AnomalyDetectionResult result = AnomalyDetection.AnomalyDetectMethod(predictor, inputTestArray, tValue);

                    // Add the data and anomaly indices to the lists
                    allData.Add(inputTestArray);
                    allAnomalyIndices.Add(result.AnomalyIndices);

                    // Collect the output strings
                    completeOutputTextString.Add(result.AnomalyOutputString);
                }
            }

            // Plot all the data with anomalies and save the plot to an HTML file
            AnomalyPlotter.PlotGraphWithAnomalies(allData, allAnomalyIndices, imageFilePath);

            // Calculate the final experiment accuracy
            double finalExpAccuracy = AnomalyDetection.totalAccuracy / AnomalyDetection.listCount;

            CloudMetrics.totalAvgAccuracy = finalExpAccuracy;

            List<String> endList = new List<String>();

            // Add strings to the welcome list in one line
            endList.AddRange(new List<string>
            {
                "",
                "*********************************",
                "",
                "Final experiment accuracy: " + finalExpAccuracy + "%.",
                "",
                "*********************************"
            });

            completeOutputTextString.Add(endList);

            stopwatch.Stop();
            CloudMetrics.TrainingTimeInSeconds = stopwatch.Elapsed.TotalSeconds;

            // Initialize a StringBuilder to efficiently build a multi-line string
            StringBuilder mySB = new StringBuilder();

            // Iterate over each inner list of strings
            foreach (List<string> innerLine in completeOutputTextString)
            {
                foreach (string line in innerLine)
                {
                    // Append the current line to the StringBuilder
                    mySB.AppendLine(line);
                }
            }

            // Write the content of the StringBuilder to output text file
            File.WriteAllText(outputFilePath, mySB.ToString());
        }
    }
}