using anomalydetectionapp;

class Program
{
    static void Main(string[] args)
    {
        CloudExp ce = new CloudExp();
        ce.Run(0.2, "training_files", "predicting_files");
    }
}
