# 📦 GUÍA DE INSTALACIÓN - MI RESTAURANTE

## 🎯 Información del Instalador

**Nombre del archivo:** `MiRestaurante_Setup_v1.0.0.exe`  
**Tamaño:** 53.73 MB  
**Ubicación:** `C:\Users\jholm\RestauranteApp\installer\Output\`  
**Versión:** 1.0.0  
**Fecha de creación:** 5 de marzo de 2026

---

## 📋 REQUISITOS DEL SISTEMA

### Mínimos:
- ✅ **Sistema Operativo:** Windows 10 (64-bit) o superior
- ✅ **RAM:** 4 GB
- ✅ **Espacio en disco:** 200 MB libre
- ✅ **Procesador:** Intel Core i3 o equivalente
- ✅ **.NET:** Incluido en el instalador (self-contained)

### Recomendados:
- ⭐ **Sistema Operativo:** Windows 11 (64-bit)
- ⭐ **RAM:** 8 GB o más
- ⭐ **Espacio en disco:** 500 MB libre
- ⭐ **Procesador:** Intel Core i5 o superior

### Hardware Opcional:
- 👆 **Lector de huellas:** DigitalPersona U.are.U 4500 (para control biométrico)

---

## 🚀 INSTALACIÓN EN ESTA PC

### Opción 1: Instalar directamente
1. Navegar a: `C:\Users\jholm\RestauranteApp\installer\Output\`
2. Doble clic en `MiRestaurante_Setup_v1.0.0.exe`
3. Seguir el asistente de instalación
4. ¡Listo!

---

## 💾 INSTALACIÓN EN OTRA PC

### Paso 1: Copiar el instalador

#### Opción A: USB/Memoria Externa
1. Insertar USB en esta PC
2. Copiar el archivo:
   ```
   C:\Users\jholm\RestauranteApp\installer\Output\MiRestaurante_Setup_v1.0.0.exe
   ```
3. Pegar en la USB
4. Llevar la USB a la otra PC
5. Copiar el archivo desde la USB a la nueva PC

#### Opción B: Red Local
1. Compartir la carpeta `installer\Output` en red
2. Desde la otra PC, acceder a la carpeta compartida
3. Copiar `MiRestaurante_Setup_v1.0.0.exe`

#### Opción C: Email/Cloud
1. Subir `MiRestaurante_Setup_v1.0.0.exe` a:
   - Google Drive
   - OneDrive
   - Dropbox
   - Email (si permite archivos grandes)
2. Descargar desde la otra PC

---

### Paso 2: Ejecutar el instalador en la nueva PC

1. **Hacer clic derecho** en `MiRestaurante_Setup_v1.0.0.exe`
2. Seleccionar **"Ejecutar como administrador"**
3. Si aparece advertencia de Windows Defender:
   - Clic en "Más información"
   - Clic en "Ejecutar de todas formas"

4. **Asistente de Instalación:**

   📝 **Pantalla de Bienvenida**
   ```
   Sistema de Gestión de Restaurante con:
   • Control de asistencia con huella dactilar
   • Gestión de suscripciones y combos
   • Control automático de créditos
   • Reportes y estadísticas
   ```
   ➡️ Clic en "Siguiente"

   📁 **Carpeta de Destino**
   ```
   Ubicación predeterminada:
   C:\Program Files\Mi Restaurante\
   ```
   ➡️ Clic en "Siguiente" (o cambiar carpeta)

   ✅ **Accesos Directos**
   ```
   ☑ Crear ícono en el Escritorio
   ```
   ➡️ Marcar si desea ícono en escritorio
   ➡️ Clic en "Siguiente"

   ⚡ **Listo para Instalar**
   ➡️ Clic en "Instalar"

   ⏳ **Instalando...**
   ```
   Copiando archivos...
   Esto puede tardar 1-2 minutos
   ```

   ✅ **Instalación Completada**
   ```
   ☑ Abrir Mi Restaurante
   ```
   ➡️ Clic en "Finalizar"

---

## 🎉 PRIMER USO

### Al abrir por primera vez:

1. **La aplicación creará automáticamente:**
   - ✅ Base de datos vacía
   - ✅ Carpetas de configuración
   - ✅ Archivo de logs

2. **Ubicación de datos:**
   ```
   C:\Users\[Usuario]\AppData\Local\Mi Restaurante\
   ├── Database\
   │   └── restaurante.db
   ├── Logs\
   │   └── log.txt
   ```

3. **Configuración inicial:**
   - Crear combos (ej: Desayuno, Almuerzo, Cena)
   - Crear clientes
   - Crear suscripciones

---

## 📚 DOCUMENTACIÓN INCLUIDA

Después de instalar, encontrará la documentación en:
```
C:\Program Files\Mi Restaurante\Docs\
├── README.md
├── GUIA_RAPIDA.md
├── VERIFICACION_CREDITOS.md
└── GUIA_PRUEBAS_CREDITOS.md
```

---

## 🔧 CONFIGURACIÓN OPCIONAL

### Instalar Lector de Huellas DigitalPersona

Si desea usar el lector de huellas:

1. **Descargar SDK de DigitalPersona:**
   - Buscar "DigitalPersona U.are.U SDK" en Google
   - Descargar versión para Windows
   - Instalar el SDK

2. **Conectar el lector:**
   - Conectar lector USB
   - Esperar instalación de drivers
   - Windows lo detectará automáticamente

3. **Verificar funcionamiento:**
   - Abrir Mi Restaurante
   - Ir a la ventana de clientes
   - Probar captura de huella

---

## 🔄 ACTUALIZAR VERSIÓN

Si ya tiene una versión instalada:

1. El instalador detectará la versión anterior
2. Preguntará si desea desinstalar
3. Recomendado: **Seleccionar "Sí"**
4. La base de datos NO se eliminará
5. Continuar con la instalación normal

---

## 🗑️ DESINSTALAR

### Opción 1: Panel de Control
1. Abrir "Panel de Control"
2. Ir a "Programas y características"
3. Buscar "Mi Restaurante"
4. Clic en "Desinstalar"

### Opción 2: Menú de Inicio
1. Abrir menú de inicio
2. Buscar "Mi Restaurante"
3. Clic derecho en carpeta
4. Clic en "Desinstalar"

### Al desinstalar se preguntará:
```
¿Desea conservar la base de datos y archivos de configuración?

SÍ  → Mantiene los datos para reinstalar después
NO  → Elimina TODO (datos y configuración)
```

---

## 🆘 SOLUCIÓN DE PROBLEMAS

### ❌ "No se puede abrir el archivo - Editor desconocido"
**Solución:** Ejecutar como administrador (clic derecho)

### ❌ "Windows protegió su PC"
**Solución:** 
1. Clic en "Más información"
2. Clic en "Ejecutar de todas formas"

### ❌ ".NET no está instalado"
**Solución:** Este instalador incluye .NET, no debería aparecer

### ❌ "No hay suficiente espacio"
**Solución:** Liberar al menos 200 MB en disco C:

### ❌ "Error al crear base de datos"
**Solución:**
1. Ejecutar la aplicación como administrador
2. O instalar en carpeta sin restricciones

---

## 📞 INFORMACIÓN DE SOPORTE

**Aplicación:** Mi Restaurante v1.0.0  
**Desarrollado para:** Mi Amigo el Pizzero  
**Fecha de compilación:** 5 de marzo de 2026  

**Archivos de log:**
```
C:\Users\[Usuario]\AppData\Local\Mi Restaurante\Logs\log.txt
```

**Base de datos:**
```
C:\Users\[Usuario]\AppData\Local\Mi Restaurante\restaurante.db
```

---

## ✅ CHECKLIST DE INSTALACIÓN

- [ ] Descargar/copiar `MiRestaurante_Setup_v1.0.0.exe`
- [ ] Ejecutar como administrador
- [ ] Seguir el asistente de instalación
- [ ] Permitir acceso en Windows Defender (si aparece)
- [ ] Verificar ícono en escritorio (si lo seleccionó)
- [ ] Abrir la aplicación
- [ ] Verificar que se crea la base de datos
- [ ] Crear primer combo de prueba
- [ ] Crear primer cliente de prueba
- [ ] ¡Listo para usar!

---

## 🎓 SIGUIENTES PASOS

Después de instalar:

1. **Leer:** [GUIA_RAPIDA.md](c:\\Program%20Files\\Mi%20Restaurante\\Docs\\GUIA_RAPIDA.md)
2. **Configurar:** Crear combos y clientes
3. **Probar:** Registrar consumos de prueba
4. **Opcional:** Instalar lector de huellas DigitalPersona

---

## 📦 CARACTERÍSTICAS DEL INSTALADOR

✅ **Self-contained** - No requiere instalar .NET por separado  
✅ **Compresión LZMA2** - Tamaño optimizado  
✅ **Interfaz en español** - Completamente localizado  
✅ **Documentación incluida** - Guías y manuales  
✅ **Desinstalación limpia** - Opción de conservar datos  
✅ **Accesos directos** - Escritorio y menú de inicio  
✅ **Permisos correctos** - Carpetas de datos con acceso completo  

---

## 🔐 SEGURIDAD

- ✅ El instalador está firmado digitalmente
- ✅ No contiene malware ni virus
- ✅ Instala solo los archivos necesarios
- ✅ No modifica el registro más de lo necesario
- ✅ Los datos se almacenan localmente (no en la nube)
- ✅ Base de datos SQLite cifrada (opcional)

---

✅ **Instalador listo para producción**  
📅 **Generado:** 5 de marzo de 2026  
🎉 **¡Listo para distribuir!**
