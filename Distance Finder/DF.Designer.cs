namespace Distance_Finder
{
    partial class DF
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DF));
            this.panelPictureBoxHolder = new System.Windows.Forms.Panel();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.labelToolDescription = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelPictureBoxHolder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panelPictureBoxHolder
            // 
            this.panelPictureBoxHolder.AllowDrop = true;
            this.panelPictureBoxHolder.AutoSize = true;
            this.panelPictureBoxHolder.Controls.Add(this.buttonBrowse);
            this.panelPictureBoxHolder.Controls.Add(this.labelToolDescription);
            this.panelPictureBoxHolder.Controls.Add(this.pictureBox);
            this.panelPictureBoxHolder.Location = new System.Drawing.Point(0, 0);
            this.panelPictureBoxHolder.Margin = new System.Windows.Forms.Padding(0);
            this.panelPictureBoxHolder.Name = "panelPictureBoxHolder";
            this.panelPictureBoxHolder.Size = new System.Drawing.Size(430, 538);
            this.panelPictureBoxHolder.TabIndex = 0;
            this.panelPictureBoxHolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelPictureBoxHolder_DragDrop);
            this.panelPictureBoxHolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelPictureBoxHolder_DragEnter);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(238, 485);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(100, 40);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "&Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // labelToolDescription
            // 
            this.labelToolDescription.AutoSize = true;
            this.labelToolDescription.BackColor = System.Drawing.Color.Transparent;
            this.labelToolDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelToolDescription.Location = new System.Drawing.Point(0, 371);
            this.labelToolDescription.Name = "labelToolDescription";
            this.labelToolDescription.Size = new System.Drawing.Size(162, 20);
            this.labelToolDescription.TabIndex = 2;
            this.labelToolDescription.Text = "labelToolDescription";
            // 
            // pictureBox
            // 
            this.pictureBox.BackgroundImage = global::Distance_Finder.Properties.Resources.backGroundImage;
            this.pictureBox.ErrorImage = null;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(430, 310);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseLeave += new System.EventHandler(this.pictureBox_MouseLeave);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            // 
            // DF
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(430, 540);
            this.Controls.Add(this.panelPictureBoxHolder);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DF";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "DS";
            this.Load += new System.EventHandler(this.DS_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DS_KeyDown);
            this.panelPictureBoxHolder.ResumeLayout(false);
            this.panelPictureBoxHolder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelPictureBoxHolder;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelToolDescription;
        private System.Windows.Forms.Button buttonBrowse;

    }
}