#!/bin/bash
# Build script for Sup.Modern - Cross-Platform

set -e

echo "=========================================="
echo "  Sup!? Modern - Build Script"
echo "=========================================="
echo ""

PROJECT_DIR="Sup.Modern/Sup.Desktop"
OUTPUT_DIR="dist"
VERSION="1.0.0"

# Clean previous builds
echo "🧹 Cleaning previous builds..."
rm -rf $OUTPUT_DIR
mkdir -p $OUTPUT_DIR

# Function to build for a specific runtime
build_runtime() {
    local runtime=$1
    local platform=$2
    
    echo ""
    echo "🔨 Building for $platform ($runtime)..."
    
    dotnet publish $PROJECT_DIR \
        -c Release \
        -r $runtime \
        --self-contained true \
        -p:PublishSingleFile=true \
        -p:Version=$VERSION \
        -o $OUTPUT_DIR/$platform
        
    echo "✅ Build complete for $platform"
}

# Build for all platforms
echo ""
echo "Building for all platforms..."
echo ""

build_runtime "win-x64" "windows-x64"
build_runtime "linux-x64" "linux-x64"
build_runtime "osx-x64" "macos-x64"
build_runtime "osx-arm64" "macos-arm64"

echo ""
echo "=========================================="
echo "✨ All builds completed successfully!"
echo "=========================================="
echo ""
echo "Build outputs:"
echo "  Windows (x64): $OUTPUT_DIR/windows-x64/Sup.exe"
echo "  Linux (x64):   $OUTPUT_DIR/linux-x64/Sup"
echo "  macOS (x64):   $OUTPUT_DIR/macos-x64/Sup"
echo "  macOS (ARM64): $OUTPUT_DIR/macos-arm64/Sup"
echo ""
echo "To create zip archives, run:"
echo "  ./scripts/package.sh"
echo ""
