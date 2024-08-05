using NeoCortexApi;

namespace anomalydetectionapp
{
    /// <summary>
    /// Class to hold the result of anomaly detection.
    /// </summary>
    public class AnomalyDetectionResult
    {
        public List<int>? AnomalyIndices { get; set; }
        public List<string>? AnomalyOutputString { get; set; }
    }

    /// <summary>
    /// Class which contains method for anomaly detection from a sequence of input values.
    /// </summary>
    public class AnomalyDetection
    {
        // Static variables to store total accuracy and list count
        public static double totalAccuracy { get; set; }
        public static double listCount { get; set; }

        /// <summary>
        /// Detects anomalies in a sequence of values using trained model predictor.
        /// </summary>
        /// <param name="predictor">The predictor model used for prediction.</param>
        /// <param name="list">The sequence of values to test for anomalies.</param>
        /// <param name="tolerance">The tolerance level ratio for anomaly detection.</param>
        /// <returns>An instance of AnomalyDetectionResult containing anomaly indices and output strings.</returns>
        public static AnomalyDetectionResult AnomalyDetectMethod(Predictor predictor, double[] list, double tolerance)
        {
            // Storing our output to list of strings
            List<string> anomalyOutputString = new List<string>();
            anomalyOutputString.AddRange(new List<string>
            {
                "------------------------------",
                "",
                "Testing the sequence for anomaly detection: " + string.Join(", ", list) + ".",
                ""
            });

            // Variable to store the accuracy of the HTM model while traversing the list
            double recordAccuracy = 0;

            List<int> anomalyIndices = new List<int>();

            // Loop through the list of input values
            for (int i = 0; i < list.Length; i++)
            {
                var item = list[i];

                // Passing the value to the HTM model for prediction
                var res = predictor.Predict(item);

                if (res.Count > 0)
                {
                    // Extracting the predicted value and accuracy from the HTM result
                    var value1 = res.First().PredictedInput.ToString().Split('-');
                    var value2 = res.First().Similarity;

                    if (i < list.Length - 1)
                    {
                        int nextIndex = i + 1;
                        double nextItem = list[nextIndex];
                        double predictedNextItem = double.Parse(value1.Last());

                        // Calculating the anomaly score and deviation
                        var AnomalyScore = Math.Abs(predictedNextItem - nextItem);
                        var deviation = AnomalyScore / nextItem;

                        // Checking if the deviation is within the tolerance level
                        if (deviation <= tolerance)
                        {
                            anomalyOutputString.Add("");
                            anomalyOutputString.Add("Anomaly not detected in the next value!! HTM model accuracy: " + value2 + "%.");
                            anomalyOutputString.Add("");
                            recordAccuracy += value2;
                        }
                        else
                        {
                            anomalyOutputString.Add("");
                            anomalyOutputString.Add($"Next value is an anomaly. HTM model predicts: {predictedNextItem} with accuracy: {value2}%. The actual value should be {nextItem}.");
                            anomalyOutputString.Add("");
                            i++;
                            anomalyOutputString.Add("");
                            anomalyOutputString.Add("Anomaly was detected, hence skipping to next value in list.");
                            anomalyOutputString.Add("");
                            recordAccuracy += value2;
                            anomalyIndices.Add(i);
                        }
                    }
                    else
                    {
                        anomalyOutputString.Add("");
                        anomalyOutputString.Add("The list is finished, so we are not going through further anomaly testing.");
                        anomalyOutputString.Add("");
                    }
                }
                else
                {
                    anomalyOutputString.Add("");
                    anomalyOutputString.Add("No prediction from HTM Model!!!");
                    anomalyOutputString.Add("");
                }
            }

            // Calculating the accuracy of the HTM model for each list
            double accuracyPerList = (recordAccuracy / list.Length);

            anomalyOutputString.AddRange(new List<string>
            {
                "-------------------------------------",
                "",
                "HTM engine accuracy for this list: " + accuracyPerList + "%.",
                "",
                "-------------------------------------"
            });

            // Storing the total accuracy and list count
            // This will be used to calculate the average accuracy of the HTM model
            // for all the lists in the experiment
            // This can be accessed from the main program to calculate the final accuracy
            totalAccuracy += accuracyPerList;
            listCount++;

            return new AnomalyDetectionResult
            {
                AnomalyIndices = anomalyIndices,
                AnomalyOutputString = anomalyOutputString
            };
        }
    }
}