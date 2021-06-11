/**
* |------------------------------------------|
* | noiSNESs_Graph_Injector                  |
* | File: SNESGraphicTrans.cs                |
* | v1.0, April 2014                         |
* | Author: noisecross                       |
* |------------------------------------------|
* 
* @author noisecross
* @version 1.0
* 
*/



using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;



namespace noiSNESs_Graph_Injector
{

    public static class Palettes
    {
        public static Color[] palette1b = new Color[2]{
            Color.FromArgb(0x00,0x00,0x00),
            Color.FromArgb(0x00,0xFF,0x00)
        };

        public static Color[] palette2b = new Color[4]{
            Color.Black,
            Color.DarkBlue,
            Color.LightBlue,
            Color.White
        };

        public static Color[] palette3b = new Color[8]{
            Color.Black,
            Color.DarkBlue,
            Color.LightBlue,
            Color.White,
            Color.DarkGreen,
            Color.Green,
            Color.LightGreen,
            Color.YellowGreen
        };

        public static Color[] palette4b = new Color[16]{
            Color.FromArgb(0x00,0x00,0x00),
            Color.FromArgb(0x70,0x00,0x00),
            Color.FromArgb(0xA0,0x00,0x00),
            Color.FromArgb(0xFF,0x00,0x00),

            Color.FromArgb(0x00,0x40,0x00),
            Color.FromArgb(0x00,0x80,0x00),
            Color.FromArgb(0x00,0xC0,0x00),
            Color.FromArgb(0x00,0xFF,0x00),

            Color.FromArgb(0x00,0x00,0x40),
            Color.FromArgb(0x00,0x00,0x80),
            Color.FromArgb(0x00,0x00,0xC0),
            Color.FromArgb(0x00,0x00,0xFF),

            Color.FromArgb(0x40,0x00,0x40),
            Color.FromArgb(0x80,0x00,0x80),
            Color.FromArgb(0xC0,0x00,0xC0),
            Color.FromArgb(0xFF,0x00,0xFF)
        };

        public static Color[] palette8b;

        public static Color[] initializePalette8b()
        {
            Color[] output = new Color[256];

            for (int k = 0; k < 16; k++)
            {
                output[k + 0x00] = Color.FromArgb(k * 16, k * 16, k * 16);
                output[k + 0x10] = Color.FromArgb(k * 16, k * 00, k * 00);
                output[k + 0x20] = Color.FromArgb(k * 00, k * 16, k * 00);
                output[k + 0x30] = Color.FromArgb(k * 00, k * 00, k * 16);
                output[k + 0x40] = Color.FromArgb(k * 16, k * 16, k * 00);
                output[k + 0x50] = Color.FromArgb(k * 16, k * 00, k * 16);
                output[k + 0x60] = Color.FromArgb(k * 00, k * 16, k * 16);
                output[k + 0x70] = Color.FromArgb(k * 16, k * 16, k * 08);
                output[k + 0x80] = Color.FromArgb(k * 16, k * 08, k * 16);
                output[k + 0x90] = Color.FromArgb(k * 08, k * 16, k * 16);
                output[k + 0xA0] = Color.FromArgb(k * 16, k * 08, k * 08);
                output[k + 0xB0] = Color.FromArgb(k * 08, k * 16, k * 08);
                output[k + 0xC0] = Color.FromArgb(k * 08, k * 08, k * 16);
                output[k + 0xD0] = Color.FromArgb(k * 04, k * 08, k * 16);
                output[k + 0xE0] = Color.FromArgb(k * 16, k * 04, k * 08);
                output[k + 0xF0] = Color.FromArgb(k * 08, k * 16, k * 04);
            }
            
            return output;
        }
    }



    public class Constants
    {
        public const int PAGESIZE_1b = 2048;
        public const int PAGESIZE_2b = 4096;
        public const int PAGESIZE_3b = 6144;
        public const int PAGESIZE_4b = 8192;
        public const int PAGESIZE_8b = 16384;

        public const int ZOOM_8x8 = 4;
    }



    public class Transformations{

        /*
        Verificado
        */
        public static System.Tuple<Bitmap, byte[,]> transform1b(List<byte> byteMap, int page, int offset, int widthX, int widthY, int size)
        {
            int maxY = (int)((size * 16 * 8) / Constants.PAGESIZE_1b);
            Bitmap newBitmap = new Bitmap(16 * 8, maxY);
            byte[,] outBytes = new byte[16 * 8, maxY];

            int i   = (page * Constants.PAGESIZE_1b) + offset;
            int end = i + size;

            int x = 0;
            int y = 0;
            byte mask = 0x80;

            while(i < end) for (int yi = 0; yi < 128 / widthY; yi++)
            {
                if (y + widthY > newBitmap.Height)
                {
                    i = int.MaxValue;
                    break;
                }

                /* Draw a line of tiles (1b) */
                for (int xi = 0; xi < 128 / widthX; xi++)
                {

                    /* Draw a tile (1b) */
                    for (int j = 0; j < widthY; j++)
                    {
                        if (i >= byteMap.Count || i >= end)
                            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes);

                        byte pixel1b = byteMap[i];

                        for (int k = 0; k < widthX; k++)
                        {
                            newBitmap.SetPixel(x, y, ((pixel1b & mask) == 0) ? Palettes.palette1b[0] : Palettes.palette1b[1]);
                            outBytes[x, y] = (byte)(((pixel1b & mask) == 0) ? 0 : 1);
                            x ++;
                            mask = (byte)(mask >> 1);
                            if (mask == 0)
                                mask = 0x80;
                        }
                        x -= widthX;
                        y++;
                        i++; //Next byte
                    }
                    x += widthX;
                    y -= widthY;

                }
                x=0;
                y += widthY;
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }



        /*
        Verificado 8x8
        Zelda:		0x70201-FuenteFamiliar
        Metroid 3:	0xD3403-PedazosDeMapas
        
        8x12
        ChronoTrigger: Al final
        */
        public static System.Tuple<Bitmap, byte[,]> transform2b(List<byte> byteMap, int page, int offset, int widthX, int widthY, int size)
        {
            int maxY = (int)((size * 16 * 8) / Constants.PAGESIZE_2b);
            Bitmap newBitmap = new Bitmap(16 * 8, maxY);
            byte[,] outBytes = new byte[16 * 8, maxY];

            int i   = (page * Constants.PAGESIZE_2b) + offset;
            int end = i + size;

            int x = 0;
            int y = 0;
            int cIndex = 0;

            while (i < end) for (int yi = 0; yi < 128 / widthY; yi++)
                {
                    if (y + widthY > newBitmap.Height)
                    {
                        i = int.MaxValue;
                        break;
                    }

	            /* Draw a line of tiles (2b,8x8) */
                for (int xi = 0; xi < 128 / widthX; xi++){

		            /* Draw a tile (2b,8x8) */
                    for (int j = 0; j < widthY; j++)
                    {
                        if (i >= byteMap.Count - 1 || i >= end)
                            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;

                        byte pixel2b_00 = byteMap[i++];
			            byte pixel2b_01 = byteMap[i++];
                        byte mask = 0x80;

                        for (int k = 0; k < widthX; k++)
                        {
                            cIndex = ((pixel2b_00 & mask) == 0) ? 0 : 1;
                            cIndex += ((pixel2b_01 & mask) == 0) ? 0 : 2;
                            newBitmap.SetPixel(x, y, Palettes.palette2b[cIndex]);
                            outBytes[x, y] = (byte)cIndex;
                            x++;
                            mask = (byte)(mask >> 1);
                            if (mask == 0)
                                mask = 0x80;
                        }
                        x -= widthX;
                        y++;

                    }
                    x += widthX;
                    y -= widthY;

	            }
	            x=0;
                y += widthY;
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }



        public static System.Tuple<Bitmap, byte[,]> transform2bNES(List<byte> byteMap, int page, int offset, int widthX, int widthY, int size)
        {
            int maxY = (int)((size * 16 * 8) / Constants.PAGESIZE_2b);
            Bitmap newBitmap = new Bitmap(16 * 8, maxY);
            byte[,] outBytes = new byte[16 * 8, maxY];

            int i = (page * Constants.PAGESIZE_2b) + offset + 16;
            int end = i + size;

            int x = 0;
            int y = 0;
            int cIndex = 0;

            while (i < end)
            {
                for (int yi = 0; yi < 128 / widthY; yi++)
                {
                    if (i + 256 > byteMap.Count)
                    {
                        i = int.MaxValue;
                        break;
                    }

                    /* Draw a line of tiles (2b,8x8) */
                    for (int xi = 0; xi < 128 / widthX; xi++)
                    {
                        /* Draw a tile (2b,8x8) */
                        for (int j = 0; j < widthY; j++)
                        {
                            if (i >= byteMap.Count - 1 || i >= end)
                                return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;

                            byte pixel2b_00 = byteMap[i];
                            byte pixel2b_01 = byteMap[i + 8];
                            i++;
                            byte mask = 0x80;

                            for (int k = 0; k < widthX; k++)
                            {
                                cIndex = ((pixel2b_00 & mask) == 0) ? 0 : 1;
                                cIndex += ((pixel2b_01 & mask) == 0) ? 0 : 2;
                                newBitmap.SetPixel(x, y, Palettes.palette2b[cIndex]);
                                outBytes[x, y] = (byte)cIndex;
                                x++;
                                mask = (byte)(mask >> 1);
                                if (mask == 0)
                                    mask = 0x80;
                            }
                            x -= widthX;
                            y++;

                        }
                        i += 8;
                        x += widthX;
                        y -= widthY;

                    }
                    x = 0;
                    y += widthY;
                }
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }



        /*
        Verificado 8x8
        Zelda:	0x87201-Armas
        FF2:  	0x48201-Enemigos,
		        0x5B519-Personajes,
		        0x638C1-Más Personajes
        */
        public static System.Tuple<Bitmap, byte[,]> transform3b(List<byte> byteMap, int page, int offset, int widthX, int widthY, int size)
        {
            Bitmap newBitmap = new Bitmap(16 * 8, (int)((size * 16 * 8) / Constants.PAGESIZE_3b));
            byte[,] outBytes = new byte[16 * 8, (int)((size * 16 * 8) / Constants.PAGESIZE_3b)];

            int i   = (page * Constants.PAGESIZE_3b) + offset;
            int end = i + size;

            int x = 0;
            int y = 0;
            int cIndex = 0;
            int bpl2;

            while (i < end) for (int yi = 0; yi < 128 / widthY; yi++)
            {
                if (y + widthY > newBitmap.Height)
                {
                    i = int.MaxValue;
                    break;
                }

                /* Draw a line of tiles (3b,8x8) */
                for (int xi = 0; xi < 128 / widthX; xi++)
                {

                    /* Draw a tile (3b,8x8) */
                    bpl2 = i + widthY * 2;
                    for (int j = 0; j < widthY; j++)
                    {
                        if (bpl2 >= byteMap.Count || bpl2 >= end)
                            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;

                        byte pixel3b_00 = byteMap[i++];
                        byte pixel3b_01 = byteMap[i++];
                        byte pixel3b_02 = byteMap[bpl2++]; 
                        byte mask = 0x80;

                        for (int k = 0; k < widthX; k++)
                        {
                            cIndex = (pixel3b_00 & mask) == 0 ? 0 : 1;
                            cIndex += (pixel3b_01 & mask) == 0 ? 0 : 2;
                            cIndex += (pixel3b_02 & mask) == 0 ? 0 : 4;
                            newBitmap.SetPixel(x, y, Palettes.palette3b[cIndex]);
                            outBytes[x, y] = (byte)cIndex;
                            x++;
                            mask = (byte)(mask >> 1);
                            if (mask == 0)
                                mask = 0x80;
                        }
                        x -= widthX;
                        y++;

                    }
                    x += widthX;
                    y -= widthY;
                    i += widthY; //Discard the widthY bytes of the bpl2 (previously readen)

                }
                x = 0;
                y += widthY;
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }



        /*
        Verificado 8x8
        Zelda:			0x080201-Link
        Mario AllStars:	0x1BC80F-Mario3
        FF2:			0x0D0201-Personajes
        Metroid 3:		0x072401-Zebes,
				        0x0D8201-Samus
        */
        public static System.Tuple<Bitmap, byte[,]> transform4b(List<byte> byteMap, int page, int offset, int widthX, int widthY, int size)
        {
            Bitmap newBitmap = new Bitmap(16 * 8, (int)((size * 16 * 8) / Constants.PAGESIZE_4b));
            byte[,] outBytes = new byte[16 * 8, (int)((size * 16 * 8) / Constants.PAGESIZE_4b)];

            int i   = (page * Constants.PAGESIZE_4b) + offset;
            int end = i + size;

            int x = 0;
            int y = 0;
            int cIndex = 0;
            int bpl23;

            while (i < end) for (int yi = 0; yi < 128 / widthY; yi++)
            {
                if (y + widthY > newBitmap.Height)
                {
                    i = int.MaxValue;
                    break;
                }

                /* Draw a line of tiles (4b,8x8) */
                for (int xi = 0; xi < 128 / widthX; xi++)
                {

                    /* Draw a tile (4b,8x8) */
                    bpl23 = i + widthY * 2;
                    for (int j = 0; j < widthY; j++)
                    {
                        if (bpl23 >= byteMap.Count - 1 || bpl23 >= end)
                            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;

                        byte pixel4b_00 = byteMap[i++];
                        byte pixel4b_01 = byteMap[i++];
                        byte pixel4b_02 = byteMap[bpl23++];
                        byte pixel4b_03 = byteMap[bpl23++];
                        byte mask = 0x80;

                        for (int k = 0; k < widthX; k++)
                        {
                            cIndex = (pixel4b_00 & mask) == 0 ? 0 : 1;
                            cIndex += (pixel4b_01 & mask) == 0 ? 0 : 2;
                            cIndex += (pixel4b_02 & mask) == 0 ? 0 : 4;
                            cIndex += (pixel4b_03 & mask) == 0 ? 0 : 8;
                            newBitmap.SetPixel(x, y, Palettes.palette4b[cIndex]);
                            outBytes[x, y] = (byte)cIndex;
                            x++;
                            mask = (byte)(mask >> 1);
                            if (mask == 0)
                                mask = 0x80;
                        }
                        x -= widthX;
                        y++;

                    }
                    x += widthX;
                    y -= widthY;
                    i += widthY * 2; //Discard the 16 bytes of the bpl2 & bpl3 (previously readen)

                }
                x = 0;
                y += widthY;
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }



        /*
        Verificado 8x8
        Mario AllStars:	0x008200-Título,
				        0x15820F-PantallaDeSelección
        */
        public static System.Tuple<Bitmap, byte[,]> transform8b(List<byte> byteMap, int page, int offset, int widthX, int widthY, int size)
        {
            Bitmap newBitmap = new Bitmap(16 * 8, (int)((size * 16 * 8) / Constants.PAGESIZE_8b));
            byte[,] outBytes = new byte[16 * 8, (int)((size * 16 * 8) / Constants.PAGESIZE_8b)];

            int i   = (page * Constants.PAGESIZE_8b) + offset;
            int end = i + size;

            int x = 0;
            int y = 0;
            int cIndex = 0;
            int bpl23 = 0;
            int bpl45 = 0;
            int bpl67 = 0;

            while (i < end) for (int yi = 0; yi < 128 / widthY; yi++)
            {
                if (y + widthY > newBitmap.Height)
                {
                    i = int.MaxValue;
                    break;
                }


                /* Draw a line of tiles (8b,8x8) */
                for (int xi = 0; xi < 128 / widthX; xi++)
                {

                    /* Draw a tile (8b,8x8) */
                    bpl23 = i + 2 * widthX;
                    bpl45 = i + 4 * widthX;
                    bpl67 = i + 6 * widthX;
                    for (int j = 0; j < widthY; j++)
                    {
                        if (bpl67 >= byteMap.Count - 1 || bpl67 >= end)
                            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;

                        byte pixel8b_00 = byteMap[i++];
                        byte pixel8b_01 = byteMap[i++];
                        byte pixel8b_02 = byteMap[bpl23++];
                        byte pixel8b_03 = byteMap[bpl23++];
                        byte pixel8b_04 = byteMap[bpl45++];
                        byte pixel8b_05 = byteMap[bpl45++];
                        byte pixel8b_06 = byteMap[bpl67++];
                        byte pixel8b_07 = byteMap[bpl67++];
                        byte mask = 0x80;

                        for (int k = 0; k < widthX; k++)
                        {
                            cIndex = (pixel8b_00 & mask) == 0 ? 0 : 0x01;
                            cIndex += (pixel8b_01 & mask) == 0 ? 0 : 0x02;
                            cIndex += (pixel8b_02 & mask) == 0 ? 0 : 0x04;
                            cIndex += (pixel8b_03 & mask) == 0 ? 0 : 0x08;
                            cIndex += (pixel8b_04 & mask) == 0 ? 0 : 0x10;
                            cIndex += (pixel8b_05 & mask) == 0 ? 0 : 0x20;
                            cIndex += (pixel8b_06 & mask) == 0 ? 0 : 0x40;
                            cIndex += (pixel8b_07 & mask) == 0 ? 0 : 0x80;
                            newBitmap.SetPixel(x, y, Palettes.palette8b[cIndex]);
                            outBytes[x, y] = (byte)cIndex;
                            x++;
                            mask = (byte)(mask >> 1);
                            if (mask == 0)
                                mask = 0x80;
                        }
                        x -= widthX;
                        y++;

                    }
                    x += widthX;
                    y -= widthY;
                    i += 6 * widthX; //Discard the previously readen bytes

                }
                x = 0;
                y += widthY;
            }


            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }



        /*
        Verificado
        Zelda:	0xC42010-PiezasDelMapaEscalable
        */
        public static System.Tuple<Bitmap, byte[,]> transform8bM7(List<byte> byteMap, int page, int offset, int pre = 0, int pos = 0)
        {
            Bitmap newBitmap = new Bitmap(16 * 8, 16 * 8);
            byte[,] outBytes = new byte[16 * 8, 16 * 8];

            int i = (page * Constants.PAGESIZE_8b) + offset;
            int x = 0;
            int y = 0;

            for (int yi = 0; yi < 16; yi++)
            {

                /* Draw a line of tiles (8bM7,8x8) */
                for (int xi = 0; xi < 16; xi++)
                {

                    /* Draw a tile (8bM7,8x8) */
                    for (int yj = 0; yj < 8; yj++)
                    {
                        for (int xj = 0; xj < 8; xj++)
                        {
                            i += pre;
                            if (i >= byteMap.Count - 1)
                                return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes);
                            outBytes[x, y] = byteMap[i];
                            newBitmap.SetPixel(x, y, Palettes.palette8b[byteMap[i++]]);
                            x++;
                            i += pos;
                        }
                        x-=8;
                        y++;
                    }
                    x+=8;
                    y-=8;
                }
                x=0;
                y+=8;
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }



        /*
        Format 4b Interleaved bitmap (FX)
        Verificado

        Yoshi's Island:	0x110000 Character graphics,
				        0x1C0000 All the level icons
        StarFox:		0x090200 Planets, starfields, lasershots...

        256 bytes wide bitmap. Each adjacent pixel column of the same graphic image is a byte apart, but there are actually two pixels per byte.
        This is because there are two separate images interleaved in the same space, one using the lower four bits and the other using the upper four bits.
        So even though each of the two image planes is 256 pixels wide, there are actually 512 pixels per row.
        */
        public static System.Tuple<Bitmap, byte[,]> transform4bIFX(List<byte> byteMap, int page, int offset)
        {
            Bitmap newBitmap = new Bitmap(16 * 8, 16 * 8);
            byte[,] outBytes = new byte[16 * 8, 16 * 8];

            int i = (page * Constants.PAGESIZE_4b) + offset;
            int y1 = 0x20; //32
            int y2 = 0x40; //64
            int y3 = 0x60; //96

            /* Draw a FX graphic */
            for (int y0 = 0; y0 < 0x20; y0++) //32
            {
                for (int xi = 0; xi < 0x80; xi++) //128
                {
                    if (i >= byteMap.Count - 1)
                        return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;

                    int interleaved_00 = (byteMap[i] & 0x0F);
                    int interleaved_01 = ((byteMap[i] >> 4) & 0x0F);
                    newBitmap.SetPixel(xi, y0, Palettes.palette4b[interleaved_00]);
                    newBitmap.SetPixel(xi, y1, Palettes.palette4b[interleaved_01]);
                    outBytes[xi, y0] = (byte)interleaved_00;
                    outBytes[xi, y1] = (byte)interleaved_01;
                    i++;
                }
                y1++;

                for (int xi = 0; xi < 0x80; xi++) //128
                {
                    if (i >= byteMap.Count - 1)
                        return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;

                    int interleaved_00 = (byteMap[i] & 0x0F);
                    int interleaved_01 = ((byteMap[i] >> 4) & 0x0F);
                    newBitmap.SetPixel(xi, y2, Palettes.palette4b[interleaved_00]);
                    newBitmap.SetPixel(xi, y3, Palettes.palette4b[interleaved_01]);
                    outBytes[xi, y2] = (byte)interleaved_00;
                    outBytes[xi, y3] = (byte)interleaved_01;
                    i++;
                }
                y2++;
                y3++;
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }



        /*
        Format 8b Interleaved
        
        Mode 7 VRAM dump
        */
        public static System.Tuple<Bitmap, byte[,]> transform8b2(List<byte> byteMap, int page, int offset)
        {
            Bitmap newBitmap = new Bitmap(16 * 8, 16 * 8);
            byte[,] outBytes = new byte[16 * 8, 16 * 8];

            int i = (page * Constants.PAGESIZE_8b) + offset;
            int y1 = 0x40; //64

            /* Draw a FX graphic */
            for (int y0 = 0; y0 < 0x40; y0++) //64
            {
                for (int xi = 0; xi < 0x80; xi++) //128
                {
                    if (i >= byteMap.Count - 1)
                        return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes);

                    int interleaved_00 = byteMap[i++];
                    newBitmap.SetPixel(xi, y0, Palettes.palette8b[interleaved_00]);
                    outBytes[xi, y0] = (byte)interleaved_00;
                }
                for (int xi = 0; xi < 0x80; xi++) //128
                {
                    if (i >= byteMap.Count - 1)
                        return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes);

                    int interleaved_01 = byteMap[i++];
                    newBitmap.SetPixel(xi, y1, Palettes.palette8b[interleaved_01]);
                    outBytes[xi, y1] = (byte)interleaved_01;
                }
                y1++;
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }
        


        /* Mode 7, VRAM dump, tileset */
        public static System.Tuple<Bitmap, byte[,]> transform8bM7_2(List<byte> byteMap, int page, int offset)
        {
            Bitmap newBitmap = new Bitmap(16 * 8, 16 * 8);
            byte[,] outBytes = new byte[16 * 8, 16 * 8];

            int i = (page * Constants.PAGESIZE_8b) + offset;

            /* Draw a 8bpp graphic */
            for (int y0 = 0; y0 < 0x80; y0++) //128
            {
                for (int xi = 0; xi < 0x80; xi++) //128
                {
                    if (i >= byteMap.Count - 1)
                        return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;

                    int pixel = byteMap[i++];
                    newBitmap.SetPixel(xi, y0 + 0, Palettes.palette8b[pixel]);
                    outBytes[xi, y0 + 0] = (byte)pixel;
                    i++;
                }
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }
        /* ??? 8bpp2 */
        public static System.Tuple<Bitmap, byte[,]> transform8bM72(List<byte> byteMap, int page, int offset)
        {
            Bitmap newBitmap = new Bitmap(16 * 8, 16 * 8);
            byte[,] outBytes = new byte[16 * 8, 16 * 8];

            int i = (page * Constants.PAGESIZE_8b) + offset;

            /* Draw a 8bpp graphic */
            for (int y0 = 0; y0 < 0x80; y0++) //128
            {
                for (int xi = 0; xi < 0x80; xi++) //128
                {
                    if (i >= byteMap.Count - 1)
                        return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;

                    int pixel = byteMap[i++];
                    newBitmap.SetPixel(xi, y0 + 0, Palettes.palette8b[pixel]);
                    outBytes[xi, y0 + 0] = (byte)pixel;
                }
            }

            return new System.Tuple<Bitmap, byte[,]>(newBitmap, outBytes); ;
        }
    }



    public class InvertingTransformations {


        /*
        The expected Bitmaps have a size of n x fileSize and a PixelFormat of Format8bppIndexed
        */



        public static List<byte> import1bpp(byte[] input, int widthX, int widthY, int size)
        {
            size = size / 8; 
            
            List<byte> output = new List<byte>(size);
            for (int j = 0 ; j < size; j++)
            {
                output.Add(0);
            }

            /* Input is a pixel array. First 8 bytes represent the first output byte */            
            byte mask    = 0x80;
            byte pixel1b = 0;
            int i = 0;
            int x = 0;
            int y = 0;

            while (i < size) for (int yi = 0 ; yi < 128 / widthY ; yi++)
                {
                    /* Draw a line of tiles (1b) */
                    for (int xi = 0; xi < 128 / widthX; xi++)
                    {

                        /* Draw a tile (1b) */
                        for (int j = 0 ; j < widthY ; j++)
                        {
                            if (i >= size){
                                return output;
                            }
                            //mask = 0x80;

                            for (int k = 0; k < widthX; k++)
                            {
                                pixel1b = input[x + (y * 128)];
                                output[i] += (byte)(pixel1b * mask);
                                x++;
                                mask = (byte)(mask >> 1);
                                if (mask == 0)
                                    mask = 0x80;
                            }
                            x -= widthX;
                            y++;
                            i++; //Next byte
                        }
                        x += widthX;
                        y -= widthY;

                    }
                    x = 0;
                    y += widthY;
                }

            return output;
        }



        public static List<byte> import2bpp(byte[] input, int widthX, int widthY, int size)
        {
            size = size / 4;

            List<byte> output = new List<byte>(size);
            for (int j = 0; j < size; j++)
            {
                output.Add(0);
            }

            /* Input is a pixel array. First 8 bytes represent the first output byte */
            byte mask = 0x80;
            byte pixel1b = 0;
            int  i = 0;
            int  x = 0;
            int  y = 0;

            while (i < size) for (int yi = 0; yi < 128 / widthY; yi++)
                {
                    /* Draw a line of tiles (1b) */
                    for (int xi = 0; xi < 128 / widthX; xi++)
                    {

                        /* Draw a tile (1b) */
                        for (int j = 0; j < widthY; j++)
                        {
                            if (i >= size)
                                return output;

                            mask = 0x80;

                            for (int k = 0; k < widthX; k++)
                            {
                                pixel1b        = input[(x + y * 128)];
                                output[i]     += (byte)((pixel1b & 0x01) * mask);
                                output[i+1]   += (byte)(((pixel1b & 0x02) >> 1) * mask);
                                
                                mask = (byte)(mask >> 1);
                                if (mask == 0)
                                    mask = 0x80;

                                x++;
                            }

                            x -= widthX;
                            y++;
                            i+=2; //Next byte
                        }
                        x += widthX;
                        y -= widthY;
                    }
                    x = 0;
                    y += widthY;
                }

            return output;
        }



        public static List<byte> import2bppNES(byte[] input, int widthX, int widthY, int size)
        {
            size = size / 4;

            List<byte> output = new List<byte>(size);
            for (int j = 0; j < size; j++)
            {
                output.Add(0);
            }

            /* Input is a pixel array. First 8 bytes represent the first output byte */
            byte mask = 0x80;
            byte pixel1b = 0;
            int i = 0;
            int x = 0;
            int y = 0;

            while (i < size) for (int yi = 0; yi < 128 / widthY; yi++)
                {
                    /* Draw a line of tiles (1b) */
                    for (int xi = 0; xi < 128 / widthX; xi++)
                    {

                        /* Draw a tile (1b) */
                        for (int j = 0; j < widthY; j++)
                        {
                            if (i >= size)
                                return output;

                            mask = 0x80;

                            for (int k = 0; k < widthX; k++)
                            {
                                pixel1b = input[(x + y * 128)];
                                output[i] += (byte)((pixel1b & 0x01) * mask);
                                output[i + 8] += (byte)(((pixel1b & 0x02) >> 1) * mask);

                                mask = (byte)(mask >> 1);
                                if (mask == 0)
                                    mask = 0x80;

                                x++;
                            }

                            x -= widthX;
                            y++;
                            i++; //Next byte
                        }
                        x += widthX;
                        y -= widthY;
                        i += 8;
                    }
                    x = 0;
                    y += widthY;
                }

            return output;
        }



        public static List<byte> import3bpp(byte[] input, int widthX, int widthY, int size)
        {
            size = (int)(3 * (size / 8));

            List<byte> output = new List<byte>(size);
            for (int j = 0; j < size; j++)
            {
                output.Add(0);
            }

            /* Input is a pixel array. First 8 bytes represent the first output byte */
            byte mask = 0x80;
            byte pixel1b = 0;
            int i  = 0;
            int i3 = 0;
            int x = 0;
            int y = 0;

            while (i + (widthY * 2) < size) for (int yi = 0; yi < 128 / widthY; yi++)
                {
                    /* Draw a line of tiles (1b) */
                    for (int xi = 0; xi < 128 / widthX; xi++)
                    {
                        i3 = i + (widthY * 2);

                        /* Draw a tile (1b) */
                        for (int j = 0; j < widthY; j++)
                        {
                            if (i3 >= size)
                                return output;

                            mask = 0x80;

                            for (int k = 0; k < widthX; k++)
                            {
                                pixel1b = input[(x + y * 128)];
                                output[i]     += (byte)((pixel1b & 0x01) * mask);
                                output[i + 1] += (byte)(((pixel1b & 0x02) >> 1) * mask);
                                output[i3]    += (byte)(((pixel1b & 0x04) >> 2) * mask);

                                mask = (byte)(mask >> 1);
                                if (mask == 0)
                                    mask = 0x80;

                                x++;
                            }
                            x -= widthX;
                            y++;
                            i += 2; //Next byte
                            i3++;
                        }
                        i += widthY;
                        x += widthX;
                        y -= widthY;
                    }
                    x = 0;
                    y += widthY;
                }

            return output;
        }



        public static List<byte> import4bpp(byte[] input, int widthX, int widthY, int size)
        {
            size = (int)(size / 2);

            List<byte> output = new List<byte>(size);
            for (int j = 0; j < size; j++)
            {
                output.Add(0);
            }

            /* Input is a pixel array. First 8 bytes represent the first output byte */
            byte mask = 0x80;
            byte pixel1b = 0;
            int i  = 0;
            int i4 = 0;
            int x = 0;
            int y = 0;

            while (i + (widthY * 2) < size) for (int yi = 0; yi < 128 / widthY; yi++)
                {
                    /* Draw a line of tiles (1b) */
                    for (int xi = 0; xi < 128 / widthX; xi++)
                    {
                        i4 = i + (widthY * 2);

                        /* Draw a tile (1b) */
                        for (int j = 0; j < widthY; j++)
                        {
                            if (i4 >= size)
                                return output;

                            mask = 0x80;

                            for (int k = 0; k < widthX; k++)
                            {
                                pixel1b = input[(x + y * 128)];
                                output[i]      += (byte)((pixel1b & 0x01) * mask);
                                output[i + 1]  += (byte)(((pixel1b & 0x02) >> 1) * mask);
                                output[i4]     += (byte)(((pixel1b & 0x04) >> 2) * mask);
                                output[i4 + 1] += (byte)(((pixel1b & 0x08) >> 3) * mask);

                                mask = (byte)(mask >> 1);
                                if (mask == 0)
                                    mask = 0x80;

                                x++;
                            }
                            x -= widthX;
                            y++;
                            i  += 2; //Next byte
                            i4 += 2;
                        }
                        i += widthY * 2;
                        x += widthX;
                        y -= widthY;
                    }
                    x = 0;
                    y += widthY;
                }

            return output;
        }



        public static List<byte> import8bpp(byte[] input, int widthX, int widthY, int size)
        {
            List<byte> output = new List<byte>(size);
            for (int j = 0; j < size; j++)
            {
                output.Add(0);
            }

            /* Input is a pixel array. First 8 bytes represent the first output byte */
            byte mask = 0x80;
            byte pixel1b = 0;
            int i0 = 0;
            int i2 = 0;
            int i4 = 0;
            int i6 = 0;
            int x = 0;
            int y = 0;

            while (i0 + (widthY * 6) < size) for (int yi = 0; yi < 128 / widthY; yi++)
                {
                    /* Draw a line of tiles (1b) */
                    for (int xi = 0; xi < 128 / widthX; xi++)
                    {
                        i2 = i0 + (widthY * 2);
                        i4 = i0 + (widthY * 4);
                        i6 = i0 + (widthY * 6);

                        /* Draw a tile (1b) */
                        for (int j = 0; j < widthY; j++)
                        {
                            if (i6 >= size)
                                return output;

                            mask = 0x80;

                            for (int k = 0; k < widthX; k++)
                            {
                                pixel1b = input[(x + y * 128)];
                                output[i0]     += (byte)((pixel1b & 0x01) * mask);
                                output[i0 + 1] += (byte)(((pixel1b & 0x02) >> 1) * mask);
                                output[i2]     += (byte)(((pixel1b & 0x04) >> 2) * mask);
                                output[i2 + 1] += (byte)(((pixel1b & 0x08) >> 3) * mask);
                                output[i4]     += (byte)(((pixel1b & 0x10) >> 4) * mask);
                                output[i4 + 1] += (byte)(((pixel1b & 0x20) >> 5) * mask);
                                output[i6]     += (byte)(((pixel1b & 0x40) >> 6) * mask);
                                output[i6 + 1] += (byte)(((pixel1b & 0x80) >> 7) * mask);

                                mask = (byte)(mask >> 1);
                                if (mask == 0)
                                    mask = 0x80;

                                x++;
                            }
                            x -= widthX;
                            y++;
                            i0 += 2;
                            i2 += 2;
                            i4 += 2;
                            i6 += 2;
                        }
                        i0 += widthY * 6;
                        x += widthX;
                        y -= widthY;
                    }
                    x = 0;
                    y += widthY;
                }

            return output;
        }
    


        public static List<byte> import8bM7(byte[] input)
        {
            int size = 128 * 128;
            List<byte> output = new List<byte>(size);
            for (int j = 0; j < size ; j++)
            {
                output.Add(0);
            }

            int x = 0;
            int y = 0;
            int i = 0;

            for (int yi = 0; yi < 16; yi++)
            {

                /* Draw a line of tiles (8bM7,8x8) */
                for (int xi = 0; xi < 16; xi++)
                {

                    /* Draw a tile (8bM7,8x8) */
                    for (int yj = 0; yj < 8; yj++)
                    {
                        for (int xj = 0; xj < 8; xj++)
                        {
                            if (i >= size)
                            {
                                output.Clear();
                                return output;
                            }
                            output[i++] = input[x + y * 128];

                            x++;
                        }
                        x -= 8;
                        y++;
                    }
                    x += 8;
                    y -= 8;
                }
                x = 0;
                y += 8;
            }

            return output;
        }



        public static List<byte> import4bIFX(byte[] input)
        {
            int size = 128 * 128;
            List<byte> output = new List<byte>(size);
            for (int j = 0; j < size; j++)
            {
                output.Add(0);
            }

            int i  = 0;
            int y1 = 0x20; //32
            int y2 = 0x40; //64
            int y3 = 0x60; //96

            /* Draw a FX graphic */
            for (int y0 = 0; y0 < 0x20; y0++) //32
            {
                for (int xi = 0; xi < 0x80; xi++) //128
                {
                    if (i >= size)
                    {
                        output.Clear();
                        return output;
                    }

                    output[i] = (byte)(input[xi + y0 * 128] + (input[xi + y1 * 128] << 4));

                    i++;
                }
                y1++;

                for (int xi = 0; xi < 0x80; xi++) //128
                {
                    if (i >= size)
                    {
                        output.Clear();
                        return output;
                    }

                    output[i] = (byte)(input[xi + y2 * 128] + (input[xi + y3 * 128] << 4));

                    i++;
                }
                y2++;
                y3++;
            }


            return output;
        }
    }
} 