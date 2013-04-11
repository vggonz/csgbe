/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# CPU Z80 Registers
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

	/// <summary>Registros internos de la CPU</summary>
	public class Registros{
		
		/// <summary>Registros de 8 bits</summary>
		private int A,B,C,D,E;
		/// <summary>Registros de 16 bits</summary>
		private int SP,PC,HL;
		/// <summary>Flags de condicion</summary>
		private bool flag_Z,flag_H,flag_N,flag_C;
		/// <summary>Interrupt Master Enable</summary>
		private bool IME;

		/// <summary>Interrupt Master Enable</summary>
		public bool flagIME{
			get{ return IME; }
			set{ IME = value; }
		}

		/// <summary>Registro A de 8 bits</summary>
		public int regA{
			get{ return A; }
			set{ A = value; }
		}
	
		/// <summary>Registro B de 8 bits</summary>
		public int regB{
			get{ return B; }
			set{ B = value; }
		}

		/// <summary>Registro C de 8 bits</summary>
		public int regC{
			get{ return C; }
			set{ C = value; }
		}
	
		/// <summary>Registro D de 8 bits</summary>
		public int regD{
			get{ return D; }
			set{ D = value; }
		}

		/// <summary>Registro E de 8 bits</summary>
		public int regE{
			get{ return E; }
			set{ E = value; }
		}
	
		/// <summary>Registro SP (puntero de pila) de 16 bits</summary>
		public int regSP{
			get{ return SP; }
			set{ SP = value; }
		}

		/// <summary>Registro PC (contador de programa) de 16 bits</summary>
		public int regPC{
			get{ return PC; }
			set{ PC = value; }
		}
	
		/// <summary>Registro HL de 16 bits</summary>
		public int regHL{
			get{ return HL; }
			set{ HL = value; }
		}

		/// <summary>Registro H (parte alta de HL) de 8 bits</summary>
		public int regH{
			get{ return ((HL & 0xFF00) >> 8); }
			set{ HL = (HL & 0x00FF) | (value << 8); }
		}
		
		/// <summary>Registro L (parte baja de HL) de 8 bits</summary>
		public int regL{
			get{ return (HL & 0x00FF); }
			set{ HL = (HL & 0xFF00) | value; }
		}

		/// <summary>Registro BC (combinacion de B y C) de 16 bits</summary>
		public int regBC{
			get{ return (B << 8) | C; }
			set{ B = ((value & 0xFF00) >> 8);
			     C = (short) (value & 0x00FF);
			}
		}
	
		/// <summary>Registro DE (combinacion de D y E) de 16 bits</summary>
		public int regDE{
			get{ return (D << 8) | E; }
			set{ D = ((value & 0xFF00) >> 8);
			     E = (value & 0x00FF);
			}
		}

		/// <summary>Registro AF (combinacion de A y los flags) de 16 bits</summary>
		public int regAF{
			get{ return (A << 8) | getFlag(); }
			set{ A = ((value & 0xFF00) >> 8);
			     setFlag(value & 0x00FF);
			}
		}

		/// <summary>Registro F (flags) de 8 bits</summary>
		public int regF{
			get { return getFlag(); }
			set { setFlag(value); }
		}

		/// <summary>Flag Z (zero)</summary>
		public bool flagZ{
			get{ return flag_Z; }
			set{ flag_Z = value; }
		}

		/// <summary>Flag H (half-carry)</summary>
		public bool flagH{
			get{ return flag_H; }
			set{ flag_H = value; }
		}

		/// <summary>Flag C (carry)</summary>
		public bool flagC{
			get{ return flag_C; }
			set { flag_C = value; }
		}

		/// <summary>Flag N (add/sub en DAA)</summary>
		public bool flagN{
			get{ return flag_N; }
			set{ flag_N = value; }
		}

		/// <summary>Constructor</summary>
		public Registros(){}

		/// <summary>Obtiene el valor de un registro a partir de su nombre</summary>
		/// <param name="registro">Nombre del registro (A, B, C, D, E, SP, PC, HL, BC, DE, H, L, AF, F)</param>
		/// <returns>El valor del registro</returns>
		public int getReg(string registro){
			int valor = -1;
			switch(registro){
				case "A": valor = A; break;
				case "B": valor = B; break;
				case "C": valor = C; break;
				case "D": valor = D; break;
				case "E": valor = E; break;
				case "SP": valor = SP; break;
				case "PC": valor = PC; break;
				case "HL": valor = HL; break;
				case "BC": valor = regBC; break;
				case "DE": valor = regDE; break;
				case "H": valor = regH; break;
				case "L": valor = regL; break;
				case "AF": valor = regAF; break;
				case "F": valor = regF; break;
			}
			return valor;
		}

		/// <summary>Asigna un valor a un registro identificado por su nombre</summary>
		/// <param name="registro">Nombre del registro (A, B, C, D, E, SP, PC, HL, BC, DE, H, L, AF, F)</param>
		/// <param name="valor">Nuevo valor del registro</param>
		public void setReg(string registro, int valor){
			switch(registro){
				case "A": A = valor; break;
				case "B": B = valor; break;
				case "C": C = valor; break;
				case "D": D = valor; break;
				case "E": E = valor; break;
				case "SP": SP = valor; break;
				case "PC": PC = valor; break;
				case "HL": HL = valor; break;
				case "BC": regBC = valor; break;
				case "DE": regDE = valor; break;
				case "H": regH = valor; break;
				case "L": regL = valor; break;
				case "AF": regAF = valor; break;
				case "F": regF = valor; break;
			}
		}

		/// <summary>Obtiene el estado de un flag por su nombre</summary>
		/// <param name="flag">Nombre del flag (Z, N, H, C)</param>
		/// <returns>El estado del flag, true activo y false no activo</returns>
		public bool getFlag(string flag){
			bool valor = false;
			switch(flag){
				case "Z": valor = flagZ; break;
				case "N": valor = flagN; break;
				case "H": valor = flagH; break;
				case "C": valor = flagC; break;
			}
			return valor;
		}

		/// <summary>Asigna un nuevo estado a un flag identificado por su nombre</summary>
		/// <param name="flag">El nombre del flag (Z, N, H, C)</param>
		/// <param name="valor">El nuevo estado para el flag</param>
		public void setFlag(string flag, bool valor){
			switch(flag){
				case "Z": flagZ = valor; break;
				case "N": flagN = valor; break;
				case "H": flagH = valor; break;
				case "C": flagC = valor; break;
			}
		}

		/// <summary>Obtiene el valor del registro con todos los flags</summary>
		/// <returns>El valor equivalente al estado de los flags</returns> 
		public int getFlag(){
			int flags = 0;
			int Z = 0x80; // 1000 0000
			int N = 0x40; // 0100 0000
			int H = 0x20; // 0010 0000
			int C = 0x10; // 0001 0000

			if (flagZ == true){ flags |= Z; }
			if (flagH == true){ flags |= H; }
			if (flagN == true){ flags |= N; }
			if (flagC == true){ flags |= C; }
			
			return flags;
		}

		/// <summary>Asigna un nuevo estado a todos los flags a partir de su equivalente numerico</summary>
		/// <param name="flags">Flags</param>
		public void setFlag(int flags){
			int Z = 0x80; // 1000 0000
			int N = 0x40; // 0100 0000
			int H = 0x20; // 0010 0000
			int C = 0x10; // 0001 0000

			if ((flags & Z) != 0){ flagZ = true; }else{ flagZ = false; }
			if ((flags & H) != 0){ flagH = true; }else{ flagH = false; }
			if ((flags & N) != 0){ flagN = true; }else{ flagN = false; }
			if ((flags & C) != 0){ flagC = true; }else{ flagC = false; }
		}
	}
}
