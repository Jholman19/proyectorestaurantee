# 💻 Ejemplos de Código - Diseño Visual

## 📚 Recursos Disponibles

### Todos los estilos están en: `Resources/Styles.xaml`

---

## 🎨 Cómo Usar los Estilos

### 1️⃣ Usar estilos de botón

#### Botón Principal (Verde)
```xaml
<Button Content="📋 Cargar" Style="{StaticResource ModernButton}" Click="Cargar_Click"/>
```

#### Botón Secundario (Naranja)
```xaml
<Button Content="🔄 Renovar" Style="{StaticResource SecondaryButton}" Click="Renovar_Click"/>
```

---

### 2️⃣ Usar estilos de TextBox

```xaml
<TextBox x:Name="NombreBox" Style="{StaticResource ModernTextBox}" Placeholder="Ingresa nombre"/>
```

**Características:**
- Altura: 32px
- Borde redondeado: 4px
- Borde dorado en focus
- Padding automático

---

### 3️⃣ Usar estilos de Labels

#### Para campos de formulario
```xaml
<TextBlock Text="👤 Nombre" Style="{StaticResource FormLabel}"/>
<TextBox Style="{StaticResource ModernTextBox}"/>
```

#### Para títulos de ventanas
```xaml
<TextBlock Text="🍕 Mi Formulario" Style="{StaticResource WindowHeader}"/>
```

---

### 4️⃣ Usar colores globales

```xaml
<!-- Texto de error -->
<TextBlock Text="Error al cargar" Foreground="{StaticResource ErrorColor}"/>

<!-- Fondo de éxito -->
<Border Background="{StaticResource SuccessColor}" Padding="10">
    <TextBlock Text="✅ Operación exitosa"/>
</Border>

<!-- Botón personalizado -->
<Button Background="{StaticResource PrimaryColor}" Foreground="White">
    Guardar
</Button>
```

---

## 🏗️ Estructura de una Ventana Completa

### Patrón Recomendado

```xaml
<Window x:Class="RestauranteApp.MiVentana"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="🎯 Mi Ventana"
        Height="600" Width="800"
        WindowStartupLocation="CenterOwner"
        Background="{StaticResource BackgroundLight}">

    <Grid Margin="0">
        <!-- Definir filas -->
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>      <!-- Header -->
            <RowDefinition Height="*"/>         <!-- Contenido principal -->
            <RowDefinition Height="Auto"/>      <!-- Botones -->
        </Grid.RowDefinitions>

        <!-- ============ HEADER === -->
        <Border Grid.Row="0" Style="{StaticResource HeaderBorder}" Margin="0">
            <TextBlock Text="🎯 Mi Título Largo" Style="{StaticResource TitleText}"/>
        </Border>

        <!-- ========== CONTENIDO == -->
        <Border Grid.Row="1" Padding="20" Margin="0">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                </Grid.RowDefinitions>

                <!-- Campo 1 -->
                <TextBlock Text="👤 Campo 1" Style="{StaticResource FormLabel}"/>
                <TextBox Style="{StaticResource ModernTextBox}" Margin="0,0,0,12"/>

                <!-- Campo 2 -->
                <TextBlock Grid.Row="1" Text="📝 Campo 2" Style="{StaticResource FormLabel}"/>
                <TextBox Grid.Row="1" Style="{StaticResource ModernTextBox}" Margin="0,0,0,12"/>
            </Grid>
        </Border>

        <!-- ========= BOTONES === -->
        <Border Grid.Row="2" Background="White" 
                BorderBrush="{StaticResource BorderColor}" 
                BorderThickness="0,1,0,0" 
                Padding="20">
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                <Button Content="❌ Cancelar" Width="120" 
                        Style="{StaticResource ModernButton}" 
                        Background="{StaticResource BorderColor}"/>
                <Button Content="💾 Guardar" Width="120" 
                        Style="{StaticResource ModernButton}"/>
            </StackPanel>
        </Border>
    </Grid>
</Window>
```

---

## 📊 Ejemplo: DataGrid con Estilos

```xaml
<DataGrid x:Name="MiGrid"
    AutoGenerateColumns="False"
    IsReadOnly="True"
    CanUserAddRows="False"
    SelectionMode="Single"
    AlternatingRowBackground="#FFFAFAFA"
    BorderBrush="{StaticResource BorderColor}"
    BorderThickness="1"
    RowHeight="28">

    <!-- Headers con color -->
    <DataGrid.ColumnHeaderStyle>
        <Style TargetType="DataGridColumnHeader">
            <Setter Property="Background" Value="{StaticResource PrimaryColor}"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="Padding" Value="8,6"/>
            <Setter Property="FontWeight" Value="Bold"/>
        </Style>
    </DataGrid.ColumnHeaderStyle>

    <!-- Columnas -->
    <DataGrid.Columns>
        <DataGridTextColumn Header="🆔 ID" Binding="{Binding Id}" Width="80"/>
        <DataGridTextColumn Header="👤 Nombre" Binding="{Binding Nombre}" Width="*"/>
        <DataGridTextColumn Header="☎️ Teléfono" Binding="{Binding Telefono}" Width="150"/>
        <DataGridCheckBoxColumn Header="✅ Activo" Binding="{Binding Activo}" Width="100"/>
    </DataGrid.Columns>
</DataGrid>
```

---

## 🎛️ Ejemplo: Panel de Control

```xaml
<Border BorderBrush="{StaticResource BorderColor}" 
        BorderThickness="1" 
        CornerRadius="8" 
        Padding="16" 
        Background="White">
    <StackPanel>
        <!-- Título -->
        <TextBlock Text="📊 Panel de Control" 
                   FontWeight="Bold" 
                   Foreground="{StaticResource PrimaryColor}" 
                   Margin="0,0,0,12"
                   FontSize="16"/>

        <!-- Grid de botones -->
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <Button Grid.Column="0" Content="📋 Opción 1" Style="{StaticResource ModernButton}"/>
            <Button Grid.Column="1" Content="📋 Opción 2" Style="{StaticResource ModernButton}"/>
            <Button Grid.Column="2" Content="📋 Opción 3" Style="{StaticResource ModernButton}"/>
        </Grid>

        <!-- Info -->
        <Border BorderBrush="{StaticResource BorderColor}" 
                BorderThickness="0,1,0,0" 
                Padding="0,12,0,0"
                Margin="0,12,0,0">
            <TextBlock Text="💡 Información útil aquí" 
                       Foreground="{StaticResource TextSecondary}"/>
        </Border>
    </StackPanel>
</Border>
```

---

## 🌈 Ejemplo: Sección Coloreada

```xaml
<Border BorderBrush="{StaticResource SuccessColor}" 
        BorderThickness="2" 
        CornerRadius="8"
        Padding="12" 
        Background="#F1F8E9">
    <StackPanel>
        <TextBlock Text="✅ Éxito" 
                   Foreground="{StaticResource SuccessColor}" 
                   FontWeight="Bold"/>
        <TextBlock Text="La operación se completó correctamente" 
                   Foreground="{StaticResource TextSecondary}"/>
    </StackPanel>
</Border>
```

---

## ❌ Ejemplo: Sección de Error

```xaml
<Border BorderBrush="{StaticResource ErrorColor}" 
        BorderThickness="2" 
        CornerRadius="8"
        Padding="12" 
        Background="#FFEBEE">
    <StackPanel>
        <TextBlock Text="❌ Error" 
                   Foreground="{StaticResource ErrorColor}" 
                   FontWeight="Bold"/>
        <TextBlock Text="Algo salió mal. Intenta de nuevo." 
                   Foreground="{StaticResource TextSecondary}"/>
    </StackPanel>
</Border>
```

---

## 🎨 Crear un Estilo Personalizado

### En `Resources/Styles.xaml`, agrega:

```xaml
<!-- Estilo personalizado -->
<Style x:Key="MiEstiloEspecial" TargetType="Button" BasedOn="{StaticResource ModernButton}">
    <Setter Property="Background" Value="#FF6A1B9A"/>
    <Setter Property="FontSize" Value="16"/>
    <Setter Property="Padding" Value="16,12"/>
</Style>
```

### Uso en XAML:

```xaml
<Button Content="Mi Botón Especial" Style="{StaticResource MiEstiloEspecial}"/>
```

---

## 🎭 Gradientes Personalizados

```xaml
<!-- Crear un gradiente personalizado -->
<LinearGradientBrush x:Key="MiGradiente" StartPoint="0,0" EndPoint="1,1">
    <GradientStop Color="#FFFF6F00" Offset="0"/>
    <GradientStop Color="#FFFF8F00" Offset="1"/>
</LinearGradientBrush>

<!-- Usar en un Border -->
<Border Background="{StaticResource MiGradiente}" Padding="20">
    <TextBlock Text="Fondo con gradiente" Foreground="White"/>
</Border>
```

---

## 📱 Responsive: Adaptarse a Diferentes Tamaños

```xaml
<!-- Panel que se adapta -->
<WrapPanel>
    <Button Content="📋 Opción 1" Width="120" Style="{StaticResource ModernButton}"/>
    <Button Content="📋 Opción 2" Width="120" Style="{StaticResource ModernButton}"/>
    <Button Content="📋 Opción 3" Width="120" Style="{StaticResource ModernButton}"/>
    <!-- Si no cabe, se envuelve a la siguiente línea -->
</WrapPanel>
```

---

## 🔄 Transiciones de Hover

Los estilos ya incluyen efectos hover. Para crear uno personalizado:

```xaml
<Style x:Key="MiBotonEspecial" TargetType="Button">
    <Setter Property="Background" Value="#FF2E7D32"/>
    <Setter Property="Foreground" Value="White"/>
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="#FF43A047"/>
            <!-- El color cambia cuando pasas el mouse -->
        </Trigger>
        <Trigger Property="IsPressed" Value="True">
            <Setter Property="Opacity" Value="0.8"/>
            <!-- Se oscurece cuando haces click -->
        </Trigger>
    </Style.Triggers>
</Style>
```

---

## 📐 Espaciado Estándar

Usa estos valores en toda la app para consistencia:

```
Margen pequeño:    4px
Margen normal:     8px
Margen mediano:   12px
Margen grande:    16px
Margen muy grande: 20px

Padding interno:  8-12px (en campos)
Padding section:  16-20px (en borders/panels)
```

---

## 🎯 Checklist para Nueva Ventana

Al crear una nueva ventana, usa:

- [ ] Header con `HeaderBorder` style
- [ ] Fondos con `BackgroundLight`
- [ ] Botones con `ModernButton` o `SecondaryButton`
- [ ] Campos con `ModernTextBox`
- [ ] Labels con `FormLabel`
- [ ] Colores de `Styles.xaml`
- [ ] Emojis descriptivos
- [ ] Espaciado de 12-16px
- [ ] Borders redondeados (4-8px)

---

## 🆘 Troubleshooting

**Los estilos no se aplican:**
```xaml
<!-- Asegúrate de que App.xaml tenga: -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources/Styles.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

**El color no cambia:**
- Verifica que uses `{StaticResource NombreColor}`
- Compila el proyecto
- Limpia y reconstruye si falta

**Los botones se ven extraños:**
- Verifica que uses `Style="{StaticResource ModernButton}"`
- No agregues Background directamente, usa los estilos

---

*Guía de ejemplos de código - RestauranteApp*
