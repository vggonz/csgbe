/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Bit Assembly Instructions
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

	/// <summary>Instruccion BIT sobre registro de 8 bits</summary>
	/// <remarks>Comprueba si un determinado bit de un registro o direccion esta a 0 activando el flag Z en ese caso</remarks>
	public class InstruccionBIT_R : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _registro;
		/// <summary>Mascara sobre el bit</summary>
		private byte _mask;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			if ((_mask & _registros.getReg(_registro)) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			_registros.flagH = true;
			_registros.flagN = false;
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="mask">Mascara con el bit afectado</param>
		/// <param name="registro">Registro afectado</param>
		public InstruccionBIT_R(Registros registros, Memoria memoria, byte mask, string registro) : base(registros, memoria) {
			_nombre = "BIT " + mask + "," + registro;
			_longitud = 1;
			_duracion = 8;
		
			_registro = registro;
			_mask = mask;
		}
	}

	/// <summary>Instruccion BIT sobre direccion de memoria almacenada en registro de 16 bits</summary>
	/// <remarks>Comprueba si un determinado bit de un registro o direccion esta a 0 activando el flag Z en ese caso</remarks>
	public class InstruccionBIT_RADR : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _registro;
		/// <summary>Mascara sobre el bit</summary>
		private byte _mask;
	
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			if ((_mask & _memoria.leer(_registros.getReg(_registro))) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			_registros.flagH = true;
			_registros.flagN = false;
			
			_registros.regPC += _longitud;
			return _duracion;
		}
	
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="mask">Mascara con el bit afectado</param>
		/// <param name="registro">Registro afectado</param>
		public InstruccionBIT_RADR(Registros registros, Memoria memoria, byte mask, string registro) : base(registros, memoria) {
			_nombre = "BIT " + mask + ", (" + registro + ")";
			_longitud = 1;
			_duracion = 10;
		
			_registro = registro;
			_mask = mask;
		}
	}

	/// <summary>Instruccion SET sobre registro de 8 bits</summary>
	/// <remarks>Modifica a 1 un determinado bit de un registro o direccion</remarks>
	public class InstruccionSET_R : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _regDestino;
		/// <summary>Mascara sobre el bit</summary>
		private byte _mask;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.setReg(_regDestino, _registros.getReg(_regDestino) | _mask);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="mask">Mascara con el bit afectado</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSET_R(Registros registros, Memoria memoria, byte mask, string regDestino) : base(registros, memoria){
			_nombre = "SET " + mask + ", " + regDestino;
			_longitud = 1;
			_duracion = 8;

			_regDestino = regDestino;
			_mask = mask;
		}
	}

	/// <summary>Instruccion SET sobre direccion de memoria almacenada en registro de 16 bits</summary>
	/// <remarks>Modifica a 1 un determinado bit de un registro o direccion</remarks>
	public class InstruccionSET_RADR : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _regDestino;
		/// <summary>Mascara sobre el bit</summary>
		private byte _mask;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			_memoria.escribir(_memoria.leer(direccion) | _mask, direccion);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="mask">Mascara con el bit afectado</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSET_RADR(Registros registros, Memoria memoria, byte mask, string regDestino) : base(registros, memoria){
			_nombre = "SET " + mask + ", (" + regDestino + ")";
			_longitud = 1;
			_duracion = 8;

			_regDestino = regDestino;
			_mask = mask;
		}
	}
	
	/// <summary>Instruccion RES sobre registro de 8 bits</summary>
	/// <remarks>Modifica a 0 un determinado bit de un registro o direccion</remarks>
	public class InstruccionRES_R : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _regDestino;
		/// <summary>Mascara sobre el bit</summary>
		private byte _mask;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.setReg(_regDestino, _registros.getReg(_regDestino) & ~_mask);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="mask">Mascara con el bit afectado</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionRES_R(Registros registros, Memoria memoria, byte mask, string regDestino) : base(registros, memoria){
			_nombre = "RES " + mask + ", " + regDestino;
			_longitud = 1;
			_duracion = 8;

			_regDestino = regDestino;
			_mask = mask;
		}
	}
	
	/// <summary>Instruccion RES sobre direccion de memoria almacenada en registro de 16 bits</summary>
	/// <remarks>Modifica a 0 un determinado bit de un registro o direccion</remarks>
	public class InstruccionRES_RADR : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _regDestino;
		/// <summary>Mascara sobre el bit</summary>
		private byte _mask;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			_memoria.escribir(_memoria.leer(direccion) & ~_mask, direccion);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="mask">Mascara con el bit afectado</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionRES_RADR(Registros registros, Memoria memoria, byte mask, string regDestino) : base(registros, memoria){
			_nombre = "RES " + mask + ", (" + regDestino + ")";
			_longitud = 1;
			_duracion = 11;

			_regDestino = regDestino;
			_mask = mask;
		}
	}
	
	/// <summary>Instruccion SWAP sobre registro de 8 bits</summary>
	/// <remarks>Intercambia la parte alta y baja del registro o direccion</remarks>
	public class InstruccionSWAP_R : Instruccion {

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			byte reg = (byte)_registros.getReg(_regDestino);
			reg = (byte)(((reg & 0x0F) << 4) | ((reg & 0xF0) >> 4));
			_registros.setReg(_regDestino, reg);
			_registros.setFlag(0);
			if (reg == 0) _registros.flagZ = true; else _registros.flagZ = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSWAP_R(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria) {
			_nombre = "SWAP " + regDestino;
			_regDestino = regDestino;

			_longitud = 1;
			_duracion = 8;
		}
	}
	
	/// <summary>Instruccion SWAP sobre direccion de memoria almacenada en registro de 16 bits</summary>
	/// <remarks>Intercambia la parte alta y baja del registro o direccion</remarks>
	public class InstruccionSWAP_RADR : Instruccion {

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			byte reg = (byte)_memoria.leer(_registros.getReg(_regDestino));
			reg = (byte)(((reg & 0x0F) << 4) | ((reg & 0xF0) >> 4));
			_memoria.escribir(reg, _registros.getReg(_regDestino));
			_registros.setFlag(0);
			if (reg == 0) _registros.flagZ = true; else _registros.flagZ = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSWAP_RADR(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria) {
			_nombre = "SWAP (" + regDestino + ")";
			_regDestino = regDestino;

			_longitud = 1;
			_duracion = 16;
		}
	}
}
