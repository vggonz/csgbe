/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# GameBoy object
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

namespace csgbe{

	using csgbe.kernel;
	using csgbe.perifericos;
	using csgbe.gui;
	using System.Threading;

	/// <summary>Clase que representa el conjunto de la consola GameBoy</summary>
	public class GB{

		/// <summary>Cartucho con la ROM del juego</summary>
		private Cartucho _cartucho;
		/// <summary>Memoria principal</summary>
		private Memoria _memoria;
		/// <summary>Procesador Z80</summary>
		private CPU _cpu;
		/// <summary>Hilo de ejecucion para el procesador</summary>
		private Thread _thread;

		/// <summary>Memoria de la consola</summary>
		public Memoria memoria { get { return _memoria; } }
		/// <summary>Procesador principal</summary>
		public CPU cpu { get { return _cpu; } }
		/// <summary>Cartucho</summary>
		public Cartucho cartucho { get { return _cartucho; } }
		/// <summary>Sistema grafico de la consola</summary>
		public Graphics graphics { get { return _cpu.graphics; } }

		/// <summary>Constructor</summary>
		/// <param name="nombreCartucho">Ruta al fichero de la ROM</param>
		public GB(string nombreCartucho){
			// Crea el cartucho, memoria y procesador con los parametros por defecto
			_cartucho = Cartucho.cargarCartucho(nombreCartucho);
			Debug.WriteLine(_cartucho.info());
			Debug.WriteLine();
			_memoria = new Memoria(Constantes.MEMSIZE, _cartucho);
			_cpu = new CPU(_memoria, Constantes.CPU_SPEED);
		}
	
		/// <summary>Inicia el procesador emulado</summary>	
		public void iniciar(){
			// Elimina el hilo de ejecucion anterior si lo hay
			if (_thread != null){ _thread.Abort(); }
			_cpu.reset();
			_thread = new Thread(new ThreadStart(_cpu.iniciar));
			_thread.Start();
		}

		/// <summary>Comprueba si el proceso de emulacion esta activo</summary>
		/// <returns>true si la emulacion esta iniciada o pausada o false en caso contrario</returns>
		public bool estaIniciada(){ return ((_thread != null && _thread.IsAlive == true) ? true : false); }

		/// <summary>Detiene la emulacion si esta activa</summary>
		public void parar(){ if (_thread != null && _thread.IsAlive == true) _thread.Abort(); }
		/// <summary>Suspende temporalmente la emulacion si esta activa</summary>
		public void pausar(){ if (_thread != null && _thread.IsAlive == true) _thread.Suspend(); }
		/// <summary>Reanuda la emulacion si esta activa y pausada</summary>
		public void reanudar(){ if (_thread != null && _thread.IsAlive == true) _thread.Resume(); }

	}
}
