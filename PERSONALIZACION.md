# 🎨 Guía de Personalización del Diseño

## Cómo Cambiar los Colores

Todos los colores están definidos en `Resources/Styles.xaml`. Para cambiar los colores principales:

### 1. Color Principal (Verde)
```xaml
<SolidColorBrush x:Key="PrimaryColor" Color="#FF2E7D32"/>
<SolidColorBrush x:Key="PrimaryLight" Color="#FF43A047"/>
```

**Cambiar a otros colores:**
- Azul profesional: `#FF1976D2` y `#FF2196F3`
- Rojo empresarial: `#FFC62828` y `#FFE53935`
- Púrpura moderno: `#FF6A1B9A` y `#FF8E24AA`

### 2. Color Secundario (Naranja)
```xaml
<SolidColorBrush x:Key="AccentColor" Color="#FFF57C00"/>
```

**Cambiar a otros acentos:**
- Teal: `#FF00796B`
- Rosado: `#FFC2185B`
- Limón: `#FSFBC02D`

---

## Cómo Cambiar las Marquillas

En `MainWindow.xaml` línea 26-27:

```xaml
<TextBlock Text="Software: Mi Restaurante" .../>
<TextBlock Text="Restaurante: Mi Amigo el Pizzero" .../>
```

Simplemente reemplaza el texto con el nombre de tu negocio.

---

## Cómo Cambiar el Logo

Actualmente hay un círculo blanco en la esquina. Para cambiar:

### Opción 1: Cambiar el icono emoji
En `MainWindow.xaml` línea 21:
```xaml
<TextBlock Text="🍕 Mi Restaurante" .../>
```

Reemplaza 🍕 con otro emoji. Opciones populares:
- 🍔 Hamburguesa
- 🍜 Fideos
- 🥘 Comida Española
- 🍱 Bandejas
- 🥗 Ensalada
- 🍲 Sopa

### Opción 2: Usar una imagen
Reemplaza el Ellipse con una Image:

```xaml
<Image Source="logo.png" Width="60" Height="60" Stretch="UniformToFill"/>
```

(Coloca la imagen en la carpeta del proyecto)

---

## Cómo Crear Nuevos Estilos

Si quieres crear un estilo personalizado, agrega a `Resources/Styles.xaml`:

### Estilo de Botón Personalizado
```xaml
<Style x:Key="MiBotonEspecial" TargetType="Button" BasedOn="{StaticResource ModernButton}">
    <Setter Property="Background" Value="#FFFF6F00"/>
    <Setter Property="FontSize" Value="16"/>
</Style>
```

Usa en XAML:
```xaml
<Button Content="Mi Botón" Style="{StaticResource MiBotonEspecial}"/>
```

---

## Cómo Ajustar Tamaños

### Cambiar tamaño de MainWindow
En `MainWindow.xaml`:
```xaml
Height="720" Width="1200"
```

### Cambiar altura de botones
En `Styles.xaml`, estilo `ModernButton`:
```xaml
<Setter Property="MinHeight" Value="40"/>
<Setter Property="Padding" Value="16,10"/>
```

### Cambiar tamaño de fuentes
En `Styles.xaml`:
```xaml
<Setter Property="FontSize" Value="14"/>
```

---

## Cómo Cambiar los Emojis

Toda la aplicación usa emojis. Para cambiar o agregar más:

### En Headers
`MainWindow.xaml`, `ClienteForm.xaml`, etc.
```xaml
<TextBlock Text="🍕 Título aquí"/>
```

### En Botones
```xaml
<Button Content="🔄 Actualizar"/>
```

### En Labels
```xaml
<TextBlock Text="👤 Nombre:"/>
```

**Emojis recomendados por categoría:**

**Acciones:**
- 📋 Cargar
- ➕ Agregar
- ✏️ Editar
- 🔄 Actualizar
- 🗑️ Eliminar
- 💾 Guardar
- ❌ Cancelar

**Datos:**
- 👤 Personas/Usuario
- 📞 Teléfono
- 🆔 Identificación
- 📧 Email
- 🏢 Empresa
- 🗺️ Ubicación

**Comida:**
- 🍕 Pizza
- 🍔 Hamburguesa
- 🍜 Fideos
- 🥗 Ensalada
- 🍰 Postres
- 🥤 Bebidas

---

## Cómo Cambiar el Tema Completo

### Tema Oscuro
Modifica en `Styles.xaml`:
```xaml
<SolidColorBrush x:Key="BackgroundLight" Color="#FF1E1E1E"/>
<SolidColorBrush x:Key="TextPrimary" Color="#FFFFFFFF"/>
```

### Tema Pastel
Usa colores más claros:
```xaml
<SolidColorBrush x:Key="PrimaryColor" Color="#FFB39DDB"/>
<SolidColorBrush x:Key="AccentColor" Color="#FFFFE0B2"/>
```

---

## Estructura de Colores

```
PrimaryColor       = Color principal de marca
PrimaryLight      = Versión más clara (hover)
AccentColor       = Color de acentos/alertas
SuccessColor      = Operaciones exitosas (verde)
WarningColor      = Advertencias (amarillo)
ErrorColor        = Errores (rojo)
TextPrimary       = Texto principal
TextSecondary     = Texto secundario/helper
BackgroundLight   = Fondo claro
BorderColor       = Bordes
```

---

## Ejemplo: Cambio Completo a Tema Azul

1. Abre `Resources/Styles.xaml`
2. Encuentra `<SolidColorBrush x:Key="PrimaryColor"...`
3. Reemplaza donde dice:

```xaml
<!-- ANTES -->
<SolidColorBrush x:Key="PrimaryColor" Color="#FF2E7D32"/>
<SolidColorBrush x:Key="PrimaryLight" Color="#FF43A047"/>
<SolidColorBrush x:Key="AccentColor" Color="#FFF57C00"/>

<!-- DESPUÉS - Azul Profesional -->
<SolidColorBrush x:Key="PrimaryColor" Color="#FF1565C0"/>
<SolidColorBrush x:Key="PrimaryLight" Color="#FF1976D2"/>
<SolidColorBrush x:Key="AccentColor" Color="#FF0288D1"/>
```

4. Compila: `dotnet build`
5. ¡Listo! Toda la app se verá en azul.

---

## Tips de Diseño

✅ **Hazlo consistente**: Usa los mismos estilos en toda la app
✅ **Mantén el contraste**: Los colores claros sobre fondos oscuros
✅ **Respeta el espaciado**: 16px es el estándar usado
✅ **Usa emojis sabiamente**: Uno por sección/botón
✅ **Prueba on diferentes pantallas**: Verifica responsividad

---

## Soporte Rápido

**¿Necesitas cambiar la tipografía?**
→ Busca `FontSize` en `Styles.xaml`

**¿Necesitas más redondez en los bordes?**
→ Aumenta `CornerRadius` en los `<Border>`

**¿Necesitas cambiar el espaciado?**
→ Modifica los `Margin` y `Padding`

**¿Necesitas nuevos iconos?**
→ Usa emojis o carga imágenes PNG

---

*Guía de personalización - RestauranteApp*
