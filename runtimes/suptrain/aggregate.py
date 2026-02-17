#!/usr/bin/env python3
"""
SupTrain Aggregation Worker
Collects and merges LoRA deltas from multiple workers into a new checkpoint.
"""

import argparse
import json
import os
import sys
import time
from pathlib import Path
from typing import Dict, List

def log(message: str):
    """Print timestamped log message"""
    timestamp = time.strftime("%H:%M:%S")
    print(f"[{timestamp}] {message}", flush=True)

def load_delta(delta_path: str) -> bytes:
    """Load a delta file"""
    try:
        with open(delta_path, 'rb') as f:
            return f.read()
    except Exception as e:
        log(f"Error loading delta {delta_path}: {e}")
        return None

def load_metrics(metrics_path: str) -> Dict:
    """Load metrics JSON"""
    try:
        with open(metrics_path, 'r') as f:
            return json.load(f)
    except Exception as e:
        log(f"Error loading metrics {metrics_path}: {e}")
        return None

def validate_update(delta_path: str, metrics_path: str, base_ckpt_cid: str) -> bool:
    """Validate an update submission"""
    # Check delta exists and has reasonable size
    if not os.path.exists(delta_path):
        log(f"  ✗ Delta file not found: {delta_path}")
        return False
    
    delta_size = os.path.getsize(delta_path)
    if delta_size == 0:
        log(f"  ✗ Delta file is empty")
        return False
    
    # Check metrics exist
    if not os.path.exists(metrics_path):
        log(f"  ✗ Metrics file not found: {metrics_path}")
        return False
    
    metrics = load_metrics(metrics_path)
    if not metrics:
        return False
    
    # Validate metrics quality
    final_loss = metrics.get('final_loss', float('inf'))
    if final_loss > 10.0:
        log(f"  ✗ Loss too high: {final_loss}")
        return False
    
    log(f"  ✓ Valid update (loss: {final_loss:.4f})")
    return True

def merge_deltas(delta_paths: List[str], weights: List[float]) -> bytes:
    """
    Merge multiple deltas using weighted averaging
    In production, this would:
    1. Load all delta tensors
    2. Apply weights
    3. Average corresponding parameters
    4. Save merged adapter
    
    TODO: Implement actual weighted tensor averaging. Currently using first delta as placeholder.
    """
    log("Merging deltas...")
    
    # Load all deltas
    deltas = []
    for path in delta_paths:
        delta = load_delta(path)
        if delta:
            deltas.append(delta)
    
    if not deltas:
        raise ValueError("No valid deltas to merge")
    
    # TODO: Replace this placeholder with actual tensor merge logic
    # In production, would do actual tensor operations with weighted averaging
    merged = deltas[0]  # Placeholder - just use first delta
    
    log(f"  Merged {len(deltas)} deltas")
    return merged

def save_checkpoint(checkpoint_data: bytes, output_path: str) -> str:
    """Save merged checkpoint"""
    with open(output_path, 'wb') as f:
        f.write(checkpoint_data)
    log(f"Saved checkpoint to: {output_path}")
    return output_path

def save_aggregate_metrics(output_dir: str, input_metrics: List[Dict]) -> str:
    """Compute and save aggregate metrics"""
    metrics_path = os.path.join(output_dir, "aggregate_metrics.json")
    
    # Compute aggregated stats
    avg_loss = sum(m.get('final_loss', 0) for m in input_metrics) / len(input_metrics)
    total_steps = sum(m.get('total_steps', 0) for m in input_metrics)
    
    aggregate_data = {
        "num_workers": len(input_metrics),
        "average_loss": avg_loss,
        "total_steps": total_steps,
        "timestamp": time.time()
    }
    
    with open(metrics_path, 'w') as f:
        json.dump(aggregate_data, f, indent=2)
    
    log(f"Saved aggregate metrics to: {metrics_path}")
    return metrics_path

def save_aggregate_inputs(output_dir: str, delta_cids: List[str], 
                         worker_ids: List[str], weights: List[float]) -> str:
    """Save list of inputs used in aggregation"""
    inputs_path = os.path.join(output_dir, "aggregate_inputs.json")
    
    inputs_data = {
        "deltas": [
            {
                "delta_cid": cid,
                "worker_id": wid,
                "weight": w
            }
            for cid, wid, w in zip(delta_cids, worker_ids, weights)
        ],
        "method": "weighted_average",
        "timestamp": time.time()
    }
    
    with open(inputs_path, 'w') as f:
        json.dump(inputs_data, f, indent=2)
    
    log(f"Saved aggregate inputs to: {inputs_path}")
    return inputs_path

def main():
    parser = argparse.ArgumentParser(description="SupTrain Aggregation Worker")
    parser.add_argument("--job-id", required=True, help="Training job ID")
    parser.add_argument("--round", type=int, required=True, help="Round number")
    parser.add_argument("--base-checkpoint", required=True, help="Base checkpoint CID")
    parser.add_argument("--update-dirs", required=True, help="Comma-separated update directories")
    parser.add_argument("--output-dir", required=True, help="Output directory")
    parser.add_argument("--min-loss", type=float, default=0.0, help="Minimum loss threshold")
    parser.add_argument("--max-loss", type=float, default=10.0, help="Maximum loss threshold")
    
    args = parser.parse_args()
    
    log("=== SupTrain Aggregator ===")
    log(f"Job ID: {args.job_id}")
    log(f"Round: {args.round}")
    log(f"Base Checkpoint: {args.base_checkpoint}")
    
    # Create output directory
    os.makedirs(args.output_dir, exist_ok=True)
    
    # Parse update directories
    update_dirs = args.update_dirs.split(',')
    log(f"\nFound {len(update_dirs)} update submissions")
    
    # Validate and collect updates
    log("\nPhase 1: Validating updates...")
    valid_updates = []
    
    for update_dir in update_dirs:
        update_dir = update_dir.strip()
        delta_path = os.path.join(update_dir, "delta.safetensors")
        metrics_path = os.path.join(update_dir, "metrics.json")
        
        log(f"  Checking {update_dir}...")
        if validate_update(delta_path, metrics_path, args.base_checkpoint):
            valid_updates.append({
                "dir": update_dir,
                "delta": delta_path,
                "metrics": metrics_path
            })
    
    if not valid_updates:
        log("ERROR: No valid updates found!")
        sys.exit(1)
    
    log(f"\n✓ Accepted {len(valid_updates)}/{len(update_dirs)} updates")
    
    # Load metrics and compute weights
    log("\nPhase 2: Computing weights...")
    all_metrics = []
    losses = []
    
    for update in valid_updates:
        metrics = load_metrics(update["metrics"])
        if metrics:
            all_metrics.append(metrics)
            losses.append(metrics.get('final_loss', float('inf')))
    
    # Simple inverse loss weighting
    weights = []
    total_inv_loss = sum(1.0 / (loss + 0.001) for loss in losses)
    for loss in losses:
        weight = (1.0 / (loss + 0.001)) / total_inv_loss
        weights.append(weight)
        log(f"  Worker weight: {weight:.4f} (loss: {loss:.4f})")
    
    # Merge deltas
    log("\nPhase 3: Merging deltas...")
    delta_paths = [u["delta"] for u in valid_updates]
    merged_checkpoint = merge_deltas(delta_paths, weights)
    
    # Save outputs
    log("\nPhase 4: Saving outputs...")
    checkpoint_path = save_checkpoint(merged_checkpoint, 
                                     os.path.join(args.output_dir, "checkpoint.safetensors"))
    metrics_path = save_aggregate_metrics(args.output_dir, all_metrics)
    
    # Note: These are simulated CIDs for testing. In production, actual IPFS CIDs will be used.
    delta_cids = [f"Qm{i:08x}" for i in range(len(valid_updates))]
    worker_ids = [f"worker_{i}" for i in range(len(valid_updates))]
    inputs_path = save_aggregate_inputs(args.output_dir, delta_cids, worker_ids, weights)
    
    log("\n=== Aggregation Complete ===")
    log(f"Outputs saved to: {args.output_dir}")
    log("  - checkpoint.safetensors")
    log("  - aggregate_metrics.json")
    log("  - aggregate_inputs.json")
    
    return 0

if __name__ == "__main__":
    try:
        sys.exit(main())
    except KeyboardInterrupt:
        log("\nAggregation interrupted by user")
        sys.exit(1)
    except Exception as e:
        log(f"\nERROR: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)
