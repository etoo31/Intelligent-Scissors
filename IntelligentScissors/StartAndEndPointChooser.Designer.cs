namespace IntelligentScissors
{
    partial class StartAndEndPointChooser
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
            this.StartX = new System.Windows.Forms.TextBox();
            this.StartY = new System.Windows.Forms.TextBox();
            this.EndX = new System.Windows.Forms.TextBox();
            this.EndY = new System.Windows.Forms.TextBox();
            this.StartLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.StartXlabel = new System.Windows.Forms.Label();
            this.StartYLabel = new System.Windows.Forms.Label();
            this.EndXlabel = new System.Windows.Forms.Label();
            this.EndYlabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartX
            // 
            this.StartX.Location = new System.Drawing.Point(168, 120);
            this.StartX.Name = "StartX";
            this.StartX.Size = new System.Drawing.Size(125, 27);
            this.StartX.TabIndex = 0;
            this.StartX.TextChanged += new System.EventHandler(this.StartX_TextChanged);
            this.StartX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StartX_KeyPress);
            // 
            // StartY
            // 
            this.StartY.Location = new System.Drawing.Point(168, 201);
            this.StartY.Name = "StartY";
            this.StartY.Size = new System.Drawing.Size(125, 27);
            this.StartY.TabIndex = 1;
            this.StartY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StartY_KeyPress);
            // 
            // EndX
            // 
            this.EndX.Location = new System.Drawing.Point(480, 120);
            this.EndX.Name = "EndX";
            this.EndX.Size = new System.Drawing.Size(125, 27);
            this.EndX.TabIndex = 2;
            this.EndX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EndX_KeyPress);
            // 
            // EndY
            // 
            this.EndY.Location = new System.Drawing.Point(480, 201);
            this.EndY.Name = "EndY";
            this.EndY.Size = new System.Drawing.Size(125, 27);
            this.EndY.TabIndex = 3;
            this.EndY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EndY_KeyPress);
            // 
            // StartLabel
            // 
            this.StartLabel.AutoSize = true;
            this.StartLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.StartLabel.Location = new System.Drawing.Point(202, 88);
            this.StartLabel.Name = "StartLabel";
            this.StartLabel.Size = new System.Drawing.Size(43, 20);
            this.StartLabel.TabIndex = 4;
            this.StartLabel.Text = "Start";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(527, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "End";
            // 
            // StartXlabel
            // 
            this.StartXlabel.AutoSize = true;
            this.StartXlabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.StartXlabel.Location = new System.Drawing.Point(126, 123);
            this.StartXlabel.Name = "StartXlabel";
            this.StartXlabel.Size = new System.Drawing.Size(27, 20);
            this.StartXlabel.TabIndex = 6;
            this.StartXlabel.Text = "X :";
            // 
            // StartYLabel
            // 
            this.StartYLabel.AutoSize = true;
            this.StartYLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.StartYLabel.Location = new System.Drawing.Point(126, 204);
            this.StartYLabel.Name = "StartYLabel";
            this.StartYLabel.Size = new System.Drawing.Size(26, 20);
            this.StartYLabel.TabIndex = 7;
            this.StartYLabel.Text = "Y :";
            // 
            // EndXlabel
            // 
            this.EndXlabel.AutoSize = true;
            this.EndXlabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.EndXlabel.Location = new System.Drawing.Point(445, 123);
            this.EndXlabel.Name = "EndXlabel";
            this.EndXlabel.Size = new System.Drawing.Size(31, 20);
            this.EndXlabel.TabIndex = 8;
            this.EndXlabel.Text = "X : ";
            // 
            // EndYlabel
            // 
            this.EndYlabel.AutoSize = true;
            this.EndYlabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.EndYlabel.Location = new System.Drawing.Point(445, 204);
            this.EndYlabel.Name = "EndYlabel";
            this.EndYlabel.Size = new System.Drawing.Size(26, 20);
            this.EndYlabel.TabIndex = 9;
            this.EndYlabel.Text = "Y :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(320, 270);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 10;
            this.button1.Text = "Submit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // StartAndEndPointChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 327);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.EndYlabel);
            this.Controls.Add(this.EndXlabel);
            this.Controls.Add(this.StartYLabel);
            this.Controls.Add(this.StartXlabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StartLabel);
            this.Controls.Add(this.EndY);
            this.Controls.Add(this.EndX);
            this.Controls.Add(this.StartY);
            this.Controls.Add(this.StartX);
            this.Name = "StartAndEndPointChooser";
            this.Text = "StartAndEndPointChooser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StartAndEndPointChooser_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox StartX;
        private TextBox StartY;
        private TextBox EndX;
        private TextBox EndY;
        private Label StartLabel;
        private Label label2;
        private Label StartXlabel;
        private Label StartYLabel;
        private Label EndXlabel;
        private Label EndYlabel;
        private Button button1;
    }
}