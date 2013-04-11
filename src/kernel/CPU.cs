/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# CPU Z80 Core
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

	using csgbe.gui;
	using perifericos;

	/// <summary>Procesador Z80</summary>
	public class CPU : Constantes {

		/// <summary>Velocidad del procesador en MHz</summary>
		private readonly double _MHZ;
		/// <summary>Cantidad de instrucciones emuladas desde el ultimo reinicio</summary>
		private int _instruccionesProcesadas;
		/// <summary>Microsegundos que deberia haber tardado la emulacion</summary>
		private double _tiempo;
		/// <summary>Array con el numero de ciclos transcurridos desde cada ultima interrupcion</summary>
		private int[] _ciclos;
		/// <summary>Lista de puntos de interrupcion de direcciones de memoria para el debug</summary>
		private System.Collections.ArrayList _bp;
		/// <summary>Indica si esta ejecutando el procesador en modo de depuracion</summary>
		private bool _debug;
		/// <summary>Indica si ha ocurrido algun error grave e irrecuperable en la emulacion</summary>
		private bool _error = false;

		/// <summary>Memoria principal</summary>
		private Memoria _memoria;
		/// <summary>Registros internos de la CPU</summary>
		private Registros _registros;
		/// <summary>Sistema de gestion grafica</summary>
		private Graphics _graphics;
		
		/// <summary>Modo de depuracion</summary>
		public bool debug{
			get { return _debug; }
			set { _debug = value; }
		}

		/// <summary>Memoria</summary>
		public Memoria memoria{ get { return _memoria; } }
		/// <summary>Registros</summary>
		public Registros registros{ get { return _registros; } }
		/// <summary>Gestion grafica</summary>
		public Graphics graphics { get { return _graphics; } }

		/// <summary>Constructor</summary>
		/// <param name="memoria">Memoria</param>
		/// <param name="velocidad">Velocidad en MHz</param>
		public CPU(Memoria memoria, double velocidad){
			_memoria = memoria;
			_registros = new Registros();
			_MHZ = velocidad;
			_graphics = new Graphics(_memoria);
			_ciclos = new int[3];
			_bp = new System.Collections.ArrayList();
			reset();
		}

		/// <summary>Asigna un valor por defecto a los registros y a ciertas direcciones de memoria</summary>
		/// <remarks>Equivale a un reinicio fisico de la consola</remarks>
		public void reset(){
			_instruccionesProcesadas = 0;
			_ciclos[0] = 0;
			_ciclos[1] = 0;
			_ciclos[2] = 0;
			_bp.Clear();

			// Estado inicial de los registros y flags
			_registros.flagIME = false;
			_registros.flagZ = true;
			_registros.flagN = false;
			_registros.flagH = true;
			_registros.flagC = true;
			_registros.regA = 0x11;
			_registros.regPC = 0x0100;
			_registros.regSP = 0xFFFE;
			_registros.regBC = 0x0013;
			_registros.regDE = 0x00D8;
			_registros.regHL = 0x014D;

			// Valor inicial de algunas direcciones de memoria
			_memoria.escribir(0xCF, JOYPAD);
			_memoria.escribir(0x00, SERIAL_DATA);
			_memoria.escribir(0x7E, SERIAL_CTRL);
			_memoria.escribir(0xFF, 0xFF03);
			_memoria.escribir(0xAF, DIV_CNTR);
			_memoria.escribir(0x00, TIMER_COUNT);
			_memoria.escribir(0x00, TIMER_RELOAD);
			_memoria.escribir(0xF8, TIMER_CRTL);
			_memoria.escribir(0x00, INT_FLAG);
			
			_memoria.escribir(0x80, SND_1_ENT);
			_memoria.escribir(0xBF, SND_1_WAV_LEN);
			_memoria.escribir(0xF3, SND_1_ENV);
			_memoria.escribir(0xFF, SND_1_FREQ_KICK_LOWER);
			_memoria.escribir(0xBF, SND_1_FREQ_KICK_UPPER);
			_memoria.escribir(0xFF, 0xFF15);
			_memoria.escribir(0x3F, SND_2_WAVE_LEN);
			_memoria.escribir(0x00, SND_2_ENV);
			_memoria.escribir(0xFF, SND_2_FREQ_KICK_LOWER);
			_memoria.escribir(0xBF, SND_2_FREQ_KICK_UPPER);
			_memoria.escribir(0x7F, SND_3_ON_OFF);
			_memoria.escribir(0xFF, SND_3_LEN);
			_memoria.escribir(0x9F, SND_3_VOLUME);
			_memoria.escribir(0xFF, SND_3_FREQ_KICK_LOWER);
			_memoria.escribir(0xBF, SND_3_FREQ_KICK_UPPER);
			_memoria.escribir(0xFF, 0xFF1E);
			_memoria.escribir(0xFF, 0xFF1F);
			_memoria.escribir(0xFF, SND_4_LEN);
			_memoria.escribir(0x00, SND_4_ENV);
			_memoria.escribir(0x00, SND_4_POLY_KICK_LOWER);
			_memoria.escribir(0xBF, SND_4_POLY_KICK_UPPER);
			_memoria.escribir(0x77, SND_VOICE_INP);
			_memoria.escribir(0xF3, SND_STEREO);
			_memoria.escribir(0xF1, SND_STAT);
			
			_memoria.escribir(0x06, SND_BNK_10);
			_memoria.escribir(0xFE, SND_BNK_11);
			_memoria.escribir(0x0E, SND_BNK_12);
			_memoria.escribir(0x7F, SND_BNK_13);
			_memoria.escribir(0x00, SND_BNK_14);
			_memoria.escribir(0xFF, SND_BNK_15);
			_memoria.escribir(0x58, SND_BNK_16);
			_memoria.escribir(0xDF, SND_BNK_17);
			_memoria.escribir(0x00, SND_BNK_20);
			_memoria.escribir(0xEC, SND_BNK_21);
			_memoria.escribir(0x00, SND_BNK_22);
			_memoria.escribir(0xBF, SND_BNK_23);
			_memoria.escribir(0x0C, SND_BNK_24);
			_memoria.escribir(0xED, SND_BNK_25);
			_memoria.escribir(0x03, SND_BNK_26);
			_memoria.escribir(0xF7, SND_BNK_27);
			
			_memoria.escribir(0x91, LCD_CTRL);
			_memoria.escribir(0x85, LCD_STAT);
			_memoria.escribir(0x00, LCD_SCROLL_Y);
			_memoria.escribir(0x00, LCD_SCROLL_X);
			_memoria.escribir(0x00, LCD_Y_LOC);
			_memoria.escribir(0x00, LCD_Y_COMP);
			_memoria.escribir(0x00, LCD_DMA);
			_memoria.escribir(0xFC, LCD_BACK_PALETTE);
			_memoria.escribir(0xFF, LCD_SPR0_PALETTE);
			_memoria.escribir(0xFF, LCD_SPR1_PALETTE);
			_memoria.escribir(0x00, LCD_WIN_Y);
			_memoria.escribir(0x00, LCD_WIN_X);
			_memoria.escribir(0x7E, CPU_SPEED_REG);
			_memoria.escribir(0xFF, 0xFF4E);
			_memoria.escribir(0xFE, VRAM_BANK);
			
			_memoria.escribir(0xFF, 0xFF50);
			_memoria.escribir(0x00, DMA_SRC_UPPER);
			_memoria.escribir(0x00, DMA_SRC_LOWER);
			_memoria.escribir(0x00, DMA_DST_UPPER);
			_memoria.escribir(0x00, DMA_DST_LOWER);
			_memoria.escribir(0xFF, DMA_LEN_TYPE);
			_memoria.escribir(0x00, IR_PORT);
			
			_memoria.escribir(0xC0, BGP_INDEX);
			_memoria.escribir(0x00, BGP_DATA);
			_memoria.escribir(0xC1, OBP_INDEX);
			_memoria.escribir(0x00, OBP_DATA);
			
			_memoria.escribir(0xF8, RAM_BANK);
			
			_memoria.escribir(0x00, INT_ENABLE);		
		}

		/// <summary>Crea o elimina un punto de interrupcion en una direccion de memoria</summary>
		/// <param name="pc">Direccion del contador de programa para producir el punto</param>
		public void toggleBreakpoint(int pc){ if (!_bp.Contains(pc)) _bp.Add(pc); else _bp.Remove(pc); }

		/// <summary>Inicia el proceso de emulacion y solo se detiene cuando encuentre un error</summary>
		public void iniciar(){
			while(!_error){
				if (debug){ 
					Debug.ConsolaDepuracion(this); 
					debug = false;
				}else procesarInstrucciones(1);
			}
		}

		/// <summary>Ejecuta un numero determinado de instrucciones segun el flujo de ejecucion</summary>
		/// <param name="cantidad">Numero de instrucciones a ejecutar</param>
		public void procesarInstrucciones(int cantidad){
			while(cantidad > 0 && !_error){
				// Detiene la ejecucion si encuentra un punto de interrupcion
				if (_bp.Contains(_registros.regPC)){ debug = true; cantidad = 0; }
				_instruccionesProcesadas++;
				// Ciclo principal: procesa instruccion, activa las interrupciones adecuadas simulando
				// el hardware, y finalmente ejecuta las rutinas de tratamiento de interrupciones
				procesarInstruccion();
				comprobarInterrupciones();
				dispararInterrupciones();
				cantidad--;
			}	
		}

		/// <summary>Activa las interrupciones adecuadas y simula su funcionamiento por hardware</summary>
		private void dispararInterrupciones(){
			
			// Interrupcion TIMER
			if ((_memoria.leer(TIMER_CRTL) & 0x04) != 0){
				int timer_max = 0;
				// Velocidad del temporizador
				switch(_memoria.leer(TIMER_CRTL) & 0x03){
					case 0: timer_max = CYCLES_TIMER_MODE0; break;
					case 1: timer_max = CYCLES_TIMER_MODE1; break;
					case 2: timer_max = CYCLES_TIMER_MODE2; break;
					case 3: timer_max = CYCLES_TIMER_MODE3; break;
				}
				if (_ciclos[1] > timer_max){
					_ciclos[1] = 0;
					_memoria.escribir(_memoria.leer(TIMER_COUNT) + 1, TIMER_COUNT);
					// Si desborda se activa la interrupcion y se reinicia el contador
					if (_memoria.leer(TIMER_COUNT) == 0xFF){
						_memoria.escribir(_memoria.leer(TIMER_RELOAD), TIMER_COUNT);
						_memoria.escribir(_memoria.leer(INT_FLAG) | INT_TIMER, INT_FLAG);
					}
				}
			}
			
			// Registro DIV
			if (_ciclos[0] > CYCLES_DIV){
				_memoria.escribir(_memoria.leer(DIV_CNTR) + 1, DIV_CNTR);
				_ciclos[0] = 0;
			}

			// Interrupcion LCDC
			if (_ciclos[2] > CYCLES_LCD_MODE1){
				_ciclos[2] = 0;
				// Aumento de linea de dibujo
				if(_memoria.leer(LCD_Y_LOC) == 0x99) _memoria.escribir(0, LCD_Y_LOC);
				else _memoria.escribir(_memoria.leer(LCD_Y_LOC) + 1, LCD_Y_LOC);

				// Comparacion de linea
				if (_memoria.leer(LCD_Y_LOC) == _memoria.leer(LCD_Y_COMP)){
					_memoria.escribir(_memoria.leer(LCD_STAT) | 0x04, LCD_STAT);
					if ((_memoria.leer(LCD_STAT) & 0x40) > 0) _memoria.escribir(_memoria.leer(INT_FLAG) | INT_LCDC, INT_FLAG);
				}else _memoria.escribir(_memoria.leer(LCD_STAT) & 0xFB, LCD_STAT);
			}
			
			if (_memoria.leer(LCD_Y_LOC) < 144){
				// Modo 10 (Cuando se esta accediendo entre 0xFE00 y 0xFE9F)
				if (_ciclos[2] < CYCLES_LCD_MODE2 && (_memoria.leer(LCD_STAT) & 0x03) != 0x02){
					_memoria.escribir((_memoria.leer(LCD_STAT) & 0xFC) | 0x02, LCD_STAT);	 
					if ((_memoria.leer(LCD_STAT) & 0x20) > 0) _memoria.escribir(_memoria.leer(INT_FLAG) | INT_LCDC, INT_FLAG);
				// Modo 11 
				}else if(_ciclos[2] >= CYCLES_LCD_MODE2 && _ciclos[2] < CYCLES_LCD_MODE3 && (_memoria.leer(LCD_STAT) & 0x03) != 0x03){
					// Se dibujan las primeras 144 lineas cuando se ha dejado de escribir en la zona grafica de memoria
					_graphics.hblank();
					_memoria.escribir((_memoria.leer(LCD_STAT) & 0xFC) | 0x03, LCD_STAT);	 
				// Modo 00 (Durante el HBLANK, la CPU puede acceder a la display RAM entre 0x8000 y 0x9FFF)
				}else if(_ciclos[2] >= CYCLES_LCD_MODE3 && (_memoria.leer(LCD_STAT) & 0x03) != 0){
					_memoria.escribir(_memoria.leer(LCD_STAT) & 0xFC, LCD_STAT);	 
					if ((_memoria.leer(LCD_STAT) & 0x08) > 0) _memoria.escribir(_memoria.leer(INT_FLAG) | INT_LCDC, INT_FLAG);
				}
			// Modo 01 (Periodo VBLANK, la CPU puede acceder a la display RAM entre 0x8000 y 0x9FFF)
			}else if ((_memoria.leer(LCD_Y_LOC) >= 144) && (_memoria.leer(LCD_STAT) & 0x03) != 0x01){
				// Refresco vertical
				_graphics.vblank();
				_memoria.escribir((_memoria.leer(LCD_STAT) & 0xFC) | 0x01, LCD_STAT);	 
				if ((_memoria.leer(LCD_STAT) & 0x10) > 0) _memoria.escribir(_memoria.leer(INT_FLAG) | INT_LCDC, INT_FLAG);
				_memoria.escribir(_memoria.leer(INT_FLAG) | INT_VBLANK, INT_FLAG);
			}
		}

		/// <summary>Inicia las rutinas de tratamiento de cada una de las interrupciones que esten activas</summary>
		private void comprobarInterrupciones(){
			if (_registros.flagIME == true){
				_registros.flagIME = false;
				if ((_memoria.leer(INT_FLAG) & INT_VBLANK) > 0 && (_memoria.leer(INT_ENABLE) & INT_VBLANK) > 0){
					_memoria.escribir(_memoria.leer(INT_FLAG) & ~INT_VBLANK, INT_FLAG);
					Instruccion instruccion = new InstruccionINT(_registros, _memoria, 0x40);
					instruccion.ejecutar();					
				}else if ((_memoria.leer(INT_FLAG) & INT_LCDC) > 0 && (_memoria.leer(INT_ENABLE) & INT_LCDC) > 0){
					_memoria.escribir(_memoria.leer(INT_FLAG) & ~INT_LCDC, INT_FLAG);
					Instruccion instruccion = new InstruccionINT(_registros, _memoria, 0x48);
					instruccion.ejecutar();					
				}else if ((_memoria.leer(INT_FLAG) & INT_TIMER) > 0 && (_memoria.leer(INT_ENABLE) & INT_TIMER) > 0){
					_memoria.escribir(_memoria.leer(INT_FLAG) & ~INT_TIMER, INT_FLAG);
					Instruccion instruccion = new InstruccionINT(_registros, _memoria, 0x50);
					instruccion.ejecutar();					
				}else if ((_memoria.leer(INT_FLAG) & INT_SERIALTX) > 0 && (_memoria.leer(INT_ENABLE) & INT_SERIALTX) > 0){
					_memoria.escribir(_memoria.leer(INT_FLAG) & ~INT_SERIALTX, INT_FLAG);
					Instruccion instruccion = new InstruccionINT(_registros, _memoria, 0x58);
					instruccion.ejecutar();					
				}else if ((_memoria.leer(INT_FLAG) & INT_KEY) > 0 && (_memoria.leer(INT_ENABLE) & INT_KEY) > 0){
					_memoria.escribir(_memoria.leer(INT_FLAG) & ~INT_KEY, INT_FLAG);
					Instruccion instruccion = new InstruccionINT(_registros, _memoria, 0x60);
					instruccion.ejecutar();					
				}else{
					// Sin interrupciones pendientes
				}
				_registros.flagIME = true;
			}
		}

		/// <summary>Ejecuta una instruccion del contador de programa</summary>
		private void procesarInstruccion(){
			int opCod = _memoria.leer(_registros.regPC);
			// El numero maximo argumentos que puede tomar una instruccion son 2. Se leen por si acaso.
			int parm1 = _memoria.leer(_registros.regPC + 1);
			int parm2 = _memoria.leer(_registros.regPC + 2);

			if (debug){
				Debug.Write("#" + _instruccionesProcesadas);
				Debug.Write("\t" + Debug.hexWord(_registros.regPC) + ": " + Debug.hexByte(opCod));
			}
			
			// Instruccion NOP por defecto
			Instruccion instruccion = new InstruccionNOP(_registros, _memoria);
			switch(opCod){
				case 0x00: instruccion = new InstruccionNOP(_registros, _memoria); break;
				case 0x01: instruccion = new InstruccionLD_DD_NN(_registros, _memoria, parm1, parm2, "BC"); break;
				case 0x02: instruccion = new InstruccionLD_RADR_R(_registros, _memoria, "BC", "A"); break;
				case 0x03: instruccion = new InstruccionINC_RR(_registros, _memoria, "BC"); break;
				case 0x04: instruccion = new InstruccionINC_R(_registros, _memoria, "B"); break;
				case 0x05: instruccion = new InstruccionDEC_R(_registros, _memoria, "B"); break;
				case 0x06: instruccion = new InstruccionLD_R_N(_registros, _memoria, parm1, "B"); break;
				case 0x07: instruccion = new InstruccionRLC(_registros, _memoria, "A", false); break;
				case 0x08: instruccion = new InstruccionLD_ADR_RR(_registros, _memoria, parm1, parm2, "SP"); break; 
				case 0x09: instruccion = new InstruccionADD_RR_RR(_registros, _memoria, "HL", "BC"); break;
				case 0x0A: instruccion = new InstruccionLD_R_RADR(_registros, _memoria, "A", "BC"); break;
				case 0x0B: instruccion = new InstruccionDEC_RR(_registros, _memoria, "BC"); break;
				case 0x0C: instruccion = new InstruccionINC_R(_registros, _memoria, "C"); break;
				case 0x0D: instruccion = new InstruccionDEC_R(_registros, _memoria, "C"); break;
				case 0x0E: instruccion = new InstruccionLD_R_N(_registros, _memoria, parm1, "C"); break;
				case 0x0F: instruccion = new InstruccionRRC(_registros, _memoria, "A", false); break;
				
				case 0x10: instruccion = new InstruccionSTOP(_registros, _memoria); break; 
				case 0x11: instruccion = new InstruccionLD_DD_NN(_registros, _memoria, parm1, parm2, "DE"); break;
				case 0x12: instruccion = new InstruccionLD_RADR_R(_registros, _memoria, "DE", "A"); break;
				case 0x13: instruccion = new InstruccionINC_RR(_registros, _memoria, "DE"); break;
				case 0x14: instruccion = new InstruccionINC_R(_registros, _memoria, "D"); break;
				case 0x15: instruccion = new InstruccionDEC_R(_registros, _memoria, "D"); break;
				case 0x16: instruccion = new InstruccionLD_R_N(_registros, _memoria, parm1, "D"); break;
				case 0x17: instruccion = new InstruccionRL(_registros, _memoria, "A", false); break;
				case 0x18: instruccion = new InstruccionJR_N(_registros, _memoria, parm1); break;
				case 0x19: instruccion = new InstruccionADD_RR_RR(_registros, _memoria, "HL", "DE"); break;
				case 0x1A: instruccion = new InstruccionLD_R_RADR(_registros, _memoria, "A", "DE"); break;
				case 0x1B: instruccion = new InstruccionDEC_RR(_registros, _memoria, "DE"); break;
				case 0x1C: instruccion = new InstruccionINC_R(_registros, _memoria, "E"); break;
				case 0x1D: instruccion = new InstruccionDEC_R(_registros, _memoria, "E"); break;
				case 0x1E: instruccion = new InstruccionLD_R_N(_registros, _memoria, parm1, "E"); break;
				case 0x1F: instruccion = new InstruccionRR(_registros, _memoria, "A", false); break;
					   
				case 0x20: instruccion = new InstruccionJR_CC0_N(_registros, _memoria, "Z", parm1); break;
				case 0x21: instruccion = new InstruccionLD_DD_NN(_registros, _memoria, parm1, parm2, "HL"); break;
				case 0x22: instruccion = new InstruccionLDI_RADR_R(_registros, _memoria, "HL", "A"); break;
				case 0x23: instruccion = new InstruccionINC_RR(_registros, _memoria, "HL"); break;
				case 0x24: instruccion = new InstruccionINC_R(_registros, _memoria, "H"); break;
				case 0x25: instruccion = new InstruccionDEC_R(_registros, _memoria, "H"); break;
				case 0x26: instruccion = new InstruccionLD_R_N(_registros, _memoria, parm1, "H"); break;
				case 0x27: instruccion = new InstruccionDAA(_registros, _memoria); break;
				case 0x28: instruccion = new InstruccionJR_CC1_N(_registros, _memoria, "Z", parm1); break;
				case 0x29: instruccion = new InstruccionADD_RR_RR(_registros, _memoria, "HL", "HL"); break;
				case 0x2A: instruccion = new InstruccionLDI_R_RADR(_registros, _memoria, "A", "HL"); break;
				case 0x2B: instruccion = new InstruccionDEC_RR(_registros, _memoria, "HL"); break;
				case 0x2C: instruccion = new InstruccionINC_R(_registros, _memoria, "L"); break;
				case 0x2D: instruccion = new InstruccionDEC_R(_registros, _memoria, "L"); break;
				case 0x2E: instruccion = new InstruccionLD_R_N(_registros, _memoria, parm1, "L"); break;
				case 0x2F: instruccion = new InstruccionCPL(_registros, _memoria); break;
					   
				case 0x30: instruccion = new InstruccionJR_CC0_N(_registros, _memoria, "C", parm1); break;
				case 0x31: instruccion = new InstruccionLD_DD_NN(_registros, _memoria, parm1, parm2, "SP"); break;
				case 0x32: instruccion = new InstruccionLDD_RADR_R(_registros, _memoria, "HL", "A"); break;
				case 0x33: instruccion = new InstruccionINC_RR(_registros, _memoria, "SP"); break;
				case 0x34: instruccion = new InstruccionINC_RADR(_registros, _memoria, "HL"); break;
				case 0x35: instruccion = new InstruccionDEC_RADR(_registros, _memoria, "HL"); break;
				case 0x36: instruccion = new InstruccionLD_RADR_N(_registros, _memoria, "HL", parm1); break;
				case 0x37: instruccion = new InstruccionSCF(_registros, _memoria); break;
				case 0x38: instruccion = new InstruccionJR_CC1_N(_registros, _memoria, "C", parm1); break;
				case 0x39: instruccion = new InstruccionADD_RR_RR(_registros, _memoria, "HL", "SP"); break;
				case 0x3A: instruccion = new InstruccionLDD_R_RADR(_registros, _memoria, "A", "HL"); break;
				case 0x3B: instruccion = new InstruccionDEC_RR(_registros, _memoria, "SP"); break;
				case 0x3C: instruccion = new InstruccionINC_R(_registros, _memoria, "A"); break;
				case 0x3D: instruccion = new InstruccionDEC_R(_registros, _memoria, "A"); break;
				case 0x3E: instruccion = new InstruccionLD_R_N(_registros, _memoria, parm1, "A"); break;
				case 0x3F: instruccion = new InstruccionCCF(_registros, _memoria); break;
					   
				case 0x40: instruccion = new InstruccionLD_R_R(_registros, _memoria, "B", "B"); break;
				case 0x41: instruccion = new InstruccionLD_R_R(_registros, _memoria, "B", "C"); break;
				case 0x42: instruccion = new InstruccionLD_R_R(_registros, _memoria, "B", "D"); break;
				case 0x43: instruccion = new InstruccionLD_R_R(_registros, _memoria, "B", "E"); break;
				case 0x44: instruccion = new InstruccionLD_R_R(_registros, _memoria, "B", "H"); break;
				case 0x45: instruccion = new InstruccionLD_R_R(_registros, _memoria, "B", "L"); break;
				case 0x46: instruccion = new InstruccionLD_R_RADR(_registros, _memoria, "B", "HL"); break;
				case 0x47: instruccion = new InstruccionLD_R_R(_registros, _memoria, "B", "A"); break;
				case 0x48: instruccion = new InstruccionLD_R_R(_registros, _memoria, "C", "B"); break;
				case 0x49: instruccion = new InstruccionLD_R_R(_registros, _memoria, "C", "C"); break;
				case 0x4A: instruccion = new InstruccionLD_R_R(_registros, _memoria, "C", "D"); break;
				case 0x4B: instruccion = new InstruccionLD_R_R(_registros, _memoria, "C", "E"); break;
				case 0x4C: instruccion = new InstruccionLD_R_R(_registros, _memoria, "C", "H"); break;
				case 0x4D: instruccion = new InstruccionLD_R_R(_registros, _memoria, "C", "L"); break;
				case 0x4E: instruccion = new InstruccionLD_R_RADR(_registros, _memoria, "C", "HL"); break;
				case 0x4F: instruccion = new InstruccionLD_R_R(_registros, _memoria, "C", "A"); break;
					   
		     		case 0x50: instruccion = new InstruccionLD_R_R(_registros, _memoria, "D", "B"); break;
				case 0x51: instruccion = new InstruccionLD_R_R(_registros, _memoria, "D", "C"); break;
				case 0x52: instruccion = new InstruccionLD_R_R(_registros, _memoria, "D", "D"); break;
				case 0x53: instruccion = new InstruccionLD_R_R(_registros, _memoria, "D", "E"); break;
				case 0x54: instruccion = new InstruccionLD_R_R(_registros, _memoria, "D", "H"); break;
				case 0x55: instruccion = new InstruccionLD_R_R(_registros, _memoria, "D", "L"); break;
				case 0x56: instruccion = new InstruccionLD_R_RADR(_registros, _memoria, "D", "HL"); break;
				case 0x57: instruccion = new InstruccionLD_R_R(_registros, _memoria, "D", "A"); break;
				case 0x58: instruccion = new InstruccionLD_R_R(_registros, _memoria, "E", "B"); break;
				case 0x59: instruccion = new InstruccionLD_R_R(_registros, _memoria, "E", "C"); break;
				case 0x5A: instruccion = new InstruccionLD_R_R(_registros, _memoria, "E", "D"); break;
				case 0x5B: instruccion = new InstruccionLD_R_R(_registros, _memoria, "E", "E"); break;
				case 0x5C: instruccion = new InstruccionLD_R_R(_registros, _memoria, "E", "H"); break;
				case 0x5D: instruccion = new InstruccionLD_R_R(_registros, _memoria, "E", "L"); break;
				case 0x5E: instruccion = new InstruccionLD_R_RADR(_registros, _memoria, "E", "HL"); break;
				case 0x5F: instruccion = new InstruccionLD_R_R(_registros, _memoria, "E", "A"); break;
					   
			        case 0x60: instruccion = new InstruccionLD_R_R(_registros, _memoria, "H", "B"); break;
				case 0x61: instruccion = new InstruccionLD_R_R(_registros, _memoria, "H", "C"); break;
				case 0x62: instruccion = new InstruccionLD_R_R(_registros, _memoria, "H", "D"); break;
				case 0x63: instruccion = new InstruccionLD_R_R(_registros, _memoria, "H", "E"); break;
				case 0x64: instruccion = new InstruccionLD_R_R(_registros, _memoria, "H", "H"); break;
				case 0x65: instruccion = new InstruccionLD_R_R(_registros, _memoria, "H", "L"); break;
				case 0x66: instruccion = new InstruccionLD_R_RADR(_registros, _memoria, "H", "HL"); break;
				case 0x67: instruccion = new InstruccionLD_R_R(_registros, _memoria, "H", "A"); break;
				case 0x68: instruccion = new InstruccionLD_R_R(_registros, _memoria, "L", "B"); break;
				case 0x69: instruccion = new InstruccionLD_R_R(_registros, _memoria, "L", "C"); break;
				case 0x6A: instruccion = new InstruccionLD_R_R(_registros, _memoria, "L", "D"); break;
				case 0x6B: instruccion = new InstruccionLD_R_R(_registros, _memoria, "L", "E"); break;
				case 0x6C: instruccion = new InstruccionLD_R_R(_registros, _memoria, "L", "H"); break;
				case 0x6D: instruccion = new InstruccionLD_R_R(_registros, _memoria, "L", "L"); break;
				case 0x6E: instruccion = new InstruccionLD_R_RADR(_registros, _memoria, "L", "HL"); break;
				case 0x6F: instruccion = new InstruccionLD_R_R(_registros, _memoria, "L", "A"); break;
				
				case 0x70: instruccion = new InstruccionLD_RADR_R(_registros, _memoria, "HL", "B"); break;
				case 0x71: instruccion = new InstruccionLD_RADR_R(_registros, _memoria, "HL", "C"); break;
				case 0x72: instruccion = new InstruccionLD_RADR_R(_registros, _memoria, "HL", "D"); break;
				case 0x73: instruccion = new InstruccionLD_RADR_R(_registros, _memoria, "HL", "E"); break;
				case 0x74: instruccion = new InstruccionLD_RADR_R(_registros, _memoria, "HL", "H"); break;
				case 0x75: instruccion = new InstruccionLD_RADR_R(_registros, _memoria, "HL", "L"); break;
				case 0x76: instruccion = new InstruccionHALT(_registros, _memoria); break; 
				case 0x77: instruccion = new InstruccionLD_RADR_R(_registros, _memoria, "HL", "A"); break;
				case 0x78: instruccion = new InstruccionLD_R_R(_registros, _memoria, "A", "B"); break;
				case 0x79: instruccion = new InstruccionLD_R_R(_registros, _memoria, "A", "C"); break;
				case 0x7A: instruccion = new InstruccionLD_R_R(_registros, _memoria, "A", "D"); break;
				case 0x7B: instruccion = new InstruccionLD_R_R(_registros, _memoria, "A", "E"); break;
				case 0x7C: instruccion = new InstruccionLD_R_R(_registros, _memoria, "A", "H"); break;
				case 0x7D: instruccion = new InstruccionLD_R_R(_registros, _memoria, "A", "L"); break;
				case 0x7E: instruccion = new InstruccionLD_R_RADR(_registros, _memoria, "A", "HL"); break;
				case 0x7F: instruccion = new InstruccionLD_R_R(_registros, _memoria, "A", "A"); break;

				case 0x80: instruccion = new InstruccionADD_R_R(_registros, _memoria, "A", "B"); break;
				case 0x81: instruccion = new InstruccionADD_R_R(_registros, _memoria, "A", "C"); break;
				case 0x82: instruccion = new InstruccionADD_R_R(_registros, _memoria, "A", "D"); break;
				case 0x83: instruccion = new InstruccionADD_R_R(_registros, _memoria, "A", "E"); break;
				case 0x84: instruccion = new InstruccionADD_R_R(_registros, _memoria, "A", "H"); break;
				case 0x85: instruccion = new InstruccionADD_R_R(_registros, _memoria, "A", "L"); break;
				case 0x86: instruccion = new InstruccionADD_R_RADR(_registros, _memoria, "A", "HL"); break;
				case 0x87: instruccion = new InstruccionADD_R_R(_registros, _memoria, "A", "A"); break;
				case 0x88: instruccion = new InstruccionADC_R_R(_registros, _memoria, "A", "B"); break;
				case 0x89: instruccion = new InstruccionADC_R_R(_registros, _memoria, "A", "C"); break;
				case 0x8A: instruccion = new InstruccionADC_R_R(_registros, _memoria, "A", "D"); break;
				case 0x8B: instruccion = new InstruccionADC_R_R(_registros, _memoria, "A", "E"); break;
				case 0x8C: instruccion = new InstruccionADC_R_R(_registros, _memoria, "A", "H"); break;
				case 0x8D: instruccion = new InstruccionADC_R_R(_registros, _memoria, "A", "L"); break;
				case 0x8E: instruccion = new InstruccionADC_R_RADR(_registros, _memoria, "A", "HL"); break;
				case 0x8F: instruccion = new InstruccionADC_R_R(_registros, _memoria, "A", "A"); break;

				case 0x90: instruccion = new InstruccionSUB_R_R(_registros, _memoria, "A", "B"); break;
				case 0x91: instruccion = new InstruccionSUB_R_R(_registros, _memoria, "A", "C"); break;
				case 0x92: instruccion = new InstruccionSUB_R_R(_registros, _memoria, "A", "D"); break;
				case 0x93: instruccion = new InstruccionSUB_R_R(_registros, _memoria, "A", "E"); break;
				case 0x94: instruccion = new InstruccionSUB_R_R(_registros, _memoria, "A", "H"); break;
				case 0x95: instruccion = new InstruccionSUB_R_R(_registros, _memoria, "A", "L"); break;
				case 0x96: instruccion = new InstruccionSUB_R_RADR(_registros, _memoria, "A", "HL"); break;
				case 0x97: instruccion = new InstruccionSUB_R_R(_registros, _memoria, "A", "A"); break;  
				case 0x98: instruccion = new InstruccionSBC_R_R(_registros, _memoria, "A", "B"); break;
				case 0x99: instruccion = new InstruccionSBC_R_R(_registros, _memoria, "A", "C"); break;
				case 0x9A: instruccion = new InstruccionSBC_R_R(_registros, _memoria, "A", "D"); break;
				case 0x9B: instruccion = new InstruccionSBC_R_R(_registros, _memoria, "A", "E"); break;
				case 0x9C: instruccion = new InstruccionSBC_R_R(_registros, _memoria, "A", "H"); break;
				case 0x9D: instruccion = new InstruccionSBC_R_R(_registros, _memoria, "A", "L"); break;
				case 0x9E: instruccion = new InstruccionSBC_R_RADR(_registros, _memoria, "A", "HL"); break;
				case 0x9F: instruccion = new InstruccionSBC_R_R(_registros, _memoria, "A", "A"); break;

				case 0xA0: instruccion = new InstruccionAND_R_R(_registros, _memoria, "B"); break;
				case 0xA1: instruccion = new InstruccionAND_R_R(_registros, _memoria, "C"); break;
				case 0xA2: instruccion = new InstruccionAND_R_R(_registros, _memoria, "D"); break;
				case 0xA3: instruccion = new InstruccionAND_R_R(_registros, _memoria, "E"); break;
				case 0xA4: instruccion = new InstruccionAND_R_R(_registros, _memoria, "H"); break;
				case 0xA5: instruccion = new InstruccionAND_R_R(_registros, _memoria, "L"); break;
				case 0xA6: instruccion = new InstruccionAND_RADR(_registros, _memoria, "HL"); break;
				case 0xA7: instruccion = new InstruccionAND_R_R(_registros, _memoria, "A"); break;
				case 0xA8: instruccion = new InstruccionXOR_R(_registros, _memoria, "B"); break;
				case 0xA9: instruccion = new InstruccionXOR_R(_registros, _memoria, "C"); break;
				case 0xAA: instruccion = new InstruccionXOR_R(_registros, _memoria, "D"); break;
				case 0xAB: instruccion = new InstruccionXOR_R(_registros, _memoria, "E"); break;
				case 0xAC: instruccion = new InstruccionXOR_R(_registros, _memoria, "H"); break;
				case 0xAD: instruccion = new InstruccionXOR_R(_registros, _memoria, "L"); break;
				case 0xAE: instruccion = new InstruccionXOR_RADR(_registros, _memoria, "HL"); break;
				case 0xAF: instruccion = new InstruccionXOR_R(_registros, _memoria, "A"); break;

				case 0xB0: instruccion = new InstruccionOR_R_R(_registros, _memoria, "B"); break;
				case 0xB1: instruccion = new InstruccionOR_R_R(_registros, _memoria, "C"); break;
				case 0xB2: instruccion = new InstruccionOR_R_R(_registros, _memoria, "D"); break;
				case 0xB3: instruccion = new InstruccionOR_R_R(_registros, _memoria, "E"); break;
				case 0xB4: instruccion = new InstruccionOR_R_R(_registros, _memoria, "H"); break;
				case 0xB5: instruccion = new InstruccionOR_R_R(_registros, _memoria, "L"); break;
				case 0xB6: instruccion = new InstruccionOR_R_ADR(_registros, _memoria, "HL"); break;
				case 0xB7: instruccion = new InstruccionOR_R_R(_registros, _memoria, "A"); break;
				case 0xB8: instruccion = new InstruccionCP_R(_registros, _memoria, "B"); break;
				case 0xB9: instruccion = new InstruccionCP_R(_registros, _memoria, "C"); break;
				case 0xBA: instruccion = new InstruccionCP_R(_registros, _memoria, "D"); break;
				case 0xBB: instruccion = new InstruccionCP_R(_registros, _memoria, "E"); break;
				case 0xBC: instruccion = new InstruccionCP_R(_registros, _memoria, "H"); break;
				case 0xBD: instruccion = new InstruccionCP_R(_registros, _memoria, "L"); break;
				case 0xBE: instruccion = new InstruccionCP_RADR(_registros, _memoria, "HL"); break;
				case 0xBF: instruccion = new InstruccionCP_R(_registros, _memoria, "A"); break;
				
				case 0xC0: instruccion = new InstruccionRET_CC0_ADR(_registros, _memoria, "Z"); break;
				case 0xC1: instruccion = new InstruccionPOP_RR(_registros, _memoria, "BC"); break;
				case 0xC2: instruccion = new InstruccionJP_CC0_ADR(_registros, _memoria, "Z", parm1, parm2); break;
		     		case 0xC3: instruccion = new InstruccionJP_ADR(_registros, _memoria, parm1, parm2); break;
		     		case 0xC4: instruccion = new InstruccionCALL_CC0_ADR(_registros, _memoria, "Z", parm1, parm2); break;
				case 0xC5: instruccion = new InstruccionPUSH_RR(_registros, _memoria, "BC"); break;
				case 0xC6: instruccion = new InstruccionADD_R_N(_registros, _memoria, "A", parm1); break;
				case 0xC7: instruccion = new InstruccionRST(_registros, _memoria, 0x00); break;
				case 0xC8: instruccion = new InstruccionRET_CC1_ADR(_registros, _memoria, "Z"); break;
				case 0xC9: instruccion = new InstruccionRET_ADR(_registros, _memoria); break;
				case 0xCA: instruccion = new InstruccionJP_CC1_ADR(_registros, _memoria, "Z", parm1, parm2); break;
				case 0xCB: instruccion = procesarInstruccionCB(parm1, parm2); break;
		     		case 0xCC: instruccion = new InstruccionCALL_CC1_ADR(_registros, _memoria, "Z", parm1, parm2); break;
				case 0xCD: instruccion = new InstruccionCALL_ADR(_registros, _memoria, parm1, parm2); break;
				case 0xCE: instruccion = new InstruccionADC_R_N(_registros, _memoria, "A", parm1); break;
				case 0xCF: instruccion = new InstruccionRST(_registros, _memoria, 0x08); break;

				case 0xD0: instruccion = new InstruccionRET_CC0_ADR(_registros, _memoria, "C"); break;
				case 0xD1: instruccion = new InstruccionPOP_RR(_registros, _memoria, "DE"); break;
				case 0xD2: instruccion = new InstruccionJP_CC0_ADR(_registros, _memoria, "C", parm1, parm2); break;
				case 0xD3: _error = true; break;
		     		case 0xD4: instruccion = new InstruccionCALL_CC0_ADR(_registros, _memoria, "C", parm1, parm2); break;
				case 0xD5: instruccion = new InstruccionPUSH_RR(_registros, _memoria, "DE"); break;
				case 0xD6: instruccion = new InstruccionSUB_R_N(_registros, _memoria, "A", parm1); break;
				case 0xD7: instruccion = new InstruccionRST(_registros, _memoria, 0x10); break;
				case 0xD8: instruccion = new InstruccionRET_CC1_ADR(_registros, _memoria, "C"); break;
				case 0xD9: instruccion = new InstruccionRETI(_registros, _memoria); break;
				case 0xDA: instruccion = new InstruccionJP_CC1_ADR(_registros, _memoria, "C", parm1, parm2); break;
				case 0xDB: _error = true; break;
		     		case 0xDC: instruccion = new InstruccionCALL_CC1_ADR(_registros, _memoria, "C", parm1, parm2); break;
				case 0xDD: _error = true; break;
				case 0xDE: instruccion = new InstruccionSBC_R_N(_registros, _memoria, "A", parm1); break;
				case 0xDF: instruccion = new InstruccionRST(_registros, _memoria, 0x18); break;
				
				case 0xE0: instruccion = new InstruccionLD_DADR_R(_registros, _memoria, "A", parm1); break;
				case 0xE1: instruccion = new InstruccionPOP_RR(_registros, _memoria, "HL"); break;
				case 0xE2: instruccion = new InstruccionLD_DR_R(_registros, _memoria, "C", "A"); break;
				case 0xE3: _error = true; break;
				case 0xE4: _error = true; break;
				case 0xE5: instruccion = new InstruccionPUSH_RR(_registros, _memoria, "HL"); break;
				case 0xE6: instruccion = new InstruccionAND_R_N(_registros, _memoria, parm1); break;
				case 0xE7: instruccion = new InstruccionRST(_registros, _memoria, 0x20); break;
				case 0xE8: instruccion = new InstruccionADD_R_NN(_registros, _memoria, "SP", parm1); break;
				case 0xE9: instruccion = new InstruccionJP_RADR(_registros, _memoria, "HL"); break;
				case 0xEA: instruccion = new InstruccionLD_ADR_R(_registros, _memoria, parm1, parm2, "A"); break;
				case 0xEB: _error = true; break;
				case 0xEC: _error = true; break;
				case 0xED: _error = true; break;
				case 0xEE: instruccion = new InstruccionXOR_N(_registros, _memoria, parm1); break;
				case 0xEF: instruccion = new InstruccionRST(_registros, _memoria, 0x28); break;
				
				case 0xF0: instruccion = new InstruccionLD_R_DADR(_registros, _memoria, "A", parm1); break;
				case 0xF1: instruccion = new InstruccionPOP_RR(_registros, _memoria, "AF"); break;
				case 0xF2: instruccion = new InstruccionLD_R_DR(_registros, _memoria, "A", "C"); break;
				case 0xF3: instruccion = new InstruccionDI(_registros, _memoria); break;
				case 0xF4: _error = true; break;
				case 0xF5: instruccion = new InstruccionPUSH_RR(_registros, _memoria, "AF"); break;
				case 0xF6: instruccion = new InstruccionOR_N(_registros, _memoria, parm1); break;
				case 0xF7: instruccion = new InstruccionRST(_registros, _memoria, 0x30); break;
				case 0xF8: instruccion = new InstruccionLD_R_SPD(_registros, _memoria, "HL", parm1); break;
				case 0xF9: instruccion = new InstruccionLD_R_R(_registros, _memoria, "SP", "HL"); break;
				case 0xFA: instruccion = new InstruccionLD_R_ADR(_registros, _memoria, "A", parm1, parm2); break;
				case 0xFB: instruccion = new InstruccionEI(_registros, _memoria); break;
				case 0xFC: _error = true; break;
				case 0xFD: _error = true; break;
				case 0xFE: instruccion = new InstruccionCP_N(_registros, _memoria, parm1); break;
				case 0xFF: instruccion = new InstruccionRST(_registros, _memoria, 0x38); break;
					   
				default: _error = true; break;
			}

			if (_error) Debug.WriteLine("[ERROR] Instruccion " + Debug.hexByte(opCod) + " no esperada");
			
			if (debug){			
				Debug.Write("\t" + instruccion.nombre);
				if (instruccion.nombre.Length < 8){ Debug.Write("\t\t"); } 
				else if (instruccion.nombre.Length < 16){ Debug.Write("\t"); }
			}

			// Ejecuta la instruccion y recoge su duracion teorica
			int ticks = instruccion.ejecutar();
			
			_tiempo += (ticks / _MHZ); // Microsegundos
			// Como el metodo Sleep recibe como minimo 1 ms, espera hasta acumular 1 ms al menos
			int tiempo_ms = (int)(_tiempo % 1000);
			if (tiempo_ms >= 1){
//				System.Threading.Thread.Sleep(tiempo_ms);
				_tiempo -= (tiempo_ms * 1000);
			}

			// Aumenta los contadores de ciclos para las interrupciones
			for (int i = 0; i < _ciclos.Length; i++) _ciclos[i] += ticks;

			if (debug){
				Debug.Write("\tZ:" + _registros.flagZ + "\tN:" + _registros.flagN + "\tH:" + _registros.flagH + "\tC:" + _registros.flagC);
				Debug.Write("\tA:" + Debug.hexByte(_registros.regA) + "\tB:" + Debug.hexByte(_registros.regB) + "\tC:" + Debug.hexByte(_registros.regC) + "\tD:" + Debug.hexByte(_registros.regD) + "\tE:" + Debug.hexByte(_registros.regE) + "\tH:" + Debug.hexByte(_registros.regH) + "\tL:" + Debug.hexByte(_registros.regL));
				Debug.WriteLine();
			}

		}

		/// <summary>Decodifica una instruccion extendida por el opCode CB</summary>
		/// <param name="parm1">Primer parametro</param>
		/// <param name="parm2">Segundo parametro</param>
		/// <returns>Devuelve la instruccion decodificada lista para ejecutar</returns>
		private Instruccion procesarInstruccionCB(int parm1, int parm2){
			// Instruccion NOP por defecto
			Instruccion instruccion = new InstruccionNOP(_registros, _memoria);
			switch(parm1){
				case 0x00: instruccion = new InstruccionRLC(_registros, _memoria, "B", true); break;
				case 0x01: instruccion = new InstruccionRLC(_registros, _memoria, "C", true); break;
				case 0x02: instruccion = new InstruccionRLC(_registros, _memoria, "D", true); break;
				case 0x03: instruccion = new InstruccionRLC(_registros, _memoria, "E", true); break;
				case 0x04: instruccion = new InstruccionRLC(_registros, _memoria, "H", true); break;
				case 0x05: instruccion = new InstruccionRLC(_registros, _memoria, "L", true); break;
				case 0x06: instruccion = new InstruccionRLC_RADR(_registros, _memoria, "HL"); break;
				case 0x07: instruccion = new InstruccionRLC(_registros, _memoria, "A", true); break;
				case 0x08: instruccion = new InstruccionRRC(_registros, _memoria, "B", true); break;
				case 0x09: instruccion = new InstruccionRRC(_registros, _memoria, "C", true); break;
				case 0x0A: instruccion = new InstruccionRRC(_registros, _memoria, "D", true); break;
				case 0x0B: instruccion = new InstruccionRRC(_registros, _memoria, "E", true); break;
				case 0x0C: instruccion = new InstruccionRRC(_registros, _memoria, "H", true); break;
				case 0x0D: instruccion = new InstruccionRRC(_registros, _memoria, "L", true); break;
				case 0x0E: instruccion = new InstruccionRRC_RADR(_registros, _memoria, "HL"); break;
				case 0x0F: instruccion = new InstruccionRRC(_registros, _memoria, "A", true); break;
					
				case 0x10: instruccion = new InstruccionRL(_registros, _memoria, "B", true); break;
				case 0x11: instruccion = new InstruccionRL(_registros, _memoria, "C", true); break;
				case 0x12: instruccion = new InstruccionRL(_registros, _memoria, "D", true); break;
				case 0x13: instruccion = new InstruccionRL(_registros, _memoria, "E", true); break;
				case 0x14: instruccion = new InstruccionRL(_registros, _memoria, "H", true); break;
				case 0x15: instruccion = new InstruccionRL(_registros, _memoria, "L", true); break;
				case 0x16: instruccion = new InstruccionRL_RADR(_registros, _memoria, "HL"); break;
				case 0x17: instruccion = new InstruccionRL(_registros, _memoria, "A", true); break;
				case 0x18: instruccion = new InstruccionRR(_registros, _memoria, "B", true); break;
				case 0x19: instruccion = new InstruccionRR(_registros, _memoria, "C", true); break;
				case 0x1A: instruccion = new InstruccionRR(_registros, _memoria, "D", true); break;
				case 0x1B: instruccion = new InstruccionRR(_registros, _memoria, "E", true); break;
				case 0x1C: instruccion = new InstruccionRR(_registros, _memoria, "H", true); break;
				case 0x1D: instruccion = new InstruccionRR(_registros, _memoria, "L", true); break;
				case 0x1E: instruccion = new InstruccionRR_RADR(_registros, _memoria, "HL"); break;
				case 0x1F: instruccion = new InstruccionRR(_registros, _memoria, "A", true); break;
					   
				case 0x20: instruccion = new InstruccionSLA(_registros, _memoria, "B"); break;
				case 0x21: instruccion = new InstruccionSLA(_registros, _memoria, "C"); break;
				case 0x22: instruccion = new InstruccionSLA(_registros, _memoria, "D"); break;
				case 0x23: instruccion = new InstruccionSLA(_registros, _memoria, "E"); break;
				case 0x24: instruccion = new InstruccionSLA(_registros, _memoria, "H"); break;
				case 0x25: instruccion = new InstruccionSLA(_registros, _memoria, "L"); break;
				case 0x26: instruccion = new InstruccionSLA_RADR(_registros, _memoria, "HL"); break; 
				case 0x27: instruccion = new InstruccionSLA(_registros, _memoria, "A"); break;
				case 0x28: instruccion = new InstruccionSRA(_registros, _memoria, "B"); break;
				case 0x29: instruccion = new InstruccionSRA(_registros, _memoria, "C"); break;
				case 0x2A: instruccion = new InstruccionSRA(_registros, _memoria, "D"); break;
				case 0x2B: instruccion = new InstruccionSRA(_registros, _memoria, "E"); break;
				case 0x2C: instruccion = new InstruccionSRA(_registros, _memoria, "H"); break;
				case 0x2D: instruccion = new InstruccionSRA(_registros, _memoria, "L"); break;
				case 0x2E: instruccion = new InstruccionSRA_RADR(_registros, _memoria, "HL"); break;
				case 0x2F: instruccion = new InstruccionSRA(_registros, _memoria, "A"); break;
					   
				case 0x30: instruccion = new InstruccionSWAP_R(_registros, _memoria, "B"); break;
				case 0x31: instruccion = new InstruccionSWAP_R(_registros, _memoria, "C"); break;
				case 0x32: instruccion = new InstruccionSWAP_R(_registros, _memoria, "D"); break;
				case 0x33: instruccion = new InstruccionSWAP_R(_registros, _memoria, "E"); break;
				case 0x34: instruccion = new InstruccionSWAP_R(_registros, _memoria, "H"); break;
				case 0x35: instruccion = new InstruccionSWAP_R(_registros, _memoria, "L"); break;
				case 0x36: instruccion = new InstruccionSWAP_RADR(_registros, _memoria, "HL"); break;
				case 0x37: instruccion = new InstruccionSWAP_R(_registros, _memoria, "A"); break;
				case 0x38: instruccion = new InstruccionSRL(_registros, _memoria, "B"); break;
				case 0x39: instruccion = new InstruccionSRL(_registros, _memoria, "C"); break;
				case 0x3A: instruccion = new InstruccionSRL(_registros, _memoria, "D"); break;
				case 0x3B: instruccion = new InstruccionSRL(_registros, _memoria, "E"); break;
				case 0x3C: instruccion = new InstruccionSRL(_registros, _memoria, "H"); break;
				case 0x3D: instruccion = new InstruccionSRL(_registros, _memoria, "L"); break;
				case 0x3E: instruccion = new InstruccionSRL_RADR(_registros, _memoria, "HL"); break;
				case 0x3F: instruccion = new InstruccionSRL(_registros, _memoria, "A"); break;
					   
				case 0x40: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x01, "B"); break;
				case 0x41: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x01, "C"); break;
				case 0x42: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x01, "D"); break;
				case 0x43: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x01, "E"); break;
				case 0x44: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x01, "H"); break;
				case 0x45: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x01, "L"); break;
				case 0x46: instruccion = new InstruccionBIT_RADR(_registros, _memoria, (byte)0x01, "HL"); break;
				case 0x47: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x01, "A"); break;
				case 0x48: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x02, "B"); break;
				case 0x49: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x02, "C"); break;
				case 0x4A: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x02, "D"); break;
				case 0x4B: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x02, "E"); break;
				case 0x4C: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x02, "H"); break;
				case 0x4D: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x02, "L"); break;
				case 0x4E: instruccion = new InstruccionBIT_RADR(_registros, _memoria, (byte)0x02, "HL"); break;
				case 0x4F: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x02, "A"); break;
					   
				case 0x50: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x04, "B"); break;
				case 0x51: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x04, "C"); break;
				case 0x52: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x04, "D"); break;
				case 0x53: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x04, "E"); break;
				case 0x54: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x04, "H"); break;
				case 0x55: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x04, "L"); break;
				case 0x56: instruccion = new InstruccionBIT_RADR(_registros, _memoria, (byte)0x04, "HL"); break;
				case 0x57: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x04, "A"); break;
				case 0x58: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x08, "B"); break;
				case 0x59: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x08, "C"); break;
				case 0x5A: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x08, "D"); break;
				case 0x5B: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x08, "E"); break;
				case 0x5C: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x08, "H"); break;
				case 0x5D: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x08, "L"); break;
				case 0x5E: instruccion = new InstruccionBIT_RADR(_registros, _memoria, (byte)0x08, "HL"); break;
				case 0x5F: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x08, "A"); break;
					   
				case 0x60: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x10, "B"); break;
				case 0x61: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x10, "C"); break;
				case 0x62: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x10, "D"); break;
				case 0x63: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x10, "E"); break;
				case 0x64: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x10, "H"); break;
				case 0x65: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x10, "L"); break;
				case 0x66: instruccion = new InstruccionBIT_RADR(_registros, _memoria, (byte)0x10, "HL"); break;
				case 0x67: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x10, "A"); break;
				case 0x68: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x20, "B"); break;
				case 0x69: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x20, "C"); break;
				case 0x6A: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x20, "D"); break;
				case 0x6B: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x20, "E"); break;
				case 0x6C: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x20, "H"); break;
				case 0x6D: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x20, "L"); break;
				case 0x6E: instruccion = new InstruccionBIT_RADR(_registros, _memoria, (byte)0x20, "HL"); break;
				case 0x6F: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x20, "A"); break;
					   
				case 0x70: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x40, "B"); break;
				case 0x71: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x40, "C"); break;
				case 0x72: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x40, "D"); break;
				case 0x73: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x40, "E"); break;
				case 0x74: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x40, "H"); break;
				case 0x75: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x40, "L"); break;
				case 0x76: instruccion = new InstruccionBIT_RADR(_registros, _memoria, (byte)0x40, "HL"); break;
				case 0x77: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x40, "A"); break;
				case 0x78: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x80, "B"); break;
				case 0x79: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x80, "C"); break;
				case 0x7A: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x80, "D"); break;
				case 0x7B: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x80, "E"); break;
				case 0x7C: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x80, "H"); break;
				case 0x7D: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x80, "L"); break;
				case 0x7E: instruccion = new InstruccionBIT_RADR(_registros, _memoria, (byte)0x80, "HL"); break;
				case 0x7F: instruccion = new InstruccionBIT_R(_registros, _memoria, (byte)0x80, "A"); break;

				case 0x80: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x01, "B"); break;
				case 0x81: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x01, "C"); break;
				case 0x82: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x01, "D"); break;
				case 0x83: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x01, "E"); break;
				case 0x84: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x01, "H"); break;
				case 0x85: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x01, "L"); break;
				case 0x86: instruccion = new InstruccionRES_RADR(_registros, _memoria, (byte)0x01, "HL"); break;
				case 0x87: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x01, "A"); break;
				case 0x88: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x02, "B"); break;
				case 0x89: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x02, "C"); break;
				case 0x8A: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x02, "D"); break;
				case 0x8B: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x02, "E"); break;
				case 0x8C: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x02, "H"); break;
				case 0x8D: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x02, "L"); break;
				case 0x8E: instruccion = new InstruccionRES_RADR(_registros, _memoria, (byte)0x02, "HL"); break;
				case 0x8F: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x02, "A"); break;

				case 0x90: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x04, "B"); break;
				case 0x91: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x04, "C"); break;
				case 0x92: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x04, "D"); break;
				case 0x93: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x04, "E"); break;
				case 0x94: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x04, "H"); break;
				case 0x95: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x04, "L"); break;
				case 0x96: instruccion = new InstruccionRES_RADR(_registros, _memoria, (byte)0x04, "HL"); break;
				case 0x97: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x04, "A"); break;
				case 0x98: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x08, "B"); break;
				case 0x99: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x08, "C"); break;
				case 0x9A: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x08, "D"); break;
				case 0x9B: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x08, "E"); break;
				case 0x9C: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x08, "H"); break;
				case 0x9D: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x08, "L"); break;
				case 0x9E: instruccion = new InstruccionRES_RADR(_registros, _memoria, (byte)0x08, "HL"); break;
				case 0x9F: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x08, "A"); break;
					   
				case 0xA0: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x10, "B"); break;
				case 0xA1: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x10, "C"); break;
				case 0xA2: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x10, "D"); break;
				case 0xA3: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x10, "E"); break;
				case 0xA4: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x10, "H"); break;
				case 0xA5: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x10, "L"); break;
				case 0xA6: instruccion = new InstruccionRES_RADR(_registros, _memoria, (byte)0x10, "HL"); break;
				case 0xA7: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x10, "A"); break;
				case 0xA8: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x20, "B"); break;
				case 0xA9: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x20, "C"); break;
				case 0xAA: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x20, "D"); break;
				case 0xAB: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x20, "E"); break;
				case 0xAC: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x20, "H"); break;
				case 0xAD: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x20, "L"); break;
				case 0xAE: instruccion = new InstruccionRES_RADR(_registros, _memoria, (byte)0x20, "HL"); break;
				case 0xAF: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x20, "A"); break;
					   
				case 0xB0: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x40, "B"); break;
				case 0xB1: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x40, "C"); break;
				case 0xB2: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x40, "D"); break;
				case 0xB3: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x40, "E"); break;
				case 0xB4: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x40, "H"); break;
				case 0xB5: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x40, "L"); break;
				case 0xB6: instruccion = new InstruccionRES_RADR(_registros, _memoria, (byte)0x40, "HL"); break;
				case 0xB7: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x40, "A"); break;
				case 0xB8: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x80, "B"); break;
				case 0xB9: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x80, "C"); break;
				case 0xBA: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x80, "D"); break;
				case 0xBB: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x80, "E"); break;
				case 0xBC: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x80, "H"); break;
				case 0xBD: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x80, "L"); break;
				case 0xBE: instruccion = new InstruccionRES_RADR(_registros, _memoria, (byte)0x80, "HL"); break;
				case 0xBF: instruccion = new InstruccionRES_R(_registros, _memoria, (byte)0x80, "A"); break;
					   
				case 0xC0: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x01, "B"); break;
				case 0xC1: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x01, "C"); break;
				case 0xC2: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x01, "D"); break;
				case 0xC3: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x01, "E"); break;
				case 0xC4: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x01, "H"); break;
				case 0xC5: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x01, "L"); break;
				case 0xC6: instruccion = new InstruccionSET_RADR(_registros, _memoria, (byte)0x01, "HL"); break;
				case 0xC7: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x01, "A"); break;
				case 0xC8: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x02, "B"); break;
				case 0xC9: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x02, "C"); break;
				case 0xCA: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x02, "D"); break;
				case 0xCB: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x02, "E"); break;
				case 0xCC: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x02, "H"); break;
				case 0xCD: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x02, "L"); break;
				case 0xCE: instruccion = new InstruccionSET_RADR(_registros, _memoria, (byte)0x02, "HL"); break;
				case 0xCF: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x02, "A"); break;
					   
				case 0xD0: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x04, "B"); break;
				case 0xD1: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x04, "C"); break;
				case 0xD2: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x04, "D"); break;
				case 0xD3: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x04, "E"); break;
				case 0xD4: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x04, "H"); break;
				case 0xD5: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x04, "L"); break;
				case 0xD6: instruccion = new InstruccionSET_RADR(_registros, _memoria, (byte)0x04, "HL"); break;
				case 0xD7: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x04, "A"); break;
				case 0xD8: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x08, "B"); break;
				case 0xD9: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x08, "C"); break;
				case 0xDA: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x08, "D"); break;
				case 0xDB: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x08, "E"); break;
				case 0xDC: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x08, "H"); break;
				case 0xDD: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x08, "L"); break;
				case 0xDE: instruccion = new InstruccionSET_RADR(_registros, _memoria, (byte)0x08, "HL"); break;
				case 0xDF: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x08, "A"); break;
					   
				case 0xE0: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x10, "B"); break;
				case 0xE1: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x10, "C"); break;
				case 0xE2: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x10, "D"); break;
				case 0xE3: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x10, "E"); break;
				case 0xE4: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x10, "H"); break;
				case 0xE5: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x10, "L"); break;
				case 0xE6: instruccion = new InstruccionSET_RADR(_registros, _memoria, (byte)0x10, "HL"); break;
				case 0xE7: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x10, "A"); break;
				case 0xE8: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x20, "B"); break;
				case 0xE9: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x20, "C"); break;
				case 0xEA: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x20, "D"); break;
				case 0xEB: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x20, "E"); break;
				case 0xEC: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x20, "H"); break;
				case 0xED: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x20, "L"); break;
				case 0xEE: instruccion = new InstruccionSET_RADR(_registros, _memoria, (byte)0x20, "HL"); break;
				case 0xEF: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x20, "A"); break;
					   
				case 0xF0: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x40, "B"); break;
				case 0xF1: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x40, "C"); break;
				case 0xF2: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x40, "D"); break;
				case 0xF3: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x40, "E"); break;
				case 0xF4: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x40, "H"); break;
				case 0xF5: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x40, "L"); break;
				case 0xF6: instruccion = new InstruccionSET_RADR(_registros, _memoria, (byte)0x40, "HL"); break;
				case 0xF7: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x40, "A"); break;
				case 0xF8: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x80, "B"); break;
				case 0xF9: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x80, "C"); break;
				case 0xFA: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x80, "D"); break;
				case 0xFB: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x80, "E"); break;
				case 0xFC: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x80, "H"); break;
				case 0xFD: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x80, "L"); break;
				case 0xFE: instruccion = new InstruccionSET_RADR(_registros, _memoria, (byte)0x80, "HL"); break;
				case 0xFF: instruccion = new InstruccionSET_R(_registros, _memoria, (byte)0x80, "A"); break;
					   
				default: _error = true; Debug.WriteLine("[ERROR] Instruccion CB " + Debug.hexByte(parm1) + " no esperada"); break;
			}
			// Aumenta el contador de programa en 1 siempre por el prefijo CB comun
			_registros.regPC += 1;
			return instruccion;
		}
	}
}
