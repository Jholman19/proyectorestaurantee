# 📦 ARCHIVOS DE DISTRIBUCIÓN - MI RESTAURANTE v1.0.0

## ✅ Archivos Generados

Se han creado **2 versiones** de la aplicación para llevar a otra PC:

---

## 🎯 OPCIÓN 1: INSTALADOR (RECOMENDADO)

### 📁 Archivo:
```
installer\Output\RestauranteApp_Setup_v1.0.0.exe
```

### 📊 Tamaño: 52.68 MB

### ✅ Características:
- ✨ Instalación profesional con asistente visual
- ✨ Se instala en "Archivos de Programa"
- ✨ Crea accesos directos automáticos (Escritorio + Menú Inicio)
- ✨ Se puede desinstalar desde Panel de Control
- ✨ Incluye todas las dependencias (.NET 8 Runtime)
- ✨ Configuración automática de rutas

### 🚀 Cómo Usar:
1. Copia el archivo `RestauranteApp_Setup_v1.0.0.exe` a una USB
2. Llévalo a la otra PC
3. Haz doble clic en el archivo
4. Sigue el asistente de instalación
5. ¡Listo! La aplicación estará en el Menú Inicio

### ⚠️ Nota:
- Requiere permisos de Administrador
- Se instala en: `C:\Program Files\Mi Restaurante\`
- Base de datos en: `C:\Users\[Usuario]\AppData\Local\RestauranteApp\`

---

## 🎯 OPCIÓN 2: VERSIÓN PORTABLE (SIN INSTALACIÓN)

### 📁 Archivo:
```
RestauranteApp_Portable_v1.0.0.zip
```

### 📊 Tamaño: 73.45 MB

### ✅ Características:
- ✨ NO requiere instalación
- ✨ Se ejecuta desde cualquier carpeta
- ✨ Perfecta para USB o unidades de red
- ✨ No modifica el sistema
- ✨ Incluye todas las dependencias (.NET 8 Runtime)
- ✨ Incluye archivo LEEME.txt con instrucciones

### 🚀 Cómo Usar:
1. Extrae el contenido del ZIP en cualquier carpeta
2. Haz doble clic en `RestauranteApp.exe`
3. ¡Listo! La aplicación se ejecuta directamente

### 📂 Contenido del ZIP:
```
RestauranteApp_Portable/
├── RestauranteApp.exe          ← Ejecutable principal
├── LEEME.txt                    ← Instrucciones
├── *.dll                        ← Bibliotecas necesarias
├── runtimes/                    ← Componentes del sistema
└── [otros archivos]
```

### ⚠️ Nota:
- NO requiere permisos de Administrador
- Puedes ejecutarlo desde USB directamente
- Base de datos se crea en: `C:\Users\[Usuario]\AppData\Local\RestauranteApp\`

---

## 📍 UBICACIONES DE LOS ARCHIVOS

### En tu PC actual:

| Archivo | Ruta Completa |
|---------|---------------|
| **Instalador** | `C:\Users\jholm\RestauranteApp\installer\Output\RestauranteApp_Setup_v1.0.0.exe` |
| **Portable ZIP** | `C:\Users\jholm\RestauranteApp\RestauranteApp_Portable_v1.0.0.zip` |
| **Carpeta Portable** | `C:\Users\jholm\RestauranteApp\RestauranteApp_Portable\` |

---

## 🔐 LECTOR DE HUELLAS (AMBAS VERSIONES)

Para usar el lector HID U.are.U 4500 en la otra PC:

### Requisitos Adicionales:
1. **Driver del lector**: https://www.digitalpersona.com/
2. **SDK One Touch v1.6.x**: Descargar desde DigitalPersona

### Pasos:
1. Instala primero el driver del lector
2. Instala el SDK
3. Reinicia la PC
4. Ejecuta la aplicación
5. El lector será detectado automáticamente

### ⚠️ IMPORTANTE:
- Si NO vas a usar lector de huellas, la aplicación funciona igual
- Puedes registrar asistencia manualmente sin problema
- El simulador de huellas está disponible para pruebas

---

## 💾 BASE DE DATOS

### Ubicación Automática:
```
C:\Users\[NombreUsuario]\AppData\Local\RestauranteApp\restaurante.db
```

### ¿Cómo copiar la BD a otra PC?

1. **En la PC original:**
   - Cierra la aplicación
   - Ve a: `C:\Users\jholm\AppData\Local\RestauranteApp\`
   - Copia el archivo `restaurante.db`

2. **En la PC nueva:**
   - Ejecuta la aplicación una vez (se crea la carpeta)
   - Cierra la aplicación
   - Ve a: `C:\Users\[Usuario]\AppData\Local\RestauranteApp\`
   - Pega el archivo `restaurante.db` (reemplaza si existe)
   - Abre la aplicación nuevamente

### ⚠️ Nota:
- Cada PC tiene su propia base de datos por defecto
- Las huellas registradas SÍ se copian con la base de datos
- Si copias la BD, tendrás todos los clientes, combos y suscripciones

---

## 📋 CHECKLIST DE TRANSFERENCIA

### Para Usar el Instalador:
- [ ] Copia `RestauranteApp_Setup_v1.0.0.exe` a USB
- [ ] Lleva USB a la otra PC
- [ ] Ejecuta el instalador como Administrador
- [ ] Sigue el asistente
- [ ] (Opcional) Instala driver del lector
- [ ] (Opcional) Copia la base de datos
- [ ] Ejecuta desde el Menú Inicio

### Para Usar Versión Portable:
- [ ] Copia `RestauranteApp_Portable_v1.0.0.zip` a USB
- [ ] Lleva USB a la otra PC
- [ ] Extrae el ZIP en una carpeta (ej: `C:\MisApps\RestauranteApp\`)
- [ ] Lee el archivo `LEEME.txt`
- [ ] Ejecuta `RestauranteApp.exe`
- [ ] (Opcional) Instala driver del lector
- [ ] (Opcional) Copia la base de datos

---

## 🆘 TROUBLESHOOTING

### Problema: "La aplicación no inicia"
**Solución**: 
- Windows 10/11 64-bit es requerido
- Asegúrate de que Windows Defender no esté bloqueando el ejecutable
- Verifica que tienes .NET Runtime (ya incluido en ambas versiones)

### Problema: "El lector no funciona"
**Solución**:
- Instala el driver del lector HID U.are.U 4500
- Instala el SDK One Touch v1.6.x
- Reinicia la PC
- Conecta el lector USB

### Problema: "No encuentra la base de datos"
**Solución**:
- La BD se crea automáticamente en el primer inicio
- Si copiaste la BD, asegúrate de que esté en:
  `C:\Users\[Usuario]\AppData\Local\RestauranteApp\`

### Problema: "Error de permisos"
**Solución**:
- Para el instalador: Ejecuta como Administrador (clic derecho → Ejecutar como administrador)
- Para la versión portable: No se necesitan permisos especiales

---

## ✅ RECOMENDACIONES

### Para Uso Profesional (Múltiples PCs):
✅ Usa el **Instalador** en cada PC
✅ Instala el driver del lector en todas las PCs
✅ Cada PC tendrá su propia base de datos
✅ Si quieres centralizar datos, considera usar red compartida

### Para Pruebas o Uso Temporal:
✅ Usa la **Versión Portable**
✅ Cópiala en USB y llévala contigo
✅ No requiere instalación
✅ Perfecta para demostraciones

---

## 🎉 ¡TODO LISTO!

Ambas versiones incluyen:
- ✅ .NET 8 Runtime integrado
- ✅ Todas las bibliotecas necesarias
- ✅ Sistema completo de gestión de restaurante
- ✅ Soporte para lector de huellas (con SDK instalado)
- ✅ Dashboard de reportes
- ✅ Sistema de créditos
- ✅ Control de asistencia por horarios

**¡Tu sistema está listo para ser usado en cualquier PC!** 🍕

---

**Fecha de Generación**: 6 de Marzo, 2026  
**Versión**: 1.0.0  
**Compilado con**: .NET 8.0 (win-x64, self-contained)
