namespace CogaenEditExtension
{
    partial class MyEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.objectBuilderControl1 = new CogaenEditorControls.GUI_Elements.ObjectBuilderControl();
            this.richTextBoxCtrl = new CogaenEditExtension.EditorTextBox();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.AllowDrop = true;
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(500, 500);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.objectBuilderControl1;
            //// 
            //// richTextBoxCtrl
            //// 
            //this.richTextBoxCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            //            | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
            //this.richTextBoxCtrl.FilterMouseClickMessages = false;
            //this.richTextBoxCtrl.Location = new System.Drawing.Point(0, 0);
            //this.richTextBoxCtrl.Name = "richTextBoxCtrl";
            //this.richTextBoxCtrl.Size = new System.Drawing.Size(150, 150);
            //this.richTextBoxCtrl.TabIndex = 0;
            //this.richTextBoxCtrl.Text = "";
            //this.richTextBoxCtrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBoxCtrl_KeyDown);
            //this.richTextBoxCtrl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richTextBoxCtrl_KeyPress);
            //this.richTextBoxCtrl.MouseEnter += new System.EventHandler(this.richTextBoxCtrl_MouseEnter);
            // 
            // MyEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.Controls.Add(this.elementHost1);
            this.Name = "MyEditor";
            this.Size = new System.Drawing.Size(500, 500);
            this.ResumeLayout(false);
        }

        #endregion

        private EditorTextBox richTextBoxCtrl;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private CogaenEditorControls.GUI_Elements.ObjectBuilderControl objectBuilderControl1;


    }
}
