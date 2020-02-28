namespace CodeGen
{
    partial class Form1
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
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.Symbol = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ServiceDefinitionTextBox = new System.Windows.Forms.TextBox();
            this.ServiceConfigurationTextBox = new System.Windows.Forms.TextBox();
            this.ScopeBindingTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.csdefDir = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Apply = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.button6 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Symbol
            // 
            this.Symbol.Location = new System.Drawing.Point(99, 12);
            this.Symbol.Name = "Symbol";
            this.Symbol.Size = new System.Drawing.Size(689, 20);
            this.Symbol.TabIndex = 0;
            this.Symbol.TextChanged += new System.EventHandler(this.Symbol_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Symbol";
            // 
            // ServiceDefinitionTextBox
            // 
            this.ServiceDefinitionTextBox.Location = new System.Drawing.Point(99, 38);
            this.ServiceDefinitionTextBox.Multiline = true;
            this.ServiceDefinitionTextBox.Name = "ServiceDefinitionTextBox";
            this.ServiceDefinitionTextBox.Size = new System.Drawing.Size(688, 45);
            this.ServiceDefinitionTextBox.TabIndex = 2;
            // 
            // ServiceConfigurationTextBox
            // 
            this.ServiceConfigurationTextBox.Location = new System.Drawing.Point(99, 89);
            this.ServiceConfigurationTextBox.Multiline = true;
            this.ServiceConfigurationTextBox.Name = "ServiceConfigurationTextBox";
            this.ServiceConfigurationTextBox.Size = new System.Drawing.Size(688, 47);
            this.ServiceConfigurationTextBox.TabIndex = 3;
            // 
            // ScopeBindingTextBox
            // 
            this.ScopeBindingTextBox.Location = new System.Drawing.Point(99, 142);
            this.ScopeBindingTextBox.Multiline = true;
            this.ScopeBindingTextBox.Name = "ScopeBindingTextBox";
            this.ScopeBindingTextBox.Size = new System.Drawing.Size(688, 87);
            this.ScopeBindingTextBox.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(18, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Copy";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(18, 89);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Copy";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(20, 140);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "Copy";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(99, 235);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(688, 92);
            this.textBox4.TabIndex = 8;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(18, 233);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "Copy";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // csdefDir
            // 
            this.csdefDir.Location = new System.Drawing.Point(159, 376);
            this.csdefDir.Name = "csdefDir";
            this.csdefDir.Size = new System.Drawing.Size(629, 20);
            this.csdefDir.TabIndex = 10;
            this.csdefDir.TextChanged += new System.EventHandler(this.csdefDir_TextChanged);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(20, 376);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 11;
            this.button5.Text = "Browse...";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.OnBrowseDir_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(101, 379);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "csdef Dir:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // Apply
            // 
            this.Apply.Location = new System.Drawing.Point(20, 415);
            this.Apply.Name = "Apply";
            this.Apply.Size = new System.Drawing.Size(75, 23);
            this.Apply.TabIndex = 13;
            this.Apply.Text = "Apply";
            this.Apply.UseVisualStyleBackColor = true;
            this.Apply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 65);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(62, 17);
            this.checkBox1.TabIndex = 14;
            this.checkBox1.Text = "Found1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(12, 119);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(62, 17);
            this.checkBox2.TabIndex = 14;
            this.checkBox2.Text = "Found2";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(12, 169);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(62, 17);
            this.checkBox3.TabIndex = 14;
            this.checkBox3.Text = "Found3";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(99, 334);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 15;
            this.button6.Text = "CheckAll";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.Apply);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.csdefDir);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ScopeBindingTextBox);
            this.Controls.Add(this.ServiceConfigurationTextBox);
            this.Controls.Add(this.ServiceDefinitionTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Symbol);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox Symbol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ServiceDefinitionTextBox;
        private System.Windows.Forms.TextBox ServiceConfigurationTextBox;
        private System.Windows.Forms.TextBox ScopeBindingTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox csdefDir;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Apply;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Button button6;
    }
}

