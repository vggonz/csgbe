/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Output Debug Module
#
# Copyright (C) 2004 Victor Garcia Gonzalez
#
# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU General Public License
# as published by the Free Software Foundation; either version 2
# of the License, or (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
#
############################################################################*/

using System;
namespace csgbe.perifericos{

	/// <summary>Clase con funciones auxiliares y depuracion</summary>
	public class Debug{

		/// <summary>Caracteres hexadecimales</summary>
		private static readonly string hexChars = "0123456789ABCDEF";

		/// <summary>Imprime una linea por la salida estandard</summary>
		public static void WriteLine(string cadena){ Console.WriteLine(cadena); }
		/// <summary>Imprime un salto de linea por la salida estandard</summary>
		public static void WriteLine(){ Console.WriteLine(); }
		/// <summary>Imprime texto en la linea actual de la salida estandard</summary>
		public static void Write(string cadena){ Console.Write(cadena); }
		/// <summary>Imprime un caracter en la linea actual de la salida estandard</summary>
		public static void Write(char caracter){ Console.Write(caracter); }

		/// <summary>Inicia un contador con la fecha actual</summary>
		/// <returns>Contador con la fecha y hora del momento de invocacion</returns>
		public static DateTime iniciarContador(){ return DateTime.Now; }

		/// <summary>Detiene un contador y devuelve el tiempo en segundos transcurridos desde que se inicio</summary>
		/// <param name="time">Contador a detener</param>
		/// <returns>Tiempo en segundos desde que se inicio el contador</returns>
		public static double detenerContador(DateTime time){
			DateTime stopTime = DateTime.Now;
			TimeSpan duration = stopTime - time;
			return duration.TotalSeconds;
		}

		/// <summary>Convierte un byte en su equivalente en caracteres hexadecimales</summary>
		/// <param name="b">Byte a convertir</param>
		/// <returns>Cadena de texto con dos caracteres hexadecimales</returns>
		public static string hexByte(int b){ return String.Concat(hexChars[b >> 4], hexChars[b & 0x0F]); }

		/// <summary>Convierte una palabra de dos bytes en su equivalente en caracteres hexadecimales</summary>
		/// <param name="w">Palabra a convertir</param>
		/// <returns>Cadena de texto con cuatro caracteres hexadecimales</returns>
		public static string hexWord(int w){ return (hexByte((w & 0xFF00) >> 8) + hexByte(w & 0x00FF)); }

		/// <summary>Convierte un byte expresado en caracteres hexadecimales a su equivalente numerico</summary>
		/// <param name="hex">Cadena hexadecimal</param>
		/// <returns>Equivalente numerico del byte en hexadecimal</returns>
		public static int byteHex(string hex){ hex = hex.PadLeft(2, '0').ToUpper(); return (hexChars.IndexOf(hex[1]) | (hexChars.IndexOf(hex[0]) << 4)); }

		/// <summary>Convierte una palabra de cuatro caracteres hexadecimales en su equivalente numerico</summary>
		/// <param name="hex">Cadena hexadecimal</param>
		/// <returns>Equivalente numerico de la palabra en hexadecimal</returns>
		public static int wordHex(string hex){ hex = hex.PadLeft(4, '0').ToUpper(); return ((byteHex(hex.Substring(0, 2)) << 8) | byteHex(hex.Substring(2, 2))); }

		/// <summary>Imprime una zona de memoria en un formato presentable</summary>
		/// <param name="memoria">Memoria</param>
		/// <param name="direccion">Direccion de memoria de inicio</param>
		/// <param name="longitud">Cantidad de bytes que se quieren imprimir a partir de la direccion de inicio (minimo 10)</param>
		private static void hexDump(kernel.Memoria memoria, int direccion, int longitud){
			int start = direccion & 0xFFF0;
			int lines = longitud / 16;
			if (lines == 0) lines = 1;
			for (int l = 0; l < lines; l++) {
				Write(hexWord(start + (l * 16)) + "   ");
				for (int r = start + (l * 16); r < start + (l * 16) + 16; r++) Write(hexByte(memoria.leer(r)) + " ");
				Write("   ");
				for (int r = start + (l * 16); r < start + (l * 16) + 16; r++) {
					char c = (char)memoria.leer(r);
					if ((c >= 32) && (c <= 128)) Write(c);
					else Write(".");
				}
				WriteLine();
			}
		}

		/// <summary>Imprime el valor de todos los registros</summary>
		/// <param name="registros">Registros</param>
		private static void imprimirRegistros(kernel.Registros registros){
			WriteLine("PC = " + hexWord(registros.regPC) + "\tSP = " + hexWord(registros.regSP));
			WriteLine("A = " + hexByte(registros.regA) + "\tB = " + hexByte(registros.regB) + "\tC = " + hexByte(registros.regC) + "\tD = " + hexByte(registros.regD) + "\tE = " + hexByte(registros.regE) + "\tH = " + hexByte(registros.regH) + "\tL = " + hexByte(registros.regL));
			WriteLine("Z = " + registros.flagZ + "\tN = " + registros.flagN + "\tH = " + registros.flagH + "\tC = " + registros.flagC);
		}

		/// <summary>Imprime el estado de las interrupciones y del IME (Interrupt Master Enable)</summary>
		/// <param name="registros">Registros</param>
		/// <param name="memoria">Memoria</param>
		private static void imprimirInterrupciones(kernel.Registros registros, kernel.Memoria memoria){
			WriteLine("IME = " + registros.flagIME);
			Write("VBLANK = " + ((memoria.leer(kernel.Constantes.INT_FLAG) & kernel.Constantes.INT_VBLANK) > 0));
			Write("\tLCDC = " + ((memoria.leer(kernel.Constantes.INT_FLAG) & kernel.Constantes.INT_LCDC) > 0));
			Write("\tTIMER = " + ((memoria.leer(kernel.Constantes.INT_FLAG) & kernel.Constantes.INT_TIMER) > 0));
			Write("\tSERIALTX = " + ((memoria.leer(kernel.Constantes.INT_FLAG) & kernel.Constantes.INT_SERIALTX) > 0));
			WriteLine("\tKEY = " + ((memoria.leer(kernel.Constantes.INT_FLAG) & kernel.Constantes.INT_KEY) > 0));
		}

		/// <summary>Activa o desactiva una interrupcion</summary>
		/// <param name="memoria">Memoria</param>
		/// <param name="interrupcion">Nombre de la interrupcion a modificar (VBLANK, LCDC, TIMER, SERIALTX o KEY)</param>
		/// <param name="valor">Nuevo valor para la interrupcion. 1 o 0</param>
		private static void asignarInterrupcion(kernel.Memoria memoria, string interrupcion, string valor){
			bool error = false;
			int val = 0;
			interrupcion = interrupcion.ToUpper();
			switch(interrupcion){
				case "VBLANK": val = kernel.Constantes.INT_VBLANK; break;
				case "LCDC": val = kernel.Constantes.INT_LCDC; break;
				case "TIMER": val = kernel.Constantes.INT_TIMER; break;
				case "SERIALTX": val = kernel.Constantes.INT_SERIALTX; break;
				case "KEY": val = kernel.Constantes.INT_KEY; break;
				default: error = true; break;
			}
			switch(valor){
				case "1": break;
				case "0": val = ~val; break;
				default: error = true; break;
			}
			if (!error) memoria.escribir(memoria.leer(kernel.Constantes.INT_FLAG) | val, kernel.Constantes.INT_FLAG);
		}

		/// <summary>Cambia el estado de una tecla de la consola</summary>
		/// <param name="memoria">Memoria</param>
		/// <param name="tecla">Nombre de la tecla (DOWN, UP, LEFT, RIGHT, START, SELECT, B o A)</param>
		private static void toggleTecla(kernel.Memoria memoria, string tecla){
			tecla = tecla.ToUpper();
			int idTecla = 0;
			bool error = false;
			switch(tecla){
				case "DOWN": idTecla = kernel.Constantes.KEY_DOWN; break;
				case "UP": idTecla = kernel.Constantes.KEY_UP; break;
				case "LEFT": idTecla = kernel.Constantes.KEY_LEFT; break;
				case "RIGHT": idTecla = kernel.Constantes.KEY_RIGHT; break;
				case "START": idTecla = kernel.Constantes.KEY_START; break;
				case "SELECT": idTecla = kernel.Constantes.KEY_SELECT; break;
				case "B": idTecla = kernel.Constantes.KEY_B; break;
				case "A": idTecla = kernel.Constantes.KEY_A; break;
				default: error = true; break;
			}
			if (!error) Keypad.toggleTecla(idTecla);
		}
	
		/// <summary>Procesa las peticiones de la consola de depuracion</summary>
		/// <param name="cpu">CPU principal de la consola para acceder a sus registros y memoria</param>
		public static void ConsolaDepuracion(kernel.CPU cpu){
			string linea = null;
			string[] a = null;
			WriteLine("Iniciando Consola de depuracion");
			mostrarAyudaConsolaDepuracion();
			do{
				Write("CSGBE> ");
				linea = Console.ReadLine();
				a = linea.Split(null);
				try{
					switch(a[0]){
						case "?": mostrarAyudaConsolaDepuracion(); break;
						case "r": imprimirRegistros(cpu.registros); break;
						case "i": imprimirInterrupciones(cpu.registros, cpu.memoria); break;
						case "m": hexDump(cpu.memoria, wordHex(a[1]), Int32.Parse(a[2])); break;
						case "x": cpu.reset(); break;
						case "e": cpu.procesarInstrucciones(Int32.Parse(a[1])); break;
						case "a": cpu.registros.setReg(a[1].ToUpper(), wordHex(a[2])); break;
						case "f": cpu.registros.setFlag(a[1], !cpu.registros.getFlag(a[1])); break;
						case "w": cpu.memoria.escribir(wordHex(a[2]), wordHex(a[1])); break;
						case "n": asignarInterrupcion(cpu.memoria, a[1], a[2]); break;
						case "k": toggleTecla(cpu.memoria, a[1]); break;
						case "b": cpu.toggleBreakpoint(wordHex(a[1])); break;
					}
				}catch(Exception e){ WriteLine(e.StackTrace); }
			}while(a[0] != "q");
			WriteLine("Fin de la consola de depuracion");
		}

		/// <summary>Muestra las opciones posibles de la consola de depuracion</summary>
		private static void mostrarAyudaConsolaDepuracion(){
			WriteLine("Opciones:");
			WriteLine("\t?\t\tMostrar esta ayuda");
			WriteLine("\tr\t\tMostrar registros y flags");
			WriteLine("\ti\t\tMostrar interrupciones");
			WriteLine("\tm dir long\tMuestra 'long' bytes de memoria desde 'dir'");
			WriteLine("\te len\t\tEjecuta 'len' instrucciones a partir del PC actual");
			WriteLine("\tx\t\tReinicia la consola");
			WriteLine("\ta reg valor\tAsigna 'valor' al registro 'reg'");
			WriteLine("\tf flag\t\tCambia el estado del flag 'flag' (Z, N, H, C)");
			WriteLine("\tw dir valor\tEscribe 'valor' en la direccion 'dir' de memoria");
			WriteLine("\tn int 1|0\tActiva o desactiva la interrupcion 'int' (vblank, lcdc, timer, serialtx, key)");
			WriteLine("\tk key\t\tCambia el estado de la tecla 'key' (down, up, left, right, start, select, a, b)");
			WriteLine("\tb dir\t\tInserta o elimina un punto de interrupcion en la direccion 'dir'");
			WriteLine("\tq\t\tSalir de la consola de depuracion");
			WriteLine("\t<CTRL> + c\tSalir de CSGBE");
		}
	}
}
