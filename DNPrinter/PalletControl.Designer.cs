namespace DNPrinter
{
    partial class PalletControl
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.labelPalletId = new System.Windows.Forms.Label();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Module = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Serial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DN = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Delete,
            this.Module,
            this.Serial,
            this.CustomerId,
            this.DN});
            this.dataGridView1.Location = new System.Drawing.Point(5, 39);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(690, 209);
            this.dataGridView1.TabIndex = 1;
            // 
            // labelPalletId
            // 
            this.labelPalletId.AutoSize = true;
            this.labelPalletId.Location = new System.Drawing.Point(3, 11);
            this.labelPalletId.Name = "labelPalletId";
            this.labelPalletId.Size = new System.Drawing.Size(43, 13);
            this.labelPalletId.TabIndex = 2;
            this.labelPalletId.Text = "Pallet #";
            // 
            // Delete
            // 
            this.Delete.FillWeight = 63.45178F;
            this.Delete.HeaderText = "";
            this.Delete.Name = "Delete";
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Delete.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Module
            // 
            this.Module.FillWeight = 109.1371F;
            this.Module.HeaderText = "Module";
            this.Module.Name = "Module";
            this.Module.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Module.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Module.ToolTipText = "The internal Satys module number.";
            // 
            // Serial
            // 
            this.Serial.FillWeight = 109.1371F;
            this.Serial.HeaderText = "Serial";
            this.Serial.Name = "Serial";
            this.Serial.ToolTipText = "The internal Satys serial number assigned to this particular instance of a module" +
    ".";
            // 
            // CustomerId
            // 
            this.CustomerId.FillWeight = 109.1371F;
            this.CustomerId.HeaderText = "Customer Id";
            this.CustomerId.Name = "CustomerId";
            this.CustomerId.ReadOnly = true;
            this.CustomerId.ToolTipText = "The external custom Id for this module.";
            // 
            // DN
            // 
            this.DN.FillWeight = 109.1371F;
            this.DN.HeaderText = "DN";
            this.DN.Name = "DN";
            this.DN.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.DN.ToolTipText = "A delivery note that has been previous generated and assigned to this shipout. Th" +
    "e list displayed will consist of all un-used, valid delivery notes for the given" +
    " module.";
            // 
            // PalletControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.labelPalletId);
            this.Controls.Add(this.dataGridView1);
            this.Name = "PalletControl";
            this.Size = new System.Drawing.Size(705, 271);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label labelPalletId;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
        private System.Windows.Forms.DataGridViewComboBoxColumn Module;
        private System.Windows.Forms.DataGridViewTextBoxColumn Serial;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerId;
        private System.Windows.Forms.DataGridViewComboBoxColumn DN;
    }
}
