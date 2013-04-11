/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# GameBoy Cartridge MBC0 (only ROM without RAM)
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

	/// <summary>Cartucho MBC0 (solo ROM sin RAM)</summary>
	public class CartuchoMBC0 : Cartucho{

		/// <summary>Definicion del tipo de cartucho</summary>
		private static string tipoCartucho = "MBC0";

		/// <summary>Constructor</summary>
		/// <param name="nombreFichero">Ruta al fichero del cartucho</param>
		public CartuchoMBC0(string nombreFichero) : base(nombreFichero){}

		/// <summary>Devuelve el nombre del tipo de cartucho</summary>
		/// <returns>El nombre del tipo de cartucho</returns>
		public override string getTipo(){ return tipoCartucho; }

		/// <summary>Lee un byte de la ROM del cartucho</summary>
		/// <param name="direccion">Direccion a leer</param>
		/// <returns>Byte posicionado en la direccion</returns>
                public override byte leer(int direccion){
			byte valor = 0;
			if (direccion >= 0 && direccion < _rom.Length) valor = _rom[direccion];
			return valor;
		}
		
		/// <summary>Escribe un byte en una direccion del cartucho. Funcion nula en este tipo de cartucho</summary>
		/// <param name="valor">Byte a escribir</param>
		/// <param name="direccion">Direccion donde escribir</param>
		public override void escribir(int valor, int direccion){
			// En los cartuchos MBC0 no se puede escribir ni para seleccionar banco de memoria
			// porque solo hay uno. Sin embargo, hay algunas ROMs de tipo MBC0 que intentan
			// escribir igualmente, asi que simplemente hay que ignorarlas y no interpretarlo
			// como un error de ejecucion.
		}
	}
}
