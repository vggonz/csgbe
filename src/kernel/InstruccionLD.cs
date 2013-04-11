/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Load Assembly Instructions
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

	/// <summary>Instruccion LD sobre registros de 8 bits</summary>
	/// <remarks>Copia el valor de un registro a otro</remarks>
	public class InstruccionLD_R_R : Instruccion {

		/// <summary>Registro de origen</summary>
		private string _regOrigen;
		/// <summary>Registro destino</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.setReg(_regDestino, _registros.getReg(_regOrigen));
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro destino</param>
		/// <param name="regOrigen">Registro de origen</param>
		public InstruccionLD_R_R(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base (registros, memoria){
			_nombre = "LD " + regDestino + "," + regOrigen;
			_longitud = 1;
			_duracion = 4;

			_regOrigen = regOrigen;
			_regDestino = regDestino;
		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga de un literal sobre un registro de 16 bits</remarks>
	public class InstruccionLD_DD_NN : Instruccion {

		/// <summary>Registro destino</summary>
		private string _regDestino;
		/// <summary>Valor literal</summary>
		private int _valor;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.setReg(_regDestino, _valor);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="bajo">Parte baja del valor literal</param>
		/// <param name="alto">Parte alta del valor literal</param>
		/// <param name="regDestino">Registro destino</param>
		public InstruccionLD_DD_NN(Registros registros, Memoria memoria, int bajo, int alto, string regDestino) : base (registros, memoria){
			_regDestino = regDestino;
			_valor = (alto << 8) | bajo;
			
			_nombre = "LD " + regDestino + "," + _valor;
			_longitud = 3;
			_duracion = 10;
		}
	}
	
	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga de un literal sobre un registro de 8 bits</remarks>
	public class InstruccionLD_R_N : Instruccion {
		/// <summary>Registro destino</summary>
		private string _regDestino;
		/// <summary>Valor literal</summary>
		private int _valor;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_registros.setReg(_regDestino, _valor);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="valor">Valor literal</param>
		/// <param name="regDestino">Registro destino</param>
		public InstruccionLD_R_N(Registros registros, Memoria memoria, int valor, string regDestino) : base (registros, memoria){
			_nombre = "LD " + regDestino + "," + valor;
			_longitud = 2;
			_duracion = 7;

			_regDestino = regDestino;
			_valor = valor;
		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga en una direccion de memoria el contenido de un registro de 8 bits</remarks>	
	public class InstruccionLD_ADR_R : Instruccion {
		/// <summary>Registro de origen</summary>
		private string _regOrigen;
		/// <summary>Direccion de memoria destino</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_memoria.escribir(_registros.getReg(_regOrigen), _direccion);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="baja">Parte baja de la direccion de memoria</param>
		/// <param name="alta">Parte alta de la direccion de memoria</param>
		/// <param name="regOrigen">Registro de origen</param>
		public InstruccionLD_ADR_R(Registros registros, Memoria memoria, int baja, int alta, string regOrigen) : base (registros, memoria){
			_regOrigen = regOrigen;
			_direccion = ((alta << 8) | baja) & 0xFFFF;
			
			_nombre = "LD " + _direccion + "," + regOrigen;
			_longitud = 3;
			_duracion = 13;
		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga en una direccion de memoria el contenido de un registro de 16 bits</remarks>	
	public class InstruccionLD_ADR_RR : Instruccion{
		/// <summary>Direccion de memoria destino</summary>
		private int _direccion;
		/// <summary>Registro de origen</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int valor = _registros.getReg(_regOrigen);
			_memoria.escribir(valor & 0xFF, _direccion);
			_memoria.escribir((valor >> 8) & 0xFF, _direccion + 1);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="baja">Parte baja de la direccion de memoria</param>
		/// <param name="alta">Parte alta de la direccion de memoria</param>
		/// <param name="regOrigen">Registro de origen</param>
		public InstruccionLD_ADR_RR(Registros registros, Memoria memoria, int baja, int alta, string regOrigen) : base(registros, memoria){
			_regOrigen = regOrigen;
			_direccion = (alta << 8) | baja;

			_nombre = "LD (" + _direccion + "), " + regOrigen;
			_longitud = 3;
			_duracion = 13;
		}
	}

	/// <summary>Instruccion LDD</summary>
	/// <remarks>Carga el valor de un registro a una direccion de memoria almacenada en otro registro de 16 bits que es decrementado</remarks>	
	public class InstruccionLDD_RADR_R : Instruccion {
		/// <summary>Registro de origen</summary>
		private string _regDestino;
		/// <summary>Registro destino que contiene la direccion de memoria</summary>
		private string _regOrigen;
		/// <summary>Direccion de memoria destino</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_memoria.escribir(_registros.getReg(_regOrigen), _direccion);

			int valor = _registros.getReg(_regDestino);
			if (valor == 0x00) valor = 0xFFFF; else valor = valor - 1;
			_registros.setReg(_regDestino, valor);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con la direccion destino</param>
		/// <param name="regOrigen">Registro de origen</param>
		public InstruccionLDD_RADR_R(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base (registros, memoria){
			_regDestino = regDestino;
			_regOrigen = regOrigen;
			_direccion = _registros.getReg(regDestino);
			
			_nombre = "LDD " + regDestino + "(" + _direccion + ")" + "," + regOrigen;
			_longitud = 1;
			_duracion = 15;
		}
	}
	
	/// <summary>Instruccion LDI</summary>
	/// <remarks>Carga el valor de un registro a una direccion de memoria almacenada en otro registro de 16 bits que es incrementado</remarks>	
	public class InstruccionLDI_RADR_R : Instruccion {
		/// <summary>Registro de origen</summary>
		private string _regDestino;
		/// <summary>Registro destino que contiene la direccion de memoria</summary>
		private string _regOrigen;
		/// <summary>Direccion de memoria destino</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			_memoria.escribir(_registros.getReg(_regOrigen), _direccion);

			int valor = _registros.getReg(_regDestino);
			if (valor == 0xFFFF) valor = 0x00; else valor = valor + 1;
			_registros.setReg(_regDestino, valor);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con la direccion destino</param>
		/// <param name="regOrigen">Registro de origen</param>
		public InstruccionLDI_RADR_R(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base (registros, memoria){
			_regDestino = regDestino;
			_regOrigen = regOrigen;
			_direccion = _registros.getReg(regDestino);
			
			_nombre = "LDI " + regDestino + "(" + _direccion + ")" + "," + regOrigen;
			_longitud = 1;
			_duracion = 15;

		}
	}

	/// <summary>Instruccion LDD</summary>
	/// <remarks>Carga un valor de una direccion de memoria almacenada en un registro de 16 bits a otro registro de 8 bits. Decrementa la direccion</remarks>	
	public class InstruccionLDD_R_RADR : Instruccion {
		/// <summary>Registro destino</summary>
		private string _regDestino;
		/// <summary>Registro con la direccion de memoria origen</summary>
		private string _regOrigen;
		/// <summary>Direccion de memoria origen</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int valor = _memoria.leer(_registros.getReg(_regOrigen));
			_registros.setReg(_regDestino, valor);

			int radr = _registros.getReg(_regOrigen);
			if (radr == 0x00) radr = 0xFFFF; else radr = radr - 1;
			_registros.setReg(_regOrigen, radr);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro destino</param>
		/// <param name="regOrigen">Registro con la direccion de memoria origen</param>
		public InstruccionLDD_R_RADR(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base (registros, memoria){
			_regDestino = regDestino;
			_regOrigen = regOrigen;
			_direccion = _registros.getReg(regOrigen);
			
			_nombre = "LDD " + regDestino + "," + regOrigen + "(" + _direccion + ")";
			_longitud = 1;
			_duracion = 15;
		}
	}
	
	/// <summary>Instruccion LDI</summary>
	/// <remarks>Carga un valor de una direccion de memoria almacenada en un registro de 16 bits a otro registro de 8 bits. Incrementa la direccion</remarks>	
	public class InstruccionLDI_R_RADR : Instruccion {
		/// <summary>Registro destino</summary>
		private string _regDestino;
		/// <summary>Registro con la direccion de memoria origen</summary>
		private string _regOrigen;
		/// <summary>Direccion de memoria origen</summary>
		private int _direccion;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int valor = _memoria.leer(_registros.getReg(_regOrigen));
			_registros.setReg(_regDestino, valor);

			int radr = _registros.getReg(_regOrigen);
			if (radr == 0xFFFF) radr = 0x00; else radr = radr + 1;
			_registros.setReg(_regOrigen, radr);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro destino</param>
		/// <param name="regOrigen">Registro con la direccion de memoria origen</param>
		public InstruccionLDI_R_RADR(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base (registros, memoria){
			_regDestino = regDestino;
			_regOrigen = regOrigen;
			_direccion = _registros.getReg(regOrigen);
			
			_nombre = "LDI " + regDestino + "," + regOrigen + "(" + _direccion + ")";
			_longitud = 1;
			_duracion = 15;
		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga de una direccion de memoria desplazada (base 0xFF00) a un registro de 8 bits</remarks>
	public class InstruccionLD_R_DADR : Instruccion {
		/// <summary>Registro destino</summary>
		private string _regDestino;
		/// <summary>Desplazamiento</summary>
		private int _desp;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int valor = _memoria.leer(0xFF00 + _desp);
			_registros.setReg(_regDestino, valor);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro destino</param>
		/// <param name="desp">Desplazamiento de la direccion base</param>
		public InstruccionLD_R_DADR(Registros registros, Memoria memoria, string regDestino, int desp) : base (registros, memoria){
			_nombre = "LD " + regDestino + ", 0xFF(" + desp + ")";
			_longitud = 2;
			_duracion = 15;

			_regDestino = regDestino;
			_desp = desp;
		}
	}
	
	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga de una direccion de memoria (base 0xFF00) desplzada el valor indicado en un registro de 8 bits, a un registro de 8 bits</remarks>
	public class InstruccionLD_R_DR : Instruccion {
		/// <summary>Registro destino</summary>
		private string _regDestino;
		/// <summary>Registro con el valor del desplazamiento</summary>
		private string _regOrigen;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int valor = _memoria.leer(0xFF00 + _registros.getReg(_regOrigen));
			_registros.setReg(_regDestino, valor);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro destino</param>
		/// <param name="regOrigen">Registro con el valor del desplazamiento</param>
		public InstruccionLD_R_DR(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base (registros, memoria){
			_nombre = "LD " + regDestino + "0xFF" + "(" + regOrigen + ")";
			_longitud = 1;
			_duracion = 15;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}
	
	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga un registro de 8 bits a una direccion de memoria desplazada (base 0xFF00)</remarks>
	public class InstruccionLD_DADR_R : Instruccion {
		/// <summary>Registro origen</summary>
		private string _regOrigen;
		/// <summary>Desplazamiento</summary>
		private int _desp;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int direccion = 0xFF00 + _desp;			
			_memoria.escribir(_registros.getReg(_regOrigen), direccion);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regOrigen">Registro origen</param>
		/// <param name="desp">Desplazamiento de la direccion base</param>
		public InstruccionLD_DADR_R(Registros registros, Memoria memoria, string regOrigen, int desp) : base (registros, memoria){
			_nombre = "LD 0xFF(" + desp + "), " + regOrigen;
			_longitud = 2;
			_duracion = 15;

			_regOrigen = regOrigen;
			_desp = desp;
		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga un registro de 8 bits a una direccion de memoria (base 0xFF00) desplazada el valor indicado en otro registro de 8 bits</remarks>
	public class InstruccionLD_DR_R : Instruccion {
		/// <summary>Registro con el desplazamiento</summary>
		private string _regDesp;
		/// <summary>Registro origen</summary>
		private string _regOrigen;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int direccion = 0xFF00 + _registros.getReg(_regDesp);
			_memoria.escribir(_registros.getReg(_regOrigen), direccion);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDesp">Registro que contiene el desplazamiento</param>
		/// <param name="regOrigen">Registro origen</param>
		public InstruccionLD_DR_R(Registros registros, Memoria memoria, string regDesp, string regOrigen) : base (registros, memoria){
			_nombre = "LD 0xFF(" + regDesp + "), " + regOrigen;
			_longitud = 1;
			_duracion = 15;

			_regDesp = regDesp;
			_regOrigen = regOrigen;
		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga en la direccion de memoria almacenada por un registro de 16 bits el valor de otro registro de 8 bits</remarks>
	public class InstruccionLD_RADR_R : Instruccion {
		/// <summary>Registro con la direccion de memoria destino</summary>
		private string _regDestino;
		/// <summary>Registro origen</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			_memoria.escribir(_registros.getReg(_regOrigen), direccion);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con la direccion destino</param>
		/// <param name="regOrigen">Registro origen</param>
		public InstruccionLD_RADR_R(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base (registros, memoria){
			_nombre = "LD (" + regDestino + "), " + regOrigen;
			_longitud = 1;
			_duracion = 7;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga en un registro el valor almacenado en una direccion literal</remarks>
	public class InstruccionLD_R_ADR : Instruccion {
		/// <summary>Registro destino</summary>
		private string _regDestino;
		/// <summary>Direccion de memoria origen</summary>
		private int _direccion;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.setReg(_regDestino, _memoria.leer(_direccion));
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro destino</param>
		/// <param name="baja">Parte baja de la direccion de origen</param>
		/// <param name="alta">Parte alta de la direccion de origen</param>
		public InstruccionLD_R_ADR(Registros registros, Memoria memoria, string regDestino, int baja, int alta) : base (registros, memoria){
			_direccion = (alta << 8) | baja;
			_regDestino = regDestino;

			_nombre = "LD " + regDestino + ", (" + _direccion + ")";
			_longitud = 3;
			_duracion = 13;

		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga en un registro el valor de una direccion de memoria almacenada en otro registro de 16 bits</remarks>
	public class InstruccionLD_R_RADR : Instruccion {
		/// <summary>Registro destino</summary>
		private string _regDestino;
		/// <summary>Registro con la direccion de memoria origen</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int direccion = _registros.getReg(_regOrigen);
			_registros.setReg(_regDestino, _memoria.leer(direccion));
			
			_registros.regPC += _longitud;
			return _duracion;
		}
 
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro destino</param>
		/// <param name="regOrigen">Registro con la direccion de memoria origen</param>
		public InstruccionLD_R_RADR(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base (registros, memoria){
			_regDestino = regDestino;
			_regOrigen = regOrigen;
			
			_nombre = "LD " + regDestino + ", (" + regOrigen + ")";
			_longitud = 1;
			_duracion = 7;
		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga en una direccion de memoria almacenada en un registro de 16 bits un valor literal</remarks>
	public class InstruccionLD_RADR_N : Instruccion {
		/// <summary>Registro con la direccion destino</summary>
		private string _regDestino;
		/// <summary>Valor literal</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			_memoria.escribir(_valor, direccion);
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con la direccion destino</param>
		/// <param name="valor">Valor literal</param>
		public InstruccionLD_RADR_N(Registros registros, Memoria memoria, string regDestino, int valor) : base (registros, memoria){
			_regDestino = regDestino;
			_valor = valor;
			
			_nombre = "LD (" + regDestino + "), " + valor;
			_longitud = 2;
			_duracion = 10;
		}
	}

	/// <summary>Instruccion LD</summary>
	/// <remarks>Carga en un registro de 8 bits el valor almacenado en una direccion de memoria indicada por el puntero de pila desplazado</remarks>
	public class InstruccionLD_R_SPD : Instruccion {
		/// <summary>Registro destino</summary>
		private string _regDestino;
		/// <summary>Desplazamiento sobre el puntero de pila</summary>
		private int _desp;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			_registros.setReg(_regDestino, _registros.regSP + _desp);
			_registros.setFlag(0);
			if (_registros.getReg(_regDestino) > 0xFFFF) _registros.flagC = true; else _registros.flagC = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro destino</param>
		/// <param name="desp">Desplazamiento con signo (-127, 128)</param>
		public InstruccionLD_R_SPD(Registros registros, Memoria memoria, string regDestino, int desp) : base (registros, memoria){
			_regDestino = regDestino;
                        if (desp > 127) _desp = desp - 256; else _desp = desp;

			_nombre = "LD " + regDestino + ", SP+" + _desp;
			_longitud = 2;
			_duracion = 10;
		}
	}
}
