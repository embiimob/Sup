using System;
using System.Collections.Generic;

namespace SUP
{
    /// <summary>
    /// Data models for SupTrain decentralized AI training system
    /// </summary>

    #region Job Models

    public class SupTrainJob
    {
        public string JobId { get; set; }
        public string ModelSlug { get; set; }
        public string JobName { get; set; }
        public int Round { get; set; }
        public string BaseCheckpointCID { get; set; }
        public string ManifestCID { get; set; }
        public string EvalCID { get; set; }
        public string PolicyCID { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public string LicenseHint { get; set; }
    }

    public class JobManifest
    {
        public string ModelSlug { get; set; }
        public string Description { get; set; }
        public TrainingDefaults Defaults { get; set; }
        public string TokenizerCID { get; set; }
        public string ConfigCID { get; set; }
        public List<string> AllowedDataSources { get; set; }
        public string ExcludedKeywordsCID { get; set; }
        public AggregatorRules Aggregation { get; set; }
    }

    public class TrainingDefaults
    {
        public int Epochs { get; set; }
        public double LearningRate { get; set; }
        public int BatchSize { get; set; }
        public string Precision { get; set; }
        public string OutputType { get; set; } // "lora" or "diff"
    }

    public class AggregatorRules
    {
        public int MinUpdates { get; set; }
        public int MaxUpdates { get; set; }
        public double MinLoss { get; set; }
        public double MaxLoss { get; set; }
        public bool RequireFollowed { get; set; }
    }

    #endregion

    #region Update Models

    public class WorkerUpdate
    {
        public string JobId { get; set; }
        public int Round { get; set; }
        public string BaseCheckpointCID { get; set; }
        public string DeltaCID { get; set; }
        public string MetricsCID { get; set; }
        public string UpdateCID { get; set; }
        public string WorkerId { get; set; }
        public DateTime Timestamp { get; set; }
        public TrainingParams Params { get; set; }
        public List<string> DataHashes { get; set; }
    }

    public class TrainingParams
    {
        public int Epochs { get; set; }
        public double LearningRate { get; set; }
        public int BatchSize { get; set; }
        public string Precision { get; set; }
        public string OutputType { get; set; }
        public int Steps { get; set; }
    }

    public class TrainingMetrics
    {
        public List<double> LossCurve { get; set; }
        public Dictionary<string, double> EvalScores { get; set; }
        public int TotalSteps { get; set; }
        public double TrainingTime { get; set; }
        public string GpuInfo { get; set; }
        public double FinalLoss { get; set; }
        public double FinalPerplexity { get; set; }
    }

    #endregion

    #region Checkpoint Models

    public class AggregateCheckpoint
    {
        public string JobId { get; set; }
        public int Round { get; set; }
        public string CheckpointCID { get; set; }
        public string InputsCID { get; set; }
        public string MetricsCID { get; set; }
        public string Aggregator { get; set; }
        public DateTime Timestamp { get; set; }
        public int UpdateCount { get; set; }
    }

    public class AggregateInputs
    {
        public List<DeltaInput> Deltas { get; set; }
        public Dictionary<string, double> Weights { get; set; }
        public string Method { get; set; } // "average", "weighted", "fedavg"
    }

    public class DeltaInput
    {
        public string DeltaCID { get; set; }
        public string WorkerId { get; set; }
        public double Weight { get; set; }
        public double Loss { get; set; }
    }

    #endregion

    #region Policy Models

    public class JobPolicy
    {
        public string JobId { get; set; }
        public string AllowKeywordsCID { get; set; }
        public string DenyKeywordsCID { get; set; }
        public List<string> AllowedAddresses { get; set; }
        public List<string> DeniedAddresses { get; set; }
        public long MaxDeltaSize { get; set; }
        public double MinMetricQuality { get; set; }
    }

    #endregion

    #region Monitor Models

    public class MonitorEntry
    {
        public string Type { get; set; } // "checkpoint", "update", "aggregate", "policy"
        public string JobId { get; set; }
        public int Round { get; set; }
        public string Description { get; set; }
        public string CID { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
    }

    #endregion

    #region Keyword Protocol

    /// <summary>
    /// Keyword protocol constants for SupTrain messages
    /// </summary>
    public static class SupTrainKeywords
    {
        public const string Base = "#suptrain";
        public const string JobGenesis = "#jobgenesis";
        public const string Task = "#task";
        public const string Update = "#update";
        public const string Aggregate = "#aggregate";
        public const string Checkpoint = "#checkpoint";
        public const string Policy = "#policy";
        
        public static string Model(string slug) => $"#model:{slug}";
        public static string Job(string id) => $"#job:{id}";
        public static string Round(int n) => $"#round:{n}";
        public static string Shard(int k) => $"#shard:{k}";
        public static string CID(string cid) => $"#cid:{cid}";
        public static string Manifest(string cid) => $"#manifest:{cid}";
        public static string Data(string cid) => $"#data:{cid}";
        public static string BaseCheckpoint(string cid) => $"#base:{cid}";
        public static string Delta(string cid) => $"#delta:{cid}";
        public static string Metrics(string cid) => $"#metrics:{cid}";
        public static string From(string urn) => $"#from:{urn}";
        public static string Inputs(string cid) => $"#inputs:{cid}";
        public static string Allow(string cid) => $"#allow:{cid}";
        public static string Deny(string cid) => $"#deny:{cid}";
        public static string Keywords(string cid) => $"#keywords:{cid}";
    }

    #endregion
}
