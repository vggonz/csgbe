/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Complement Assembly Instructions
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

	/// <summary>Instruccion CCF</summary>
	/// <remarks>Invierte el estado del registro C de carry</remarks>
	public class InstruccionCCF : Instruccion {
		
		/// <summary>Ejecuta la instruccion</summary>
		/// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar() {
			_registros.flagN = false;
			_registros.flagH = false;
			if (_registros.flagC == false) _registros.flagC = true; else _registros.flagC = false;
		
			_registros.regPC += _longitud;
			return _duracion;
		}
	
		/// <summary>Constructor</summary>
		/// <param name="registros">Registros de la CPU</param>
		/// <param name="memoria">Memoria</param>
		public InstruccionCCF(Registros registros, Memoria memoria) : base(registros, memoria) {
			_nombre = "CCF";
			_longitud = 1;
			_duracion = 4;
		}
	}

	/// <summary>Instruccion CPL</summary>
	/// <remarks>Realiza la operacion complemento sobre el valor del acumulador</remarks>
	public class InstruccionCPL:Instruccion {
		
		/// <summary>Ejecuta la instruccion</summary>
		/// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar() {
			_registros.flagN = true;
			_registros.flagH = true;
			
			_registros.regA ^= 0xFF;
			_registros.regA &= 0xFF;
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="registros">Registros de la CPU</param>
		/// <param name="memoria">Memoria</param>
		public InstruccionCPL(Registros registros, Memoria memoria):base(registros,memoria) {
			_nombre = "CPL";
			_longitud = 1;
			_duracion = 4;
		}
	}
	
	/// <summary>Instruccion SCF</summary>
	/// <remarks>Activa el flag C de carry</remarks>
	public class InstruccionSCF : Instruccion {
		
		/// <summary>Ejecuta la instruccion</summary>
		/// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar() {
			_registros.flagH = false;
			_registros.flagN = false;
			_registros.flagC = true;
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="registros">Registros de la CPU</param>
		/// <param name="memoria">Memoria</param>
		public InstruccionSCF(Registros registros, Memoria memoria):base(registros,memoria){
			_nombre = "SCF";
			_longitud = 1;
			_duracion = 4;
		}
	}
}
