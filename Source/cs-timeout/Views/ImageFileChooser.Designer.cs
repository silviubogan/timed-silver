namespace cs_timed_silver
{
    partial class ImageFileChooser
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnSetImageFile = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPath = new cs_timed_silver.EnhancedLabel();
            this.btnRecentFilesMenu = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSetImageFile
            // 
            this.btnSetImageFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSetImageFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetImageFile.Location = new System.Drawing.Point(0, 0);
            this.btnSetImageFile.Margin = new System.Windows.Forms.Padding(0);
            this.btnSetImageFile.Name = "btnSetImageFile";
            this.btnSetImageFile.Size = new System.Drawing.Size(100, 63);
            this.btnSetImageFile.TabIndex = 0;
            this.btnSetImageFile.Text = "Se&t time-out background image file...";
            this.btnSetImageFile.UseVisualStyleBackColor = true;
            this.btnSetImageFile.Click += new System.EventHandler(this.btnSetImageFile_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnReset.Location = new System.Drawing.Point(588, 0);
            this.btnReset.Margin = new System.Windows.Forms.Padding(0);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(10, 63);
            this.btnReset.TabIndex = 4;
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnSetImageFile, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPath, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRecentFilesMenu, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnReset, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(598, 63);
            this.tableLayoutPanel1.TabIndex = 12;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // lblPath
            // 
            this.lblPath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPath.Location = new System.Drawing.Point(129, 0);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(440, 63);
            this.lblPath.TabIndex = 2;
            this.lblPath.Text = "No image";
            this.lblPath.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRecentFilesMenu
            // 
            this.btnRecentFilesMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRecentFilesMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecentFilesMenu.Location = new System.Drawing.Point(100, 0);
            this.btnRecentFilesMenu.Margin = new System.Windows.Forms.Padding(0);
            this.btnRecentFilesMenu.Name = "btnRecentFilesMenu";
            this.btnRecentFilesMenu.Size = new System.Drawing.Size(26, 63);
            this.btnRecentFilesMenu.TabIndex = 1;
            this.btnRecentFilesMenu.Text = "▼";
            this.btnRecentFilesMenu.UseVisualStyleBackColor = true;
            this.btnRecentFilesMenu.Click += new System.EventHandler(this.btnRecentFilesMenu_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(575, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(10, 57);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.WaitOnLoad = true;
            // 
            // ImageFileChooser
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ImageFileChooser";
            this.Size = new System.Drawing.Size(600, 65);
            this.Load += new System.EventHandler(this.ImageFileChooser_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSetImageFile;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnRecentFilesMenu;
        private System.Windows.Forms.ToolTip toolTip1;
        internal EnhancedLabel lblPath;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
