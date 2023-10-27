namespace BianryParser
{
    partial class MainFrom
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ReadBtn = new Button();
            SaveBtn = new Button();
            CreateBtn = new Button();
            ProtoTypes = new ComboBox();
            FileTtile = new Label();
            FilePath = new Label();
            DataPanel = new Panel();
            SuspendLayout();
            // 
            // ReadBtn
            // 
            ReadBtn.Location = new Point(356, 6);
            ReadBtn.Name = "ReadBtn";
            ReadBtn.Size = new Size(75, 23);
            ReadBtn.TabIndex = 0;
            ReadBtn.Text = "读取文件";
            ReadBtn.UseVisualStyleBackColor = true;
            ReadBtn.Click += ReadBtn_Click;
            // 
            // SaveBtn
            // 
            SaveBtn.Location = new Point(437, 6);
            SaveBtn.Name = "SaveBtn";
            SaveBtn.Size = new Size(75, 23);
            SaveBtn.TabIndex = 1;
            SaveBtn.Text = "保存文件";
            SaveBtn.UseVisualStyleBackColor = true;
            SaveBtn.Click += SaveBtn_Click;
            // 
            // CreateBtn
            // 
            CreateBtn.Location = new Point(518, 6);
            CreateBtn.Name = "CreateBtn";
            CreateBtn.Size = new Size(75, 23);
            CreateBtn.TabIndex = 2;
            CreateBtn.Text = "创建文件";
            CreateBtn.UseVisualStyleBackColor = true;
            CreateBtn.Click += CreateBtn_Click;
            // 
            // ProtoTypes
            // 
            ProtoTypes.DropDownStyle = ComboBoxStyle.DropDownList;
            ProtoTypes.FormattingEnabled = true;
            ProtoTypes.Location = new Point(12, 6);
            ProtoTypes.Name = "ProtoTypes";
            ProtoTypes.Size = new Size(227, 25);
            ProtoTypes.TabIndex = 3;
            ProtoTypes.SelectedIndexChanged += ProtoType_Changed;
            // 
            // FileTtile
            // 
            FileTtile.AutoSize = true;
            FileTtile.Location = new Point(12, 39);
            FileTtile.Name = "FileTtile";
            FileTtile.Size = new Size(59, 17);
            FileTtile.TabIndex = 4;
            FileTtile.Text = "当前文件:";
            // 
            // FilePath
            // 
            FilePath.AutoSize = true;
            FilePath.ForeColor = SystemColors.ActiveBorder;
            FilePath.Location = new Point(77, 39);
            FilePath.Name = "FilePath";
            FilePath.Size = new Size(0, 17);
            FilePath.TabIndex = 5;
            // 
            // DataPanel
            // 
            DataPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            DataPanel.Location = new Point(12, 75);
            DataPanel.Name = "DataPanel";
            DataPanel.Size = new Size(564, 351);
            DataPanel.TabIndex = 6;
            // 
            // MainFrom
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(613, 450);
            Controls.Add(ReadBtn);
            Controls.Add(FilePath);
            Controls.Add(FileTtile);
            Controls.Add(ProtoTypes);
            Controls.Add(CreateBtn);
            Controls.Add(SaveBtn);
            Controls.Add(DataPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "MainFrom";
            Text = "Protobuffer读取器";
            Load += MainFrom_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ReadBtn;
        private Button SaveBtn;
        private Button CreateBtn;
        private ComboBox ProtoTypes;
        private Label FileTtile;
        private Label FilePath;
        private Panel DataPanel;
    }
}