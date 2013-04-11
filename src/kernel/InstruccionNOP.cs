/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Special NOP Assembly Instruction
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

	/// <summary>Instruccion NOP</summary>
	/// <remarks>No realiza ninguna operacion</remarks>
	public class InstruccionNOP:Instruccion{

		/// <summary>Ejecuta la instruccion</summary>
		/// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			_registros.regPC += _longitud;
			return _duracion;
		}

		/// <summary>Constructor</summary>
		/// <param name="registros">Registros de la CPU</param>
		/// <param name="memoria">Memoria</param>
		public InstruccionNOP(Registros registros, Memoria memoria) : base(registros, memoria){
			_nombre = "NOP";
			_longitud = 1;
			_duracion = 4;
		}
	}
}
