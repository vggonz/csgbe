/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Drawing functions
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

using csgbe.kernel;
	
namespace csgbe.gui{

	/// <summary>Gestion de la zona grafica de memoria</summary>
	public class Graphics : Constantes{

		/// <summary>Pantalla de dibujo</summary>
		private Pantalla _pantalla;
		/// <summary>Memoria</summary>
		private Memoria _memoria;
		/// <summary>Frames renderizados con exito</summary>
		private int _frames = 0;
		
		/// <summary>Cantidad de frames renderizados hasta el momento</summary>
		public int frames{ get{ return _frames; } }

		/// <summary>Pantalla de dibujo</summary>
		public Pantalla pantalla{
			get{ return _pantalla; }
			set{ _pantalla = value; }
		}

		/// <summary>Constructor</summary>
		/// <param name="memoria">Memoria</param>
		public Graphics(Memoria memoria){ _memoria = memoria; }
		
		/// <summary>Interrupcion vertical</summary>
		/// <remarks>Solicita a la pantalla un refresco total de la imagen. La aplicacion tiene dos hilos de ejecucion
		/// principales: la interfaz grafica y la emulacion del procesador. La pantalla pertenece al primer grupo y
		/// este metodo es el nexo entre ambos hilos. Gtk no soporta interaccion entre hilos, por lo que es necesario
		/// realizar esta invocacion especial para poder solicitar algo al hilo de la interfaz grafica.
		/// Mas informacion en http://www.mono-project.com/Responsive_Applications</remarks>
		public void vblank(){ Gtk.Application.Invoke(delegate { _pantalla.actualizarPantalla(); }); _frames++; }

		/// <summary>Interrupcion horizontal</summary>
		/// <remarks>Actualiza el fondo, ventana y sprites de la linea actual de dibujo</remarks>
		public void hblank(){ 
			int scanLine = (_memoria.leer(LCD_Y_LOC) & 0xFF);

			actualizarBG(scanLine); 
			actualizarVentana(scanLine);
			actualizarSprites(scanLine); 
		}

		/// <summary>Obtiene el identificador de color del fondo/ventana en una posicion concreta</summary>
		/// <param name="x">Posicion X de la pantalla</param>
		/// <param name="y">Posicion Y de la pantalla</param>
		/// <returns>El identificador de color</returns>
		private int getIdColor(int x, int y){
			int mapAddress = (_memoria.leer(LCD_CTRL) & 0x08) != 0 ? 0x9C00 : 0x9800;
			int tileAddress = (_memoria.leer(LCD_CTRL) & 0x10) != 0 ? 0x8000 : 0x8800;
			int scrollX = _memoria.leer(LCD_SCROLL_X);
			int scrollY = _memoria.leer(LCD_SCROLL_Y);

			if ((scrollY + y) > 255) scrollY -= 255;
			if ((scrollX + x) > 255) scrollX -= 255;

			int idTile = getIdTile((scrollX + x) >> 3, (scrollY + y) >> 3, mapAddress, tileAddress);
			int tile = getTile(idTile, tileAddress, (scrollX + x) & 0x07, (scrollY + y) & 0x07);
			return tile;
		}

		/// <summary>Transforma un identificador de color en su color real usando una paleta de colores</summary>
		/// <param name="id">Identificador de color (0-3)</param>
		/// <param name="direccion">Direccion de memoria donde se encuentra la paleta de colores</param>
		/// <returns>El color correspondiente al identificador de color</returns>
		private int id2color(int id, int direccion){
			// 11 10 01 00
			int valor = _memoria.leer(direccion);
			int color = (valor & (0x03 << (id * 2))) >> (id * 2);
			return color;
		}

		/// <summary>Actualiza el fondo de la pantalla de una linea concreta</summary>
		/// <param name="scanLine">Linea a actualizar</param>
		private void actualizarBG(int scanLine){
			// Solo dibuja las lineas visibles (0-144)
			if (((_memoria.leer(LCD_CTRL) & 0x01) != 0) && scanLine < 144){
				int mapAddress = (_memoria.leer(LCD_CTRL) & 0x08) != 0 ? 0x9C00 : 0x9800; // BG Tile Map Display Select
				int tileAddress = (_memoria.leer(LCD_CTRL) & 0x10) != 0 ? 0x8000 : 0x8800; // BG & Window Tile Data Select
				int scrollX = _memoria.leer(LCD_SCROLL_X);
				int scrollY = _memoria.leer(LCD_SCROLL_Y);

				// La linea tiene 160 pixeles de ancho
				for(int x = 0; x < 160; x++){
					if ((scrollY + scanLine) > 255) scrollY -= 255;
					if ((scrollX + x) > 255) scrollX -= 255;

					// Tile
					int xTile = (scrollX + x) >> 3;
					int yTile = (scrollY + scanLine) >> 3;
					// Pixel dentro del tile
					int bitX = (scrollX + x) & 0x07;
					int bitY = (scrollY + scanLine) & 0x07;

					int idTile = getIdTile(xTile, yTile, mapAddress, tileAddress);
					int tile = getTile(idTile, tileAddress, bitX, bitY);
					_pantalla.dibujarPixel(x, scanLine, id2color(tile, LCD_BACK_PALETTE));
				}
			}
		}

		/// <summary>Actualiza la ventana de una linea concreta</summary>
		/// <remarks>La ventana es como otro fondo superpuesto al anterior, pero colocado en una posicion determinada
		/// de la pantalla. Usa los mismos tiles que el fondo</remarks>
		/// <param name="scanLine">Linea a actualizar</param>
		private void actualizarVentana(int scanLine){
			int winY = _memoria.leer(LCD_WIN_Y);

			if (((_memoria.leer(LCD_CTRL) & 0x20) != 0) && winY <= scanLine){

				int winX = _memoria.leer(LCD_WIN_X) - 7;
				int mapAddress = (_memoria.leer(LCD_CTRL) & 0x40) != 0 ? 0x9C00 : 0x9800;
				int tileAddress = (_memoria.leer(LCD_CTRL) & 0x10) != 0 ? 0x8000 : 0x8800;

				for(int wx = 0; wx < (160 - winX); wx++){
					int xTile = wx >> 3;
					int yTile = (scanLine - winY) >> 3;

					int bitX = wx & 0x07;
					int bitY = (scanLine - winY) & 0x07;

					int idTile = getIdTile(xTile, yTile, mapAddress, tileAddress);
					int tile = getTile(idTile, tileAddress, bitX, bitY);
					if ((wx + winX) < 160 && (wx + winX) >= 0) _pantalla.dibujarPixel(wx + winX, scanLine, id2color(tile, LCD_BACK_PALETTE));
				}
			}
		}

		/// <summary>Actualiza los sprites que aparecen en una linea de la pantalla</summary>
		/// <param name="scanLine">Linea a actualizar</param>
		private void actualizarSprites(int scanLine){
			if ((_memoria.leer(LCD_CTRL) & 0x02) != 0){
				// Los sprites pueden ser de 8x8 o 8x16 pixeles	
				int spriteSize = (_memoria.leer(LCD_CTRL) & 0x04) != 0 ? 16 : 8;
				// El sprite 0 es el de menor prioridad
				for (int i = 39; i >= 0; i--){
					int spriteY = _memoria.leer(0xFE00 + (i * 4));
					int spriteX = _memoria.leer(0xFE01 + (i * 4));
				
					if((spriteY <= scanLine + 16) && (spriteY > scanLine + (16 - spriteSize))){
						int tileNum = _memoria.leer(0xFE02 + (i * 4));
						int attributes = _memoria.leer(0xFE03 + (i * 4));

						// Paleta a utilizar
						bool pal = (attributes & 0x10) == 0x10;
						// Inversion horizontal
						bool flipX = (attributes & 0x20) == 0x20;
						// Inversion vertical
						bool flipY = (attributes & 0x40) == 0x40;
						// Prioridad sobre el fondo/ventana
						bool priority = (attributes & 0x80) == 0x80;
						
						// Todos los sprites tiene 8 pixeles de ancho
						for (int j = 0; j < 8; j++){						
							int posX = flipX ? spriteX - 1 - j : spriteX + j - 8;
							int posY = flipY ? spriteSize - (scanLine - spriteY + 17) : scanLine - spriteY + 16;
							int tile = getTile(tileNum, 0x8000, j, posY);
							if (posX >= 0 && tile != 0 && (!priority || (priority && getIdColor(posX, scanLine) == 0))) _pantalla.dibujarPixel(posX, scanLine, id2color(tile, pal ? LCD_SPR1_PALETTE : LCD_SPR0_PALETTE));
						}
					 }
				}
			}
		}

		/// <summary>Obtiene el numero de tile correspondiente a una posicion de la pantalla</summary>
		/// <remarks>La pantalla esta dividida en tiles de 8x8 pixeles. La ubicacion de los sprites es necesaria
		/// porque si estan en la direccion 0x8000 su identificador del mapa tiene signo</remarks>
		/// <param name="xTile">Tile horizontal</param>
		/// <param name="yTile">Tile vertical</param>
		/// <param name="mapAddress">Direccion del mapa de colocacion de tiles</param>
		/// <param name="tileAddress">Direccion de los datos de los sprites</param>
		/// <returns>El identificador de tile</returns>
		private int getIdTile(int xTile, int yTile, int mapAddress, int tileAddress){
			int idTile = _memoria.leer(mapAddress + (yTile << 5) + xTile);
			if (tileAddress != 0x8000) idTile ^= 0x80;
			return idTile;
		}

		/// <summary>Obtiene el valor del color de un pixel de un tile en concreto</summary>
		/// <param name="idTile">Identificador de tile</param>
		/// <param name="tileAddress">Direccion de memoria donde se encuentran todos los tiles</param>
		/// <param name="bitX">Posicion X del pixel dentro del tile</param>
		/// <param name="bitY">Posicion Y del pixel dentro del tile</param>
		private int getTile(int idTile, int tileAddress, int bitX, int bitY){
			// El color de un pixel esta compuesto por dos bits (4 colores, blanco, negro y dos niveles de gris)
			int a = ((_memoria.leer(tileAddress + 1 + (bitY << 1) + (idTile << 4)) >> (7 - bitX)) & 0x01) << 1;
			int b = (_memoria.leer(tileAddress + (bitY << 1) + (idTile << 4)) >> (7 - bitX)) & 0x01;
			return a | b;
		}
	}
}
