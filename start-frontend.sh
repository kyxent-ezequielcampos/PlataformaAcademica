#!/bin/bash

echo "🎨 Iniciando Frontend del Sistema Académico..."
echo ""

cd frontend

echo "📦 Restaurando dependencias..."
dotnet restore

echo ""
echo "🔨 Compilando proyecto..."
dotnet build

echo ""
echo "▶️  Ejecutando frontend..."
echo ""
dotnet run
