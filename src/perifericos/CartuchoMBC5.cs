/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# GameBoy Cartridge MBC5
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

	/// <summary>Cartucho MBC5</summary>
	public class CartuchoMBC5 : Cartucho{

		/// <summary>Definicion del tipo de cartucho</summary>
		private static string tipoCartucho = "MBC5";
		/// <summary>Determina si la zona de RAM esta habilitada</summary>
		private bool _ramEnabled = false;
		/// <summary>Banco de ROM que se encuentra actualmente proyectado</summary>
		private int _romPage = 1;
		/// <summary>Banco de RAM que se encuentra actualmente proyectado</summary>
		private int _ramPage = 0;

		/// <summary>Constructor</summary>
		/// <param name="nombreFichero">Ruta al fichero del cartucho</param>
		public CartuchoMBC5(string nombreFichero) : base(nombreFichero){}

		/// <summary>Devuelve el nombre del tipo de cartucho</summary>
		/// <returns>El nombre del tipo de cartucho</returns>
		public override string getTipo(){ return tipoCartucho; }

		/// <summary>Lee un byte de la ROM del cartucho</summary>
		/// <param name="direccion">Direccion a leer</param>
		/// <returns>Byte posicionado en la direccion</returns>
                public override byte leer(int direccion){
			byte valor = 0;
			if (direccion >= 0){
				// Banco 0
				if (direccion < 0x4000) valor = _rom[direccion];
				// Banco 1-n
				else if (direccion < 0x8000) valor = _rom[(direccion - 0x4000) + (_romPage * 0x4000)];
				// Banco 0-n de RAM
				else if (direccion >= 0xA000 && direccion < 0xC000 && _ramEnabled) valor = _ram[(direccion - 0xA000) + (_ramPage * 0x2000)];
			}
			return valor;
		}
		
		/// <summary>Escribe un byte en una direccion del cartucho</summary>
		/// <param name="valor">Byte a escribir</param>
		/// <param name="direccion">Direccion donde escribir</param>
		public override void escribir(int valor, int direccion){
			// 0-0x2000: Activa a desctiva la RAM
			if (direccion < 0x2000) _ramEnabled = (valor & 0x0F) == 0x0A ? true : false;
			// 0x2000-0x3000: Seleccion parte baja del banco de ROM
			else if (direccion < 0x3000) _romPage = (_romPage & 0x100) | (valor & 0xFF); 
			// 0x3000-0x4000: Seleccion parte alta del banco de ROM
			else if (direccion < 0x4000) _romPage |= ((valor & 0x01) << 8);
			// 0x4000-0x6000: Seleccion banco de RAM
			else if (direccion < 0x6000) _ramPage = valor & 0x0F;
			// RAM
			else if (direccion >= 0xA000 && direccion < 0xC000 && _ramEnabled) _ram[(direccion - 0xA000) + (_ramPage * 0x2000)] = (byte)valor;
		}
	}
}
