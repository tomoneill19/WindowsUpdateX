namespace UpdateUI
{
    partial class Alerts
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
            this.RECEIVE_ALERT_ERRORS = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.RECEIVE_ALERT_FOR_WARNING = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RECEIVE_ALERT_ERRORS
            // 
            this.RECEIVE_ALERT_ERRORS.AutoSize = true;
            this.RECEIVE_ALERT_ERRORS.Location = new System.Drawing.Point(241, 43);
            this.RECEIVE_ALERT_ERRORS.Name = "RECEIVE_ALERT_ERRORS";
            this.RECEIVE_ALERT_ERRORS.Size = new System.Drawing.Size(15, 14);
            this.RECEIVE_ALERT_ERRORS.TabIndex = 0;
            this.RECEIVE_ALERT_ERRORS.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Receive email alerts for errors";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Receive email alerts for warnings";
            // 
            // RECEIVE_ALERT_FOR_WARNING
            // 
            this.RECEIVE_ALERT_FOR_WARNING.AutoSize = true;
            this.RECEIVE_ALERT_FOR_WARNING.Location = new System.Drawing.Point(241, 72);
            this.RECEIVE_ALERT_FOR_WARNING.Name = "RECEIVE_ALERT_FOR_WARNING";
            this.RECEIVE_ALERT_FOR_WARNING.Size = new System.Drawing.Size(15, 14);
            this.RECEIVE_ALERT_FOR_WARNING.TabIndex = 2;
            this.RECEIVE_ALERT_FOR_WARNING.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(197, 226);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Alerts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RECEIVE_ALERT_FOR_WARNING);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RECEIVE_ALERT_ERRORS);
            this.Name = "Alerts";
            this.Text = "Alerts";
            this.Load += new System.EventHandler(this.Alerts_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox RECEIVE_ALERT_ERRORS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox RECEIVE_ALERT_FOR_WARNING;
        private System.Windows.Forms.Button button1;
    }
}