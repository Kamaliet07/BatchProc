using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CryptoUtility
{
    /// <summary>
    /// FormCryptoUtil - utility for MyWay Data Encryption and Integrity.
    /// </summary>
    public partial class FormCryptoUtil : Form
    {
        #region DataMembers

        private OpenFileDialog _ofdFromFolder;
        private OpenFileDialog _ofdHashFileLoc;
        private FolderBrowserDialog _fbdToFolder;
        private ProgressBar _pbProgress;
        
        private string _encryptionOptions = string.Empty;
        private string _toFolderLoc = string.Empty;
        
        //File extensions
        private const string _encExtension = ".enc";
        private const string _hashExtension = ".hash";

        //Button captions        
        private const string _encryptFile = "Encrypt File";
        private const string _decryptFile = "Decrypt File";
        private const string _encryptAndGenerateHash = "Encrypt && Generate Hash Key";
        private const string _generateHashKey = "Generate Hash Key";
        private const string _verifyHashKey = "Verify Hash Key";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FormCryptoUtil"/> class.
        /// </summary>
        public FormCryptoUtil()
        {
            InitializeComponent();

            this._ofdFromFolder = new OpenFileDialog();
            this._ofdHashFileLoc = new OpenFileDialog();
            this._fbdToFolder = new FolderBrowserDialog();
            this._pbProgress = new ProgressBar();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            
            this._btnCancel.CausesValidation = false;
            this._radiobuttonEncryptFile.CausesValidation = false;
            this._radioButtonDecryptFile.CausesValidation = false;
            this._radioButtonEncryptHash.CausesValidation = false;
            this._radioButtonGenHash.CausesValidation = false;
            this._radioButtonVerifyHash.CausesValidation = false;

        }
        #endregion

        #region Events

        /// <summary>
        /// Handles the Load event of the FormCryptoUtil control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void FormCryptoUtil_Load(object sender, EventArgs e)
        {
            _encryptionOptions = ConfigurationManager.AppSettings["CryptoMode"];

            // Only provide Decrypt Option if the CryptoMode is not set to Full. 
            if (!(_encryptionOptions == "Full"))
            {
                _radiobuttonEncryptFile.Visible = false;
                _radioButtonEncryptHash.Visible = false;
                _radioButtonGenHash.Visible = false;
                _radioButtonVerifyHash.Visible = false;
                _lblHashFile.Visible = false;
                _txtHashFile.Visible = false;
                _btnHashFile.Visible = false;
                _radioButtonDecryptFile.Location = new System.Drawing.Point(13, 40);
                _radioButtonDecryptFile.Checked = true;
                _btnOK.Enabled = false;

                //select the first encrypted file from the startup directory. 
                string[] files = Directory.GetFiles(Application.StartupPath);
                foreach (string filename in files)
                {
                    if (_encExtension == Path.GetExtension(filename))
                    {
                        _txtFromFolder.Text = filename;
                        _txtToFolder.Text = Path.GetDirectoryName(filename);
                        break;
                    }

                }
            }
            else
            {
                // Default option is Encrypt File.
                _radiobuttonEncryptFile.Checked = true;
                _btnOK.Enabled = false;
            }
            // Set the Focus to password textbox.
            _txtPassword.Focus();
            
        }

        /// <summary>
        /// Handles the Click event of the _btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                switch (_btnOK.Text)
                {
                    case _encryptFile:
                        string encFile = EncryptFile();
                        break;
                    case _encryptAndGenerateHash:
                        EncryptAndGenerateHash();
                        break;
                    case _generateHashKey:
                        GenerateHashKey(string.Empty);
                        break;
                    case _verifyHashKey:
                        VerifyHash();
                        break;
                    case _decryptFile:
                        DecryptFile();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the _btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _btnCancel_Click(object sender, EventArgs e)
        {
            this._txtPassword.CausesValidation = false;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the _btnFromFolderBrowse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _btnFromFolderBrowse_Click(object sender, EventArgs e)
        {
            _tsStatusInfoLabel.Text = string.Empty;
            if (DialogResult.OK == _ofdFromFolder.ShowDialog())
            {
                _txtFromFolder.Text = _ofdFromFolder.FileName;
                //Prepopulate Output Folder
                _txtToFolder.Text = Path.GetDirectoryName(_txtFromFolder.Text);
                _toFolderLoc = _txtToFolder.Text;
                if ( (!string.IsNullOrEmpty(_txtFromFolder.Text)) && (_radioButtonVerifyHash.Checked ))
                    _txtHashFile.Text = _txtFromFolder.Text + _hashExtension;
                
            }
        }

        /// <summary>
        /// Handles the Click event of the _btnToFolderBrowse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _btnToFolderBrowse_Click(object sender, EventArgs e)
        {
            _tsStatusInfoLabel.Text = string.Empty;
            if (DialogResult.OK == _fbdToFolder.ShowDialog())
            {
                _txtToFolder.Text = _fbdToFolder.SelectedPath;

            }
        }

        /// <summary>
        /// Handles the Click event of the _btnHashFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _btnHashFile_Click(object sender, EventArgs e)
        {
            _tsStatusInfoLabel.Text = string.Empty;
            if (DialogResult.OK == _ofdHashFileLoc.ShowDialog())
            {
                _txtHashFile.Text = _ofdHashFileLoc.FileName;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the _radioButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _radioButton_CheckedChanged(object sender, EventArgs e)
        {
            this._txtPassword.CausesValidation = false;
            
            RadioButton rb = (RadioButton)sender;
            string name = rb.Text;

            // Change the focus depending on which radio button was selected. 
            if ((name.Equals(_encryptFile)) || name.Equals(_decryptFile) || name.Equals(_encryptAndGenerateHash))
            {
                this._txtPassword.Focus();
            }
            else
            {
                if (name.Equals(_generateHashKey) || name.Equals(_verifyHashKey))
                    this._txtFromFolder.Focus();
            }
            
            // change control settings based on what radio button is selected.
            _txtConfirmPassword.ReadOnly = (name.Equals(_encryptFile)) || name.Equals(_decryptFile) || name.Equals(_encryptAndGenerateHash)
                ? false : true;
            _txtPassword.ReadOnly = (name.Equals(_encryptFile)) || name.Equals(_decryptFile) || name.Equals(_encryptAndGenerateHash)
                ? false : true;
            _txtPassword.Text = (name.Equals(_encryptFile)) || name.Equals(_decryptFile) || name.Equals(_encryptAndGenerateHash)
               ? _txtPassword.Text : string.Empty;
            _txtConfirmPassword.Text = (name.Equals(_encryptFile)) || name.Equals(_decryptFile) || name.Equals(_encryptAndGenerateHash)
               ? _txtConfirmPassword.Text : string.Empty;

            _txtToFolder.ReadOnly = (name.Equals(_encryptFile)) || name.Equals(_decryptFile) || name.Equals(_encryptAndGenerateHash)
                || name.Equals(_generateHashKey) ? false : true;
            _btnToFolderBrowse.Enabled = (name.Equals(_encryptFile)) || name.Equals(_decryptFile) || name.Equals(_encryptAndGenerateHash)
                || name.Equals(_generateHashKey) ? true : false;
            _txtToFolder.Text = (name.Equals(_verifyHashKey)) ? string.Empty : _toFolderLoc;

            _txtHashFile.ReadOnly = (name.Equals(_verifyHashKey)) ? false : true;
            
            if (name.Equals(_verifyHashKey))
            {
                if (!string.IsNullOrEmpty(_txtFromFolder.Text))
                    _txtHashFile.Text = _txtFromFolder.Text + _hashExtension;
                else
                {
                    _txtHashFile.Text = string.Empty;
                }
            }
            else
            {
                _txtHashFile.Text = string.Empty;
            }

            _btnHashFile.Enabled = (name.Equals(_verifyHashKey)) ? true : false;

            _lblGenHashKey.Visible = false;
            _txtGenHashKey.Visible = false;

            _lblProvidedHashKey.Visible = false;
            _txtProvidedHashKey.Visible = false;

            _tsStatusInfoLabel.Text = string.Empty;
            _btnOK.Text = name;

            // Set the OK button state
            EnableOkButton();

        }


        /// <summary>
        /// Handles the KeyDown event of the _txtbox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void _txtbox_KeyDown(object sender, KeyEventArgs e)
        {
            _tsStatusInfoLabel.Text = string.Empty;

        }

        /// <summary>
        /// Handles the TextChanged event of the _txtbox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _txtbox_TextChanged(object sender, EventArgs e)
        {
            EnableOkButton();

        }

        /// <summary>
        /// Handles the Validating event of the _txtPassword control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void _txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (!CryptoHelper.IsValidPassword(_txtPassword.Text))
            {
                MessageBox.Show(
                    "Please enter 4-10 digit password, Password should contain at least one digit and one alphabetic character, and must not conatin special characters.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                _txtPassword.Text = string.Empty;
                _txtPassword.Focus();
                return;
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Progresses the call back encrypt.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <param name="value">The value.</param>
        private void ProgressCallBackEncrypt(int min, int max, int value)
        {
            _pbProgress.Minimum = min;
            _pbProgress.Maximum = max;
            _pbProgress.Value = value;
            Application.DoEvents();
        }

        /// <summary>
        /// Progresses the call back decrypt.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <param name="value">The value.</param>
        private void ProgressCallBackDecrypt(int min, int max, int value)
        {
            _pbProgress.Maximum = max;
            _pbProgress.Minimum = min;
            _pbProgress.Value = value;
            Application.DoEvents();
        }

        /// <summary>
        /// Encrypts the file.
        /// </summary>
        private string EncryptFile()
        {
            string caption = "Encryption";
            string encryptFile = string.Empty;
            if (!(string.IsNullOrEmpty(_txtPassword.Text)))
            {
                if (!(_txtPassword.Text == _txtConfirmPassword.Text))
                {
                    MessageBox.Show("Password and ConfirmPassword don't match, please confirm the password again.",
                                    caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtConfirmPassword.Text = string.Empty;
                    _txtConfirmPassword.Focus();
                }
                else
                {
                    if (_txtFromFolder.Text != String.Empty)
                    {
                        if (File.Exists(_txtFromFolder.Text))
                        {
                            string inFile = _txtFromFolder.Text;
                            string outFolder = _txtToFolder.Text;
                            string password = _txtPassword.Text;
                            CryptoProgressCallBack cb = new CryptoProgressCallBack(this.ProgressCallBackEncrypt);

                            _tsStatusInfoLabel.Text = "In Progress ...";
                            encryptFile = CryptoHelper.EncryptFile(inFile, outFolder, password, cb);
                            _pbProgress.Value = 0;
                            _tsStatusInfoLabel.Text = "File Encryption Complete.";
                        }
                        else
                        {
                            MessageBox.Show("File doesn't exist, Please select the file to Encrypt.", caption,
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            _btnFromFolderBrowse.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select file to Encrypt.", caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        _btnFromFolderBrowse.Focus();
                        
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter password and confirm it to Encrypt and Generate Hash.", caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtPassword.Focus();
                
            }
            return encryptFile;

        }

        /// <summary>
        /// Generates the hash.
        /// </summary>
        private void GenerateHashKey(string inFile)
        {
            string outFolder = string.Empty;
            string hashFile = string.Empty;
            string message = "Hash key Generation Complete.";

            if (string.IsNullOrEmpty(inFile))
            {
                hashFile = _txtFromFolder.Text;
                outFolder = _txtToFolder.Text;

            }
            else
            {
                if (Path.GetExtension(inFile) == ".enc")
                {
                    hashFile = inFile;
                    outFolder = Path.GetDirectoryName(inFile);
                    message = "Encryption and Hash key Generation Complete.";
                }
            }

            _tsStatusInfoLabel.Text = "In Progress ...";
            string hashValue = CryptoHelper.GenerateHashFile(hashFile, outFolder);
            _lblGenHashKey.Visible = true;
            _txtGenHashKey.Visible = true;
            _txtGenHashKey.Text = hashValue;
            _tsStatusInfoLabel.Text = message;

        }

        /// <summary>
        /// Verifies the hash.
        /// </summary>
        private void VerifyHash()
        {
            string caption = "Integrity";
            bool compareHash = false;
            string expectedHashValue = string.Empty;
            string hashValue = string.Empty;
            if (!(string.IsNullOrEmpty(_txtFromFolder.Text)))
            {
                if (!(string.IsNullOrEmpty(_txtHashFile.Text)))
                {
                    string inFile = _txtFromFolder.Text;
                    string hashFile = _txtHashFile.Text;

                    _tsStatusInfoLabel.Text = "In Progress ...";

                    //Read the hash file.
                    StreamReader sr = new StreamReader(hashFile);
                    StringBuilder sb = new StringBuilder(sr.ReadToEnd());
                    sr.Close();
                    hashValue = sb.ToString();

                    expectedHashValue = CryptoHelper.GenerateHash(inFile);

                    compareHash = (expectedHashValue == hashValue);

                }
                else
                {
                    MessageBox.Show("Please choose hash file to verify hash values.", caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtHashFile.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please select file to generate hash.", caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtFromFolder.Focus();
                return;
            }

            _lblGenHashKey.Visible = true;
            _txtGenHashKey.Visible = true;
            _txtGenHashKey.Text = expectedHashValue;
            _lblProvidedHashKey.Visible = true;
            _txtProvidedHashKey.Visible = true;
            _txtProvidedHashKey.Text = hashValue;
            _tsStatusInfoLabel.Text = (compareHash) ? "Verify Hash Complete : Hash keys match !" :_tsStatusInfoLabel.Text = "Verify Hash Complete : Possible File Corruption - Hash values do not match.";
                
        }

        /// <summary>
        /// Decrypts the file.
        /// </summary>
        private void DecryptFile()
        {
            string caption = "Decryption";
            if (!(string.IsNullOrEmpty(_txtPassword.Text)))
            {
                if (!(_txtPassword.Text == _txtConfirmPassword.Text))
                {
                    MessageBox.Show("Password and ConfirmPassword don't match, please confirm the password again.",
                                    caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtConfirmPassword.Text = string.Empty;
                    _txtConfirmPassword.Focus();

                }
                else
                {
                    if (_txtFromFolder.Text != String.Empty)
                    {
                        if (File.Exists(_txtFromFolder.Text))
                        {
                            if (_encExtension == Path.GetExtension(_txtFromFolder.Text))
                            {
                                string password = _txtPassword.Text;
                                CryptoProgressCallBack cb = new CryptoProgressCallBack(this.ProgressCallBackDecrypt);

                                _tsStatusInfoLabel.Text = "In Progress...";
                                CryptoHelper.DecryptFile(_txtFromFolder.Text, _txtToFolder.Text, password, cb);
                                _pbProgress.Value = 0;
                                _tsStatusInfoLabel.Text = "File Decryption Complete.";
                            }
                            else
                            {
                                MessageBox.Show("Please select encrypted file with extension enc to decrypt.", caption,
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                _btnToFolderBrowse.Focus();

                            }
                        }
                        else
                        {
                            MessageBox.Show("File doesn't exist, Please select the file to decrypt.", caption,
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            _btnFromFolderBrowse.Focus();

                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select file to decrypt.", caption, MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        _btnFromFolderBrowse.Focus();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter password and confirm it.", caption, MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                _txtPassword.Focus();

            }
        }

        private void EncryptAndGenerateHash()
        {
            string encFile = EncryptFile();
            // only generate hash if we have a encrypted file.
            if(File.Exists(encFile))
                GenerateHashKey(encFile);
        }

        private void EnableOkButton()
        {
            // enable OK button if the required fields for each option are provided.
            if ((_radiobuttonEncryptFile.Checked) || (_radioButtonEncryptHash.Checked) ||
                (_radioButtonDecryptFile.Checked))
            {
                if ((string.IsNullOrEmpty(_txtFromFolder.Text)) || (string.IsNullOrEmpty(_txtPassword.Text)) ||
                    (string.IsNullOrEmpty(_txtConfirmPassword.Text)))
                    _btnOK.Enabled = false;
                else
                {
                    _btnOK.Enabled = true;
                }
            }
            if (_radioButtonVerifyHash.Checked)
            {
                if ((string.IsNullOrEmpty(_txtFromFolder.Text)) || (string.IsNullOrEmpty(_txtHashFile.Text)))
                    _btnOK.Enabled = false;
                else
                {
                    _btnOK.Enabled = true;
                }
            }
            if (_radioButtonGenHash.Checked)
            {
                if (string.IsNullOrEmpty(_txtFromFolder.Text))
                    _btnOK.Enabled = false;
                else
                {
                    _btnOK.Enabled = true;
                }
            }
        }

        #endregion
        
    }
}
