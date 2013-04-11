
namespace csgbe.perifericos{

	using csgbe.kernel;
	using Gdk;

	/// <summary>Gestiona la pulsacion de las teclas de la consola emulada</summary>
	public class Keypad : Constantes{

		/// <summary>Estado actual de las teclas</summary>
		//                                Down,  Up,    Left,  Right, Start, Select, B,    A
		private static bool[] _teclas = {false, false, false, false, false, false, false, false};

		/// <summary>Invierte el estado de una tecla</summary>
		/// <param name="tecla">Identificador de tecla a modificar</param>
		public static void toggleTecla(int tecla){ _teclas[tecla] = !_teclas[tecla]; }

		/// <summary>Registra la pulsacion de una tecla</summary>
		/// <param name="tecla">Tecla pulsada</param>
		public static void teclaPulsada(Key tecla){
			switch(tecla){
				case Key.Down: 		_teclas[0] = true; break;
				case Key.Up: 		_teclas[1] = true; break;
				case Key.Left: 		_teclas[2] = true; break;
				case Key.Right: 	_teclas[3] = true; break;
				case Key.Return: 	_teclas[4] = true; break;
				case Key.Shift_R: 	_teclas[5] = true; break;
				case Key.z: 		_teclas[6] = true; break;
				case Key.x: 		_teclas[7] = true; break;
			}
		}
		
		/// <summary>Registra la liberacion de una tecla pulsada previamente</summary>
		/// <param name="tecla">Tecla liberada</param>
		public static void teclaLiberada(Key tecla){
			switch(tecla){
				case Key.Down: 		_teclas[0] = false; break;
				case Key.Up: 		_teclas[1] = false; break;
				case Key.Left: 		_teclas[2] = false; break;
				case Key.Right: 	_teclas[3] = false; break;
				case Key.Return: 	_teclas[4] = false; break;
				case Key.Shift_R: 	_teclas[5] = false; break;
				case Key.z: 		_teclas[6] = false; break;
				case Key.x: 		_teclas[7] = false; break;
			}
		}

		/// <summary>Actualiza la direccion de memoria adecuada segun la matriz de teclas solicitada con el estado
		/// actual de las teclas</summary>
		/// <param name="memoria">Memoria</param>
		public static void actualizar(Memoria memoria){
			byte joypad = memoria.ram[JOYPAD];
			joypad &= 0xF0;
			// Segun la peticion, se actualiza la memoria con las teclas apropiadas
			switch(joypad){
				case 0x30: joypad = 0x3F; break;
				case 0x20: if (!_teclas[0]) joypad |= 0x08; // Down
					   if (!_teclas[1]) joypad |= 0x04; // Up
					   if (!_teclas[2]) joypad |= 0x02; // Left
					   if (!_teclas[3]) joypad |= 0x01; // Right
					   break;
				case 0x10: if (!_teclas[4]) joypad |= 0x08; // Start
					   if (!_teclas[5]) joypad |= 0x04; // Select
					   if (!_teclas[6]) joypad |= 0x02; // B
					   if (!_teclas[7]) joypad |= 0x01; // A
					   break;
			}
			// Accede directamente al array de la memoria para evitar bucles infinitos con las funciones de gestion
			// del acceso a memoria
			memoria.ram[JOYPAD] = joypad;
		}
	}
}
