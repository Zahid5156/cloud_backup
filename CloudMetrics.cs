namespace anomalydetectionapp
{
    /// <summary>
    /// This class is using for storing runtime metrics for our cloud experiment.
    /// </summary>
    public static class CloudMetrics
    {
        public static string? InputTrainingFolderPath { get; set; }

        public static string? InputPredictingFolderPath { get; set; }

        public static string? ExperimentOutputPath { get; set; }
        
        public static double TrainingTimeInSeconds { get; set; }
        
        public static double totalAvgAccuracy { get; set; }
    }
}