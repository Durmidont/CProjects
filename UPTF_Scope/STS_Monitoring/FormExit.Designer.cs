namespace UPTF_Scope
{
    partial class FormExit
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
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonTurnOff = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonHide = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(14, 91);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(199, 66);
            this.buttonExit.TabIndex = 1;
            this.buttonExit.Text = "Закрыть приложение";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonTurnOff
            // 
            this.buttonTurnOff.Location = new System.Drawing.Point(14, 169);
            this.buttonTurnOff.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonTurnOff.Name = "buttonTurnOff";
            this.buttonTurnOff.Size = new System.Drawing.Size(199, 66);
            this.buttonTurnOff.TabIndex = 2;
            this.buttonTurnOff.Text = "Выключть панель";
            this.buttonTurnOff.UseVisualStyleBackColor = true;
            this.buttonTurnOff.Click += new System.EventHandler(this.buttonTurnOff_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(14, 247);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(199, 66);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Отмена";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonHide
            // 
            this.buttonHide.Location = new System.Drawing.Point(16, 13);
            this.buttonHide.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonHide.Name = "buttonHide";
            this.buttonHide.Size = new System.Drawing.Size(199, 66);
            this.buttonHide.TabIndex = 3;
            this.buttonHide.Text = "Свернуть приложение";
            this.buttonHide.UseVisualStyleBackColor = true;
            this.buttonHide.Click += new System.EventHandler(this.buttonHide_Click);
            // 
            // FormExit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(229, 328);
            this.ControlBox = false;
            this.Controls.Add(this.buttonHide);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonTurnOff);
            this.Controls.Add(this.buttonExit);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "FormExit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonTurnOff;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonHide;
    }
}