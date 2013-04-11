/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Shift Assembly Instructions
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

	/// <summary>Instruccion RLC sobre un registro de 8 bits</summary>
	/// <remarks>Realiza una rotacion a izquierdas del valor</remarks>
	public class InstruccionRLC : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;
		/// <summary>Indica si el flag Z debe modificarse o no segun el resultado</summary>
		private bool _zero;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int valor = _registros.getReg(_regDestino);
			int carry = (valor & (0x01 << 7)) >> 7;
			if ((valor & 0x80) != 0) _registros.flagC = true; else _registros.flagC = false;
			if (_zero) if ((valor & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_registros.setReg(_regDestino, ((valor << 1) | carry) & 0xFF);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		/// <param name="zero">Indica si el flag Z debe modificarse en consecuencia o no</param>
		public InstruccionRLC(Registros registros, Memoria memoria, string regDestino, bool zero) : base(registros,memoria){
			_nombre = "RLC " + regDestino;
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_zero = zero;
		}
	}

	/// <summary>Instruccion RLC sobre una direccion almacenada en un registro de 16 bits</summary>
	/// <remarks>Realiza una rotacion a izquierdas del valor</remarks>
	public class InstruccionRLC_RADR : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			_registros.flagH = false;
			_registros.flagN = false;

			int direccion = _registros.getReg(_regDestino);
			int valor = _memoria.leer(direccion);
			int carry = (valor & (0x01 << 7)) >> 7;
			if ((valor & 0x80) != 0) _registros.flagC = true; else _registros.flagC = false;
			if ((valor & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			_memoria.escribir((valor << 1) | carry, direccion);
			
			_registros.regPC += _longitud;
			return _duracion;
		}
		
                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionRLC_RADR(Registros registros, Memoria memoria, string regDestino) : base(registros,memoria){
			_nombre = "RLC (" + regDestino + ")";
			_longitud = 1;
			_duracion = 15;

			_regDestino = regDestino;
		}
	}
	
	/// <summary>Instruccion SLA sobre un registro de 8 bits</summary>
	/// <remarks>Realiza un desplazamiento a izquierdas del valor</remarks>
	public class InstruccionSLA : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int res = _registros.getReg(_regDestino) << 1;
			if ((res & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if (res > 0xFF) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSLA(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria){
			_nombre = "SLA " + regDestino;
			_longitud = 1;
			_duracion = 8;

			_regDestino = regDestino;
		}
	}

	/// <summary>Instruccion SLA sobre una direccion almacenada en un registro de 16 bits</summary>
	/// <remarks>Realiza un desplazamiento a izquierdas del valor</remarks>
	public class InstruccionSLA_RADR : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			int res = _memoria.leer(direccion) << 1;
			if ((res & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if (res > 0xFF) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_memoria.escribir(res & 0xFF, direccion);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSLA_RADR(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria){
			_nombre = "SLA (" + regDestino + ")";
			_longitud = 1;
			_duracion = 15;

			_regDestino = regDestino;
		}
	}

	/// <summary>Instruccion SRA sobre un registro de 8 bits</summary>
	/// <remarks>Realiza un desplazamiento a derechas del valor</remarks>
	public class InstruccionSRA : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int res = ((_registros.getReg(_regDestino) >> 1) | (_registros.getReg(_regDestino) & 0x80)) & 0xFF;
			if (res == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((_registros.getReg(_regDestino) & 0x01) == 0x01) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSRA(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria){
			_nombre = "SRA " + regDestino;
			_longitud = 1;
			_duracion = 8;

			_regDestino = regDestino;
		}
	}
	
	/// <summary>Instruccion SRA sobre una direccion almacenada en un registro de 16 bits</summary>
	/// <remarks>Realiza un desplazamiento a derechas del valor</remarks>
	public class InstruccionSRA_RADR : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			int res = ((_memoria.leer(direccion) >> 1) | (_memoria.leer(direccion) & 0x80)) & 0xFF;
			if (res == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((_memoria.leer(direccion) & 0x01) == 0x01) _registros.flagC = true; else _registros.flagC = false;
			_memoria.escribir(res, direccion);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSRA_RADR(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria){
			_nombre = "SRA (" + regDestino + ")";
			_longitud = 1;
			_duracion = 15;

			_regDestino = regDestino;
		}
	}
	
	/// <summary>Instruccion SRL sobre un registro de 8 bits</summary>
	/// <remarks>Realiza un desplazamiento a derechas del valor insertando 0</remarks>
	public class InstruccionSRL : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int res = (_registros.getReg(_regDestino) >> 1) & 0xFF;
			if (res == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((_registros.getReg(_regDestino) & 0x01) == 0x01) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSRL(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria){
			_nombre = "SRL " + regDestino;
			_longitud = 1;
			_duracion = 8;

			_regDestino = regDestino;
		}
	}
	
	/// <summary>Instruccion SRL sobre una direccion almacenada en un registro de 16 bits</summary>
	/// <remarks>Realiza un desplazamiento a derechas del valor insertando 0</remarks>
	public class InstruccionSRL_RADR : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			int res = (_memoria.leer(direccion) >> 1) & 0xFF;
			if (res == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if ((_memoria.leer(direccion) & 0x01) == 0x01) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_memoria.escribir(res, direccion);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionSRL_RADR(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria){
			_nombre = "SRL (" + regDestino + ")";
			_longitud = 1;
			_duracion = 15;

			_regDestino = regDestino;
		}
	}
	
	/// <summary>Instruccion RL sobre un registro de 8 bits</summary>
	/// <remarks>Realiza un desplazamiento a izquierdas con el bit de carry</remarks>
	public class InstruccionRL : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;
		/// <summary>Indica si el flag Z debe modificarse o no segun el resultado</summary>
		private bool _zero;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int res = (_registros.getReg(_regDestino) << 1) | (_registros.flagC == true ? 1 : 0);
			if (_zero) if ((res & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if (res > 0xFF) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		/// <param name="zero">Indica si el flag Z debe modificarse en consecuencia o no</param>
		public InstruccionRL(Registros registros, Memoria memoria, string regDestino, bool zero) : base (registros, memoria){
			_nombre = "RL " + regDestino;
			_longitud = 1;
			_duracion = 8;

			_regDestino = regDestino;
			_zero = zero;
		}
	}

	/// <summary>Instruccion RL sobre una direccion almacenada en un registro de 16 bits</summary>
	/// <remarks>Realiza un desplazamiento a izquierdas con el bit de carry</remarks>
	public class InstruccionRL_RADR : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			int res = (_memoria.leer(direccion) << 1) | (_registros.flagC == true ? 1 : 0);
			if ((res & 0xFF) == 0) _registros.flagZ = true; else _registros.flagZ = false;
			if (res > 0xFF) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_memoria.escribir(res & 0xFF, direccion);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionRL_RADR(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria){
			_nombre = "RL (" + regDestino + ")";
			_longitud = 1;
			_duracion = 15;

			_regDestino = regDestino;
		}
	}

	/// <summary>Instruccion RRC sobre un registro de 8 bits</summary>
	/// <remarks>Realiza una rotacion a derechas del valor copiando el bit desplazada al flag de carry</remarks>
	public class InstruccionRRC : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;
		/// <summary>Indica si el flag Z debe modificarse o no segun el resultado</summary>
		private bool _zero;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int res = _registros.getReg(_regDestino);
			res = (res >> 1) | ((res & 0x01) << 7);
			if (_zero) if (res == 0) _registros.flagZ = true;
			if ((_registros.getReg(_regDestino) & 0x01) == 0x01) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		/// <param name="zero">Indica si el flag Z debe modificarse en consecuencia o no</param>
		public InstruccionRRC(Registros registros, Memoria memoria, string regDestino, bool zero) : base (registros, memoria){
			_nombre = "RRC " + regDestino;
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_zero = zero;
		}
	}

	/// <summary>Instruccion RRC sobre una direccion almacenada en un registro de 16 bits</summary>
	/// <remarks>Realiza una rotacion a derechas del valor copiando el bit desplazado al flag de carry</remarks>
	public class InstruccionRRC_RADR : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			int res = _memoria.leer(direccion);
			res = (res >> 1) | ((res & 0x01) << 7);
			if (res == 0) _registros.flagZ = true;
			if ((_memoria.leer(direccion) & 0x01) == 0x01) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_memoria.escribir(res & 0xFF, direccion);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionRRC_RADR(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria){
			_nombre = "RRC (" + regDestino + ")";
			_longitud = 1;
			_duracion = 15;

			_regDestino = regDestino;
		}
	}

	/// <summary>Instruccion RR sobre un registro de 8 bits</summary>
	/// <remarks>Rota el valor a derechas copiando el bit desplazado al flag carry</remarks> 
	public class InstruccionRR : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;
		/// <summary>Indica si el flag Z debe modificarse o no segun el resultado</summary>
		private bool _zero;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int res = _registros.getReg(_regDestino);
			res = (res >> 1) | ((_registros.flagC ? 1 : 0) << 7);
			if (_zero) if (res == 0) _registros.flagZ = true;
			if ((_registros.getReg(_regDestino) & 0x01) == 0x01) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_registros.setReg(_regDestino, res & 0xFF);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		/// <param name="zero">Indica si el flag Z debe modificarse en consecuencia o no</param>
		public InstruccionRR(Registros registros, Memoria memoria, string regDestino, bool zero) : base (registros, memoria){
			_nombre = "RR " + regDestino;
			_longitud = 1;
			_duracion = 4;

			_regDestino = regDestino;
			_zero = zero;
		}
	}
	
	/// <summary>Instruccion RR sobre una direccion almacenada en un registro de 16 bits</summary>
	/// <remarks>Rota el valor a derechas copiando el bit desplazado al flag carry</remarks> 
	public class InstruccionRR_RADR : Instruccion{

		/// <summary>Registro afectado</summary>
		private string _regDestino;

                /// <summary>Ejecuta la instruccion</summary>
                /// <returns>Duracion teorica de la ejecucion en ciclos</returns>
		public override int ejecutar(){
			int direccion = _registros.getReg(_regDestino);
			int res = _memoria.leer(direccion);
			res = (res >> 1) | ((_registros.flagC ? 1 : 0) << 7);
			if (res == 0) _registros.flagZ = true;
			if ((_memoria.leer(direccion) & 0x01) == 0x01) _registros.flagC = true; else _registros.flagC = false;
			_registros.flagN = false;
			_registros.flagH = false;
			_memoria.escribir(res & 0xFF, direccion);

			_registros.regPC += _longitud;
			return _duracion;
		}

                /// <summary>Constructor</summary>
                /// <param name="registros">Registros de la CPU</param>
                /// <param name="memoria">Memoria</param>
		/// <param name="regDestino">Registro afectado</param>
		public InstruccionRR_RADR(Registros registros, Memoria memoria, string regDestino) : base (registros, memoria){
			_nombre = "RR (" + regDestino + ")";
			_longitud = 1;
			_duracion = 15;

			_regDestino = regDestino;
		}
	}
}
