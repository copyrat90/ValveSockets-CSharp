#!/bin/sh

set -e

# Install ClangSharpPInvokeGenerator

dotnet tool list -g | grep "ClangSharpPInvokeGenerator" > /dev/null
if [ $? -eq 1 ]; then
  dotnet tool install --global ClangSharpPInvokeGenerator
fi

# Build clang

if [ ! -d "./llvm-project" ]; then
  git clone --single-branch --branch llvmorg-15.0.0 https://github.com/llvm/llvm-project
  cd "llvm-project"
  mkdir -p "artifacts/bin"
  cd "artifacts/bin"
  cmake -DLLVM_ENABLE_PROJECTS=clang -DCMAKE_INSTALL_PREFIX=../install -DCMAKE_BUILD_TYPE=Release -G "Unix Makefiles" ../../llvm
  make -j$(nproc)
  make install
  cd ../../../
fi

# Build libClangSharp

if [ ! -d "./clangsharp" ]; then
  git clone https://github.com/dotnet/clangsharp
  cd "clangsharp"
  mkdir -p artifacts/bin/native
  cd "artifacts/bin/native"
  cmake -DPATH_TO_LLVM=$(pwd)/../../../../llvm-project/artifacts/install ../../..
  make -j$(nproc)
  cd ../../../../
fi

# Clone GameNetworkingSockets if not present

if [ ! -d "./GameNetworkingSockets" ]; then
  git clone https://github.com/ValveSoftware/GameNetworkingSockets
fi

# Remove output directory if preset and recreate it

if [ -d "../Valve.Sockets" ]; then
  rm -r "../Valve.Sockets"
fi

mkdir "../Valve.Sockets"

# Run the generator

LD_LIBRARY_PATH=$(pwd)/clangsharp/artifacts/bin/native/lib:$(pwd)/llvm-project/artifacts/install/lib ClangSharpPInvokeGenerator @generate.rsp
