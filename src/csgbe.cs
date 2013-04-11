/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Main File
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

	using csgbe.gui;
	using System;

	/// <summary>Clase principal que crea la interfaz grafica del emulador</summary>
	public class csgbe{

		/// <summary>Punto de entrada principal</summary>
		/// <param name="args"> A list of command line arguments</param>
		public static void Main(string[] args){
			try{
				// Puede recibir una ruta hacia una ROM que cargara automaticamente o ninguno
				if (args.Length == 1) new GUI(args[0]); else new GUI();
			}catch(Exception e){
				Console.WriteLine(e.StackTrace);
			}
		}
	}
}
