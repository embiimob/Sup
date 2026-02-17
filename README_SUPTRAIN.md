# SupTrain - Decentralized AI Training Module

## Overview

SupTrain is a decentralized AI training coordination system integrated into Sup!?. It enables users to collaboratively train AI models using IPFS for data distribution and keyword-based coordination.

## Key Features

- **Decentralized Job Discovery**: Find training jobs via #keywords on the Sup!? network
- **IPFS Integration**: Download models and upload training deltas via IPFS
- **Local GPU Training**: Run training on your laptop GPU with LoRA adapters
- **Keyword Protocol**: Coordinated updates using structured hashtags
- **Aggregation Support**: Merge worker updates into new checkpoints
- **Monitor Feed**: Track job progress, updates, and announcements

## User Workflow

1. **Discover**: Search for training jobs using keywords like `#suptrain` or `#model:suplm`
2. **Configure**: Select job, add local training data, set training parameters
3. **Run**: Execute training locally, producing a LoRA delta and metrics
4. **Publish**: Upload delta and metrics to IPFS, announce via Sup!? message
5. **Monitor**: Track updates from other workers and latest checkpoints

## Keyword Protocol

All SupTrain messages use the base keyword `#suptrain` plus specific types:

### Job Genesis
```
#suptrain #jobgenesis #model:<slug> #job:<id> #cid:<jobCID> #manifest:<manifestCID> #checkpoint:<baseCkptCID>
```

### Worker Update
```
#suptrain #update #job:<id> #round:<n> #base:<baseCkptCID> #delta:<deltaCID> #metrics:<metricsCID> #from:<address>
```

### Aggregate Checkpoint
```
#suptrain #aggregate #job:<id> #round:<n> #checkpoint #cid:<newCkptCID> #inputs:<listCID> #metrics:<metricsCID>
```

## IPFS Data Formats

### job.json
```json
{
  "job_id": "job123",
  "model_slug": "suplm",
  "description": "Community language model",
  "license_hint": "MIT",
  "created_time": "2026-02-17T00:00:00Z",
  "genesis_checkpoint_cid": "Qm...",
  "manifest_cid": "Qm...",
  "eval_cid": "Qm...",
  "policy_cid": "Qm..."
}
```

### manifest.json
```json
{
  "model_slug": "suplm",
  "description": "Training recipe for suplm",
  "defaults": {
    "epochs": 1,
    "learning_rate": 0.0001,
    "batch_size": 1,
    "precision": "fp16",
    "output_type": "lora"
  },
  "tokenizer_cid": "Qm...",
  "config_cid": "Qm...",
  "allowed_data_sources": ["*.txt", "*.json"],
  "excluded_keywords_cid": "Qm..."
}
```

### metrics.json
```json
{
  "loss_curve": [2.5, 2.3, 2.1, ...],
  "eval_scores": {
    "accuracy": 0.85,
    "f1": 0.82
  },
  "total_steps": 100,
  "training_time": 3600.0,
  "gpu_info": "NVIDIA RTX 3080",
  "final_loss": 1.85,
  "final_perplexity": 6.36
}
```

## Python Training Worker

Located in `runtimes/suptrain/worker.py`:

```bash
python worker.py \
  --job-id job123 \
  --model-slug suplm \
  --round 1 \
  --base-checkpoint Qm... \
  --manifest manifest.json \
  --data /path/to/data \
  --epochs 1 \
  --lr 0.0001 \
  --batch-size 1 \
  --precision fp16 \
  --output-dir ./output
```

## Python Aggregator

Located in `runtimes/suptrain/aggregate.py`:

```bash
python aggregate.py \
  --job-id job123 \
  --round 1 \
  --base-checkpoint Qm... \
  --update-dirs ./update1,./update2,./update3 \
  --output-dir ./aggregated
```

## Safety and Abuse Controls

- **Policy Manifests**: Define allowed/denied keywords and addresses per job
- **Quality Thresholds**: Filter updates by loss metrics
- **Address Filtering**: Only accept updates from followed addresses
- **Delta Size Limits**: Reject oversized submissions
- **Reputation**: Prefer updates from trusted contributors

## Integration with Sup!?

SupTrain reuses existing Sup!? infrastructure:
- Keyword-based message discovery
- RPC transaction posting
- IPFS daemon integration via `IpfsHelper`
- Network toggle (testnet/mainnet)

## Current Implementation Status

✅ UI integration (toolbar button, form, tabs)
✅ Basic service layer (SupTrainService, SupTrainModels)
✅ Python worker and aggregator (simulation mode)
⚠️ TODO: Connect to actual Sup!? blockchain message search
⚠️ TODO: Implement real blockchain message posting
⚠️ TODO: Full PyTorch + PEFT training integration

## Future Enhancements

- Multi-aggregator consensus
- Reputation scoring for workers
- Automated checkpoint selection
- Real-time training progress streaming
- Support for full model fine-tuning (not just LoRA)
- Integration with existing Sup!? object system
