# 🎨 DASHBOARD DE REPORTES - GUÍA COMPLETA

## ✨ ¿Qué se ha agregado?

Se ha creado un **Dashboard profesional y en tiempo real** que muestra:

1. **📊 Estadísticas del día completo**
   - Desayunos, Almuerzos, Cenas
   - Conteo de consumidos vs faltantes
   - Barras de progreso visuales

2. **📍 Lista de Faltantes**
   - Nombres específicos de quiénes no han consumido
   - Organizados por tipo de comida
   - Actualización en tiempo real

3. **🎯 Información Profesional**
   - Logo del restaurante en la parte superior
   - Fecha y hora actual
   - Botón de actualización
   - Timestamp de última actualización

---

## 📁 CARPETAS CREADAS

```
RestauranteApp/
  └── Resources/
      └── Images/
          ├── Logos/          ← 🎨 AQUÍ VAS A COLOCAR TU LOGO
          └── Icons/
```

**Ruta completa:**
```
C:\Users\jholm\RestauranteApp\RestauranteApp\Resources\Images\Logos\
```

---

## 🖼️ CÓMO AGREGAR TU LOGO

### Paso 1: Prepara la imagen

1. Tienes una imagen PNG del logo de tu restaurante
2. Redimensiona a aproximadamente **300 x 150 píxeles** (o proporcional)
3. Guarda en formato **PNG** (preferiblemente con fondo transparente)

### Paso 2: Copia el logo

1. **Navega a:** `RestauranteApp\Resources\Images\Logos\`
2. **Copia tu imagen PNG** aquí
3. **Ejemplo:** `logo-restaurante.png`

### Paso 3: Verifica el archivo

```powershell
# En PowerShell, verifica que el archivo está en el lugar correcto
Test-Path "C:\Users\jholm\RestauranteApp\RestauranteApp\Resources\Images\Logos\logo-restaurante.png"
# Debería retornar: True
```

### Paso 4: ¡Listo!

El Dashboard automáticamente mostrará tu logo. El archivo se llama internamente `logo-restaurante.png`, así que **asegúrate de que tu archivo tenga ese nombre exacto**.

**Si usaste otro nombre**, edita [DashboardWindow.xaml](../RestauranteApp/DashboardWindow.xaml) línea ~24:

```xml
<Image Source="Resources/Images/Logos/TU_NOMBRE_ARCHIVO.png" 
       Height="120" Width="240" Margin="0,0,0,10"
       HorizontalAlignment="Center"
       RenderOptions.BitmapScalingMode="HighQuality"/>
```

---

## 🚀 CÓMO ABRIR EL DASHBOARD

### Opción 1: Desde el botón de la UI (Recomendado)

1. Abre la aplicación principal
2. En la barra de botones superior, hace clic en **"📊 Reportes"**
3. Se abre la ventana del Dashboard

### Opción 2: Desde código (Desarrolladores)

```csharp
// En cualquier ventana
var dashboard = new DashboardWindow();
dashboard.Owner = this;
dashboard.ShowDialog();
```

---

## 📊 QUÉ MUESTRA EL DASHBOARD

### Sección Superior: Logo y Título

```
┌─────────────────────────────────────────┐
│                                         │
│           [   TU LOGO AQUÍ   ]         │
│                                         │
│      📊 ESTADÍSTICAS DEL DÍA           │
│      Lunes, 15 de Enero de 2024        │
│                                         │
└─────────────────────────────────────────┘
```

### Sección Media: Estadísticas de Comidas

Para cada tipo de comida (Desayuno, Almuerzo, Cena):

```
┌─────────────────────────────────────────┐
│  ☀️ DESAYUNO (7:00 - 9:30)             │
│                                         │
│  24 / 45 consumieron                    │
│  [████████░░░░░░░░░░░░░░░░░░░░░░░░]    │
│  53% - 21 faltantes                     │
│                                         │
└─────────────────────────────────────────┘
```

**Leyenda de colores:**
- 🟢 **Verde** (Desayuno)
- 🔵 **Azul** (Almuerzo)
- 🟣 **Púrpura** (Cena)

### Sección Inferior: Listado de Faltantes

```
┌─────────────────────────────────────────┐
│  📍 FALTANTES HOY                       │
│                                         │
│  ☀️ Desayuno (21 faltantes):           │
│     Ana García, Carlos López,           │
│     Elena Martínez... y 18 más          │
│                                         │
│  🌤️ Almuerzo (27 faltantes):          │
│     Pedro García, Gabriela Rodríguez... │
│                                         │
│  🌙 Cena (45 en espera):               │
│     Aún no inicia el horario            │
│                                         │
└─────────────────────────────────────────┘
```

### Botones de Control

- **🔄 Actualizar** - Recalcula las estadísticas en tiempo real
- **Cerrar** - Cierra la ventana

---

## 🔄 ACTUALIZACIONES EN TIEMPO REAL

El Dashboard se **actualiza automáticamente** cuando:

1. **Se abre la ventana** - Carga inicial de datos
2. **Se captura una huella** (consumo registrado) - Actualiza conteos
3. **Se hace clic en "Actualizar"** - Recalcula manualmente

Cada actualización:
- ✅ Recuenta todos los consumos del día
- ✅ Identifica quiénes fallaron
- ✅ Actua liza barras de progreso
- ✅ Muestra nombre específicos de faltantes

---

## 🎯 CARACTERÍSTICAS TÉCNICAS

### Comportamiento Automático

```
Al capturar huella en SuscripcionesWindow:
  ↓
ConsumoService registra el consumo
  ↓
ReportesService obtiene datos
  ↓
Dashboard se actualiza (si la ventana está abierta)
  ↓
Usuario ve cambios en tiempo real
```

### Datos que Muestra

- **Total con suscripción vigente:** Clientes aptos para consumir
- **Consumieron:** Clientes que ya registraron consumo (origen=1)
- **Avisos:** Clientes que justificaron ausencia
- **Faltantes:** Aquellos que no han consumido ni avisado

### Cálculos por Tipo de Comida

```
Total Apto = Clientes con suscripción activa + comida incluida en combo
Consumidos = COUNT(Consumos) WHERE Dia=hoy AND TipoComida=X AND Origen=1
Avisos = COUNT(Avisos) WHERE Dia=hoy AND TipoComida=X
Faltantes = Total Apto - Consumidos - Avisos
Porcentaje = (Consumidos / Total Apto) * 100
```

---

## 🖌️ PERSONALIZACIÓN DEL TAMAÑO DEL LOGO

Si quieres ajustar el tamaño del logo, edita [DashboardWindow.xaml](../RestauranteApp/DashboardWindow.xaml) línea ~24:

```xml
<Image Source="Resources/Images/Logos/logo-restaurante.png" 
       Height="120"    ← Altura en píxeles (aumenta para logo más grande)
       Width="240"     ← Ancho en píxeles
       Margin="0,0,0,10"/>
```

### Ejemplos de Tamaños

| Tamaño | Height | Width |
|--------|--------|-------|
| Pequeño (compacto) | 80 | 160 |
| Mediano (recomendado) | 120 | 240 |
| Grande (destacado) | 160 | 320 |
| Extra Grande | 200 | 400 |

---

## 🐛 TROUBLESHOOTING

### Problema: "El logo no aparece"

**Causa:** El archivo no existe o tiene otro nombre

**Soluciones:**

1. Verifica que el archivo existe:
```powershell
Get-ChildItem "C:\Users\jholm\RestauranteApp\RestauranteApp\Resources\Images\Logos\"
```

2. Asegúrate de que el nombre coincida exactamente con `logo-restaurante.png`

3. Si usaste otro nombre, edita `DashboardWindow.xaml` línea 24 con el nombre correcto

### Problema: "Error al abrir Dashboard"

**Causa:** Probablemente falta ReportesService

**Solución:** Verifica que el servicio está registrado en App.xaml.cs:

```csharp
services.AddScoped<ReportesService>();  // Debe estar presente
```

### Problema: "El conteo está mal"

**Causa:** Probablemente la base de datos está inconsistente

**Solución:** 

1. Cierra la aplicación
2. Borra la base de datos (restaurante.db) en `AppData\Local\RestauranteApp\`
3. Reabre la aplicación (se recreará la BD)
4. Los conteos deberán ser correctos ahora

---

## 📋 ARCHIVOS MODIFICADOS O CREADOS

### Nuevos Archivos:
- ✅ `RestauranteApp\DashboardWindow.xaml` - Interfaz visual
- ✅ `RestauranteApp\DashboardWindow.xaml.cs` - Lógica del dashboard
- ✅ `Resources/Images/Logos/` - Carpeta para logos

### Archivos Modificados:
- ✅ `MainWindow.xaml` - Agregado botón "📊 Reportes"
- ✅ `MainWindow.xaml.cs` - Agregado evento Reportes_Click()

### Servicio Existente Usado:
- ✅ `ReportesService.cs` - (creado anteriormente, reutilizado aquí)

---

## ✅ CHECKLIST FINAL

- [ ] Creaste la carpeta `Resources/Images/Logos/`
- [ ] Copiar imagen PNG a esa carpeta
- [ ] Verificaste el nombre del archivo (debe ser `logo-restaurante.png`)
- [ ] Compilaste la aplicación (`dotnet build -c Release`)
- [ ] Abriste la app y viste el nuevo botón "📊 Reportes"
- [ ] Hiciste clic en "📊 Reportes" y se abrió el Dashboard
- [ ] Viste tu logo en la parte superior
- [ ] Viste las estadísticas (desayuno/almuerzo/cena)
- [ ] Viste los nombres de faltantes

---

## 📞 SOPORTE

Si hay problemas:

1. Verifica la consola de VS Code para mensajes de error
2. Revisa el archivo de log en `AppData\Local\RestauranteApp\`
3. Asegúrate de que ReportesService está disponible
4. Verifica que la BD está actualizada después de capturar una huella

---

## 🎉 ¡LISTO!

Tu Dashboard profesional está configurado y funcionando. El logo se mostrará automáticamente en el encabezado, y los conteos se actualizarán en tiempo real mientras los clientes consumen.

**El sistema ahora muestra:**
- ✅ Cuántos han consumido (En tiempo real)
- ✅ Cuántos faltan (Con nombres)
- ✅ Progreso visual (Barras de progreso)
- ✅ Logo profesional (Tu marca)
- ✅ Updated timestamp (Sabe cuándo fue la última actualización)

