/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Jump Assembly Instructions
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

	/// <summary>Instruccion JP</summary>
	/// <remarks>Realiza un salto incondicional a una direccion</remarks>
	public class InstruccionJP_ADR : Instruccion {
		/// <summary>Direccion de salto</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC = _direccion;
			return _duracion;
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="direccionBaja">Parte baja de la direccion de salto</param>
		/// <param name="direccionAlta">Parte alta de la direccion de salto</param>
		public InstruccionJP_ADR(Registros registros, Memoria memoria, int direccionBaja, int direccionAlta) : base(registros, memoria){
			_direccion = (direccionAlta << 8) | direccionBaja;

			_nombre = "JP " + _direccion;
			_longitud = 3;
			_duracion = 10;
		}
	}
	
	/// <summary>Instruccion JP</summary>
	/// <remarks>Realiza un salto incondicional a una direccion almacenada en un registro de 16 bits</remarks>
	public class InstruccionJP_RADR : Instruccion {
		/// <summary>Registro que almacena la direccion de salto</summary>
		private string _registro;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC = _registros.getReg(_registro);
			return _duracion;
		}
	
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="registro">Registro que contiene la nueva direccion</param>
		public InstruccionJP_RADR(Registros registros, Memoria memoria, string registro) : base(registros, memoria){
			_registro = registro;

			_nombre = "JP (" + registro + ")";
			_longitud = 1;
			_duracion = 4;
		}
	}

	/// <summary>Instruccion JP</summary>
	/// <remarks>Realiza un salto si un determinado flag no esta activo</remarks>
	public class InstruccionJP_CC0_ADR : Instruccion {
		/// <summary>Flag de comprobacion</summary>
		private string _flag;
		/// <summary>Direccion de salto</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			if (_registros.getFlag(_flag) == false) _registros.regPC = _direccion;
			return _duracion;
			
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="flag">Flag de condicion</param>
		/// <param name="direccionBaja">Parte baja de la direccion de salto</param>
		/// <param name="direccionAlta">Parte alta de la direccion de salto</param>
		public InstruccionJP_CC0_ADR(Registros registros, Memoria memoria, string flag, int direccionBaja, int direccionAlta) : base(registros, memoria){
			_direccion = (direccionAlta << 8) | direccionBaja;
			_flag = flag;
			
			_nombre = "JP N" + flag + " " + _direccion;
			_longitud = 3;
			_duracion = 10;
		}
	}
	
	/// <summary>Instruccion JP</summary>
	/// <remarks>Realiza un salto si un determinado flag esta activo</remarks>
	public class InstruccionJP_CC1_ADR : Instruccion {
		/// <summary>Flag de comprobacion</summary>
		private string _flag;
		/// <summary>Direccion de salto</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			if (_registros.getFlag(_flag) == true) _registros.regPC = _direccion;
			return _duracion;
			
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="flag">Flag de condicion</param>
		/// <param name="direccionBaja">Parte baja de la direccion de salto</param>
		/// <param name="direccionAlta">Parte alta de la direccion de salto</param>
		public InstruccionJP_CC1_ADR(Registros registros, Memoria memoria, string flag, int direccionBaja, int direccionAlta) : base(registros, memoria){
			_direccion = (direccionAlta << 8) | direccionBaja;
			_flag = flag;
			
			_nombre = "JP " + flag + " " + _direccion;
			_longitud = 3;
			_duracion = 10;
		}
	}

	/// <summary>Instruccion JR</summary>
	/// <remarks>Realiza un salto incondicional y relativo a la direccion actual</remarks>
	public class InstruccionJR_N : Instruccion {
		/// <summary>Desplazamiento</summary>
		private int _desp;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _desp;
			_registros.regPC += _longitud;

			return _duracion;
		}

		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="desp">Desplazamiento con signo (-127, 127)</param>
		public InstruccionJR_N(Registros registros, Memoria memoria, int desp) : base(registros, memoria){
			if (desp > 127)	_desp = desp - 256; else _desp = desp;
			
			_nombre = "JR " + _desp;
			_longitud = 2;
			_duracion = 12;
		}
	}

	/// <summary>Instruccion JR</summary>
	/// <remarks>Realiza un salto condicional si un flag no esta activo y relativo a la direccion actual</remarks>	
	public class InstruccionJR_CC0_N : Instruccion {
		/// <summary>Desplazamiento</summary>
		private int _desp;
		/// <summary>Flag de condicion</summary>
		private string _flag;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			
			if (_registros.getFlag(_flag) == false){
				_registros.regPC += _desp;
				_duracion = 12;
			}
			return _duracion;			
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="flag">Flag de condicion</param>
		/// <param name="desp">Desplazamiento con signo (-127, 127)</param>
		public InstruccionJR_CC0_N(Registros registros, Memoria memoria, string flag, int desp) : base(registros, memoria){
			_flag = flag;
			if (desp > 127)	_desp = desp - 256; else _desp = desp;
			
			_nombre = "JR N" + flag + "," + _desp;
			_longitud = 2;
			_duracion = 7;
		}
	}
	
	/// <summary>Instruccion JR</summary>
	/// <remarks>Realiza un salto condicional si un flag esta activo y relativo a la direccion actual</remarks>	
	public class InstruccionJR_CC1_N : Instruccion {
		/// <summary>Desplazamiento</summary>
		private int _desp;
		/// <summary>Flag de condicion</summary>
		private string _flag;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			if (_registros.getFlag(_flag) == true){
				_registros.regPC += _desp;
				_duracion = 12;
			}
			return _duracion;			
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="flag">Flag de condicion</param>
		/// <param name="desp">Desplazamiento con signo (-127, 127)</param>
		public InstruccionJR_CC1_N(Registros registros, Memoria memoria, string flag, int desp) : base(registros, memoria){
			_flag = flag;
			if (desp > 127) _desp = desp - 256; else _desp = desp;
			
			_nombre = "JR " + flag + "," + _desp;
			_longitud = 2;
			_duracion = 7;
		}
	}

	/// <summary>Instruccion CALL</summary>
	/// <remarks>Salta incondicionalmente a una direccion pero guarda en la pila la direccion actual</remarks>
	public class InstruccionCALL_ADR : Instruccion {
		/// <summary>Direccion de salto</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			
			_registros.regSP--;
			_memoria.escribir((_registros.regPC & 0xFF00) >> 8, _registros.regSP);

			_registros.regSP--;
			_memoria.escribir(_registros.regPC & 0x00FF, _registros.regSP);
			
			_registros.regPC = _direccion;
			return _duracion;
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="direccionBaja">Parte baja de la direccion de salto</param>
		/// <param name="direccionAlta">Parte alta de la direccion de salto</param>
		public InstruccionCALL_ADR(Registros registros, Memoria memoria, int direccionBaja, int direccionAlta) : base(registros, memoria){
			_direccion = (direccionAlta << 8) | direccionBaja;

			_nombre = "CALL " + _direccion;
			_longitud = 3;
			_duracion = 17;
		}
	}

	/// <summary>Instruccion CALL</summary>
	/// <remarks>Realiza un salto condicional si un flag no esta activo y guarda en la pila la direccion actual</remarks>
	public class InstruccionCALL_CC0_ADR : Instruccion {
		/// <summary>Flag de condicion</summary>
		private string _flag;
		/// <summary>Direccion de salto</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			
			if (_registros.getFlag(_flag) == false){
				_registros.regSP--;
				_memoria.escribir((_registros.regPC & 0xFF00) >> 8, _registros.regSP);
	
				_registros.regSP--;
				_memoria.escribir(_registros.regPC & 0x00FF, _registros.regSP);
			
				_registros.regPC = _direccion;
				
				_duracion = 17;
			}
			return _duracion;
			
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="flag">Flag de condicion</param>
		/// <param name="direccionBaja">Parte baja de la direccion de salto</param>
		/// <param name="direccionAlta">Parte alta de la direccion de salto</param>
		public InstruccionCALL_CC0_ADR(Registros registros, Memoria memoria, string flag, int direccionBaja, int direccionAlta) : base(registros, memoria){
			_direccion = (direccionAlta << 8) | direccionBaja;
			_flag = flag;
			
			_nombre = "CALL N" + flag + " " + _direccion;
			_longitud = 3;
			_duracion = 10;
		}
	}

	/// <summary>Instruccion CALL</summary>
	/// <remarks>Realiza un salto condicional si un flag esta activo y guarda en la pila la direccion actual</remarks>
	public class InstruccionCALL_CC1_ADR : Instruccion {
		/// <summary>Flag de condicion</summary>
		private string _flag;
		/// <summary>Direccion de salto</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			
			if (_registros.getFlag(_flag) == true){
				_registros.regSP--;
				_memoria.escribir((_registros.regPC & 0xFF00) >> 8, _registros.regSP);
	
				_registros.regSP--;
				_memoria.escribir(_registros.regPC & 0x00FF, _registros.regSP);
			
				_registros.regPC = _direccion;
				
				_duracion = 17;
			}
			return _duracion;
			
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="flag">Flag de condicion</param>
		/// <param name="direccionBaja">Parte baja de la direccion de salto</param>
		/// <param name="direccionAlta">Parte alta de la direccion de salto</param>
		public InstruccionCALL_CC1_ADR(Registros registros, Memoria memoria, string flag, int direccionBaja, int direccionAlta) : base(registros, memoria){
			_direccion = (direccionAlta << 8) | direccionBaja;
			_flag = flag;
			
			_nombre = "CALL " + flag + " " + _direccion;
			_longitud = 3;
			_duracion = 10;
		}
	}

	/// <summary>Instruccion PUSH</summary>
	/// <remarks>Guarda en la pila el valor de un registro de 16 bits</remarks>
	public class InstruccionPUSH_RR : Instruccion {
		/// <summary>Registro para guardar</summary>
		private string _registro;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.regSP--;
			_memoria.escribir((_registros.getReg(_registro) & 0xFF00) >> 8, _registros.regSP);

			_registros.regSP--;
			_memoria.escribir(_registros.getReg(_registro) & 0x00FF, _registros.regSP);

			_registros.regPC += longitud;
			return _duracion;
		}

		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="registro">Registro a guardar</param>
		public InstruccionPUSH_RR(Registros registros, Memoria memoria, string registro) : base (registros, memoria){

			_nombre = "PUSH " + registro;
			_longitud = 1;
			_duracion = 11;

			_registro = registro;
		}
	}

	/// <summary>Instruccion POP</summary>
	/// <remarks>Extrae de la pila dos valores hacia un registro de 16 bits</remarks>
	public class InstruccionPOP_RR : Instruccion {
		/// <summary>Registro destino</summary>
		private string _registro;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int valorL = _memoria.leer(_registros.regSP);
			_registros.regSP++;
			int valorH = _memoria.leer(_registros.regSP);
			_registros.regSP++;
			int valor = (valorH << 8) | valorL;

			_registros.setReg(_registro, valor);
			
			_registros.regPC += longitud;
			return _duracion;
		}

		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="registro">Registro destino</param>
		public InstruccionPOP_RR(Registros registros, Memoria memoria, string registro) : base (registros, memoria){

			_nombre = "POP " + registro;
			_longitud = 1;
			_duracion = 11;

			_registro = registro;
		}
	}
                       
	/// <summary>Instruccion RET</summary>
	/// <remarks>Extrae de la pila el contador de programa si no esta activo cierto flag</remarks>
	public class InstruccionRET_CC0_ADR : Instruccion {
		/// <summary>Flag de condicion</summary>
		private string _flag;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			
			if (_registros.getFlag(_flag) == false){
				int pcL = _memoria.leer(_registros.regSP);
				_registros.regSP++;
				
				int pcH = _memoria.leer(_registros.regSP);
				_registros.regSP++;
				
				int pc = (pcH << 8) | pcL;
				_registros.regPC = pc;
				
				_duracion = 11;
			}
			return _duracion;
			
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="flag">Flag de condicion</param>
		public InstruccionRET_CC0_ADR(Registros registros, Memoria memoria, string flag) : base(registros, memoria){
			_nombre = "RET N" + flag;
			_longitud = 1;
			_duracion = 5;
			
			_flag = flag;
		}
	}
	
	/// <summary>Instruccion RET</summary>
	/// <remarks>Extrae de la pila el contador de programa si esta activo cierto flag</remarks>
	public class InstruccionRET_CC1_ADR : Instruccion {
		/// <summary>Flag de condicion</summary>
		private string _flag;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			
			if (_registros.getFlag(_flag) == true){
				int pcL = _memoria.leer(_registros.regSP);
				_registros.regSP++;
				int pcH = _memoria.leer(_registros.regSP);
				_registros.regSP++;
				int pc = (pcH << 8) | pcL;
				_registros.regPC = pc;
				
				_duracion = 11;
			}
			return _duracion;
			
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="flag">Flag de condicion</param>
		public InstruccionRET_CC1_ADR(Registros registros, Memoria memoria, string flag) : base(registros, memoria){
			_nombre = "RET " + flag;
			_longitud = 1;
			_duracion = 5;
			
			_flag = flag;
		}
	}
	
	/// <summary>Instruccion RET</summary>
	/// <remarks>Extrae de la pila el contador de programa incondicionalmente</remarks>
	public class InstruccionRET_ADR : Instruccion {
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			
			int pcL = _memoria.leer(_registros.regSP);
			_registros.regSP++;
				
			int pcH = _memoria.leer(_registros.regSP);
			_registros.regSP++;
			
			int pc = (pcH << 8) | pcL;
			_registros.regPC = pc;
				
			return _duracion;
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		public InstruccionRET_ADR(Registros registros, Memoria memoria) : base(registros, memoria){
			_nombre = "RET";
			_longitud = 1;
			_duracion = 10;
		}
	}

	/// <summary>Instruccion INT</summary>	
	/// <remarks>Salta incondicionalmente a una direccion literal y guarda en la pila el contador de programa actual</remarks>
	public class InstruccionINT : Instruccion {
		/// <summary>Instruccion de salto</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			
			_registros.regSP--;
			_memoria.escribir((_registros.regPC & 0xFF00) >> 8, _registros.regSP);

			_registros.regSP--;
			_memoria.escribir(_registros.regPC & 0x00FF, _registros.regSP);
			
			_registros.regPC = _direccion;
			return _duracion;
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="direccion">Direccion de salto</param>
		public InstruccionINT(Registros registros, Memoria memoria, int direccion) : base(registros, memoria){
			_nombre = "INT " + direccion;
			_longitud = 0;
			_duracion = 0;
			
			_direccion = direccion;
		}
	}

	/// <summary>Instruccion RETI</summary>
	/// <remarks>Extrae de la pila la direccion del contador de programa y activa el IME</remarks>
	public class InstruccionRETI : Instruccion {
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;
			
			int pcL = _memoria.leer(_registros.regSP);
			_registros.regSP++;
				
			int pcH = _memoria.leer(_registros.regSP);
			_registros.regSP++;
			
			int pc = (pcH << 8) | pcL;
			_registros.regPC = pc;

			_registros.flagIME = true;
				
			return _duracion;
		}
		
		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		public InstruccionRETI(Registros registros, Memoria memoria) : base(registros, memoria){
			_nombre = "RETI";
			_longitud = 1;
			_duracion = 14;
		}
	}

	/// <summary>Instruccion RST</summary>
	/// <remarks>Salta incondiconalmente a una direccion literal y guarda el contador de programa actual en la pila</remarks>
	public class InstruccionRST : Instruccion {
		/// <summary>Direccion de salto</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.regPC += _longitud;

			_registros.regSP--;
			_memoria.escribir((_registros.regPC & 0xFF00) >> 8, _registros.regSP);

			_registros.regSP--;
			_memoria.escribir(_registros.regPC & 0x00FF, _registros.regSP);
			
			_registros.regPC = _direccion;
			return _duracion;
		}

		/// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="direccion">Direccion de salto</param>
		public InstruccionRST(Registros registros, Memoria memoria, int direccion) : base (registros, memoria){
			_nombre = "RST " + direccion;
			_longitud = 1;
			_duracion = 11;

			_direccion = direccion;
		}
	}
}
