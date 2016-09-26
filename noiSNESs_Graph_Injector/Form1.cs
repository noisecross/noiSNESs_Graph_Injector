/**
* |------------------------------------------|
* | noiSNESs_Graph_Injector                  |
* | File: Form1.cs                           |
* | v1.02, September 2016                    |
* | Author: noisecross                       |
* |------------------------------------------|
* 
* @author noisecross
* @version 1.02
* 
*/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;



namespace noiSNESs_Graph_Injector
{
    public partial class Form1 : Form
    {
        const string windowName      = "noiSNESs_Graph_Injector";
        const string version         = "Ver. v1.02 (September 2016)";
        string       currentFileName = "";
        bool         unsavedChanges  = false;

        List<byte> byteList    = new List<byte>();
        List<byte> transformed = new List<byte>();
        int    page   = 0;
        int    offset = 0;
        int tileSizeX = 8;
        int tileSizeY = 8;
        int addressShown = 0;
        int currentFileSize = 0;

        int currentPageMult = 0;

        System.Drawing.Color[] currentPallette = Palettes.palette1b;



        public Form1()
        {
            InitializeComponent();
            Palettes.palette8b = Palettes.initializePalette8b();
            comboBoxTileSize.SelectedIndex = 0;
            comboBoxMode.SelectedIndex = 0;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }



        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if (unsavedChanges)
            {
                if (MessageBox.Show("The file have unsaved changes\r\nClose anyway?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }



        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFileName = "";
            Form1.ActiveForm.Text = windowName;
            unsavedChanges = false;

            byteList.Clear();
        }



        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* Displays an OpenFileDialog so the user can select a res */
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SMC File|*.smc";
            openFileDialog.Title  = "Choose a SMC file";

            /* Show the Dialog */
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (openFileDialog.FileName != "")
                {
                    currentFileName = openFileDialog.SafeFileName;
                    unsavedChanges  = false;
                    Form1.ActiveForm.Text = windowName + " - " + currentFileName;

                    System.IO.FileStream fs = (System.IO.FileStream)openFileDialog.OpenFile();
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);

                    currentFileSize = 0;

                    try
                    {
                        int i = 0;

                        br.BaseStream.Position = 0;
                        //toolStripProgressBar.Maximum = 1 + (int)br.BaseStream.Length / 128;
                        byteList.Clear();

                        /*
                        while (br.BaseStream.Position != br.BaseStream.Length)
                        {
                            byteList.Add(br.ReadByte());

                            if (i++ % 128 == 127)
                                toolStripProgressBar.Value++;
                        }
                        */
                        //toolStripProgressBar.Value = 0;
                        byteList.AddRange(br.ReadBytes((int)br.BaseStream.Length).ToList());

                        currentFileSize = (int)br.BaseStream.Length;
                        comboBoxMode_SelectedIndexChanged(null, null);
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("Error reading the file: " + error.ToString(), "Error");
                        //toolStripProgressBar.Value = 0;
                    }

                    br.Close();
                    fs.Close();
                }
            }
            openFileDialog.Dispose();
            openFileDialog = null;
        }



        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {    
            /* Displays an OpenFileDialog so the user can select a res */
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SMC File|*.smc";
            saveFileDialog.Title = "Choose a SMC file";

            /* Show the Dialog */
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (saveFileDialog.FileName != "")
                {
                    currentFileName = saveFileDialog.FileName;
                    Form1.ActiveForm.Text = windowName + " - " + currentFileName;
                    unsavedChanges = false;

                    //Debug
                    /*
                    string message = "";
                    int address = 0x0446AA;

                    while (address < 0x05E5E8)
                    {
                        saveFileDialog.FileName = currentFileName + address.ToString("X6") + ".brr";
                        message += (address + 0xC00000).ToString("X6") + "\r\n";

                        address++;
                        address++;

                        //C4/BA78, C5/136A, C5/928A
                        System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                        System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
                        
                        List<Byte> sample = new List<byte>();

                        byte header = 0xFF;
                        do{
                            header = byteList[address++];
                            sample.Add(header);
                            sample.Add(byteList[address++]);
                            sample.Add(byteList[address++]);
                            sample.Add(byteList[address++]);
                            sample.Add(byteList[address++]);
                            sample.Add(byteList[address++]);
                            sample.Add(byteList[address++]);
                            sample.Add(byteList[address++]);
                            sample.Add(byteList[address++]);
                        } while ((header & 0x01) == 0);

                        bw.Write(sample.ToArray());
                        
                        bw.Close();
                        fs.Close();
                    }

                    MessageBox.Show(message);*/

                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                    System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);

                    try
                    {
                        bw.Write(byteList.ToArray());
                        //foreach (byte item in byteList) bw.Write(item);
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("Error writing the file: " + error.ToString(), "Error");
                    }

                    bw.Close();
                    fs.Close();
                }
            }
            saveFileDialog.Dispose();
            saveFileDialog = null;
        }



        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newIndex  = 0;
            int newOffset = 0;
            int nPages    = 0;

            switch (comboBoxMode.SelectedIndex)
            {
                case 0:
                    /* 1bpp */
                    newIndex  = addressShown / Constants.PAGESIZE_1b;
                    newOffset = addressShown - (newIndex * Constants.PAGESIZE_1b);
                    nPages = currentFileSize / Constants.PAGESIZE_1b;
                    currentPallette = Palettes.palette1b;
                    break;
                case 1:
                    /* 2bpp */
                    newIndex  = addressShown / Constants.PAGESIZE_2b;
                    newOffset = addressShown - (newIndex * Constants.PAGESIZE_2b);
                    nPages = currentFileSize / Constants.PAGESIZE_2b;
                    currentPallette = Palettes.palette2b;
                    break;
                case 2:
                    /* 3bpp */
                    newIndex  = addressShown / Constants.PAGESIZE_3b;
                    newOffset = addressShown - (newIndex * Constants.PAGESIZE_3b);
                    nPages = currentFileSize / Constants.PAGESIZE_3b;
                    currentPallette = Palettes.palette3b;
                    break;
                case 3:
                    /* 4bpp */
                    newIndex  = addressShown / Constants.PAGESIZE_4b;
                    newOffset = addressShown - (newIndex * Constants.PAGESIZE_4b);
                    nPages = currentFileSize / Constants.PAGESIZE_4b;
                    currentPallette = Palettes.palette4b;
                    break;
                case 4:
                    /* 4bpp(Interleaved FX) */
                    newIndex  = addressShown / Constants.PAGESIZE_4b;
                    newOffset = addressShown - (newIndex * Constants.PAGESIZE_4b);
                    nPages = currentFileSize / Constants.PAGESIZE_4b;
                    currentPallette = Palettes.palette4b;
                    break;
                case 5:
                    /* 8bpp */
                    newIndex  = addressShown / Constants.PAGESIZE_8b;
                    newOffset = addressShown - (newIndex * Constants.PAGESIZE_8b);
                    nPages = currentFileSize / Constants.PAGESIZE_8b;
                    currentPallette = Palettes.palette8b;
                    break;
                case 6:
                    /* 8bpp(M7) */
                    newIndex  = addressShown / Constants.PAGESIZE_8b;
                    newOffset = addressShown - (newIndex * Constants.PAGESIZE_8b);
                    nPages = currentFileSize / Constants.PAGESIZE_8b;
                    currentPallette = Palettes.palette8b;
                    break;
                case 7:
                    /* 8bpp(M7 pre) */
                    newIndex = addressShown / Constants.PAGESIZE_8b;
                    newOffset = addressShown - (newIndex * Constants.PAGESIZE_8b);
                    nPages = currentFileSize / Constants.PAGESIZE_8b;
                    currentPallette = Palettes.palette8b;
                    break;
                case 8:
                    /* 8bpp(M7 pos) */
                    newIndex = addressShown / Constants.PAGESIZE_8b;
                    newOffset = addressShown - (newIndex * Constants.PAGESIZE_8b);
                    nPages = currentFileSize / Constants.PAGESIZE_8b;
                    currentPallette = Palettes.palette8b;
                    break;
                default:
                    break;
            }

            panelMain.Visible = false;

            comboBoxPage.Items.Clear();
            for (int i = 0; i < nPages; i++)
            {
                comboBoxPage.Items.Add(i.ToString("X3"));
            }
            if (newIndex < comboBoxPage.Items.Count)
            {
                comboBoxPage.SelectedIndex = newIndex;
            }
            else
            {
                comboBoxPage.SelectedIndex = newIndex - 1;
            }
            numericUpDownOffset.Value  = newOffset;

            panelMain.Visible = true;

            panelMain.Refresh();
        }



        private Tuple<Bitmap, byte[,]> generateBitmap(int size=0)
        {
            Tuple<Bitmap, byte[,]> bitmap;

            switch (comboBoxMode.SelectedIndex)
            {
                case 0:
                    /* 1bbp */
                    bitmap = Transformations.transform1b(byteList, page, offset,
                        tileSizeX, tileSizeY, (size != 0) ? size : Constants.PAGESIZE_1b);
                    currentPageMult = Constants.PAGESIZE_1b;
                    break;
                case 1:
                    /* 2bbp */
                    bitmap = Transformations.transform2b(byteList, page, offset,
                        tileSizeX, tileSizeY, (size != 0) ? size : Constants.PAGESIZE_2b);
                    currentPageMult = Constants.PAGESIZE_2b;
                    break;
                case 2:
                    /* 3bbp */
                    bitmap = Transformations.transform3b(byteList, page, offset,
                        tileSizeX, tileSizeY, (size != 0) ? size : Constants.PAGESIZE_3b);
                    currentPageMult = Constants.PAGESIZE_3b;
                    break;
                case 3:
                    /* 4bbp */
                    bitmap = Transformations.transform4b(byteList, page, offset,
                        tileSizeX, tileSizeY, (size != 0) ? size : Constants.PAGESIZE_4b);
                    currentPageMult = Constants.PAGESIZE_4b;
                    break;
                case 4:
                    /* 4bbp (Interleaved FX) */
                    bitmap = Transformations.transform4bIFX(byteList, page, offset);
                    currentPageMult = Constants.PAGESIZE_4b;
                    break;
                case 5:
                    /* 8bbp */
                    bitmap = Transformations.transform8b(byteList, page, offset,
                        tileSizeX, tileSizeY, (size != 0) ? size : Constants.PAGESIZE_8b);
                    currentPageMult = Constants.PAGESIZE_8b;
                    break;
                case 6:
                    /*8bbp(M7)*/
                    bitmap = Transformations.transform8bM7(byteList, page, offset);
                    currentPageMult = Constants.PAGESIZE_8b;
                    break;
                case 7:
                    /*8bbp(M7 tilemap)*/
                    bitmap = Transformations.transform8bM7_2(byteList, page, offset);
                    currentPageMult = Constants.PAGESIZE_8b;
                    break;
                case 8:
                    /*8bbp(M7 tileset)*/
                    bitmap = Transformations.transform8bM7(byteList, page, offset, 1, 0);
                    currentPageMult = Constants.PAGESIZE_8b;
                    break;
                default:
                    bitmap = new Tuple<Bitmap,byte[,]>(new Bitmap(8, 8), new byte[1,1]);
                    currentPageMult = 0;
                    break;
            }

            return bitmap;
        }



        private void panelMain_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bitmap = generateBitmap().Item1;

            addressShown = currentPageMult * page + offset;
            labelInitOffset.Text = "0x" + (currentPageMult * page + offset).ToString("X6");
            labelEndOffset.Text = "0x" + (currentPageMult * (page + 1) + offset).ToString("X6");

            /* Scaling graph to zoom it */
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(bitmap, 1, 1, 513, 513);

            for (int j = 0; j < 512; j+=tileSizeY * Constants.ZOOM_8x8){
                for (int i = 0; i < 512; i += tileSizeX * Constants.ZOOM_8x8)
                {
                    e.Graphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Silver),
                        i, j, tileSizeX * Constants.ZOOM_8x8, tileSizeY * Constants.ZOOM_8x8);
                }
            }
        }



        private void domainUpDownOffset_SelectedItemChanged(object sender, EventArgs e)
        {
            try
            {
                offset = Int32.Parse(numericUpDownOffset.Text, System.Globalization.NumberStyles.HexNumber);
                panelMain.Refresh();
            }
            catch (FormatException error)
            {
                numericUpDownOffset.Text = "000000";
            }

        }



        private void comboBoxPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            page = comboBoxPage.SelectedIndex;
            panelMain.Refresh();
        }



        private void comboBoxTileSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            tileSizeX = ((comboBoxTileSize.SelectedIndex) / 3) * 4 + 8;
            tileSizeY = ((comboBoxTileSize.SelectedIndex) % 3) * 4 + 8;

            panelMain.Refresh();
        }



        private void numericUpDownOffset_ValueChanged(object sender, EventArgs e)
        {
            offset = (int)numericUpDownOffset.Value;
            panelMain.Refresh();
        }



        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void exportBPMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int pages = comboBoxPage.Items.Count;

            if (pages < 1)
            {
                MessageBox.Show("You must load a file in order to export a PNG","Warning");
                return;
            }

            FormExportDialog exportDialogForm = new FormExportDialog(pages);
            exportDialogForm.groupBox.Text += " " + addressShown.ToString("X6");
            exportDialogForm.ShowDialog();

            if (!exportDialogForm.okToExport)
                return;

            int bytesToExport = exportDialogForm.nBytesToExport + exportDialogForm.pagesToExport * currentPageMult;
            exportDialogForm.Dispose();
            

            /* Start the export */


            /* Displays an OpenFileDialog so the user can select a res */
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter   = "PNG File|*.png";
            saveFileDialog.Title    = "Choose a PNG file";
            saveFileDialog.FileName = addressShown.ToString("X6") + "_" +
                comboBoxMode.Text + "_" +
                comboBoxTileSize.Text;

            /* Show the Dialog */
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (saveFileDialog.FileName != "")
                {
                    try
                    {
                        Tuple<Bitmap,byte[,]> tuple = generateBitmap(bytesToExport);
                        byte[] byteArray            = new byte[tuple.Item2.Length];
                        int i                       = 0;

                        /* Set bytemap */
                        for (int k = 0; k < tuple.Item1.Height; k++)
                        {
                            for (int j = 0; j < tuple.Item1.Width; j++)
                            {
                                byteArray[i++] = tuple.Item2[j, k];
                            }
                        }

                        /* Create a new 8bpp Bitmap */
                        Bitmap bitmap = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                        Bitmap newBitmap = tuple.Item1.Clone(new Rectangle(0, 0, tuple.Item1.Width, tuple.Item1.Height),
                            System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                        /* Open it to edit */
                        System.Drawing.Imaging.BitmapData data = newBitmap.LockBits(
                            new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
                            System.Drawing.Imaging.ImageLockMode.WriteOnly,
                            System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                        /* Recover the original values (now lost because of the conversion) */
                        System.Runtime.InteropServices.Marshal.Copy(byteArray, 0, data.Scan0, tuple.Item2.Length);
                        newBitmap.UnlockBits(data);

                        /* Set current palette */
                        System.Drawing.Imaging.ColorPalette pal = bitmap.Palette;
                        for (int j = 0; j < currentPallette.Length; j++)
                        {
                            pal.Entries[j] = currentPallette[j];
                        }
                        for (int j = currentPallette.Length; j < pal.Entries.Length; j++)
                        {
                            pal.Entries[j] = System.Drawing.Color.Magenta;
                        }
                        newBitmap.Palette = pal;

                        /* Save file */
                        newBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);

                        newBitmap.Dispose();
                        bitmap.Dispose();
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("Error writing the file: " + error.ToString(), "Error");
                    }
                }
            }
            saveFileDialog.Dispose();
            saveFileDialog = null;

        }



        private void importPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
             * The expected Bitmaps have a size of n x fileSize and a PixelFormat of Format8bppIndexed
             */

            /* Displays an OpenFileDialog so the user can select a res */
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG File|*.png|BMP File|*.bmp";
            openFileDialog.Title = "Choose a file";

            /* Show the Dialog */
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (openFileDialog.FileName != "")
                {
                    try
                    {
                        Bitmap bitmap = new Bitmap(openFileDialog.FileName);
                        int size = bitmap.Width * bitmap.Height;
                        byte[] bitmapPixels = new byte[size];

                        /* Open it to edit */
                        System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                            System.Drawing.Imaging.ImageLockMode.WriteOnly,
                            System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                        /* Recover the original values (now lost because of the conversion) */
                        for (int i = 0 ; i < size ; i++){
                            bitmapPixels[i] = System.Runtime.InteropServices.Marshal.ReadByte(data.Scan0, i);
                        }


                        List<byte> newBytes;
                        int        nColumns = (int)Math.Truncate(128.0 / tileSizeX);

                        switch (comboBoxMode.SelectedIndex)
                        {
                            case 0:
                                /* 1bbp */
                                size = size - (size % ((tileSizeX * nColumns) * tileSizeY));
                                newBytes = InvertingTransformations.import1bpp(bitmapPixels, tileSizeX, tileSizeY, size);
                                break;
                            case 1:
                                /* 2bbp */
                                size = size - (size % ((tileSizeX * nColumns) * tileSizeY));
                                newBytes = InvertingTransformations.import2bpp(bitmapPixels, tileSizeX, tileSizeY, size);
                                break;
                            case 2:
                                /* 3bbp */
                                size = size - (size % ((tileSizeX * nColumns) * tileSizeY));
                                newBytes = InvertingTransformations.import3bpp(bitmapPixels, tileSizeX, tileSizeY, size);
                                break;
                            case 3:
                                /* 4bbp */
                                size = size - (size % ((tileSizeX * nColumns) * tileSizeY));
                                newBytes = InvertingTransformations.import4bpp(bitmapPixels, tileSizeX, tileSizeY, size);
                                break;
                            case 4:
                                /* 4bbp (Interleaved FX) */
                                size = size - (size % (128 * 128));
                                newBytes = InvertingTransformations.import4bIFX(bitmapPixels);
                                break;
                            case 5:
                                /* 8bbp */
                                size = size - (size % ((tileSizeX * nColumns) * tileSizeY));
                                newBytes = InvertingTransformations.import8bpp(bitmapPixels, tileSizeX, tileSizeY, size);
                                break;
                            case 6:
                                /*8bbp(M7)*/
                                size = size - (size % (128 * 128));
                                newBytes = InvertingTransformations.import8bM7(bitmapPixels);
                                break;
                            default:
                                newBytes = new List<byte>();
                                break;
                        }

                        for (int i = 0; i < newBytes.Count; i++)
                        {
                            if (addressShown + i >= byteList.Count)
                                break;
                            byteList[addressShown + i] = newBytes[i];
                        }

                        Form1.ActiveForm.Text = windowName + " - " + currentFileName + "*";
                        unsavedChanges = true;

                        panelMain.Refresh();
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("Error reading the file: " + error.ToString(), "Error");
                        //toolStripProgressBar.Value = 0;
                    }
                }
            }
            openFileDialog.Dispose();
            openFileDialog = null;
        }



        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String outputMessage = "";

            outputMessage += "Developed by Noisecross:";
            outputMessage += "\r\n";
            outputMessage += version;
            outputMessage += "\r\n";
            outputMessage += "\r\n";
            outputMessage += "This tool is not under any kind of support, but for any questions please read the readme.docx file or contact the developer by email (dalastnecromancer@gmail.com)";

            MessageBox.Show(outputMessage, "About");
        }



    }
}
