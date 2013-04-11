/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Graphic Interface in GTK#
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

namespace csgbe.gui{

	using System;
	using Gtk;
	using csgbe.perifericos;

	/// <summary>Interfaz grafica de la aplicacion</summary>
	class GUI {

		/// <summary>Consola</summary>
		private static GB _consola;
		/// <summary>Pantalla</summary>
		private static Pantalla _pantalla;

		private static MenuBar menu;
		private static AccelGroup group;
		private static Menu menuArchivo, menuAyuda, menuConsola, menuConsolaZoom;
		private static MenuItem menuItemConsola, menuItemArchivo, menuItemAyuda,
			menuItemAyudaAcerca, menuItemConsolaReanudar, menuItemConsolaPausa,
			menuItemConsolaParar, menuItemConsolaZoom, menuItemConsolaZoom1x, 
			menuItemConsolaZoom2x, menuItemConsolaZoom3x, menuItemConsolaZoom4x, 
			menuItemConsolaDebug, menuItemConsolaIniciar, menuItemConsolaReset;
		private static ImageMenuItem menuItemArchivoSalir, menuItemArchivoAbrir;
		private static Statusbar statusBar;
		private static string _lastDir;

		/// <summary>Constructor por defecto</summary>
		/// <remarks>Crea el bucle principal de Gtk</remarks>
		public GUI(){ 
			Application.Init();
			iniciarGUI();
			Application.Run();
		}

		/// <summary>Constructor con cartucho inicial</summary>
		/// <remarks>Crea una consola con el cartucho y lo inicia</remarks>
		public GUI(String rom){
			Application.Init();
			iniciarGUI();
			iniciarConsola(rom);
			Start_Emu(null, null);
			Application.Run();
		}

		/// <summary>Inicializa los objectos de la interfaz grafica</summary>
		private void iniciarGUI(){
			// Pantalla de 144x160 pixeles y zoom por defecto de 2
			_pantalla = new Pantalla(144, 160, 2);

			VBox vbox = new VBox(false, 0);
			Gtk.Window window = new Gtk.Window ("CSGBE: C# GameBoy Emulator");
			window.Icon = Gdk.Pixbuf.LoadFromResource("gb_icon");
			window.DeleteEvent += new DeleteEventHandler (Window_Delete);
			window.KeyPressEvent += new KeyPressEventHandler(Key_Pressed);
			window.KeyReleaseEvent += new KeyReleaseEventHandler(Key_Released);

			statusBar = new Statusbar();
			vbox.PackStart(crearMenu(), false, false, 0);
			VBox pantallaBox = new VBox();
			pantallaBox.PackStart(_pantalla);
			vbox.PackStart(pantallaBox, false, false, 0);
			vbox.PackStart(statusBar, false, false, 0);
			window.Add(vbox);
			window.Resizable = false;
			window.ShowAll();
		}

		/// <summary>Crea una nueva consola y la enlaza con la pantalla</summary>
		/// <param name="rom">Ruta al fichero del cartucho</param>
		private static void iniciarConsola(String rom){
			try{
				// Detiene cualquier emulacion previa
				if (_consola != null){
					_consola.parar();
					_pantalla.limpiar();
				}
				_consola = new GB(rom);
				_consola.graphics.pantalla = _pantalla;
				menuItemConsolaIniciar.Sensitive = true;
				menuItemConsolaDebug.Sensitive = true;
				
				statusBar.Pop(0);
				statusBar.Push(0, _consola.cartucho.nombre);
			}catch(Exception e){ 
				perifericos.Debug.WriteLine(e.Message); 
				perifericos.Debug.WriteLine(e.StackTrace); 
			}
		}

		/// <summary>Crea el menu de la aplicacion</summary>
		/// <returns>El menu superior de la ventana</returns>
		private MenuBar crearMenu(){
			menu = new MenuBar();
			group = new AccelGroup();
			menuArchivo = new Menu();
			menuAyuda = new Menu();
			menuConsola = new Menu();
			menuConsolaZoom = new Menu();
			menuItemConsola = new MenuItem("_Emulacion");
			menuItemArchivo = new MenuItem("_Archivo");
			menuItemAyuda = new MenuItem("A_yuda");
			menuItemAyudaAcerca = new MenuItem("A_cerca de...");
			menuItemConsolaZoom = new MenuItem("_Zoom");
			menuItemConsolaZoom1x = new MenuItem("1x");
			menuItemConsolaZoom2x = new MenuItem("2x");
			menuItemConsolaZoom3x = new MenuItem("3x");
			menuItemConsolaZoom4x = new MenuItem("4x");
			menuItemConsolaIniciar = new MenuItem("_Iniciar");
			menuItemConsolaReset = new MenuItem("Re_set");
			menuItemConsolaReanudar = new MenuItem("_Reanudar");
			menuItemConsolaPausa = new MenuItem("_Pausa");
			menuItemConsolaParar = new MenuItem("Pa_rar");
			menuItemConsolaDebug = new MenuItem("_Debug");
			menuItemArchivoSalir = new ImageMenuItem(Gtk.Stock.Quit, group);
			menuItemArchivoAbrir = new ImageMenuItem(Gtk.Stock.Open, group);

			menuItemAyuda.Submenu = menuAyuda;
			menuAyuda.Append(menuItemAyudaAcerca);
			menuItemArchivo.Submenu = menuArchivo;
			menuArchivo.Append(menuItemArchivoAbrir);
			menuArchivo.Append(menuItemArchivoSalir);
			menuItemConsola.Submenu = menuConsola;
			menuConsola.Append(menuItemConsolaZoom);
			menuConsola.Append(menuItemConsolaIniciar);
			menuConsola.Append(menuItemConsolaReanudar);
			menuConsola.Append(menuItemConsolaPausa);
			menuConsola.Append(menuItemConsolaParar);
			menuConsola.Append(menuItemConsolaDebug);
			menuConsolaZoom.Append(menuItemConsolaZoom1x);
			menuConsolaZoom.Append(menuItemConsolaZoom2x);
			menuConsolaZoom.Append(menuItemConsolaZoom3x);
			menuConsolaZoom.Append(menuItemConsolaZoom4x);
			menuItemConsolaZoom.Submenu = menuConsolaZoom;
			menu.Append(menuItemArchivo);
			menu.Append(menuItemConsola);
			menu.Append(menuItemAyuda);

			menuItemAyuda.RightJustified = true;
			menuItemConsolaIniciar.Sensitive = false;
			menuItemConsolaReanudar.Sensitive = false;
			menuItemConsolaPausa.Sensitive = false;
			menuItemConsolaParar.Sensitive = false;
			menuItemConsolaDebug.Sensitive = false;

			menuItemArchivoSalir.Activated += new EventHandler(Exit_App);
			menuItemArchivoAbrir.Activated += new EventHandler(Open_File);
			menuItemConsolaParar.Activated += new EventHandler(Stop_Emu);
			menuItemConsolaPausa.Activated += new EventHandler(Pause_Emu);
			menuItemConsolaIniciar.Activated += new EventHandler(Start_Emu);
			menuItemConsolaReset.Activated += new EventHandler(Reset_Emu);
			menuItemConsolaReanudar.Activated += new EventHandler(Resume_Emu);
			menuItemConsolaZoom1x.Activated += new EventHandler(Zoom_1x);
			menuItemConsolaZoom2x.Activated += new EventHandler(Zoom_2x);
			menuItemConsolaZoom3x.Activated += new EventHandler(Zoom_3x);
			menuItemConsolaZoom4x.Activated += new EventHandler(Zoom_4x);
			menuItemConsolaDebug.Activated += new EventHandler(Debug);
			menuItemAyudaAcerca.Activated += new EventHandler(About);

			return menu;
		}

		/// <summary>Cambia la pantalla por una nueva con el nuevo zoom</summary>
		/// <param name="zoom">El nuevo zoom de la pantalla</param>
		static void CambiarZoom(int zoom){
			// Elimina la pantalla actual
			Box box = (Box)_pantalla.Parent;
			box.Remove(_pantalla);
			// Crea e inserta la nueva pantalla
			_pantalla = new Pantalla(144, 160, zoom);
			_pantalla.ShowAll();
			box.PackEnd(_pantalla, false, false, 0);
			// Si hay un proceso de emulacion activo se le asigna la nueva pantalla
			if (_consola != null){
				_consola.pausar();
				_consola.graphics.pantalla = _pantalla;
				_consola.reanudar();
			}
		}
		
		static void Zoom_1x (object o, EventArgs args){	CambiarZoom(1); }
		static void Zoom_2x (object o, EventArgs args){ CambiarZoom(2); }
		static void Zoom_3x (object o, EventArgs args){ CambiarZoom(3); }
		static void Zoom_4x (object o, EventArgs args){ CambiarZoom(4); }

		static void Debug (object o, EventArgs args){
			if (_consola != null){
				_consola.cpu.debug = true;
				if (_consola.estaIniciada() == false) _consola.iniciar();
			}
		}

		static void Start_Emu (object o, EventArgs args){
			if (_consola != null){ 
				_consola.iniciar();
				menuItemConsolaReanudar.Sensitive = true;
				menuItemConsolaPausa.Sensitive = true;
				menuItemConsolaParar.Sensitive = true;
			}
		}

		static void Stop_Emu (object o, EventArgs args){
			if (_consola != null){
				_consola.parar();
				menuItemConsolaReanudar.Sensitive = false;
				menuItemConsolaPausa.Sensitive = false;
				menuItemConsolaParar.Sensitive = false;
			}
		}

		static void Pause_Emu (object o, EventArgs args){
			if (_consola != null){
				_consola.pausar();
				menuItemConsolaReanudar.Sensitive = true;
				menuItemConsolaPausa.Sensitive = false;
				menuItemConsolaParar.Sensitive = true;
			}
		}

		static void Resume_Emu (object o, EventArgs args){
			if (_consola != null){
				_consola.reanudar();
				menuItemConsolaReanudar.Sensitive = false;
				menuItemConsolaPausa.Sensitive = true;
				menuItemConsolaParar.Sensitive = true;
			}
		}

		static void Reset_Emu(object o, EventArgs args){ if (_consola != null) _consola.cpu.reset(); }

		static void Window_Delete (object o, DeleteEventArgs args){
			Application.Quit ();
			if (_consola != null) _consola.parar();
			args.RetVal = true;
		}

		static void Open_File (object o, EventArgs args){
			FileFilter filter = new FileFilter();
			filter.Name = "GameBoy ROMs";
			filter.AddPattern("*.gb");

			FileChooserDialog fs = new FileChooserDialog("Abrir una ROM de GameBoy", null, FileChooserAction.Open);
			if (_lastDir != null) fs.SetCurrentFolder(_lastDir);
			fs.AddFilter(filter);
			fs.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			fs.AddButton(Gtk.Stock.Open, Gtk.ResponseType.Ok);
			fs.DefaultResponse = Gtk.ResponseType.Ok;
			fs.Action = FileChooserAction.Open;
			fs.ShowHidden = false;
			fs.SelectMultiple = false;
			fs.LocalOnly = true;
			int res = fs.Run();
			if (res == (int)ResponseType.Ok){
				// Recuerda el ultimo directorio accedido
				_lastDir = fs.CurrentFolder;
				iniciarConsola(fs.Filename);
			}
			fs.Destroy();
		}

		static void Exit_App (object o, EventArgs args){
			if (_consola != null) _consola.parar();
			Application.Quit();
		}
		
		static void About (object o, EventArgs args){
			Gtk.AboutDialog acerca = new Gtk.AboutDialog();
			acerca.Name = "CSGBE";
			acerca.Version = "0.1";
			acerca.Website = "http://www.denibol.com/proyectos/csgbe/";
			acerca.WebsiteLabel = "http://www.denibol.com";
			acerca.Comments = "C# GameBoy Emulator";
			acerca.Copyright = "Copyright (C) 2006 Victor Garcia";
			acerca.License = @"Copyright (C) 2006 Victor Garcia <vggonz@denibol.com>
			
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.
                        
This program is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
GNU Library General Public License for more details.
                        
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.";
			acerca.Run();
		}

		[GLib.ConnectBefore]
		static void Key_Pressed (object o, KeyPressEventArgs args){ Keypad.teclaPulsada(args.Event.Key); }
		static void Key_Released (object o, KeyReleaseEventArgs args){ Keypad.teclaLiberada(args.Event.Key); }
	}
}
