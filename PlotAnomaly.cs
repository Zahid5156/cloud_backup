using XPlot.Plotly;

namespace anomalydetectionapp
{
    public class AnomalyPlotter
    {
        #pragma warning disable // Disable all warnings
        public static void PlotGraphWithAnomalies(List<double[]> allData, List<List<int>> allAnomalyIndices, string outputFilePath)
        {
            List<Scatter> allGraphs = new List<Scatter>();
            List<Scatter> allAnomalies = new List<Scatter>();

            for (int i = 0; i < allData.Count; i++)
            {
                double[] data = allData[i];
                List<int> anomalyIndices = allAnomalyIndices[i];

                var graph = new Scatter
                {
                    x = Enumerable.Range(0, data.Length).ToArray(),
                    y = data,
                    mode = "lines",
                    name = "Sequence" + i
                };

                var anomalies = new Scatter
                {
                    x = anomalyIndices.ToArray(),
                    y = anomalyIndices.Select(index => data[index]).ToArray(),
                    mode = "markers",
                    name = "Anomalies in sequence" + i,
                    marker = new Marker { color = "red" }
                };

                allGraphs.Add(graph);
                allAnomalies.Add(anomalies);
            }

            var chart = Chart.Plot(allGraphs.Concat(allAnomalies));
            chart.WithTitle("Graph with Anomalies");
            chart.WithXTitle("X-axis(Index of data point)");
            chart.WithYTitle("Y-axis(Value of data point)");

            // Save the plot to an HTML file
            File.WriteAllText(outputFilePath, chart.GetHtml());
        }
    }
}