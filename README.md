Introducción
=====

CSGBE (C# GameBoy Emulator) es un emulador de la consola GameBoy escrito en lenguaje C# 1.0
Inicialmente pensado como un proyecto personal, finalmente se convirtió en mi PFC (Proyecto de fin de Carrera) de Ingeniería Informática en la UC3M (Universidad Carlos III de Madrid). Se concluyó a finales de diciembre del 2006 y se presentó el día martes 13 de febrero del 2007 obteniendo la nota de Matrícula de Honor.

Funcionalidades
=====

CSGBE emula los siguientes elementos:

- Procesador DMG basado en el Z80 de ZiLOG Inc. y su juego completo de instrucciones.
- Registros y mapa de direcciones de memoria.
- Interrupciones excepto la de E/S por puerto serie.
- Cartuchos de tipo MBC0 y algunas combinaciones de MBC1, MBC2, MBC3 y MBC5

CSGBE NO soporta los siguientes elementos:

- Sonido
- Guardado de partidas
- Transferencia serie
- RTC en el tipo de cartucho MBC3 (usado por ejemplo en los juegos Pokèmon para controlar el día y la noche) o Rumble (vibración) en el tipo de cartucho MBC5
- GameBoy Color
- Otros periféricos como SuperGameBoy (SGB), GameBoy Printer o GameBoy Camera

**Aviso importante**

    Este emulador no está pensado para su uso habitual. Tiene multitud de fallos, funcionalidades sin implementar y no está optimizado en ningún aspecto, por lo que sólo debería usarse para uso docente o investigador.
    Existen multitud de emuladores centrados en la compatibilidad y rendimiento con muchos años de trabajo detrás de ellos y muchas más prestaciones, como por ejemplo VisualBoyAdvance que soporta GameBoy, GameBoy Color y GameBoy Advance, o KiGB

CSGBE permite:

- Cargar de fichero los ficheros que contienen las ROMs con los programas o juegos
- Controlar el proceso de emulación: pausar, reanudar, detener y reiniciar
- 4 niveles de zoom
- Extenso modo de depuración donde se puede:
  - Mostrar y modificar el estado de cualquier registro, flag de estado o dirección de memoria
  - Visualizar regiones completas de memoria
  - Puntos de interrupción en el flujo de ejecución
  - Información detallada sobre el resultado de la ejecución de cada instrucción de ensamblador

Existe más información sobre su utilización en el manual de usuario, anexo de la memoria de proyecto.
Las teclas no son configurables: Z y X se corresponden con los botones A y B respectivamente y la tecla Return con el botón Start.
