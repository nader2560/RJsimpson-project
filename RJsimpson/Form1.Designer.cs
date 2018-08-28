using Istrib.Sound;

namespace RJsimpson
{
    partial class AnyAppBroadcaster
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
            this.SoftwareName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanelWaveForm = new System.Windows.Forms.FlowLayoutPanel();
            this.mp3SoundCapture = new Istrib.Sound.Mp3SoundCapture(this.components);
            this.mp3MicCapture = new Istrib.Sound.Mp3SoundCapture(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.StopRecPictureBox = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.PlayingIcon = new System.Windows.Forms.PictureBox();
            this.Settings = new System.Windows.Forms.PictureBox();
            this.Play = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.audioLevelsUIControl1 = new RJsimpson.AudioLevelsUIControl();
            this.BroadCastingLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanelWaveForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StopRecPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayingIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Settings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Play)).BeginInit();
            this.SuspendLayout();
            // 
            // SoftwareName
            // 
            this.SoftwareName.AutoSize = true;
            this.SoftwareName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SoftwareName.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.SoftwareName.Location = new System.Drawing.Point(328, 200);
            this.SoftwareName.Name = "SoftwareName";
            this.SoftwareName.Size = new System.Drawing.Size(129, 20);
            this.SoftwareName.TabIndex = 3;
            this.SoftwareName.Text = "Choose an app...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(128, 255);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Play";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(110, 445);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Settings";
            // 
            // flowLayoutPanelWaveForm
            // 
            this.flowLayoutPanelWaveForm.Controls.Add(this.audioLevelsUIControl1);
            this.flowLayoutPanelWaveForm.Location = new System.Drawing.Point(332, 243);
            this.flowLayoutPanelWaveForm.Name = "flowLayoutPanelWaveForm";
            this.flowLayoutPanelWaveForm.Size = new System.Drawing.Size(259, 222);
            this.flowLayoutPanelWaveForm.TabIndex = 8;
            // 
            // mp3SoundCapture
            // 
            this.mp3SoundCapture.NormalizeVolume = false;
            this.mp3SoundCapture.OutputType = Istrib.Sound.Mp3SoundCapture.Outputs.Wav;
            this.mp3SoundCapture.UseSynchronizationContext = true;
            this.mp3SoundCapture.WaitOnStop = true;
            // 
            // mp3MicCapture
            // 
            this.mp3MicCapture.NormalizeVolume = false;
            this.mp3MicCapture.OutputType = Istrib.Sound.Mp3SoundCapture.Outputs.Wav;
            this.mp3MicCapture.UseSynchronizationContext = true;
            this.mp3MicCapture.WaitOnStop = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(244, 485);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Made From Tunisia with ♥ by eDonec";
            // 
            // StopRecPictureBox
            // 
            this.StopRecPictureBox.Image = global::RJsimpson.Properties.Resources.stop;
            this.StopRecPictureBox.Location = new System.Drawing.Point(76, 112);
            this.StopRecPictureBox.Name = "StopRecPictureBox";
            this.StopRecPictureBox.Size = new System.Drawing.Size(141, 130);
            this.StopRecPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.StopRecPictureBox.TabIndex = 11;
            this.StopRecPictureBox.TabStop = false;
            this.StopRecPictureBox.Click += new System.EventHandler(this.StopRecPictureBox_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::RJsimpson.Properties.Resources._002_delete;
            this.pictureBox2.Location = new System.Drawing.Point(547, 13);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(44, 39);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.Reduire_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RJsimpson.Properties.Resources._001_remove_button;
            this.pictureBox1.Location = new System.Drawing.Point(606, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(44, 39);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.Fermer_Click);
            // 
            // PlayingIcon
            // 
            this.PlayingIcon.Location = new System.Drawing.Point(332, 112);
            this.PlayingIcon.Name = "PlayingIcon";
            this.PlayingIcon.Size = new System.Drawing.Size(97, 85);
            this.PlayingIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PlayingIcon.TabIndex = 2;
            this.PlayingIcon.TabStop = false;
            // 
            // Settings
            // 
            this.Settings.Image = global::RJsimpson.Properties.Resources.settings1;
            this.Settings.Location = new System.Drawing.Point(76, 303);
            this.Settings.Name = "Settings";
            this.Settings.Size = new System.Drawing.Size(141, 130);
            this.Settings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Settings.TabIndex = 1;
            this.Settings.TabStop = false;
            this.Settings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // Play
            // 
            this.Play.Image = global::RJsimpson.Properties.Resources.play1;
            this.Play.Location = new System.Drawing.Point(76, 112);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(141, 130);
            this.Play.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Play.TabIndex = 0;
            this.Play.TabStop = false;
            this.Play.Click += new System.EventHandler(this.Play_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Brown;
            this.label4.Location = new System.Drawing.Point(358, 485);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "♥";
            // 
            // audioLevelsUIControl1
            // 
            this.audioLevelsUIControl1.AudioMonitor = null;
            this.audioLevelsUIControl1.Location = new System.Drawing.Point(3, 3);
            this.audioLevelsUIControl1.Name = "audioLevelsUIControl1";
            this.audioLevelsUIControl1.Size = new System.Drawing.Size(256, 219);
            this.audioLevelsUIControl1.TabIndex = 0;
            this.audioLevelsUIControl1.Text = "audioLevelsUIControl1";
            // 
            // BroadCastingLabel
            // 
            this.BroadCastingLabel.AutoSize = true;
            this.BroadCastingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BroadCastingLabel.ForeColor = System.Drawing.Color.Red;
            this.BroadCastingLabel.Location = new System.Drawing.Point(96, 255);
            this.BroadCastingLabel.Name = "BroadCastingLabel";
            this.BroadCastingLabel.Size = new System.Drawing.Size(103, 20);
            this.BroadCastingLabel.TabIndex = 13;
            this.BroadCastingLabel.Text = "Boradcasting";
            // 
            // AnyAppBroadcaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(662, 507);
            this.Controls.Add(this.BroadCastingLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.StopRecPictureBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.flowLayoutPanelWaveForm);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SoftwareName);
            this.Controls.Add(this.PlayingIcon);
            this.Controls.Add(this.Settings);
            this.Controls.Add(this.Play);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AnyAppBroadcaster";
            this.Text = "AnyAppStreamer";
            this.flowLayoutPanelWaveForm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.StopRecPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayingIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Settings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Play)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Play;
        private System.Windows.Forms.PictureBox Settings;
        private System.Windows.Forms.PictureBox PlayingIcon;
        private System.Windows.Forms.Label SoftwareName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelWaveForm;
        private AudioLevelsUIControl audioLevelsUIControl1 = new AudioLevelsUIControl();
        private  Mp3SoundCapture mp3SoundCapture;
        private  Mp3SoundCapture mp3MicCapture;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox StopRecPictureBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label BroadCastingLabel;
    }
}

