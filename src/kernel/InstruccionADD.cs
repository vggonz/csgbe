/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Add Assembly Instructions
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

	/// <summary>Instruccion DEC</summary>
	/// <remarks>Decrementa el valor de un registro de 8 bits</remarks>
	public class InstruccionDEC_R : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _registro;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int valor = _registros.getReg(_registro);
			if (valor == 0x00) valor = 0xFF; else valor = valor - 1;
			if (valor == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((valor & 0xF) == 0xF) _registros.flagH = true; else _registros.flagH = false;
			_registros.flagN = true;
			
			_registros.setReg(_registro, valor & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;     
		}
			
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="registro">Registro afectado</param>
		public InstruccionDEC_R(Registros registros, Memoria memoria, string registro) : base(registros, memoria){
			_nombre = "DEC " + registro;
			_longitud = 1;
			_duracion = 4;
			
			_registro = registro;
		}
	}

	/// <summary>Instruccion DEC</summary>
	/// <remarks>Decrementa una direccion de memoria almacenada en un registro de 16 bits</remarks>	
	public class InstruccionDEC_RADR : Instruccion {
		/// <summary>Registro con la direccion</summary>
		private string _registro;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int valor = _memoria.leer(_registros.getReg(_registro));
			if (valor == 0x00) valor = 0xFF; else valor = valor - 1;
			if (valor == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((valor & 0xF) == 0xF) _registros.flagH = true; else _registros.flagH = false;
			_registros.flagN = true;
			
			_memoria.escribir(valor, _registros.getReg(_registro));

			_registros.regPC += _longitud;
			return _duracion;     
		}
			
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="registro">Registro con la direccion afectada</param>
		public InstruccionDEC_RADR(Registros registros, Memoria memoria, string registro) : base(registros, memoria){
			_nombre = "DEC (" + registro + ")";
			_longitud = 1;
			_duracion = 11;
			
			_registro = registro;
		}
	}

	/// <summary>Instruccion INC</summary>
	/// <remarks>Incrementa el valor de un registro de 8 bits</remarks>
	public class InstruccionINC_R : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _registro;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int valor = _registros.getReg(_registro);
			if (valor == 0xFF) valor = 0x00; else valor = valor + 1;
			if (valor == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((valor & 0xF) == 0xF) _registros.flagH = true; else	_registros.flagH = false;
			_registros.flagN = true;
			
			_registros.setReg(_registro, valor & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;     
		}
			
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="registro">Registro afectado</param>
		public InstruccionINC_R(Registros registros, Memoria memoria, string registro) : base(registros, memoria){
			_nombre = "INC " + registro;
			_longitud = 1;
			_duracion = 4;
			
			_registro = registro;
		}
	}

	/// <summary>Instruccion INC</summary>
	/// <remarks>Incrementa una direccion de memoria almacenada en un registro de 16 bits</remarks>	
	public class InstruccionINC_RADR : Instruccion {
		/// <summary>Registro que contiene la direccion afectada</summary>
		private string _registro;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int valor = _memoria.leer(_registros.getReg(_registro));
			if (valor == 0xFF) valor = 0x00; else valor = valor + 1;
			if (valor == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((valor & 0xF) == 0xF) _registros.flagH = true; else	_registros.flagH = false;
			_registros.flagN = true;
			
			_memoria.escribir(valor, _registros.getReg(_registro));

			_registros.regPC += _longitud;
			return _duracion;     
		}
			
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="registro">Registro con la direccion afectada</param>
		public InstruccionINC_RADR(Registros registros, Memoria memoria, string registro) : base(registros, memoria){
			_nombre = "INC (" + registro + ")";
			_longitud = 1;
			_duracion = 11;
			
			_registro = registro;
		}
	}

	/// <summary>Instruccion DEC</summary>
	/// <remarks>Decrementa el valor de un registro de 16 bits</remarks>
	public class InstruccionDEC_RR : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _registro;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int valor = _registros.getReg(_registro);
			if (valor == 0x00) valor = 0xFFFF; else valor--;
			_registros.setReg(_registro, valor & 0xFFFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="registro">Registro afectado</param>
		public InstruccionDEC_RR(Registros registros, Memoria memoria, string registro) : base (registros, memoria){
			_nombre = "DEC " + registro;
			_longitud = 1;
			_duracion = 6;

			_registro = registro;
		}
	}
	
	/// <summary>Instruccion INC</summary>
	/// <remarks>Incrementa el valor de un registro de 16 bits</remarks>
	public class InstruccionINC_RR : Instruccion {
		/// <summary>Registro afectado</summary>
		private string _registro;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int valor = _registros.getReg(_registro);
			if (valor == 0xFFFF) valor = 0x00; else valor++;
			_registros.setReg(_registro, valor & 0xFFFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="registro">Registro afectado</param>
		public InstruccionINC_RR(Registros registros, Memoria memoria, string registro) : base (registros, memoria){
			_nombre = "INC " + registro;
			_longitud = 1;
			_duracion = 6;

			_registro = registro;
		}
	}

	/// <summary>Instruccion ADD</summary>
	/// <remarks>Suma el contenido de dos registros de 8 bits y deja el resultado en el primero</remarks>	
	public class InstruccionADD_R_R : Instruccion {
		/// <summary>Primer operando</summary>
		private string _regOrigen;
		/// <summary>Segundo operando</summary>
		private string _regDestino;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int regOrigen = _registros.getReg(_regOrigen);
			int regDestino = _registros.getReg(_regDestino);
			int aux = regOrigen + regDestino;
			
			if ((aux & 0xFF) == 0 ) _registros.flagZ = true; else _registros.flagZ = false;
			if ((regDestino & 0x0F) + (regOrigen & 0x0F) > 0x0F) _registros.flagH = true; else _registros.flagH = false;
			if (aux > 0xFF) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;

			_registros.setReg(_regDestino, aux & 0xFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="regOrigen">Registro con el segundo operando</param>
		public InstruccionADD_R_R(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base(registros, memoria){
			_nombre = "ADD " + regDestino + ", " + regOrigen;
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}
	
	/// <summary>Instruccion ADD</summary>
	/// <remarks>Suma el contenido de un registro de 8 bits con una direccion de memoria almacenada en otro registro de 16 bits</remarks>
	public class InstruccionADD_R_RADR : Instruccion {
		private string _regOrigen;
		private string _regDestino;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int regOrigen = _memoria.leer(_registros.getReg(_regOrigen));
			int regDestino = _registros.getReg(_regDestino);
			int aux = regOrigen + regDestino;
			
			if ((aux & 0xFF) == 0 ) _registros.flagZ = true; else _registros.flagZ = false;
			if ((regDestino & 0x0F) + (regOrigen & 0x0F) > 0x0F) _registros.flagH = true; else _registros.flagH = false;
			if (aux > 0xFF) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;

			_registros.setReg(_regDestino, aux & 0xFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="regOrigen">Registro con la direccion de memoria del segundo operando</param>
		public InstruccionADD_R_RADR(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base(registros, memoria){
			_nombre = "ADD " + regDestino + ", (" + regOrigen + ")";
			_longitud = 1;
			_duracion = 7;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}

	/// <summary>Instruccion ADD</summary>
	/// <remarks>Suma el contenido de dos registros de 16 bits y deja el resultado en el primero</remarks>
	public class InstruccionADD_RR_RR : Instruccion {
		/// <summary>Primer operando</summary>
		private string _regOrigen;
		/// <summary>Segundo operando</summary>
		private string _regDestino;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			
			int regOrigen = _registros.getReg(_regOrigen);
			int regDestino = _registros.getReg(_regDestino);
			int aux = regOrigen + regDestino;
			
			if (((regDestino & 0x0FFF) + (regOrigen & 0x0FFF)) > 0x0FFF) _registros.flagH = true; else _registros.flagH = false;
			if (aux > 0xFFFF) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;

			_registros.setReg(_regDestino, aux & 0xFFFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro que contiene el primer operando y donde se guardara el resultado</param>
		/// <param name="regOrigen">Registro que contiene el segundo operando</param>
		public InstruccionADD_RR_RR(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base(registros, memoria){
			_nombre = "ADD " + regDestino + ", " + regOrigen;
			_longitud = 1;
			_duracion = 11;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}
	
	/// <summary>Instruccion ADD</summary>
	/// <remarks>Suma el contenido de un registro con un valor literal con signo</remarks>
	public class InstruccionADD_R_NN : Instruccion{
		/// <summary>Primer operando</summary>
		private string _regDestino;
		/// <summary>Valor del segundo operando</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int val = _valor + _registros.getReg(_regDestino);
			_registros.setReg(_regDestino, val & 0xFFFF);
			_registros.setFlag(0);
			if (val > 0xFFFF) _registros.flagC = true; else _registros.flagC = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro que contiene el primer operando y donde se guardara el resultado</param>
		/// <param name="valor">Valor con signo del segundo operando</param>
		public InstruccionADD_R_NN(Registros registros, Memoria memoria, string regDestino, int valor) : base(registros, memoria){
			if (valor > 127) _valor = valor - 256;
			else _valor = valor;
						
			_nombre ="ADD " + regDestino + ", " + valor;
			_longitud = 2;
			_duracion = 7;

			_regDestino = regDestino;
		}
	}

	/// <summary>Instruccion ADD</summary>
	/// <remarks>Suma el contenido de un registro de 8 bits con un valor literal sin signo</remarks>
	public class InstruccionADD_R_N : Instruccion{
		/// <summary>Primer operando</summary>
		private string _regDestino;
		/// <summary>Valor del segundo operando</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int val = _valor + _registros.getReg(_regDestino);
			_registros.setReg(_regDestino, val & 0xFF);
			if (val > 0xFFFF) _registros.flagC = true; else _registros.flagC = false;			
			if ((val & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((val & 0x0F) + (_valor & 0x0F) > 0x0F) _registros.flagH = true; else _registros.flagH = false;
			_registros.flagN = false;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro que contiene el primer operando y donde se guardara el resultado</param>
		/// <param name="valor">Valor sin signo del segundo operando</param>
		public InstruccionADD_R_N(Registros registros, Memoria memoria, string regDestino, int valor) : base(registros, memoria){
			_nombre ="ADD " + regDestino + ", " + valor;
			_longitud = 2;
			_duracion = 7;

			_regDestino = regDestino;
			_valor = valor;
		}
	}

	/// <summary>Instruccion CP</summary>
	/// <remarks>Compara el contenido del acumulador con un registro de 8 bits</remarks>	
	public class InstruccionCP_R : Instruccion{
		/// <summary>Registro para comparar</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			if (_registros.regA == _registros.getReg(_regOrigen)) _registros.flagZ = true; else _registros.flagZ = false;
			if (_registros.regA < _registros.getReg(_regOrigen)) _registros.flagC = true; else _registros.flagC = false;
			if ((_registros.regA & 0x0F) < (_registros.getReg(_regOrigen) & 0x0F)) _registros.flagH = true; else _registros.flagH = false;
			_registros.flagN = true;
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regOrigen">Registro usado para comparar</param>
		public InstruccionCP_R(Registros registros, Memoria memoria, string regOrigen) : base (registros, memoria){
			_nombre = "CP A, " + regOrigen;
			_longitud = 1;
			_duracion = 4;

			_regOrigen = regOrigen;
		}
	}
	
	/// <summary>Instruccion CP</summary>
	/// <remarks>Compara el acumulador con una direccion de memoria almacenada en un registro de 16 bits</remarks>
	public class InstruccionCP_RADR : Instruccion{
		/// <summary>Registro que contiene la direccion del valor a comparar</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
                        int valor = _memoria.leer(_registros.getReg(_regOrigen));
						
			if (_registros.regA == valor) _registros.flagZ = true; else _registros.flagZ = false;
			if (_registros.regA < valor) _registros.flagC = true; else _registros.flagC = false;
			if ((_registros.regA & 0x0F) < (valor & 0x0F)) _registros.flagH = true; else _registros.flagH = false;
			_registros.flagN = true;
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regOrigen">Registro que contiene la direccion con el valor para comparar</param>
		public InstruccionCP_RADR(Registros registros, Memoria memoria, string regOrigen) : base (registros, memoria){
			_nombre = "CP A, (" + regOrigen + ")";
			_longitud = 1;
			_duracion = 7;

			_regOrigen = regOrigen;
		}
	}
	
	/// <summary>Instruccion CP</summary>
	/// <remarks>Compara el acumulador con un valor literal</remarks>
	public class InstruccionCP_N : Instruccion{
		/// <summary>Valor literal para comparara</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			if (_registros.regA == _valor) _registros.flagZ = true; else _registros.flagZ = false;
			if (_registros.regA < _valor) _registros.flagC = true; else _registros.flagC = false;
			if ((_registros.regA & 0x0F) < (_valor & 0x0F))	_registros.flagH = true; else _registros.flagH = false;
			_registros.flagN = true;
			
			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="valor">Valor literal usado para comparar</param>
		public InstruccionCP_N(Registros registros, Memoria memoria, int valor) : base (registros, memoria){
			_nombre = "CP A," + valor;
			_longitud = 2;
			_duracion = 7;

			_valor = valor;
		}
	}

	/// <summary>Instruccion SBC</summary>
	/// <remarks>Resta dos registros de 8 bits con acarreo</remarks>
	public class InstruccionSBC_R_R : Instruccion {
		/// <summary>Primer operando</summary>
		private string _regOrigen;
		/// <summary>Segundo operando</summary>
		private string _regDestino;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			
			int regOrigen = _registros.getReg(_regOrigen);
			int regDestino = _registros.getReg(_regDestino);
			int aux = regDestino - regOrigen - (_registros.flagC ? 1 : 0);
			
			if ((aux & 0xFF) == 0 )	_registros.flagZ = true; else _registros.flagZ = false;
			if ((regDestino & 0x0F) < ((regOrigen - (_registros.flagC ? 1 : 0)) & 0x0F)) _registros.flagH = true; else _registros.flagH = false;
			if (aux < 0) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = true;

			_registros.setReg(_regDestino, aux & 0xFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="regOrigen">Registro con el segundo operando</param>
		public InstruccionSBC_R_R(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base(registros, memoria){
			_nombre = "SBC " + regDestino + ", " + regOrigen;
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}

	/// <summary>Instruccion SUB</summary>
	/// <remarks>Resta dos registros de 8 bits sin tener en cuenta el acarreo</remarks>
	public class InstruccionSUB_R_R : Instruccion{
		/// <summary>Primer operando</summary>
		private string _regDestino;
		/// <summary>Segundo operando</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int res = _registros.getReg(_regDestino) - _registros.getReg(_regOrigen);
			if ((res & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if (res < 0) _registros.flagC = true; else _registros.flagC = false;
			if ((_registros.getReg(_regDestino) & 0x0F) < (_registros.getReg(_regOrigen) & 0x0F)) _registros.flagH = true; else _registros.flagH = false;
			_registros.flagN = true;
			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="regOrigen">Registro con el segundo operando</param>
		public InstruccionSUB_R_R(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base(registros, memoria){
			_nombre = "SUB " + regDestino + ", " + regOrigen;
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}

	/// <summary>Instruccion SUB</summary>
	/// <remarks>Resta un registro de 8 bits con un valor en memoria cuya direccion esta en un registro de 16 bits</remarks>
	public class InstruccionSUB_R_RADR : Instruccion{
		/// <summary>Primer operando</summary>
		private string _regDestino;
		/// <summary>Direccion de memoria del segundo operando</summary>
		private string _regOrigen;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int direccion = _registros.getReg(_regOrigen);
			int res = _registros.getReg(_regDestino) - _memoria.leer(direccion);
			if ((res & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if (res < 0) _registros.flagC = true; else _registros.flagC = false;
			if ((_registros.getReg(_regDestino) & 0x0F) < (_memoria.leer(direccion) & 0x0F)) _registros.flagH = true; else _registros.flagH = false;
			_registros.flagN = true;
			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="regOrigen">Registro con la direccion de memoria del segundo operando</param>
		public InstruccionSUB_R_RADR(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base(registros, memoria){
			_nombre = "SUB " + regDestino + ", (" + regOrigen + ")";
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}

	/// <summary>Instruccion SBC</summary>
	/// <remarks>Resta un registro de 8 bits con un valor literal y el acarreo</remarks>	
	public class InstruccionSBC_R_N : Instruccion{
		/// <summary>Primer operando</summary>
		private string _regDestino;
		/// <summary>Valor literal</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int res = _registros.getReg(_regDestino) - _valor;
			if (_registros.flagC == true) res -= 1;

			if ((res & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((_registros.getReg(_regDestino) & 0x0F) < ((_valor - (_registros.flagC == true ? 1 : 0)) & 0x0F)) _registros.flagH = true; else _registros.flagH = false;
			if (res < 0) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = true;

			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="valor">Valor literal</param>
		public InstruccionSBC_R_N(Registros registros, Memoria memoria, string regDestino, int valor) : base(registros, memoria) {
			_nombre = "SBC " + regDestino + ", " + valor;
			_longitud = 2;
			_duracion = 4;

			_regDestino = regDestino;
			_valor = valor;
		}
	}
	
	/// <summary>Instruccion ADC</summary>
	/// <remarks>Suma dos registros de 8 bits con acarreo</remarks>
	public class InstruccionADC_R_R : Instruccion {
		/// <summary>Primer operando</summary>
		private string _regOrigen;
		/// <summary>Segundo operando</summary>
		private string _regDestino;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			
			int regOrigen = _registros.getReg(_regOrigen);
			int regDestino = _registros.getReg(_regDestino);
			int aux = regDestino + regOrigen + (_registros.flagC ? 1 : 0);
			
			if ((aux & 0xFF) == 0 )	_registros.flagZ = true; else _registros.flagZ = false;
			if (((regDestino & 0x0F) + (regOrigen & 0x0F) + (_registros.flagC ? 1 : 0)) > 0x0F) _registros.flagH = true; else _registros.flagH = false;
			if (aux < 0) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;

			_registros.setReg(_regDestino, aux & 0xFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="regOrigen">Registro con el segundo operando</param>
		public InstruccionADC_R_R(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base(registros, memoria){
			_nombre = "ADC " + regDestino + ", " + regOrigen;
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}

	/// <summary>Instruccion ADC</summary>
	/// <remarks>Suma un registro de 8 bits con un valor en memoria cuya direccion esta en un registro de 16 bits</remarks>
	public class InstruccionADC_R_RADR : Instruccion {
		/// <summary>Primer operando</summary>
		private string _regOrigen;
		/// <summary>Registro con la direccion del segundo operando</summary>
		private string _regDestino;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			int regOrigen = _memoria.leer(_registros.getReg(_regOrigen));
			int regDestino = _registros.getReg(_regDestino);
			int aux = regDestino + regOrigen + (_registros.flagC ? 1 : 0);
			
			if ((aux & 0xFF) == 0 )	_registros.flagZ = true; else _registros.flagZ = false;
			if (((regDestino & 0x0F) + (regOrigen & 0x0F) + (_registros.flagC ? 1 : 0)) > 0x0F) _registros.flagH = true; else _registros.flagH = false;
			if (aux > 0xFF) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;

			_registros.setReg(_regDestino, aux & 0xFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="regOrigen">Registro con la direccion de memoria del segundo operando</param>
		public InstruccionADC_R_RADR(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base(registros, memoria){
			_nombre = "ADC " + regDestino + ", (" + regOrigen + ")";
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}

	/// <summary>Instruccion ADC</summary>	
	/// <remarks>Suma un registro de 8 bits con un valor literal y el acarreo</remarks>	
	public class InstruccionADC_R_N : Instruccion{
		/// <summary>Primer operando</summary>
		private string _regDestino;
		/// <summary>Valor literal</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int res = _registros.getReg(_regDestino) + _valor;
			if (_registros.flagC == true) res += 1;

			if ((res & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if (((_registros.getReg(_regDestino) & 0x0F) + (_valor & 0x0F) + (_registros.flagC ? 1 : 0)) > 0x0F) _registros.flagH = true; else _registros.flagH = false;
			if (res < 0) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = true;

			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="valor">Valor literal</param>
		public InstruccionADC_R_N(Registros registros, Memoria memoria, string regDestino, int valor) : base(registros, memoria) {
			_nombre = "ADC " + regDestino + ", " + valor;
			_longitud = 2;
			_duracion = 4;

			_regDestino = regDestino;
			_valor = valor;
		}
	}
	
	/// <summary>Instruccion SUB</summary>
	/// <remarks>Suma un registro de 8 bits con un valor literal</remarks>	
	public class InstruccionSUB_R_N : Instruccion{
		/// <summary>Primer operando</summary>
		private string _regDestino;
		/// <summary>Valor literal</summary>
		private int _valor;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){
			int res = _registros.getReg(_regDestino) - _valor;
			if ((res & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if (res < 0) _registros.flagC = true; else _registros.flagC = false;
			if ((_registros.getReg(_regDestino) & 0x0F) < (_valor & 0x0F)) _registros.flagH = true; else _registros.flagH = false;
			_registros.flagN = true;
			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="valor">Valor literal</param>
		public InstruccionSUB_R_N(Registros registros, Memoria memoria, string regDestino, int valor) : base(registros, memoria){
			_nombre = "SUB " + regDestino + ", " + valor;
			_longitud = 2;
			_duracion = 4;

			_regDestino = regDestino;
			_valor = valor;
		}
	}
	
	/// <summary>Instruccion SBC</summary>
	/// <remarks>Resta un registro de 8 bits con un valor en memoria cuya direccion esta en un registro de 16 bits y con acarreo</remarks>
	public class InstruccionSBC_R_RADR : Instruccion {
		/// <summary>Primer operando</summary>
		private string _regOrigen;
		/// <summary>Registro con la direccion del segundo operando</summary>
		private string _regDestino;
		
                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar() {
			
			int direccion = _registros.getReg(_regOrigen);
			int regDestino = _registros.getReg(_regDestino);
			int aux = regDestino - _memoria.leer(direccion) - (_registros.flagC ? 1 : 0);
			
			if ((aux & 0xFF) == 0 )	_registros.flagZ = true; else _registros.flagZ = false;
			if ((regDestino & 0x0F) < ((_memoria.leer(direccion) - (_registros.flagC ? 1 : 0)) & 0x0F)) _registros.flagH = true; else _registros.flagH = false;
			if (aux < 0) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = true;

			_registros.setReg(_regDestino, aux & 0xFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro con el primer operando y donde se guardara el resultado</param>
		/// <param name="regOrigen">Registro con la direccion de memoria del segundo operando</param>
		public InstruccionSBC_R_RADR(Registros registros, Memoria memoria, string regDestino, string regOrigen) : base(registros, memoria){
			_nombre = "SBC " + regDestino + ", (" + regOrigen + ")";
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_regOrigen = regOrigen;
		}
	}

	/// <summary>Instruccion DAA</summary>
	/// <remarks>Convierte el contenido del acumulador en su equivalente en BCD</remarks>
	public class InstruccionDAA : Instruccion {

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>		
		public override int ejecutar(){

			int res = _registros.regA;
			res = (_registros.regA / 10) << 4;
			res |= (_registros.regA % 10) & 0xF;
			if (res == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if (_registros.regA >= 99) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagH = false;
			_registros.regA = res & 0xFF;

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		public InstruccionDAA(Registros registros, Memoria memoria) : base (registros, memoria){
			_nombre = "DAA";
			_longitud = 1;
			_duracion = 4;
		}
	}
}
