namespace CryptoUtility
{
    partial class FormCryptoUtil
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCryptoUtil));
            this._groupBoxDataSecurity = new System.Windows.Forms.GroupBox();
            this._btnHashFile = new System.Windows.Forms.Button();
            this._txtHashFile = new System.Windows.Forms.TextBox();
            this._lblHashFile = new System.Windows.Forms.Label();
            this._txtConfirmPassword = new System.Windows.Forms.TextBox();
            this._txtPassword = new System.Windows.Forms.TextBox();
            this._btnToFolderBrowse = new System.Windows.Forms.Button();
            this._btnFromFolderBrowse = new System.Windows.Forms.Button();
            this._lblConfirmPassword = new System.Windows.Forms.Label();
            this._lblPassword = new System.Windows.Forms.Label();
            this._txtToFolder = new System.Windows.Forms.TextBox();
            this._txtFromFolder = new System.Windows.Forms.TextBox();
            this._lblToFolder = new System.Windows.Forms.Label();
            this._lblFromFolder = new System.Windows.Forms.Label();
            this._btnOK = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._lblGenHashKey = new System.Windows.Forms.Label();
            this._txtGenHashKey = new System.Windows.Forms.TextBox();
            this._groupBoxSecurityOptions = new System.Windows.Forms.GroupBox();
            this._radioButtonGenHash = new System.Windows.Forms.RadioButton();
            this._radiobuttonEncryptFile = new System.Windows.Forms.RadioButton();
            this._radioButtonEncryptHash = new System.Windows.Forms.RadioButton();
            this._radioButtonVerifyHash = new System.Windows.Forms.RadioButton();
            this._radioButtonDecryptFile = new System.Windows.Forms.RadioButton();
            this._lblProvidedHashKey = new System.Windows.Forms.Label();
            this._txtProvidedHashKey = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this._tsStatusInfoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._groupBoxDataSecurity.SuspendLayout();
            this._groupBoxSecurityOptions.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _groupBoxDataSecurity
            // 
            this._groupBoxDataSecurity.Controls.Add(this._btnHashFile);
            this._groupBoxDataSecurity.Controls.Add(this._txtHashFile);
            this._groupBoxDataSecurity.Controls.Add(this._lblHashFile);
            this._groupBoxDataSecurity.Controls.Add(this._txtConfirmPassword);
            this._groupBoxDataSecurity.Controls.Add(this._txtPassword);
            this._groupBoxDataSecurity.Controls.Add(this._btnToFolderBrowse);
            this._groupBoxDataSecurity.Controls.Add(this._btnFromFolderBrowse);
            this._groupBoxDataSecurity.Controls.Add(this._lblConfirmPassword);
            this._groupBoxDataSecurity.Controls.Add(this._lblPassword);
            this._groupBoxDataSecurity.Controls.Add(this._txtToFolder);
            this._groupBoxDataSecurity.Controls.Add(this._txtFromFolder);
            this._groupBoxDataSecurity.Controls.Add(this._lblToFolder);
            this._groupBoxDataSecurity.Controls.Add(this._lblFromFolder);
            this._groupBoxDataSecurity.Location = new System.Drawing.Point(12, 103);
            this._groupBoxDataSecurity.Name = "_groupBoxDataSecurity";
            this._groupBoxDataSecurity.Size = new System.Drawing.Size(560, 223);
            this._groupBoxDataSecurity.TabIndex = 0;
            this._groupBoxDataSecurity.TabStop = false;
            this._groupBoxDataSecurity.Text = "Data Security";
            // 
            // _btnHashFile
            // 
            this._btnHashFile.Location = new System.Drawing.Point(453, 179);
            this._btnHashFile.Name = "_btnHashFile";
            this._btnHashFile.Size = new System.Drawing.Size(75, 23);
            this._btnHashFile.TabIndex = 7;
            this._btnHashFile.Text = "Browse ...";
            this._btnHashFile.UseVisualStyleBackColor = true;
            this._btnHashFile.Click += new System.EventHandler(this._btnHashFile_Click);
            // 
            // _txtHashFile
            // 
            this._txtHashFile.Location = new System.Drawing.Point(122, 179);
            this._txtHashFile.Name = "_txtHashFile";
            this._txtHashFile.Size = new System.Drawing.Size(303, 20);
            this._txtHashFile.TabIndex = 6;
            this._txtHashFile.TextChanged += new System.EventHandler(this._txtbox_TextChanged);
            this._txtHashFile.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtbox_KeyDown);
            // 
            // _lblHashFile
            // 
            this._lblHashFile.AutoSize = true;
            this._lblHashFile.Location = new System.Drawing.Point(10, 185);
            this._lblHashFile.Name = "_lblHashFile";
            this._lblHashFile.Size = new System.Drawing.Size(95, 13);
            this._lblHashFile.TabIndex = 16;
            this._lblHashFile.Text = "Hash File Location";
            // 
            // _txtConfirmPassword
            // 
            this._txtConfirmPassword.Location = new System.Drawing.Point(122, 65);
            this._txtConfirmPassword.Name = "_txtConfirmPassword";
            this._txtConfirmPassword.PasswordChar = '*';
            this._txtConfirmPassword.Size = new System.Drawing.Size(153, 20);
            this._txtConfirmPassword.TabIndex = 1;
            this._txtConfirmPassword.TextChanged += new System.EventHandler(this._txtbox_TextChanged);
            this._txtConfirmPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtbox_KeyDown);
            // 
            // _txtPassword
            // 
            this._txtPassword.Location = new System.Drawing.Point(122, 27);
            this._txtPassword.Name = "_txtPassword";
            this._txtPassword.PasswordChar = '*';
            this._txtPassword.Size = new System.Drawing.Size(153, 20);
            this._txtPassword.TabIndex = 0;
            this._txtPassword.TextChanged += new System.EventHandler(this._txtbox_TextChanged);
            this._txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtbox_KeyDown);
            this._txtPassword.Validating += new System.ComponentModel.CancelEventHandler(this._txtPassword_Validating);
            // 
            // _btnToFolderBrowse
            // 
            this._btnToFolderBrowse.Location = new System.Drawing.Point(453, 141);
            this._btnToFolderBrowse.Name = "_btnToFolderBrowse";
            this._btnToFolderBrowse.Size = new System.Drawing.Size(75, 23);
            this._btnToFolderBrowse.TabIndex = 5;
            this._btnToFolderBrowse.Text = "Browse ...";
            this._btnToFolderBrowse.UseVisualStyleBackColor = true;
            this._btnToFolderBrowse.Click += new System.EventHandler(this._btnToFolderBrowse_Click);
            // 
            // _btnFromFolderBrowse
            // 
            this._btnFromFolderBrowse.Location = new System.Drawing.Point(453, 103);
            this._btnFromFolderBrowse.Name = "_btnFromFolderBrowse";
            this._btnFromFolderBrowse.Size = new System.Drawing.Size(75, 23);
            this._btnFromFolderBrowse.TabIndex = 3;
            this._btnFromFolderBrowse.Text = "Browse ...";
            this._btnFromFolderBrowse.UseVisualStyleBackColor = true;
            this._btnFromFolderBrowse.Click += new System.EventHandler(this._btnFromFolderBrowse_Click);
            // 
            // _lblConfirmPassword
            // 
            this._lblConfirmPassword.AutoSize = true;
            this._lblConfirmPassword.Location = new System.Drawing.Point(10, 66);
            this._lblConfirmPassword.Name = "_lblConfirmPassword";
            this._lblConfirmPassword.Size = new System.Drawing.Size(91, 13);
            this._lblConfirmPassword.TabIndex = 5;
            this._lblConfirmPassword.Text = "Confirm Password";
            // 
            // _lblPassword
            // 
            this._lblPassword.AutoSize = true;
            this._lblPassword.Location = new System.Drawing.Point(10, 27);
            this._lblPassword.Name = "_lblPassword";
            this._lblPassword.Size = new System.Drawing.Size(53, 13);
            this._lblPassword.TabIndex = 4;
            this._lblPassword.Text = "Password";
            // 
            // _txtToFolder
            // 
            this._txtToFolder.Location = new System.Drawing.Point(122, 141);
            this._txtToFolder.Name = "_txtToFolder";
            this._txtToFolder.Size = new System.Drawing.Size(303, 20);
            this._txtToFolder.TabIndex = 4;
            this._txtToFolder.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtbox_KeyDown);
            // 
            // _txtFromFolder
            // 
            this._txtFromFolder.Location = new System.Drawing.Point(122, 103);
            this._txtFromFolder.Name = "_txtFromFolder";
            this._txtFromFolder.Size = new System.Drawing.Size(303, 20);
            this._txtFromFolder.TabIndex = 2;
            this._txtFromFolder.TextChanged += new System.EventHandler(this._txtbox_TextChanged);
            this._txtFromFolder.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtbox_KeyDown);
            // 
            // _lblToFolder
            // 
            this._lblToFolder.AutoSize = true;
            this._lblToFolder.Location = new System.Drawing.Point(10, 144);
            this._lblToFolder.Name = "_lblToFolder";
            this._lblToFolder.Size = new System.Drawing.Size(52, 13);
            this._lblToFolder.TabIndex = 1;
            this._lblToFolder.Text = "To Folder";
            // 
            // _lblFromFolder
            // 
            this._lblFromFolder.AutoSize = true;
            this._lblFromFolder.Location = new System.Drawing.Point(10, 105);
            this._lblFromFolder.Name = "_lblFromFolder";
            this._lblFromFolder.Size = new System.Drawing.Size(62, 13);
            this._lblFromFolder.TabIndex = 0;
            this._lblFromFolder.Text = "From Folder";
            // 
            // _btnOK
            // 
            this._btnOK.Enabled = false;
            this._btnOK.Location = new System.Drawing.Point(279, 417);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(196, 23);
            this._btnOK.TabIndex = 0;
            this._btnOK.Text = "OK";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.Location = new System.Drawing.Point(481, 417);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 1;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _lblGenHashKey
            // 
            this._lblGenHashKey.AutoSize = true;
            this._lblGenHashKey.Location = new System.Drawing.Point(22, 342);
            this._lblGenHashKey.Name = "_lblGenHashKey";
            this._lblGenHashKey.Size = new System.Drawing.Size(106, 13);
            this._lblGenHashKey.TabIndex = 18;
            this._lblGenHashKey.Text = "Generated Hash Key";
            this._lblGenHashKey.Visible = false;
            // 
            // _txtGenHashKey
            // 
            this._txtGenHashKey.Location = new System.Drawing.Point(134, 342);
            this._txtGenHashKey.Name = "_txtGenHashKey";
            this._txtGenHashKey.ReadOnly = true;
            this._txtGenHashKey.Size = new System.Drawing.Size(303, 20);
            this._txtGenHashKey.TabIndex = 17;
            this._txtGenHashKey.Visible = false;
            // 
            // _groupBoxSecurityOptions
            // 
            this._groupBoxSecurityOptions.Controls.Add(this._radioButtonGenHash);
            this._groupBoxSecurityOptions.Controls.Add(this._radiobuttonEncryptFile);
            this._groupBoxSecurityOptions.Controls.Add(this._radioButtonEncryptHash);
            this._groupBoxSecurityOptions.Controls.Add(this._radioButtonVerifyHash);
            this._groupBoxSecurityOptions.Controls.Add(this._radioButtonDecryptFile);
            this._groupBoxSecurityOptions.Location = new System.Drawing.Point(12, 12);
            this._groupBoxSecurityOptions.Name = "_groupBoxSecurityOptions";
            this._groupBoxSecurityOptions.Size = new System.Drawing.Size(560, 85);
            this._groupBoxSecurityOptions.TabIndex = 19;
            this._groupBoxSecurityOptions.TabStop = false;
            this._groupBoxSecurityOptions.Text = "Security Options";
            // 
            // _radioButtonGenHash
            // 
            this._radioButtonGenHash.AutoSize = true;
            this._radioButtonGenHash.Location = new System.Drawing.Point(248, 29);
            this._radioButtonGenHash.Name = "_radioButtonGenHash";
            this._radioButtonGenHash.Size = new System.Drawing.Size(118, 17);
            this._radioButtonGenHash.TabIndex = 1;
            this._radioButtonGenHash.TabStop = true;
            this._radioButtonGenHash.Text = "Generate Hash Key";
            this._radioButtonGenHash.UseVisualStyleBackColor = true;
            this._radioButtonGenHash.CheckedChanged += new System.EventHandler(this._radioButton_CheckedChanged);
            // 
            // _radiobuttonEncryptFile
            // 
            this._radiobuttonEncryptFile.AutoSize = true;
            this._radiobuttonEncryptFile.Location = new System.Drawing.Point(13, 29);
            this._radiobuttonEncryptFile.Name = "_radiobuttonEncryptFile";
            this._radiobuttonEncryptFile.Size = new System.Drawing.Size(80, 17);
            this._radiobuttonEncryptFile.TabIndex = 0;
            this._radiobuttonEncryptFile.TabStop = true;
            this._radiobuttonEncryptFile.Text = "Encrypt File";
            this._radiobuttonEncryptFile.UseVisualStyleBackColor = true;
            this._radiobuttonEncryptFile.CheckedChanged += new System.EventHandler(this._radioButton_CheckedChanged);
            // 
            // _radioButtonEncryptHash
            // 
            this._radioButtonEncryptHash.AutoSize = true;
            this._radioButtonEncryptHash.Location = new System.Drawing.Point(13, 52);
            this._radioButtonEncryptHash.Name = "_radioButtonEncryptHash";
            this._radioButtonEncryptHash.Size = new System.Drawing.Size(166, 17);
            this._radioButtonEncryptHash.TabIndex = 3;
            this._radioButtonEncryptHash.TabStop = true;
            this._radioButtonEncryptHash.Text = "Encrypt && Generate Hash Key";
            this._radioButtonEncryptHash.UseVisualStyleBackColor = true;
            this._radioButtonEncryptHash.CheckedChanged += new System.EventHandler(this._radioButton_CheckedChanged);
            // 
            // _radioButtonVerifyHash
            // 
            this._radioButtonVerifyHash.AutoSize = true;
            this._radioButtonVerifyHash.Location = new System.Drawing.Point(248, 52);
            this._radioButtonVerifyHash.Name = "_radioButtonVerifyHash";
            this._radioButtonVerifyHash.Size = new System.Drawing.Size(100, 17);
            this._radioButtonVerifyHash.TabIndex = 4;
            this._radioButtonVerifyHash.TabStop = true;
            this._radioButtonVerifyHash.Text = "Verify Hash Key";
            this._radioButtonVerifyHash.UseVisualStyleBackColor = true;
            this._radioButtonVerifyHash.CheckedChanged += new System.EventHandler(this._radioButton_CheckedChanged);
            // 
            // _radioButtonDecryptFile
            // 
            this._radioButtonDecryptFile.AutoSize = true;
            this._radioButtonDecryptFile.Location = new System.Drawing.Point(453, 29);
            this._radioButtonDecryptFile.Name = "_radioButtonDecryptFile";
            this._radioButtonDecryptFile.Size = new System.Drawing.Size(81, 17);
            this._radioButtonDecryptFile.TabIndex = 2;
            this._radioButtonDecryptFile.TabStop = true;
            this._radioButtonDecryptFile.Text = "Decrypt File";
            this._radioButtonDecryptFile.UseVisualStyleBackColor = true;
            this._radioButtonDecryptFile.CheckedChanged += new System.EventHandler(this._radioButton_CheckedChanged);
            // 
            // _lblProvidedHashKey
            // 
            this._lblProvidedHashKey.AutoSize = true;
            this._lblProvidedHashKey.Location = new System.Drawing.Point(22, 383);
            this._lblProvidedHashKey.Name = "_lblProvidedHashKey";
            this._lblProvidedHashKey.Size = new System.Drawing.Size(98, 13);
            this._lblProvidedHashKey.TabIndex = 20;
            this._lblProvidedHashKey.Text = "Provided Hash Key";
            this._lblProvidedHashKey.Visible = false;
            // 
            // _txtProvidedHashKey
            // 
            this._txtProvidedHashKey.Location = new System.Drawing.Point(134, 380);
            this._txtProvidedHashKey.Name = "_txtProvidedHashKey";
            this._txtProvidedHashKey.ReadOnly = true;
            this._txtProvidedHashKey.Size = new System.Drawing.Size(303, 20);
            this._txtProvidedHashKey.TabIndex = 21;
            this._txtProvidedHashKey.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsStatusInfoLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 447);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(586, 22);
            this.statusStrip1.TabIndex = 23;
            // 
            // _tsStatusInfoLabel
            // 
            this._tsStatusInfoLabel.Name = "_tsStatusInfoLabel";
            this._tsStatusInfoLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._tsStatusInfoLabel.Size = new System.Drawing.Size(571, 17);
            this._tsStatusInfoLabel.Spring = true;
            this._tsStatusInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormCryptoUtil
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 469);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this._txtProvidedHashKey);
            this.Controls.Add(this._lblProvidedHashKey);
            this.Controls.Add(this._groupBoxSecurityOptions);
            this.Controls.Add(this._lblGenHashKey);
            this.Controls.Add(this._txtGenHashKey);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._groupBoxDataSecurity);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormCryptoUtil";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MyWay Data Encryption and Integrity Tool";
            this.Load += new System.EventHandler(this.FormCryptoUtil_Load);
            this._groupBoxDataSecurity.ResumeLayout(false);
            this._groupBoxDataSecurity.PerformLayout();
            this._groupBoxSecurityOptions.ResumeLayout(false);
            this._groupBoxSecurityOptions.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox _groupBoxDataSecurity;
        private System.Windows.Forms.Label _lblFromFolder;
        private System.Windows.Forms.Label _lblToFolder;
        private System.Windows.Forms.TextBox _txtToFolder;
        private System.Windows.Forms.TextBox _txtFromFolder;
        private System.Windows.Forms.Button _btnToFolderBrowse;
        private System.Windows.Forms.Button _btnFromFolderBrowse;
        private System.Windows.Forms.Label _lblConfirmPassword;
        private System.Windows.Forms.Label _lblPassword;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.TextBox _txtConfirmPassword;
        private System.Windows.Forms.TextBox _txtPassword;
        private System.Windows.Forms.Label _lblGenHashKey;
        private System.Windows.Forms.TextBox _txtGenHashKey;
        private System.Windows.Forms.GroupBox _groupBoxSecurityOptions;
        private System.Windows.Forms.RadioButton _radioButtonGenHash;
        private System.Windows.Forms.RadioButton _radiobuttonEncryptFile;
        private System.Windows.Forms.RadioButton _radioButtonEncryptHash;
        private System.Windows.Forms.RadioButton _radioButtonVerifyHash;
        private System.Windows.Forms.RadioButton _radioButtonDecryptFile;
        private System.Windows.Forms.Button _btnHashFile;
        private System.Windows.Forms.TextBox _txtHashFile;
        private System.Windows.Forms.Label _lblHashFile;
        private System.Windows.Forms.Label _lblProvidedHashKey;
        private System.Windows.Forms.TextBox _txtProvidedHashKey;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel _tsStatusInfoLabel;
    }
}

