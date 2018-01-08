namespace PckTool
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxRes = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listViewPck = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonPack = new System.Windows.Forms.Button();
            this.checkStrict = new System.Windows.Forms.CheckBox();
            this.checkCompress = new System.Windows.Forms.CheckBox();
            this.checkEncrypt = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Res directory:";
            // 
            // comboBoxRes
            // 
            this.comboBoxRes.AllowDrop = true;
            this.comboBoxRes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRes.FormattingEnabled = true;
            this.comboBoxRes.Location = new System.Drawing.Point(100, 12);
            this.comboBoxRes.Name = "comboBoxRes";
            this.comboBoxRes.Size = new System.Drawing.Size(376, 23);
            this.comboBoxRes.TabIndex = 1;
            this.comboBoxRes.DragDrop += new System.Windows.Forms.DragEventHandler(this.comboBoxRes_DragDrop);
            this.comboBoxRes.DragEnter += new System.Windows.Forms.DragEventHandler(this.comboBoxRes_DragEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Original PCK file:";
            // 
            // listViewPck
            // 
            this.listViewPck.AllowDrop = true;
            this.listViewPck.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPck.LargeImageList = this.imageList;
            this.listViewPck.Location = new System.Drawing.Point(12, 59);
            this.listViewPck.Name = "listViewPck";
            this.listViewPck.Size = new System.Drawing.Size(240, 94);
            this.listViewPck.TabIndex = 3;
            this.listViewPck.UseCompatibleStateImageBehavior = false;
            this.listViewPck.View = System.Windows.Forms.View.Tile;
            this.listViewPck.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewPck_DragDrop);
            this.listViewPck.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewPck_DragEnter);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonPack
            // 
            this.buttonPack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPack.Location = new System.Drawing.Point(373, 59);
            this.buttonPack.Name = "buttonPack";
            this.buttonPack.Size = new System.Drawing.Size(103, 94);
            this.buttonPack.TabIndex = 6;
            this.buttonPack.Text = "&Pack...";
            this.buttonPack.UseVisualStyleBackColor = true;
            this.buttonPack.Click += new System.EventHandler(this.buttonPack_Click);
            // 
            // checkStrict
            // 
            this.checkStrict.AutoSize = true;
            this.checkStrict.Location = new System.Drawing.Point(118, 40);
            this.checkStrict.Name = "checkStrict";
            this.checkStrict.Size = new System.Drawing.Size(132, 19);
            this.checkStrict.TabIndex = 2;
            this.checkStrict.Text = "Strict replace mode";
            this.checkStrict.UseVisualStyleBackColor = true;
            // 
            // checkCompress
            // 
            this.checkCompress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkCompress.AutoSize = true;
            this.checkCompress.Checked = true;
            this.checkCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkCompress.Location = new System.Drawing.Point(258, 62);
            this.checkCompress.Name = "checkCompress";
            this.checkCompress.Size = new System.Drawing.Size(109, 19);
            this.checkCompress.TabIndex = 4;
            this.checkCompress.Text = "Keep Compress";
            this.checkCompress.UseVisualStyleBackColor = true;
            // 
            // checkEncrypt
            // 
            this.checkEncrypt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkEncrypt.AutoSize = true;
            this.checkEncrypt.Checked = true;
            this.checkEncrypt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkEncrypt.Location = new System.Drawing.Point(258, 84);
            this.checkEncrypt.Name = "checkEncrypt";
            this.checkEncrypt.Size = new System.Drawing.Size(96, 19);
            this.checkEncrypt.TabIndex = 5;
            this.checkEncrypt.Text = "Keep Encrypt";
            this.checkEncrypt.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(488, 165);
            this.Controls.Add(this.checkEncrypt);
            this.Controls.Add(this.checkCompress);
            this.Controls.Add(this.checkStrict);
            this.Controls.Add(this.buttonPack);
            this.Controls.Add(this.listViewPck);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxRes);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(364, 180);
            this.Name = "FormMain";
            this.Text = "Repack the PCK file...";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxRes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView listViewPck;
        private System.Windows.Forms.Button buttonPack;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.CheckBox checkStrict;
        private System.Windows.Forms.CheckBox checkCompress;
        private System.Windows.Forms.CheckBox checkEncrypt;
    }
}

