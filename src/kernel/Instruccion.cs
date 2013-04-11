/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Generic Assembly Instruction
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

	using System;

	/// <summary>Instruccion de ensamblador</summary>
	public abstract class Instruccion{

		/// <summary>Nombre mnemonico de la instruccion</summary>
		protected String _nombre;
		/// <summary>Longitud en bytes de la instruccion contando sus parametros</summary>
		protected int _longitud;
		/// <summary>Duracion teorica de ejecucion en ciclos</summary>
		protected int _duracion;
		/// <summary>Registros de la CPU</summary>
		protected Registros _registros;
		/// <summary>Memoria</summary>
		protected Memoria _memoria;

		/// <summary>Nombre personalizado de la instruccion</summary>
		public String nombre{ get{ return _nombre; } }
		/// <summary>Longitud en bytes de la instruccion</summary>
		public int longitud{ get{ return _longitud; } }
		/// <summary>Duracion teorica de ejecucion en ciclos</summary>
		public int duracion{ get{ return _duracion; } }

		/// <summary>Constructor</summary>
		/// <param name="registros">Registros de la CPU</param>
		/// <param name="memoria">Memoria</param>
		public Instruccion(Registros registros, Memoria memoria){
			_registros = registros;
			_memoria = memoria;
		}

		/// <summary>Ejecutar la instruccion</summary>
		/// <returns>La duracion teorica de la ejecucion</returns>
		public abstract int ejecutar();
	}
}
