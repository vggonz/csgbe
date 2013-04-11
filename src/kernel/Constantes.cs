/*###########################################################################
#
# csgbe - C# GameBoy Emulator
# Constant Memory Values and Addresses
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

	/// <summary>Constantes globales a todo el proceso de emulacion</summary>
	public class Constantes{

		public const int JOYPAD                = 0xFF00;
		public const int SERIAL_DATA           = 0xFF01;
		public const int SERIAL_CTRL           = 0xFF02;
		public const int DIV_CNTR              = 0xFF04;
		public const int TIMER_COUNT           = 0xFF05;
		public const int TIMER_RELOAD          = 0xFF06;
		public const int TIMER_CRTL            = 0xFF07;
		public const int INT_FLAG              = 0xFF0F;

		public const int SND_1_ENT             = 0xFF10;
		public const int SND_1_WAV_LEN         = 0xFF11;
		public const int SND_1_ENV             = 0xFF12;
		public const int SND_1_FREQ_KICK_LOWER = 0xFF13;
		public const int SND_1_FREQ_KICK_UPPER = 0xFF14;
		public const int SND_2_WAVE_LEN        = 0xFF16;
		public const int SND_2_ENV             = 0xFF17;
		public const int SND_2_FREQ_KICK_LOWER = 0xFF18;
		public const int SND_2_FREQ_KICK_UPPER = 0xFF19;
		public const int SND_3_ON_OFF          = 0xFF1A;
		public const int SND_3_LEN             = 0xFF1B;
		public const int SND_3_VOLUME          = 0xFF1C;
		public const int SND_3_FREQ_KICK_LOWER = 0xFF1D;
		public const int SND_3_FREQ_KICK_UPPER = 0xFF1E;

		public const int SND_4_LEN             = 0xFF20;
		public const int SND_4_ENV             = 0xFF21;
		public const int SND_4_POLY_KICK_LOWER = 0xFF22;
		public const int SND_4_POLY_KICK_UPPER = 0xFF23;
		public const int SND_VOICE_INP         = 0xFF24;
		public const int SND_STEREO            = 0xFF25;
		public const int SND_STAT              = 0xFF26;

		public const int SND_BNK_10            = 0xFF30;
		public const int SND_BNK_11            = 0xFF31;
		public const int SND_BNK_12            = 0xFF32;
		public const int SND_BNK_13            = 0xFF33;
		public const int SND_BNK_14            = 0xFF34;
		public const int SND_BNK_15            = 0xFF35;
		public const int SND_BNK_16            = 0xFF36;
		public const int SND_BNK_17            = 0xFF37;
		public const int SND_BNK_20            = 0xFF38;
		public const int SND_BNK_21            = 0xFF39;
		public const int SND_BNK_22            = 0xFF3A;
		public const int SND_BNK_23            = 0xFF3B;
		public const int SND_BNK_24            = 0xFF3C;
		public const int SND_BNK_25            = 0xFF3D;
		public const int SND_BNK_26            = 0xFF3E;
		public const int SND_BNK_27            = 0xFF3F;

		public const int LCD_CTRL              = 0xFF40;
		public const int LCD_STAT              = 0xFF41;
		public const int LCD_SCROLL_Y          = 0xFF42;
		public const int LCD_SCROLL_X          = 0xFF43;
		public const int LCD_Y_LOC             = 0xFF44;
		public const int LCD_Y_COMP            = 0xFF45;
		public const int LCD_DMA               = 0xFF46;
		public const int LCD_BACK_PALETTE      = 0xFF47;
		public const int LCD_SPR0_PALETTE      = 0xFF48;
		public const int LCD_SPR1_PALETTE      = 0xFF49;
		public const int LCD_WIN_Y             = 0xFF4A;
		public const int LCD_WIN_X             = 0xFF4B;
		public const int CPU_SPEED_REG         = 0xFF4D;
		public const int VRAM_BANK             = 0xFF4F;

		public const int DMA_SRC_UPPER         = 0xFF51;
		public const int DMA_SRC_LOWER         = 0xFF52;
		public const int DMA_DST_UPPER         = 0xFF53;
		public const int DMA_DST_LOWER         = 0xFF54;
		public const int DMA_LEN_TYPE          = 0xFF55; 
		public const int IR_PORT               = 0xFF56; 

		public const int BGP_INDEX             = 0xFF68; 
		public const int BGP_DATA              = 0xFF69; 
		public const int OBP_INDEX             = 0xFF6A; 
		public const int OBP_DATA              = 0xFF6B; 

		public const int RAM_BANK              = 0xFF70; 
		public const int INT_ENABLE            = 0xFFFF;				    

		public const int INSTR_HBLANK          = 60;
		public const int INSTR_VBLANK          = 90000;
		public const int INSTR_TIMA	       = 6000;
		public const int INSTR_DIV	       = 33;
		
		// Tiempos en ciclos
		public const int CYCLES_DIV            = 256;
		public const int CYCLES_TIMER_MODE0    = 1024;
		public const int CYCLES_TIMER_MODE1    = 16;
		public const int CYCLES_TIMER_MODE2    = 64;
		public const int CYCLES_TIMER_MODE3    = 256;
		public const int CYCLES_LCD_MODE0      = 375; // 376 / 375
		public const int CYCLES_LCD_MODE1      = 456;
		public const int CYCLES_LCD_MODE2      = 82; // 80 / 82
		public const int CYCLES_LCD_MODE3      = 172;

		public const int INT_VBLANK            = 0x01;
		public const int INT_LCDC              = 0x02;
		public const int INT_TIMER             = 0x04;
		public const int INT_SERIALTX          = 0x08;
		public const int INT_KEY               = 0x10;

		public const int KEY_DOWN              = 0;
		public const int KEY_UP                = 1;
		public const int KEY_LEFT              = 2;
		public const int KEY_RIGHT             = 3;
		public const int KEY_START             = 4;
		public const int KEY_SELECT            = 5;
		public const int KEY_B                 = 6;
		public const int KEY_A                 = 7;

		public const double CPU_SPEED          = 4.194304; // MHz
		public const int MEMSIZE               = 65536; // Bytes
	}
}
