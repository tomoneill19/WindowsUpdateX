namespace UpdateUI
{
    partial class EmailForm
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
            this.SMTP_HOST_NAME = new System.Windows.Forms.TextBox();
            this.SMTP_PORT = new System.Windows.Forms.TextBox();
            this.FROM_EMAIL = new System.Windows.Forms.TextBox();
            this.FROM_DISPLAY_NAME = new System.Windows.Forms.TextBox();
            this.FROM_PASSWORD = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.DEFAULT_SEND_EMAIL = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SMTP_HOST_NAME
            // 
            this.SMTP_HOST_NAME.Location = new System.Drawing.Point(121, 6);
            this.SMTP_HOST_NAME.Name = "SMTP_HOST_NAME";
            this.SMTP_HOST_NAME.Size = new System.Drawing.Size(267, 20);
            this.SMTP_HOST_NAME.TabIndex = 0;
            // 
            // SMTP_PORT
            // 
            this.SMTP_PORT.Location = new System.Drawing.Point(121, 32);
            this.SMTP_PORT.Name = "SMTP_PORT";
            this.SMTP_PORT.Size = new System.Drawing.Size(267, 20);
            this.SMTP_PORT.TabIndex = 1;
            // 
            // FROM_EMAIL
            // 
            this.FROM_EMAIL.Location = new System.Drawing.Point(121, 58);
            this.FROM_EMAIL.Name = "FROM_EMAIL";
            this.FROM_EMAIL.Size = new System.Drawing.Size(267, 20);
            this.FROM_EMAIL.TabIndex = 2;
            // 
            // FROM_DISPLAY_NAME
            // 
            this.FROM_DISPLAY_NAME.Location = new System.Drawing.Point(121, 84);
            this.FROM_DISPLAY_NAME.Name = "FROM_DISPLAY_NAME";
            this.FROM_DISPLAY_NAME.Size = new System.Drawing.Size(267, 20);
            this.FROM_DISPLAY_NAME.TabIndex = 3;
            // 
            // FROM_PASSWORD
            // 
            this.FROM_PASSWORD.Location = new System.Drawing.Point(121, 110);
            this.FROM_PASSWORD.Name = "FROM_PASSWORD";
            this.FROM_PASSWORD.Size = new System.Drawing.Size(267, 20);
            this.FROM_PASSWORD.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "SMTP Host Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "SMTP Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "From Email Address";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "From Display Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "From Password";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(319, 218);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(69, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 218);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(140, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Validate Email Setup";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Default Send Email";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // DEFAULT_SEND_EMAIL
            // 
            this.DEFAULT_SEND_EMAIL.Location = new System.Drawing.Point(121, 158);
            this.DEFAULT_SEND_EMAIL.Name = "DEFAULT_SEND_EMAIL";
            this.DEFAULT_SEND_EMAIL.Size = new System.Drawing.Size(267, 20);
            this.DEFAULT_SEND_EMAIL.TabIndex = 12;
            this.DEFAULT_SEND_EMAIL.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // EmailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 253);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.DEFAULT_SEND_EMAIL);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FROM_PASSWORD);
            this.Controls.Add(this.FROM_DISPLAY_NAME);
            this.Controls.Add(this.FROM_EMAIL);
            this.Controls.Add(this.SMTP_PORT);
            this.Controls.Add(this.SMTP_HOST_NAME);
            this.Name = "EmailForm";
            this.Text = "EmailForm";
            this.Load += new System.EventHandler(this.EmailForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SMTP_HOST_NAME;
        private System.Windows.Forms.TextBox SMTP_PORT;
        private System.Windows.Forms.TextBox FROM_EMAIL;
        private System.Windows.Forms.TextBox FROM_DISPLAY_NAME;
        private System.Windows.Forms.TextBox FROM_PASSWORD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox DEFAULT_SEND_EMAIL;
    }
}