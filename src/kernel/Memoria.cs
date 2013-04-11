/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# System Main Memory
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

namespace csgbe.kernel{

	using csgbe.perifericos;

	/// <summary>Memoria</summary>	
	public class Memoria : Constantes{

		/// <summary>Array con toda la memoria principal</summary>
		private byte[] _ram;
		/// <summary>Cartucho cargado</summary>
		private Cartucho _cartucho;
		/// <summary>Numero de lecturas realizadas</summary>
		private static int _lecturas;
		/// <summary>Numero de escrituras realizadas</summary>
		private static int _escrituras;

		/// <summary>Lecturas</summary>
		public int lecturas { get { return _lecturas; } }
		/// <summary>Escrituras</summary>
		public int escrituras { get { return _escrituras; } }
		/// <summary>Acceso alternativo a la memoria principal. No se aconseja su uso salvo casos MUY excepcionales</summary>
		public byte[] ram { get { return _ram; } }
		
		/// <summary>Constructor</summary>
		/// <param name="tamanyo">Tamanyo en bytes</param>
		/// <param name="cartucho">Cartucho cargado</param>
		public Memoria(int tamanyo, Cartucho cartucho){
			_ram = new byte[tamanyo];
			_cartucho = cartucho;
			_lecturas = 0;
			_escrituras = 0;
		}

		/// <summary>Lee una posicion de memoria</summary>
		/// <param name="direccion">Direccion a leer</param>
		/// <returns>Byte posicionado en la direccion</returns>
		public byte leer(int direccion){
			byte valor = 0;
			try{
				direccion &= 0xFFFF;
				// 0-0x8000: ROM del cartucho
				if (direccion >= 0 && direccion < 0x8000) valor = _cartucho.leer(direccion);
				// 0xA000-0xC000: RAM del cartucho
				else if (direccion >= 0xA000 && direccion < 0xC000) valor = _cartucho.leer(direccion);
				else valor = _ram[direccion];
				_lecturas++;
			}catch(System.Exception e){ throw new System.Exception("[ERROR] No se puede leer en " + perifericos.Debug.hexWord(direccion)); }
			return valor;
		}

		/// <summary>Escribe un valor en una direccion de memoria</summary>
		/// <remarks>Esta funcion solo debe ser accedida por las instrucciones, el resto de objetos como
		/// perifericos de pantalla o teclado deberan acceder directamente a la memoria sin pasar
		/// por estas funciones porque podrian producir un bucle infinito</remarks>
		/// <param name="valor">Nuevo valor para la direccion de memoria</param>
		/// <param name="direccion">Posicion en la memoria para escribir el nuevo valor</param>
		public void escribir(int valor, int direccion){
			try{
				direccion &= 0xFFFF;
				valor &= 0xFF;
				// 0-0x8000: ROM del cartucho
				if (direccion >= 0 && direccion < 0x8000) _cartucho.escribir(valor, direccion);
				// 0xA000-0xC000: RAM del cartucho
				else if (direccion >= 0xA000 && direccion < 0xC000) _cartucho.escribir(valor, direccion);
				// Echo Memory??
				else if (direccion >= 0xC000 && direccion < 0xE000) _ram[direccion] = /*_ram[direccion + 0x2000] =*/ (byte)valor;
				// 0xFF00-0xFFFF: Registros de IO
				else if (direccion >= 0xFF00) escribirIO(valor, direccion);
				else _ram[direccion] = (byte)valor;
				_escrituras++;
			}catch(System.Exception e){ throw new System.Exception("[ERROR] No se puede escribir " + perifericos.Debug.hexWord(valor) + " en " + perifericos.Debug.hexWord(direccion)); }
		}

		/// <summary>Escribe y realiza un tratamiento especial en las direccion de entrada / salida</summary>
		/// <param name="valor">Nuevo valor</param>
		/// <param name="direccion">Direccion para escribir el nuevo valor</param>
		private void escribirIO(int valor, int direccion){
			switch(direccion){
//				case LCD_Y_LOC: _ram[LCD_Y_LOC] = 0x00; break;
				// Actualiza la pulsacion de las teclas en cuanto recibe la solicitud
				case JOYPAD: _ram[JOYPAD] = (byte)valor; Keypad.actualizar(this); break;
				// Transferencia DMA de 160 bytes a partir de la direccion dada a 0xFE00
				case LCD_DMA: _ram[LCD_DMA] = (byte)valor; System.Array.Copy(_ram, _ram[LCD_DMA] << 8, _ram, 0xFE00, 0xA0); break;
				// Reset del control de DIV
				case DIV_CNTR: _ram[DIV_CNTR] = 0x00; break;
				default: _ram[direccion] = (byte)valor; break;
			}
		}
	}
}
