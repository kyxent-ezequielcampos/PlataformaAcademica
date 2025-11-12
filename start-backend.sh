#!/bin/bash

echo "🚀 Iniciando Backend del Sistema Académico..."
echo ""

cd backend

echo "📦 Restaurando dependencias..."
dotnet restore

echo ""
echo "🔨 Compilando proyecto..."
dotnet build

echo ""
echo "▶️  Ejecutando backend en http://localhost:5130..."
echo ""
dotnet run
