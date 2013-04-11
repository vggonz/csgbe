/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Logic Assembly Instructions
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

	/// <summary>Instruccion XOR sobre registro de 8 bits</summary>
	/// <remarks>Realiza la operacion logica XOR de un registro o direccion con el valor del acumulador</remarks>
	public class InstruccionXOR_R : Instruccion{

		/// <summary>Registro con el que realiza la operacion</summary>
		private string _registro;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.regA ^= _registros.getReg(_registro);
			_registros.regA &= 0xFF;
			_registros.setFlag(0);
			if (_registros.regA == 0) _registros.flagZ = true; else _registros.flagZ = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
                /// <param name="registro">Registro afectado</param>
		public InstruccionXOR_R(Registros registros, Memoria memoria, string registro) : base(registros, memoria){
			_nombre = "XOR A," + registro;
			_longitud = 1;
			_duracion = 4;

			_registro = registro;
		}
	}

	/// <summary>Instruccion XOR sobre direccion de memoria almacenada en registro de 16 bits</summary>
	/// <remarks>Realiza la operacion logica XOR de un registro o direccion con el valor del acumulador</remarks>
	public class InstruccionXOR_RADR : Instruccion{

		/// <summary>Registro con el que realiza la operacion</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			int res = _memoria.leer(direccion);
			
			_registros.regA ^= res;
			_registros.regA &= 0xFF;
			_registros.setFlag(0);
			if (_registros.regA == 0) _registros.flagZ = true; else _registros.flagZ = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
                /// <param name="regDestino">Registro afectado</param>
		public InstruccionXOR_RADR(Registros registros, Memoria memoria, string regDestino) : base(registros, memoria){
			_nombre = "XOR (" + regDestino + ")";
			_longitud = 1;
			_duracion = 7;

			_regDestino = regDestino;
		}
	}

	/// <summary>Instruccion XOR con un valor literal</summary>
	/// <remarks>Realiza la operacion logica XOR del acumulador con un valor literal</remarks>
	public class InstruccionXOR_N : Instruccion{

		/// <summary>Valor literal para la operacion</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.regA ^= _valor;
			_registros.regA &= 0xFF;
			_registros.setFlag(0);
			if (_registros.regA == 0) _registros.flagZ = true; else _registros.flagZ = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
                /// <param name="valor">Valor literal</param>
		public InstruccionXOR_N(Registros registros, Memoria memoria, int valor) : base(registros, memoria){
			_nombre = "XOR " + valor;
			_longitud = 2;
			_duracion = 7;

			_valor = valor;
		}
	}

	/// <summary>Instruccion OR con un registro de 8 bits</summary>
	/// <remarks>Realiza la operacion logica OR del acumulador con un registro o direccion</remarks>
	public class InstruccionOR_R_R : Instruccion{

		/// <summary>Registro con el que realiza la operacion</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.regA |= _registros.getReg(_regOrigen);

			_registros.setFlag(0);
			if (_registros.regA == 0x00) _registros.flagZ = true; else _registros.flagZ = false;
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
                /// <param name="regOrigen">Registro usado para la operacion</param>
		public InstruccionOR_R_R(Registros registros, Memoria memoria, string regOrigen) : base(registros, memoria){
			_nombre = "OR A," + regOrigen;
			_longitud = 1;
			_duracion = 4;

			_regOrigen = regOrigen;
		}

	}

	/// <summary>Instruccion AND con un registro de 8 bits</summary>
	/// <remarks>Realiza la operacion logica AND del acumulador con un registro o direccion</remarks>
	public class InstruccionAND_R_R : Instruccion{

		/// <summary>Registro con el que realiza la operacion</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.regA &= _registros.getReg(_regOrigen);

			if (_registros.regA == 0x00) _registros.flagZ = true; else _registros.flagZ = false;
			_registros.flagH = true;
			_registros.flagN = false;
			_registros.flagC = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
                /// <param name="regOrigen">Registro usado para la operacion</param>
		public InstruccionAND_R_R(Registros registros, Memoria memoria, string regOrigen) : base(registros, memoria){
			_nombre = "AND A," + regOrigen;
			_longitud = 1;
			_duracion = 4;

			_regOrigen = regOrigen;
		}
	}
			
	/// <summary>Instruccion AND con un valor literal</summary>
	/// <remarks>Realiza la operacion logica AND del acumulador con un valor literal</remarks>
	public class InstruccionAND_R_N : Instruccion{

		/// <summary>Valor literal para la operacion</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.regA &= _valor;

			if (_registros.regA == 0x00) _registros.flagZ = true; else _registros.flagZ = false;
			_registros.flagH = true;
			_registros.flagN = false;
			_registros.flagC = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
                /// <param name="valor">Valor literal</param>
		public InstruccionAND_R_N(Registros registros, Memoria memoria, int valor) : base(registros, memoria){
			_nombre = "AND A," + valor;
			_longitud = 2;
			_duracion = 7;

			_valor = valor;
		}
	}

	/// <summary>Instruccion AND con un registro de 8 bits</summary>
	/// <remarks>Realiza la operacion logica AND del acumulador con un registro o direccion</remarks>
	public class InstruccionAND_RADR : Instruccion {

		/// <summary>Registro con el que realiza la operacion</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			int res = _memoria.leer(direccion);
			
			_registros.regA &= res;
			if (_registros.regA == 0x00) _registros.flagZ = true; else _registros.flagZ = false;
			_registros.flagH = true;
			_registros.flagN = false;
			_registros.flagC = false;
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
                /// <param name="regDestino">Registro usado para la operacion</param>
		public InstruccionAND_RADR(Registros registros, Memoria memoria, string regDestino) : base(registros, memoria){
			_nombre = "AND (" + regDestino + ")";
			_longitud = 1;
			_duracion = 7;

			_regDestino = regDestino;
		}
	}

	/// <summary>Instruccion OR con una direccion de memoria almacenada en un registro de 16 bits</summary>
	/// <remarks>Realiza la operacion logica OR del acumulador con un registro o direccion</remarks>
	public class InstruccionOR_R_ADR : Instruccion{

		/// <summary>Registro con el que realiza la operacion</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int direccion = _registros.getReg(_regOrigen);
			int valor = _memoria.leer(direccion);
			_registros.regA |= valor;

			_registros.setFlag(0);
			if (_registros.regA == 0x00) _registros.flagZ = true; else _registros.flagZ = false;

			_registros.regPC += _longitud;

			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
                /// <param name="regOrigen">Registro usado para la operacion</param>
		public InstruccionOR_R_ADR(Registros registros, Memoria memoria, string regOrigen) : base(registros, memoria){
			_nombre = "OR A,(" + regOrigen + ")";
			_longitud = 1;
			_duracion = 7;

			_regOrigen = regOrigen;
		}

	}

	/// <summary>Instruccion OR con un valor literal</summary>
	/// <remarks>Realiza la operacion logica OR del acumulador con un valor literal</remarks>
	public class InstruccionOR_N : Instruccion {

		/// <summary>Valor literal para la operacion</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.regA |= _valor;

			_registros.setFlag(0);
			if (_registros.regA == 0x00) _registros.flagZ = true; else _registros.flagZ = false;
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
                /// <param name="valor">Valor literal</param>
		public InstruccionOR_N(Registros registros, Memoria memoria, int valor) : base(registros, memoria) {
			_valor = valor;

			_nombre = "OR A," + valor;
			_longitud = 2;
			_duracion = 7;
		}
	}
}
