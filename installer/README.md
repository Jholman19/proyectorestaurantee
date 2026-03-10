# 📦 INSTALADOR DE MI RESTAURANTE

## ✅ INSTALADOR GENERADO EXITOSAMENTE

**Archivo:** `MiRestaurante_Setup_v1.0.0.exe`  
**Ubicación:** `Output\MiRestaurante_Setup_v1.0.0.exe`  
**Tamaño:** 53.73 MB  
**Fecha:** 5 de marzo de 2026

---

## 🚀 USO RÁPIDO

### Para instalar en esta PC:
1. Ir a carpeta `Output`
2. Doble clic en `MiRestaurante_Setup_v1.0.0.exe`
3. Seguir el asistente

### Para instalar en otra PC:
1. Copiar `Output\MiRestaurante_Setup_v1.0.0.exe` a USB o compartir por red
2. Llevar a la otra PC
3. Ejecutar como administrador
4. Seguir el asistente

---

## 📁 CONTENIDO DE ESTA CARPETA

```
installer/
├── Output/
│   └── MiRestaurante_Setup_v1.0.0.exe ← INSTALADOR FINAL
├── RestauranteApp_Simple.iss          ← Script usado (simplificado)
├── RestauranteApp.iss                 ← Script completo (backup)
└── README.md                          ← Este archivo
```

---

## 🔄 RECOMPILAR INSTALADOR

Si necesitas recompilar el instalador después de hacer cambios:

### 1. Publicar aplicación actualizada:
```powershell
cd C:\Users\jholm\RestauranteApp
dotnet publish RestauranteApp\RestauranteApp.csproj -c Release -r win-x64 --self-contained true -p:PublishReadyToRun=true
```

### 2. Compilar instalador:
```powershell
cd C:\Users\jholm\RestauranteApp\installer
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" RestauranteApp_Simple.iss
```

### 3. Nuevo instalador estará en:
```
Output\MiRestaurante_Setup_v1.0.0.exe
```

---

## 📝 MODIFICAR INSTALADOR

Para cambiar configuración del instalador, editar:
- `RestauranteApp_Simple.iss` - Script simplificado (recomendado)
- `RestauranteApp.iss` - Script completo con más opciones

**Herramienta:** Inno Setup 6.7.1 o superior  
**Descargar:** https://jrsoftware.org/isdl.php

---

## ✅ CARACTERÍSTICAS INCLUIDAS

- ✅ Instalador completo self-contained
- ✅ No requiere instalar .NET
- ✅ Incluye todas las DLLs necesarias
- ✅ Documentación integrada
- ✅ Creación automática de base de datos
- ✅ Accesos directos en escritorio y menú inicio
- ✅ Desinstalación limpia
- ✅ Compatible con Windows 10/11 (64-bit)

---

## 📋 REQUISITOS PARA RECOMPILAR

- ✅ Inno Setup 6.7.1+
- ✅ .NET 8.0 SDK
- ✅ Windows 10/11
- ✅ Espacio libre: ~500 MB

---

✅ **Listo para distribuir**
🎉 **¡Todo compilado correctamente!**
