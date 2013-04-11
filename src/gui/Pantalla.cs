/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Display object
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

	using System.Drawing;
	using Gtk;

	/// <summary>Elemento de dibujo por pixeles</summary>	
	public class Pantalla : DrawingArea {

		/// <summary>Alto en pixeles de la pantalla</summary>
		private int _alto;
		/// <summary>Ancho en pixeles de la pantalla</summary>
		private int _ancho;
		/// <summary>Zoom de la pantalla</summary>
		private int _zoom;
		/// <summary>Imagen con el frame actual de la pantalla</summary>
		private Bitmap _displayBuffer;	
		/// <summary>Buffer de dibujo para acceder a la imagen</summary>
		private System.Drawing.Graphics _buffer;
		/// <summary>Paleta de colores usados</summary>
		private Color[] _colores;
		
		/// <summary>Ancho original de la pantalla (sin zoom)</summary>
		public int ancho { get { return _ancho / _zoom; } }
		/// <summary>Alto original de la pantalla (sin zoom)</summary>
		public int alto { get { return _alto / _zoom; } }

		/// <summary>Constructor</summary>
		/// <param name="height">Altura</param>
		/// <param name="width">Anchura</param>
		/// <param name="zoom">Zoom</param>
		public Pantalla(int height, int width, int zoom){
			_alto = height * zoom;
			_ancho = width * zoom;
			_zoom = zoom;
			SetSizeRequest(_ancho, _alto);
			_displayBuffer = new Bitmap(_ancho, _alto);			
			// Paleta de colores por defecto. Del blanco al negro pasando por dos niveles de gris.
	       		_colores = new Color[] { Color.White, Color.LightGray, Color.DarkGray, Color.Black };
		}

		/// <summary>Dibuja un pixel de un color en la pantalla</summary>
		/// <param name="posx">Posicion X en la pantalla</param>
		/// <param name="posy">Posicion Y en la pantalla</param>
		/// <param name="id_color">Identificador de color del pixel. (0-3)</param>
		public void dibujarPixel(int posx, int posy, int id_color){
			if (((posx * _zoom) < _ancho) && ((posy * _zoom) < _alto)){
				// Un pixel pueden ser varios en la pantalla si el zoom > 1
				for (int i = 0; i < _zoom; i++){
					for (int j = 0; j < _zoom; j++){
						_displayBuffer.SetPixel((posx * _zoom) + i, (posy * _zoom) + j, _colores[id_color]);
					}
				}
			}
		}

		/// <summary>Limpia la pantalla de dibujo a negro</summary>
		public void limpiar(){
			for (int i = 0; i < _ancho; i++) for (int j = 0; j < _alto; j++) dibujarPixel(i, j, 3);
			_buffer.Clear(Color.Black);
		}

		/// <summary>Actualiza la pantalla dibujando el frame que tiene actualmente en el buffer</summary>
		public void actualizarPantalla(){ _buffer.DrawImage(_displayBuffer, 0, 0); }

		/// <summary>Evento de redibujo. Vuelve a mostrar el mismo frame</summary>
		protected override bool OnExposeEvent (Gdk.EventExpose args){
			_buffer.DrawImage(_displayBuffer, 0, 0);
			return true;
		}
		
		/// <summary>Evento de creacion o redimension de la pantalla</summary>
		protected override bool OnConfigureEvent (Gdk.EventConfigure args){
			// Crea el enlace entre Gtk y la API de System.Drawing
			_buffer = Gtk.DotNet.Graphics.FromDrawable (args.Window);
			_buffer.DrawImage(_displayBuffer, 0, 0);
			return true;
		}
	}
}
