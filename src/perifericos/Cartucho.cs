/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Generic Cartridge Object
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

namespace csgbe.perifericos{

	using System.IO;

	/// <summary>Representa un cartucho de GameBoy con la ROM</summary>
	public abstract class Cartucho{

		/// <summary>Ruta al fichero que contiene la ROM</summary>
		protected string _nombreFichero;
		/// <summary>Nombre interno de la ROM</summary>
		protected string _nombre;
		/// <summary>Numero de bloques de programa (16 Kb cada uno)</summary>
		protected int _romBloques;
		/// <summary>Numero de bloques de RAM internos del cartucho (8 Kb cada uno). Puede ser 0 si no tiene</summary>
		protected int _ramBloques;
		/// <summary>ROM</summary>
		protected byte[] _rom;
		/// <summary>RAM. Puede ser un array vacio si no tiene</summary>
		protected byte[] _ram;

		/// <summary>Nombre interno del cartucho</summary>
		public string nombre{ get { return _nombre; } }

		/// <summary>Constructor de cartuchos</summary>
		/// <param name="nombreFichero">Ruta al fichero que contiene la ROM</param>
		public Cartucho(string nombreFichero){
			_nombreFichero = nombreFichero;
			cargar();
		}

		/// <summary>Carga en memoria toda la ROM del cartucho y la RAM si tiene</summary>
		private void cargar(){
			// Primero carga los primeros 16 Kb (tamanyo minimo) para conocer el tamanyo real del cartucho
			FileStream fs = File.OpenRead(_nombreFichero);
			_rom = new byte[0x4000];
			fs.Read(_rom, 0, 0x4000);
			switch(_rom[0x0148]){
				case 0: _romBloques = 2; break;
				case 1: _romBloques = 4; break;
				case 2: _romBloques = 8; break;
				case 3: _romBloques = 16; break;
				case 4: _romBloques = 32; break;
				case 5: _romBloques = 64; break;
				case 6: _romBloques = 128; break;
				case 0x52: _romBloques = 72; break;
				case 0x53: _romBloques = 80; break;
				case 0x54: _romBloques = 96; break;
				default: _romBloques = 0; break;
			}

			switch(_rom[0x149]){
				case 0: _ramBloques = 0; break;
				case 1: _ramBloques = 1; break;
				case 2: _ramBloques = 1; break;
				case 3: _ramBloques = 4; break;
				case 4: _ramBloques = 16; break;
				case 5: _ramBloques = 32; break;
				default: _ramBloques = 0; break;
			}
			
			fs.Close();
			fs = File.OpenRead(_nombreFichero);
			                                                                                           
			_rom = new byte[0x4000 * _romBloques]; // Bloques de 16 Kb
			
			fs.Read(_rom, 0, _rom.Length);
			fs.Close();

			_ram = new byte[0x2000 * _ramBloques]; // Bloques de 8 Kb
			
			// Lee el nombre interno de la ROM
			for (int i = 0x134; i <= 0x142; i++) _nombre = _nombre + (char)_rom[i];
		}

		/// <summary>Devuelve informacion sobre el cartucho cargado</summary>
		/// <returns>Informacion sobre el cartucho: tipo, tamanyo de la ROM, tamanyo de la RAM, nombre interno, y resultado
		/// del test de checksum</returns>
		public string info(){
			return 	"\nTipo: " + getTipo() + " (" + _rom[0x0147] + ")" +
				"\nRom: " + _romBloques + " (" + (_romBloques * 16) + " Kb)" +
				"\nRam: " + _ramBloques + " (" + (_ramBloques * 8) + " Kb)" +
				"\nCheckSum: " + checksum() +
				"\nNombre: " + _nombre;
		}

		/// <summary>Obtiene el nombre del tipo de cartucho</summary>
		/// <returns>El nombre del tipo de cartucho</returns>
		public abstract string getTipo();
		/// <summary>Lee un byte de una direccion</summary>
		/// <param name="direccion">Direccion del cartucho</param>
		/// <returns>Byte leido de la direccion</returns>
		public abstract byte leer(int direccion);
		/// <summary>Escribe un byte en una direccion</summary>
		/// <param name="valor">Valor a escribir</param>
		/// <param name="direccion">Direccion donde se quiere escribir</param>
		public abstract void escribir(int valor, int direccion);

		/// <summary>Comprueba la integridad de la ROM mediante una operacion de checksum</summary>
		/// <returns>true si la prueba tiene exito, false en caso contrario</returns>
		private bool checksum(){
			// El resultado del checksum son 2 bytes
			int checkSum = (_rom[0x14E] << 8) + _rom[0x14F];
			int total = 0;
			// Se suman todos los bytes de la rom a excepcion de los del resultado
			for (int i = 0; i < _rom.Length; i++) {
				if (i != 0x14E && i != 0x14F) {
					total = (total + _rom[i]) & 0x0000FFFF;
				}
			}
			return checkSum == total;
		}

		/// <summary>Crea un tipo especifico de cartucho</summary>
		/// <param name="nombreFichero">Ruta al fichero del cartucho</param>
		/// <returns>Cartucho listo para leer y escribir</returns>
		public static Cartucho cargarCartucho(string nombreFichero){
			Cartucho cartucho;
			// Carga los primeros 16 kb del cartucho (tamanyo minimo) para conocer el tipo de cartucho
			FileStream fs = File.OpenRead(nombreFichero);
			byte[] rom = new byte[0x4000];
			fs.Read(rom, 0, 0x4000);
			fs.Close();
			
			switch(rom[0x0147]){
				case 0x00: cartucho = new CartuchoMBC0(nombreFichero); break; // ROM Only
				case 0x01: cartucho = new CartuchoMBC1(nombreFichero); break; // ROM + MBC1
				case 0x02: cartucho = new CartuchoMBC1(nombreFichero); break; // ROM + MBC1 + RAM
				case 0x03: cartucho = new CartuchoMBC1(nombreFichero); break; // ROM + MBC1 + RAM + BATTERY
//				case 0x04:
				case 0x05: cartucho = new CartuchoMBC2(nombreFichero); break; // ROM + MBC2
				case 0x06: cartucho = new CartuchoMBC2(nombreFichero); break; // ROM + MBC2 + BATTERY
//				case 0x07:
				case 0x08: cartucho = new CartuchoMBC0(nombreFichero); break; // ROM + RAM
				case 0x09: cartucho = new CartuchoMBC0(nombreFichero); break; // ROM + RAM + BATTERY
//				case 0x0A:
//				case 0x0B: cartucho = new CartuchoMBC0(nombreFichero); break; // ROM + MMM01
//				case 0x0C: cartucho = new CartuchoMBC0(nombreFichero); break; // ROM + MMM01 + SRAM
//				case 0x0D: cartucho = new CartuchoMBC0(nombreFichero); break; // ROM + MMM01 + SRAM + BATTERY
//				case 0x0E:
//				case 0x0F: cartucho = new CartuchoMBC3(nombreFichero); break; // ROM + MBC3 + TIMER + BATTERY
//				case 0x10: cartucho = new CartuchoMBC3(nombreFichero); break; // ROM + MBC3 + TIMER + RAM + BATTERY
				case 0x11: cartucho = new CartuchoMBC3(nombreFichero); break; // ROM + MBC3
				case 0x12: cartucho = new CartuchoMBC3(nombreFichero); break; // ROM + MBC3 + RAM
				case 0x13: cartucho = new CartuchoMBC3(nombreFichero); break; // ROM + MBC3 + RAM + BATTERY
//				case 0x14: 
//				case 0x15: cartucho = new CartuchoMBC4(nombreFichero); break; // ROM + MBC4
//				case 0x16: cartucho = new CartuchoMBC4(nombreFichero); break; // ROM + MBC4 + RAM
//				case 0x17: cartucho = new CartuchoMBC4(nombreFichero); break; // ROM + MBC4 + RAM + BATTERY
//				case 0x18: 
				case 0x19: cartucho = new CartuchoMBC5(nombreFichero); break; // ROM + MBC5
				case 0x1A: cartucho = new CartuchoMBC5(nombreFichero); break; // ROM + MBC5 + RAM
				case 0x1B: cartucho = new CartuchoMBC5(nombreFichero); break; // ROM + MBC5 + RAM + BATTERY
//				case 0x1C: cartucho = new CartuchoMBC5(nombreFichero); break; // ROM + MBC5 + RUMBLE
//				case 0x1D: cartucho = new CartuchoMBC5(nombreFichero); break; // ROM + MBC5 + RUMBLE + SRAM
//				case 0x1E: cartucho = new CartuchoMBC5(nombreFichero); break; // ROM + MBC5 + RUMBLE + SRAM + BATTERY
//				case 0x1F: cartucho = new CartuchoMBC0(nombreFichero); break; // POCKET CAMERA
				
//				case 0xFD: cartucho = new CartuchoMBC0(nombreFichero); break; // BANDAI TAMA5
//				case 0xFE: cartucho = new CartuchoMBC3(nombreFichero); break; // HUDSON HuC-3
//				case 0xFF: cartucho = new CartuchoMBC1(nombreFichero); break; // HUDSON HuC-1
				
				default: throw new System.Exception("[ERROR] Tipo de cartucho " + rom[0x0147] + " desconocido");
			}

			if (rom[0x143] == 0x80) throw new System.Exception("[ERROR] GameBoy Color no soportada");
//			if (rom[0x146] != 0x00) throw new System.Exception("[ERROR] SuperGameBoy no soportada");

			return cartucho;
		}

	}

}
