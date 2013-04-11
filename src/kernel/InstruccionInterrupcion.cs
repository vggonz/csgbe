/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Interruptions Assembly Instructions
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

	/// <summary>Instruccion DI</summary>
	/// <remarks>Deshabilita el IME para impedir la ejecucion de interrupciones</remarks>
	public class InstruccionDI : Instruccion {

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.flagIME = false;
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		public InstruccionDI(Registros registros, Memoria memoria) : base (registros, memoria){
			_nombre = "DI";
			_longitud = 1;
			_duracion = 4;
		}
	}

	/// <summary>Instruccion EI</summary>
	/// <remarks>Habilita el IME para permitir la ejecucion de interrupciones</remarks>
	public class InstruccionEI : Instruccion {
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.flagIME = true;
			_registros.regPC += _longitud;
			return _duracion;
		}
	
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		public InstruccionEI(Registros registros, Memoria memoria) : base (registros, memoria) {
			_nombre = "EI";
			_longitud = 1;
			_duracion = 4;
		}
	}

	/// <summary>Instruccion HALT</summary>
	/// <remarks>Detiene la ejecucion hasta que ocurra una interrupcion</remarks>
	public class InstruccionHALT : Instruccion {

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			if (_registros.flagIME == true && (_memoria.leer(Constantes.INT_FLAG) & _memoria.leer(Constantes.INT_ENABLE)) > 0) _registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		public InstruccionHALT(Registros registros, Memoria memoria) : base (registros, memoria) {
			_nombre = "HALT";
			_longitud = 1;
			_duracion = 4;
		}
	}

	/// <summary>Instruccion STOP</summary>
	/// <remarks>Detiene la ejecucion hasta que se pulse una tecla</remarks>
	public class InstruccionSTOP : Instruccion {

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		public InstruccionSTOP(Registros registros, Memoria memoria) : base (registros, memoria) {
			_nombre = "STOP";
			_longitud = 2;
			_duracion = 4;
		}
	}
}
