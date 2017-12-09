using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Threading;


namespace Distance_Finder
{
    
    public partial class DF : Form
    {
        List<Bitmap> listCachedZoomedImage = new List<Bitmap>();
        List<DrawDimension> listOfPositionsToFindDistance = new List<DrawDimension>();
        List<DrawDimension> listOfPositionsToRemove = new List<DrawDimension>();
        DrawDimension objectOfDrawDimension = new DrawDimension();

        private Bitmap originalImageFile;
        private Bitmap imageFile;
        private Bitmap tempImage;
        private Bitmap tempImage1;

        private double zoomFactor=1;
        private string formDisplayName = "Spatial Analysis ";
        private string fileExtension = string.Empty;

        private int currentX = 0;
        private int currentY = 0;
        private int originalImageWidth = 0;
        private int originalImageHeight = 0;
        private int left = 0;
        private int right = 0;
        private int top = 0;
        private int bottom = 0;
        private int maxHeightOfRectangle = 40;
        private int maxWidthOfRectangle = 40;
        private int thichnessOfRectangleToDraw = 2;
        private int maxZoomLevelAllowed = 4;

        private int[] widthDimension = new int[2];
        private int[] tempWidthDimension = new int[2];
        private int[] heightDimension = new int[2];
        private int[] tempHeightDimension = new int[2];


        Color seedPixelColor = new Color();
        Color tempPixelColor = new Color();
        Color tempXAxisPixelColor = new Color();
        Color tempYAxisPixelColor = new Color();
        Color penColorToDrawWidth = Color.Black;
        Color penColorToDrawHeight = Color.Red;
        Color displayPixelColor = new Color();

        Brush brushForWrtingWidth = Brushes.Blue;
        Brush brushForWrtingHeight = Brushes.Green;

        private string fontStyleForWritingWidth = "Calibri";
        private int fontSizeForWritingWidth = 20;
        Font fontForWrtingWidth = new Font("Calibri", 10);

        private string fontStyleForWritingHeight = "Calibri";
        private int fontSizeForWritingHeight = 20;
        Font fontForWrtingHeight = new Font("Calibri", 10);

        Font tempFontForWriting = null;

        private Size newSize = new Size();

        Pen penWidth = new Pen(Color.Black, 1);
        Pen penHeight = new Pen(Color.Red, 1);

        Thread threadForCachingZoomedImage = null;



        public DF()
        {
            InitializeComponent();
            this.Location = new Point(0, 0);
            SetProcessDPIAware();
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.AutoScroll = true;
            this.KeyPreview = true;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        private void DS_Load(object sender, EventArgs e)
        {
            this.Text = formDisplayName;
            this.pictureBox.MouseWheel += pictureBox_MouseWheel;
            this.DoubleBuffered = true;

            labelToolDescription.Text = "1. Paste  OR  Drag-Drop  your  image" + Environment.NewLine +
                                          "2. Left/Right  mouse  click  for  finding  distance  between  2 points " + Environment.NewLine +
                                          "3. Ctrl + Scroll-UP/Down  OR Key UP/Down to  Zoom-In/Zoom-Out" + Environment.NewLine +
                                          "4. Ctrl + C/ Ctrl + S to  copy & save image" + Environment.NewLine +
                                          "5. Ctrl + Z/Ctrl + Y  to  Undo-Redo  changes  in  image";
            this.pictureBox.Image = this.pictureBox.BackgroundImage;
            labelToolDescription.Parent = this.panelPictureBoxHolder;
            labelToolDescription.BackColor = System.Drawing.Color.Transparent;
            //string imageFilePath = @"C:\Users\Sachin\Desktop\a.png";
            this.originalImageFile = new Bitmap(this.pictureBox.Image);
            this.imageFile = new Bitmap(this.originalImageFile);
            ////this.Size = originalImageFile.Size;
            ////this.panelPictureBoxHolder.Size = originalImageFile.Size;
            ////this.pictureBox.Size = originalImageFile.Size;
            ////this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.pictureBox.Image = imageFile;

            //threadForCachingZoomedImage = new Thread(LoadZoomedImageInList);
            //CacheZoomedImage();
 
            listCachedZoomedImage.Clear();
            //setImageToPictureBox(); 
        }

        private void pictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                else if (e.Delta < 0)
                {
                    ZoomOut();
                }
            }  
        }

        private void CacheZoomedImage()
        {
            if (threadForCachingZoomedImage!=null && threadForCachingZoomedImage.IsAlive)
            {
                threadForCachingZoomedImage.Abort();
            }
            threadForCachingZoomedImage = new Thread(LoadZoomedImageInList);
            threadForCachingZoomedImage.Priority = ThreadPriority.Lowest;
            threadForCachingZoomedImage.Start();
        }

        
        

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            this.Text = formDisplayName;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            this.currentX = getRelativePosition(e.Location.X);
            this.currentY = getRelativePosition(e.Location.Y);
            displayPixelColor=this.originalImageFile.GetPixel(this.currentX, this.currentY);
            this.Text = formDisplayName + " x=" + this.currentX + " y=" + this.currentY + "  R=" + displayPixelColor.R.ToString() + "  G=" + displayPixelColor.G.ToString() + "  B=" + displayPixelColor.B.ToString();
            //this.Focus();
        }

        private void DS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    copyPictureBoxImageToClipBoard();
                }
                else if (e.KeyCode == Keys.S)
                {
                    newSize = new Size((int)(originalImageFile.Width), (int)(originalImageFile.Height));
                    Bitmap tempImage = new Bitmap(this.imageFile, newSize);
                    saveImage(tempImage);
                }
                else if (e.KeyCode == Keys.V)
                {
                    copyImageFromClipBoardToPictureBox();
                }
                else if (e.KeyCode == Keys.Up)
                {
                    ZoomIn();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    ZoomOut();
                }
                else if (e.KeyCode == Keys.Z)
                {
                    RemoveDrawnRectangle();
                }
                else if (e.KeyCode == Keys.Y)
                {
                    AddLastRemovedRectanle();
                }
            }
        }

        private void AddLastRemovedRectanle()
        {
            if (listOfPositionsToRemove.Count > 0)
            {
                listOfPositionsToFindDistance.Add(listOfPositionsToRemove[listOfPositionsToRemove.Count-1]);
                listOfPositionsToRemove.RemoveAt(listOfPositionsToRemove.Count-1);
                this.imageFile = (Bitmap)(listCachedZoomedImage[(int)(zoomFactor - 1)]).Clone();
                DrawAllDimensions();
                this.pictureBox.Image = this.imageFile;
                this.pictureBox.Refresh();
                
            }
        }

        private void RemoveDrawnRectangle()
        {
            if (listOfPositionsToFindDistance.Count > 0)
            {
                listOfPositionsToRemove.Add(listOfPositionsToFindDistance[(listOfPositionsToFindDistance.Count-1)]);
                listOfPositionsToFindDistance.RemoveAt(listOfPositionsToFindDistance.Count - 1);
                this.imageFile = (Bitmap)(listCachedZoomedImage[(int)(zoomFactor - 1)]).Clone();
                DrawAllDimensions();
                this.pictureBox.Image = this.imageFile;                
                this.pictureBox.Refresh();
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drawWidth(this.currentX, this.currentY);
            }
            else if (e.Button == MouseButtons.Right)
            {
                drawHeight(this.currentX, this.currentY);
            }
            
        }

        private void AddPositionsToList(int posX, int posY, int width, int height, bool toDrawWidth)
        {
            objectOfDrawDimension = new DrawDimension();
            objectOfDrawDimension.PosX = posX;
            objectOfDrawDimension.PosY=posY;
            objectOfDrawDimension.Width = width;
            objectOfDrawDimension.Height = height;
            objectOfDrawDimension.IsDrawWidth = toDrawWidth;
            listOfPositionsToFindDistance.Add(objectOfDrawDimension);
        }

        private void panelPictureBoxHolder_DragDrop(object sender, DragEventArgs e)
        {
            string[] allFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            copyImageFileFromClipboard(allFilePaths);
           
        }

        private void copyImageFileFromClipboard(string[] allFilePaths)
        {
            try
            {
                foreach (string filePath in allFilePaths)
                {
                    fileExtension = Path.GetExtension(filePath).ToLower();
                    if ((fileExtension == ".jpg") || (fileExtension == ".jpeg") || (fileExtension == ".png") || (fileExtension == ".bmp"))
                    {
                        zoomFactor = 1;
                        this.originalImageFile = new Bitmap(filePath);
                        this.imageFile = new Bitmap(this.originalImageFile);
                        //this.Size = originalImageFile.Size;
                        //this.panelPictureBoxHolder.Size = originalImageFile.Size;
                        //this.pictureBox.Size = originalImageFile.Size;
                        //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
                        this.pictureBox.Image = imageFile;
                        //CacheZoomedImage();
                        listCachedZoomedImage.Clear();
                        listOfPositionsToFindDistance.Clear();
                        listOfPositionsToRemove.Clear();
                        setImageToPictureBox();
                    }
                    break;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error ocurred\n"+ex.Message);
            }
        }

        private void panelPictureBoxHolder_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void drawRectangle(Pen penWithColorAndWidth, int posX, int posY, int rectangleWidth, int rectangleHeight, bool writeWidthText)
        {
            using (var graphics = Graphics.FromImage(this.imageFile))
            {
                graphics.DrawRectangle(penWithColorAndWidth, posX, posY, rectangleWidth, rectangleHeight);
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                float X = (float)(posX + (5 * zoomFactor));
                float Y = (float)(posY + (5 * zoomFactor));
                if (writeWidthText)
                {
                    tempFontForWriting = getFontForWritingWidthOfRectangle();
                    X = getUpdatedXPositionForWritng(tempFontForWriting, X);
                    Y = getUpdatedYPositionForWritng(tempFontForWriting, Y);
                    graphics.DrawString(Convert.ToString(rectangleWidth / zoomFactor), tempFontForWriting, getBrushForWritingWidth(), X, Y);
                }
                else
                {
                    tempFontForWriting = getFontForWritingHeightOfRectangle();
                    X = getUpdatedXPositionForWritng(tempFontForWriting, X);
                    Y = getUpdatedYPositionForWritng(tempFontForWriting, Y);
                    graphics.DrawString(Convert.ToString(rectangleHeight / zoomFactor), tempFontForWriting, getBrushForWritingHeight(), X, Y);
                }
                this.pictureBox.Image = imageFile;
            }
        }

        private float getUpdatedXPositionForWritng(Font tempFont, float X)
        {
            if ((tempFont.Height + (int)X + 20 * zoomFactor) > this.imageFile.Width)
            {
                X = (float)(this.imageFile.Width - (int)tempFont.Height - (20 * zoomFactor));
            }
            return X;
        }

        private float getUpdatedYPositionForWritng(Font tempFont, float Y)
        {
            if ((tempFont.Height + (int)Y) > this.imageFile.Height)
            {
                Y =  (float)(this.imageFile.Height - (int)tempFont.Height - (2*zoomFactor));
            }
            return Y;
        }

        private void saveImage(Image imageToSave)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                }
                PictureBox tempPictureBox = new PictureBox();
                tempPictureBox.Image = imageToSave;
                tempPictureBox.Image.Save(sfd.FileName, format);
            }
        }

        private Brush getBrushForWritingWidth()
        {
            return brushForWrtingWidth;
        }

        private Brush getBrushForWritingHeight()
        {
            return brushForWrtingHeight;
        }

        private Font getFontForWritingWidthOfRectangle()
        {
            return new Font(fontStyleForWritingWidth, (int)(fontSizeForWritingWidth * zoomFactor), FontStyle.Bold, GraphicsUnit.Pixel);
        }

        private Font getFontForWritingHeightOfRectangle()
        {
            return new Font(fontStyleForWritingHeight, (int)(fontSizeForWritingHeight * zoomFactor), FontStyle.Bold, GraphicsUnit.Pixel);
        }

        private void drawHeight(int currentX, int currentY)
        {
            tempHeightDimension = calculateHeight(currentX, currentY);
            int widthOfRectangle = getWidthOfRectangleToDraw(tempHeightDimension[0], tempHeightDimension[1], currentX);
            penHeight = getPenToDrawHeight();

            int posX, posY, width, height;
            posX = (int)(currentX * zoomFactor);
            posY = (int)(tempHeightDimension[0] * zoomFactor);
            width = (int)(widthOfRectangle*zoomFactor);
            height = (int)((tempHeightDimension[1] - tempHeightDimension[0] + 1) * zoomFactor);

            drawRectangle(penHeight, posX, posY, width, height, false);

            posX = (int)(currentX );
            posY = (int)(tempHeightDimension[0] );
            width = (int)(widthOfRectangle );
            height = (int)((tempHeightDimension[1] - tempHeightDimension[0] + 1) );
            AddPositionsToList(posX, posY, width, height, false);
        }

        private void drawWidth(int currentX, int currentY)
        {
            tempWidthDimension = calculateWidth(currentX, currentY);
            int heightOfRectangle = getHeightOfRectangleToDraw(tempWidthDimension[0], tempWidthDimension[1], currentY);
            penWidth = getPenToDrawWidth();
            int posX, posY, width, height;
            posX = (int)((tempWidthDimension[0] )* zoomFactor);
            posY = (int)(currentY * zoomFactor);
            width = (int)((tempWidthDimension[1] - tempWidthDimension[0] + 1) * zoomFactor);
            height = (int)(heightOfRectangle * zoomFactor);

            drawRectangle(penWidth, posX, posY, width, height, true);

            posX = (int)((tempWidthDimension[0]));
            posY = (int)(currentY);
            width = (int)((tempWidthDimension[1] - tempWidthDimension[0] + 1));
            height = (int)(heightOfRectangle);

            AddPositionsToList(posX, posY, width, height, true);
        }

        
        private int[] calculateWidth(int seedX, int seedY)
        {
            seedPixelColor = originalImageFile.GetPixel(seedX, seedY);
            tempPixelColor = seedPixelColor;
            originalImageWidth = this.originalImageFile.Width;

            this.left = seedX;
            this.right = seedX;

            while ((tempPixelColor == seedPixelColor) && (this.left <= seedX && this.left > 0))
            {
                this.left = this.left - 1;
                tempPixelColor = originalImageFile.GetPixel(this.left, seedY);
            }
            if ((tempPixelColor != seedPixelColor) )
            {
                this.left = this.left + 1;
            }
            tempPixelColor = seedPixelColor;
            while ((tempPixelColor == seedPixelColor) && (this.right >= seedX && this.right < (originalImageWidth-1)))
            {
                this.right = this.right + 1;
                tempPixelColor = originalImageFile.GetPixel(this.right, seedY);
            }
            if ((tempPixelColor != seedPixelColor) )
            {
                this.right = this.right - 1;
            }

            widthDimension[0] = this.left;
            widthDimension[1] = this.right;
            return widthDimension;
        }

        private int[] calculateHeight(int seedX, int seedY)
        {
            seedPixelColor = this.originalImageFile.GetPixel(seedX, seedY);
            tempPixelColor = seedPixelColor;
            originalImageHeight = this.originalImageFile.Height;

            this.top = seedY;
            this.bottom = seedY;

            while (tempPixelColor == seedPixelColor && (this.top <= seedY && this.top > 0))
            {
                this.top = this.top - 1;
                tempPixelColor = this.originalImageFile.GetPixel(seedX, this.top);
            }
            if ((tempPixelColor != seedPixelColor))
            {
                this.top = this.top + 1;
            }

            tempPixelColor = seedPixelColor;

            while (tempPixelColor == seedPixelColor && (this.bottom >= seedY && this.bottom < (originalImageHeight-1)))
            {
                this.bottom = this.bottom + 1;
                tempPixelColor = this.originalImageFile.GetPixel(seedX, this.bottom); ;	
            }
            if ((tempPixelColor != seedPixelColor))
            {
                this.bottom = this.bottom - 1;
            }

            heightDimension[0] = this.top;
            heightDimension[1] = this.bottom;
            return heightDimension;
        }

        private int getWidthOfRectangleToDraw(int top, int bottom, int seedX)
        {
            this.left = seedX;
            seedPixelColor = this.originalImageFile.GetPixel(seedX, top);

            this.right = seedX + 1;
            tempPixelColor = this.originalImageFile.GetPixel(this.right, top);

            bool sameColor = true;
            int tempY=0;
            while (seedPixelColor == tempPixelColor && sameColor == true && this.right <= (seedX + maxWidthOfRectangle) && this.right < this.originalImageFile.Width - 1)
            {
                tempY = top + 1;
                tempPixelColor = this.originalImageFile.GetPixel(this.right, tempY);

                while (sameColor == true && tempY < bottom)
                {
                    tempY = tempY + 1;
                    tempPixelColor = this.originalImageFile.GetPixel(this.right, tempY);
                    if (seedPixelColor != tempPixelColor)
                    {
                        sameColor = false;
                    }
                }

                this.right = this.right + 1;
                tempPixelColor = this.originalImageFile.GetPixel(this.right, top);
            }

            if (this.right != (seedX + 1))
            {
                this.right = this.right - 1;
            }

            return (this.right - seedX);
        }

        private int getHeightOfRectangleToDraw(int left, int right, int seedY)
        {		
            seedPixelColor=originalImageFile.GetPixel(left,seedY);
            this.bottom = seedY + 1;
            tempYAxisPixelColor = originalImageFile.GetPixel(left, this.bottom);
            bool sameColor=true;
            int tempX = 0;
            while (seedPixelColor == tempYAxisPixelColor && sameColor == true && this.bottom <= (seedY + maxHeightOfRectangle) && this.bottom < this.originalImageFile.Height - 1)
            {
                tempX = left + 1;
                tempXAxisPixelColor = originalImageFile.GetPixel(tempX, this.bottom);

                while (sameColor == true && tempX <= right)
                {
                    tempXAxisPixelColor = originalImageFile.GetPixel(tempX, this.bottom);
                    tempX = tempX + 1;

                    if (seedPixelColor != tempXAxisPixelColor)
                    {
                        sameColor = false;
                    }
                }

                this.bottom = this.bottom + 1;
                tempYAxisPixelColor = originalImageFile.GetPixel(left, this.bottom);
            }
            

            if (sameColor == false || (this.bottom > (seedY + maxHeightOfRectangle)))
            {
                this.bottom = this.bottom - 1;
            }

            return (this.bottom - seedY);            
        }

        private void copyImageFromClipBoardToPictureBox()
        {
            IDataObject d = Clipboard.GetDataObject();
            if (Clipboard.ContainsImage())
            {
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                this.originalImageFile = new Bitmap(Clipboard.GetImage());
                this.imageFile = this.originalImageFile;
                //this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
                //this.AutoScroll = true;
                //this.Size = this.originalImageFile.Size;
                //this.panelPictureBoxHolder.Size = originalImageFile.Size;
                this.pictureBox.Size = originalImageFile.Size;
                //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
                this.pictureBox.Image = imageFile;
                //CacheZoomedImage();
                listCachedZoomedImage.Clear();
                listOfPositionsToFindDistance.Clear();
                listOfPositionsToRemove.Clear();
                setImageToPictureBox();
            }
            else if (d.GetDataPresent(DataFormats.FileDrop))
            {
                string[] allFilePath = (string[])d.GetData(DataFormats.FileDrop);
                copyImageFileFromClipboard(allFilePath);
            }
            else
            {
                MessageBox.Show("Please copy image and then paste.");
            }
        }

        private bool IsAnImage(string filename)
        {
            try
            {
                Image newImage = Image.FromFile(filename);
            }
            catch
            {
                // Image.FromFile will throw this if file is invalid.
                return false;
            }
            return true;

        }
          
        private void ZoomIn()
        {
            if (zoomFactor < maxZoomLevelAllowed)
            {
                zoomFactor = zoomFactor + 1;
                setImageToPictureBox();
            }
        }

        private void ZoomOut()
        {
            if (zoomFactor > 1)
            {
                zoomFactor = zoomFactor - 1;
                setImageToPictureBox();
            }
        }

        private int getRelativePosition(int pos)
        {
            if (zoomFactor > 1)
            {
                return (Convert.ToInt16(Math.Ceiling(Convert.ToDecimal((pos + 1) / zoomFactor) - 1)));
            }
            else
            {
                return (Convert.ToInt16(Math.Ceiling(Convert.ToDecimal((pos + 1) * zoomFactor) - 1)));
            }
        }

        private void LoadZoomedImageInList()
        {
            listCachedZoomedImage = new List<Bitmap>();
            Bitmap copyOfOriginalImage, tempImageToCache,tempOriginalImageFile;
            //copyOfOriginalImage = (Bitmap)this.originalImageFile.Clone();
            copyOfOriginalImage = new Bitmap(this.originalImageFile);
            tempOriginalImageFile = (Bitmap)copyOfOriginalImage.Clone();
            tempImageToCache = (Bitmap)copyOfOriginalImage.Clone();
            Size tempImageSize;
            for (int zoomLevelCount = 1; zoomLevelCount <= maxZoomLevelAllowed; zoomLevelCount++)
            {
                copyOfOriginalImage =  (Bitmap)tempOriginalImageFile.Clone();
                tempImageSize = new Size((int)(copyOfOriginalImage.Width * zoomLevelCount), (int)(copyOfOriginalImage.Height * zoomLevelCount));
                copyOfOriginalImage = new Bitmap(tempOriginalImageFile, tempImageSize);
                //tempImageToCache = new Bitmap(copyOfOriginalImage, tempImageSize);

                using (Graphics gr = Graphics.FromImage(copyOfOriginalImage))
                {
                    gr.Clear(Color.Transparent);
                    gr.CompositingQuality = CompositingQuality.HighQuality;
                    //gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                    //gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.DrawImage(tempImageToCache, 0, 0, tempImageSize.Width, tempImageSize.Height);
                }
                tempImageToCache = copyOfOriginalImage;
                listCachedZoomedImage.Add((Bitmap)copyOfOriginalImage.Clone());
            }
        }

        private void setImageToPictureBox()
        {
            Cursor.Current = Cursors.WaitCursor;
            this.labelToolDescription.Visible = false;
            this.buttonBrowse.Visible = false;


            //while (listCachedZoomedImage != null && (listCachedZoomedImage.Count > (zoomFactor - 1)) == false)
            //{
            //    Thread.Sleep(100);
            //}

            //if (listCachedZoomedImage != null && listCachedZoomedImage.Count == 0)
            //{
            //    listCachedZoomedImage.Add(new Bitmap(this.imageFile));
            //}

            if (listCachedZoomedImage != null && listCachedZoomedImage.Count > (zoomFactor - 1))
            {
                this.imageFile = (Bitmap)(listCachedZoomedImage[(int)(zoomFactor - 1)].Clone());
            }

            else
            {
                newSize = new Size((int)(originalImageFile.Width * zoomFactor), (int)(originalImageFile.Height * zoomFactor));
                this.tempImage = new Bitmap(this.originalImageFile, newSize);
                tempImage1 = new Bitmap(this.originalImageFile, newSize);

                using (Graphics gr = Graphics.FromImage(this.tempImage))
                {
                    gr.Clear(Color.Transparent);
                    gr.CompositingQuality = CompositingQuality.HighQuality;
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                    //gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.DrawImage(originalImageFile, 0, 0, this.tempImage.Width, this.tempImage.Height);
                }
                listCachedZoomedImage.Add((Bitmap)tempImage.Clone());
                this.imageFile = tempImage;
                
            }

            DrawAllDimensions();
            this.pictureBox.Size = this.imageFile.Size;
            this.pictureBox.Image = this.imageFile;
            //this.pictureBox.Refresh();
            Cursor.Current = Cursors.Default;
        }

        private void DrawAllDimensions()
        {
            penWidth = getPenToDrawWidth();
            penHeight = getPenToDrawHeight();
            foreach (DrawDimension objectOfDrawDimension in listOfPositionsToFindDistance)
            {
                if (objectOfDrawDimension.IsDrawWidth == true)
                {
                    drawRectangle(penWidth, (int)(objectOfDrawDimension.PosX * zoomFactor), (int)(objectOfDrawDimension.PosY * zoomFactor), (int)(objectOfDrawDimension.Width * zoomFactor), (int)(objectOfDrawDimension.Height * zoomFactor), true);
                }
                else
                {
                    drawRectangle(penHeight, (int)(objectOfDrawDimension.PosX * zoomFactor), (int)(objectOfDrawDimension.PosY * zoomFactor), (int)(objectOfDrawDimension.Width * zoomFactor), (int)(objectOfDrawDimension.Height * zoomFactor), false);
                }
            }
        }

        private Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            }
            return result;
        }

        private Pen getPenToDrawWidth()
        {
            penWidth = new Pen(penColorToDrawWidth, (float)(thichnessOfRectangleToDraw * zoomFactor));
            penWidth.Alignment = PenAlignment.Inset;
            return penWidth;
        }

        private Pen getPenToDrawHeight()
        {
            penHeight = new Pen(penColorToDrawHeight, (float)(thichnessOfRectangleToDraw * zoomFactor));
            penHeight.Alignment = PenAlignment.Inset;
            return penHeight;
        }

        
        private void copyPictureBoxImageToClipBoard()
        {
            if (this.imageFile != null)
            {
                newSize = new Size((int)(this.originalImageFile.Width), (int)(this.originalImageFile.Height));
                Clipboard.SetImage(new Bitmap(this.imageFile, newSize));
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Open Text File";
                openFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.originalImageFile = new Bitmap(openFileDialog.FileName.ToString());
                    this.imageFile = new Bitmap(this.originalImageFile);
                    this.pictureBox.Image = imageFile;
                    setImageToPictureBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error ocurred\n" + ex.Message);
            }
        }

       
    }

    class DrawDimension
    {
        int posX;
        int posY;
        int width;        
        int height;        

        public int PosX
        {
            get { return posX; }
            set { posX = value; }
        }

        public int PosY
        {
            get { return posY; }
            set { posY = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public bool IsDrawWidth
        {
            get { return isDrawWidth; }
            set { isDrawWidth = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        bool isDrawWidth;
    }

}
