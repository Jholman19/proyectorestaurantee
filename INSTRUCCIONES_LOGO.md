# 🎨 Instrucciones: Cómo Agregar el Logo del Restaurante

## 📂 Ubicación de la Carpeta

El logo debe colocarse en:
```
RestauranteApp/
  └── Resources/
      └── Images/
          └── Logos/
              └── [TU_LOGO_AQUI]
```

**Ruta completa:**
```
c:\Users\jholm\RestauranteApp\RestauranteApp\Resources\Images\Logos\
```

---

## 🖼️ Formato y Especificaciones Recomendadas

### Tipos de archivo aceptados:
- ✅ **PNG** (recomendado - fondo transparente)
- ✅ **JPG/JPEG**
- ✅ **BMP**
- ✅ **TIFF**

### Dimensiones recomendadas:
- **Ancho:** 200-400 píxeles
- **Alto:** 100-200 píxeles
- **Proporción:** 2:1 o cuadrado

### Ejemplos de nombres válidos:
- `logo-restaurante.png`
- `Logo_Restaurant.png`
- `mi-restaurante-logo.png`
- `LogoApp.png`

---

## 🔧 Pasos para Agregar tu Logo

### Paso 1: Prepara tu imagen
1. Tienes una imagen del logo del restaurante
2. Redimensiona a ~300x150 píxeles (cualquier tamaño dentro del rango)
3. Guarda en formato PNG (preferiblemente con fondo transparente)

### Paso 2: Copia el archivo
1. Navega a: `RestauranteApp/Resources/Images/Logos/`
2. Coloca tu archivo PNG aquí
3. Por ejemplo: `RestauranteApp/Resources/Images/Logos/logo-restaurante.png`

### Paso 3: Actualiza las referencias en el código

En el archivo **RestauranteApp/DashboardWindow.xaml**:

Busca esta línea (aproximadamente línea 20):
```xml
<Image Source="Resources/Images/Logos/logo-restaurante.png" 
       Height="120" Width="240" Margin="0,0,0,10"/>
```

Y reemplaza `logo-restaurante.png` con el nombre de tu archivo:
```xml
<Image Source="Resources/Images/Logos/TU_NOMBRE_ARCHIVO.png" 
       Height="120" Width="240" Margin="0,0,0,10"/>
```

### Paso 4: Recompila
```powershell
cd c:\Users\jholm\RestauranteApp
dotnet build -c Release
```

---

## 🎬 Dónde Aparece el Logo

El logo aparece en la **Ventana de Dashboard de Reportes**:
- En la parte superior
- Centrado horizontalmente
- Bajo el logo aparece el título "📊 ESTADÍSTICAS DEL DÍA"

### Para abrir el Dashboard:
1. En la ventana principal (MainWindow)
2. Haz clic en el botón "📊 Reportes" (si existe)
3. O desde el menú: Ver → Reportes

---

## 📏 Personalización del Tamaño del Logo

Si quieres cambiar el tamaño del logo, edita estas propiedades en **DashboardWindow.xaml**:

```xml
<!-- Búsca línea similar a esta -->
<Image Source="Resources/Images/Logos/logo-restaurante.png" 
       Height="120"    <!-- ← Altura en píxeles -->
       Width="240"     <!-- ← Ancho en píxeles -->
       Margin="0,0,0,10"/>
```

### Ejemplos de tamaños:

| Efecto | Height | Width |
|--------|--------|-------|
| Pequeño (compacto) | 80 | 160 |
| Medio (recomendado) | 120 | 240 |
| Grande (destacado) | 160 | 320 |

---

## ✅ Checklist

- [ ] Tengo la imagen del logo en PNG
- [ ] La imagen está redimensionada a ~300x150 píxeles
- [ ] Copié el archivo a `RestauranteApp/Resources/Images/Logos/`
- [ ] Actualicé el nombre en `DashboardWindow.xaml` linea ~20
- [ ] Compilé con `dotnet build -c Release`
- [ ] Veo el logo en el Dashboard al abrir la ventana

---

## 🐛 Troubleshooting

### Problema: "El logo no aparece"
**Solución 1:** Verifica la ruta
```powershell
Test-Path "c:\Users\jholm\RestauranteApp\RestauranteApp\Resources\Images\Logos\logo-restaurante.png"
# Debería retornar: True
```

**Solución 2:** Verifica el nombre en XAML
```xml
<!-- Debe coincidir exactamente con el archivo -->
<Image Source="Resources/Images/Logos/logo-restaurante.png"/>
```

**Solución 3:** Limpia la compilación
```powershell
cd c:\Users\jholm\RestauranteApp
dotnet clean
dotnet build -c Release
```

### Problema: "Error de compilación - archivo no encontrado"
**Solución:** El archivo .csproj necesita incluir los recursos
```xml
<!-- En RestauranteApp.csproj, dentro de <ItemGroup> -->
<Content Include="Resources/Images/Logos/**" CopyToOutputDirectory="PreserveNewest" />
```

---

## 📱 Cómo se ve el Logo en la Interfaz

```
╔════════════════════════════════════════════════════════════╗
║                                                            ║
║                  [LOGO AQUI - 240x120]                    ║
║                                                            ║
║              📊 ESTADÍSTICAS DEL DÍA                       ║
║              Lunes, 15 de Enero de 2024                    ║
║                                                            ║
╠════════════════════════════════════════════════════════════╣
║                                                            ║
║  ☀️  DESAYUNO (7:00 - 9:30)                              ║
║     24 / 45 consumieron    |████████░░░░░░░░░░░░░░|       ║
║     Faltantes: 21                                         ║
║                                                            ║
║  🌤️  ALMUERZO (11:00 - 16:00)                            ║
║     18 / 45 consumieron    |██████░░░░░░░░░░░░░░░░░░░|   ║
║     Faltantes: 27                                         ║
║                                                            ║
║  🌙 CENA (18:00 - 21:00)                                 ║
║     0 / 45 - Sin iniciar                                  ║
║     Pendiente: 45                                         ║
║                                                            ║
║  ┌─────────────────────────────────────────────────────┐  ║
║  │ 📍 ÚLTIMAS PERSONAS QUE CONSUMIERON:               │  ║
║  │   • Juan García - Desayuno (09:15)                │  ║
║  │   • María López - Almuerzo (12:45)                │  ║
║  │   • Pedro García - Desayuno (09:22)               │  ║
║  └─────────────────────────────────────────────────────┘  ║
║                                                            ║
║                [Actualizar] [Cerrar]                     ║
║                                                            ║
╚════════════════════════════════════════════════════════════╝
```

---

## 🎓 Nota Final

- La carpeta ya está creada: `Resources/Images/Logos/`
- Solo debes **copiar tu archivo PNG** ahí
- Y actualizar el nombre en `DashboardWindow.xaml` (línea ~20)
- ¡Eso es todo! El logo aparecerá automáticamente en el Dashboard

