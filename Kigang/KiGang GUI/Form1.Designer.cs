namespace KiGang_GUI
{
    partial class therapeut
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.aufnahme = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.speichern = new System.Windows.Forms.Button();
            this.wiedergabe = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // aufnahme
            // 
            this.aufnahme.Location = new System.Drawing.Point(31, 452);
            this.aufnahme.Name = "aufnahme";
            this.aufnahme.Size = new System.Drawing.Size(75, 23);
            this.aufnahme.TabIndex = 0;
            this.aufnahme.Text = "Aufnahme";
            this.aufnahme.UseVisualStyleBackColor = true;
            // 
            // stop
            // 
            this.stop.Location = new System.Drawing.Point(141, 452);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(75, 23);
            this.stop.TabIndex = 1;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            // 
            // speichern
            // 
            this.speichern.Location = new System.Drawing.Point(239, 431);
            this.speichern.Name = "speichern";
            this.speichern.Size = new System.Drawing.Size(75, 23);
            this.speichern.TabIndex = 2;
            this.speichern.Text = "Speichern";
            this.speichern.UseVisualStyleBackColor = true;
            // 
            // wiedergabe
            // 
            this.wiedergabe.Location = new System.Drawing.Point(239, 477);
            this.wiedergabe.Name = "wiedergabe";
            this.wiedergabe.Size = new System.Drawing.Size(75, 23);
            this.wiedergabe.TabIndex = 3;
            this.wiedergabe.Text = "Wiedergabe";
            this.wiedergabe.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(336, 452);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 4;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(768, 119);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 5;
            // 
            // therapeut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 512);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.wiedergabe);
            this.Controls.Add(this.speichern);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.aufnahme);
            this.HelpButton = true;
            this.Name = "therapeut";
            this.Text = "Therapeut";
            this.Load += new System.EventHandler(this.therapeut_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button aufnahme;

        // Abfrage ob Aufnahme nicht gespeichert werden soll

        private System.Windows.Forms.Button stop;

        // goto frame 1 von der alktuellen Aufnahme
        //
        private System.Windows.Forms.Button speichern;

        //Sicherheitsabfrage : Name der Übung -> Textfeld zur Namensgebung und ablegen
        private System.Windows.Forms.Button wiedergabe;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox1;
    }
}

