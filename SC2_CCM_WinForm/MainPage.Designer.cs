using Microsoft.VisualBasic;
using SC2_CCM_Common;
using System.Net;

namespace SC2_CCM_WinForm
{
    partial class MainPage
    {

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPage));
            this.importButton = new System.Windows.Forms.Button();
            this.aboutButton = new System.Windows.Forms.Button();
            this.messagesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.messageLabel = new System.Windows.Forms.Label();
            this.messagesLabel = new System.Windows.Forms.Label();
            this.installLocationButton = new System.Windows.Forms.Button();
            this.wolLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.wolTitle = new System.Windows.Forms.Label();
            this.wolEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.wolCampaignLabel = new System.Windows.Forms.Label();
            this.wolCampaignSelect = new System.Windows.Forms.ComboBox();
            this.wolDetailsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.wolAuthorDescLabel = new System.Windows.Forms.Label();
            this.wolModVersionLabel = new System.Windows.Forms.Label();
            this.wolVersion = new System.Windows.Forms.Label();
            this.wolAuthorLabel = new System.Windows.Forms.Label();
            this.wolDescDescriptionLabel = new System.Windows.Forms.Label();
            this.wolDescriptionLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.wolDescription = new System.Windows.Forms.Label();
            this.campaignLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.hosLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.hosTitle = new System.Windows.Forms.Label();
            this.hosEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.hosCampaignLabel = new System.Windows.Forms.Label();
            this.hosCampaignSelect = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.hosAuthorDescLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.hosAuthorLabel = new System.Windows.Forms.Label();
            this.hosVersionLabel = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.hosDescriptionLabel = new System.Windows.Forms.Label();
            this.lotvLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.lotvTitle = new System.Windows.Forms.Label();
            this.lotvEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lotvCampaignSelect = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lotvAuthorLabel = new System.Windows.Forms.Label();
            this.lotvVersionLabel = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lotvDescriptionLabel = new System.Windows.Forms.Label();
            this.ncoLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.ncoTitle = new System.Windows.Forms.Label();
            this.ncoEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ncoCampaignSelect = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.ncoAuthorLabel = new System.Windows.Forms.Label();
            this.ncoVersionLabel = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.ncoDescriptionLabel = new System.Windows.Forms.Label();
            this.messagesPanel.SuspendLayout();
            this.wolLayout.SuspendLayout();
            this.wolDetailsLayout.SuspendLayout();
            this.wolDescriptionLayout.SuspendLayout();
            this.campaignLayoutPanel.SuspendLayout();
            this.hosLayout.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.lotvLayout.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.ncoLayout.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // importButton
            // 
            this.importButton.Enabled = false;
            this.importButton.Location = new System.Drawing.Point(8, 10);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(196, 30);
            this.importButton.TabIndex = 0;
            this.importButton.Text = "Import Custom Campaign";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point(221, 10);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(64, 30);
            this.aboutButton.TabIndex = 1;
            this.aboutButton.Text = "About";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // messagesPanel
            // 
            this.messagesPanel.AutoScroll = true;
            this.messagesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.messagesPanel.Controls.Add(this.messageLabel);
            this.messagesPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.messagesPanel.Location = new System.Drawing.Point(12, 512);
            this.messagesPanel.Name = "messagesPanel";
            this.messagesPanel.Size = new System.Drawing.Size(921, 131);
            this.messagesPanel.TabIndex = 3;
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new System.Drawing.Point(3, 0);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(0, 15);
            this.messageLabel.TabIndex = 0;
            // 
            // messagesLabel
            // 
            this.messagesLabel.AutoSize = true;
            this.messagesLabel.Location = new System.Drawing.Point(12, 494);
            this.messagesLabel.Name = "messagesLabel";
            this.messagesLabel.Size = new System.Drawing.Size(58, 15);
            this.messagesLabel.TabIndex = 4;
            this.messagesLabel.Text = "Messages";
            // 
            // installLocationButton
            // 
            this.installLocationButton.Location = new System.Drawing.Point(304, 10);
            this.installLocationButton.Name = "installLocationButton";
            this.installLocationButton.Size = new System.Drawing.Size(206, 30);
            this.installLocationButton.TabIndex = 5;
            this.installLocationButton.Text = "Set StarCraft II Install Location";
            this.installLocationButton.UseVisualStyleBackColor = true;
            this.installLocationButton.Visible = false;
            this.installLocationButton.Click += new System.EventHandler(this.installLocationButton_Click);
            // 
            // wolLayout
            // 
            this.wolLayout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.wolLayout.Controls.Add(this.wolTitle);
            this.wolLayout.Controls.Add(this.wolEnabledCheckbox);
            this.wolLayout.Controls.Add(this.wolCampaignLabel);
            this.wolLayout.Controls.Add(this.wolCampaignSelect);
            this.wolLayout.Controls.Add(this.wolDetailsLayout);
            this.wolLayout.Controls.Add(this.wolDescDescriptionLabel);
            this.wolLayout.Controls.Add(this.wolDescriptionLayout);
            this.wolLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.wolLayout.Location = new System.Drawing.Point(3, 3);
            this.wolLayout.Name = "wolLayout";
            this.wolLayout.Size = new System.Drawing.Size(200, 392);
            this.wolLayout.TabIndex = 0;
            // 
            // wolTitle
            // 
            this.wolTitle.AutoSize = true;
            this.wolTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.wolTitle.Location = new System.Drawing.Point(3, 0);
            this.wolTitle.Name = "wolTitle";
            this.wolTitle.Size = new System.Drawing.Size(188, 32);
            this.wolTitle.TabIndex = 0;
            this.wolTitle.Text = "Wings of Liberty";
            this.wolTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // wolEnabledCheckbox
            // 
            this.wolEnabledCheckbox.AutoSize = true;
            this.wolEnabledCheckbox.Location = new System.Drawing.Point(3, 42);
            this.wolEnabledCheckbox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.wolEnabledCheckbox.Name = "wolEnabledCheckbox";
            this.wolEnabledCheckbox.Size = new System.Drawing.Size(164, 19);
            this.wolEnabledCheckbox.TabIndex = 1;
            this.wolEnabledCheckbox.Text = "Enable Custom Campaign";
            this.wolEnabledCheckbox.UseVisualStyleBackColor = true;
            // 
            // wolCampaignLabel
            // 
            this.wolCampaignLabel.AutoSize = true;
            this.wolCampaignLabel.Location = new System.Drawing.Point(3, 71);
            this.wolCampaignLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.wolCampaignLabel.Name = "wolCampaignLabel";
            this.wolCampaignLabel.Size = new System.Drawing.Size(98, 15);
            this.wolCampaignLabel.TabIndex = 2;
            this.wolCampaignLabel.Text = "Active Campaign";
            // 
            // wolCampaignSelect
            // 
            this.wolCampaignSelect.Enabled = false;
            this.wolCampaignSelect.FormattingEnabled = true;
            this.wolCampaignSelect.Location = new System.Drawing.Point(3, 89);
            this.wolCampaignSelect.Name = "wolCampaignSelect";
            this.wolCampaignSelect.Size = new System.Drawing.Size(121, 23);
            this.wolCampaignSelect.TabIndex = 3;
            // 
            // wolDetailsLayout
            // 
            this.wolDetailsLayout.ColumnCount = 2;
            this.wolDetailsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.wolDetailsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73F));
            this.wolDetailsLayout.Controls.Add(this.wolAuthorDescLabel, 0, 0);
            this.wolDetailsLayout.Controls.Add(this.wolModVersionLabel, 0, 1);
            this.wolDetailsLayout.Controls.Add(this.wolVersion, 1, 1);
            this.wolDetailsLayout.Controls.Add(this.wolAuthorLabel, 1, 0);
            this.wolDetailsLayout.Location = new System.Drawing.Point(3, 122);
            this.wolDetailsLayout.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.wolDetailsLayout.Name = "wolDetailsLayout";
            this.wolDetailsLayout.RowCount = 2;
            this.wolDetailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.wolDetailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.wolDetailsLayout.Size = new System.Drawing.Size(188, 43);
            this.wolDetailsLayout.TabIndex = 4;
            // 
            // wolAuthorDescLabel
            // 
            this.wolAuthorDescLabel.AutoSize = true;
            this.wolAuthorDescLabel.Location = new System.Drawing.Point(3, 0);
            this.wolAuthorDescLabel.Name = "wolAuthorDescLabel";
            this.wolAuthorDescLabel.Size = new System.Drawing.Size(44, 21);
            this.wolAuthorDescLabel.TabIndex = 0;
            this.wolAuthorDescLabel.Text = "Author:";
            // 
            // wolModVersionLabel
            // 
            this.wolModVersionLabel.AutoSize = true;
            this.wolModVersionLabel.Location = new System.Drawing.Point(3, 21);
            this.wolModVersionLabel.Name = "wolModVersionLabel";
            this.wolModVersionLabel.Size = new System.Drawing.Size(38, 22);
            this.wolModVersionLabel.TabIndex = 2;
            this.wolModVersionLabel.Text = "Version:";
            // 
            // wolVersion
            // 
            this.wolVersion.AutoSize = true;
            this.wolVersion.Location = new System.Drawing.Point(53, 21);
            this.wolVersion.Name = "wolVersion";
            this.wolVersion.Size = new System.Drawing.Size(29, 15);
            this.wolVersion.TabIndex = 3;
            this.wolVersion.Text = "N/A";
            // 
            // wolAuthorLabel
            // 
            this.wolAuthorLabel.AutoSize = true;
            this.wolAuthorLabel.Location = new System.Drawing.Point(53, 0);
            this.wolAuthorLabel.Name = "wolAuthorLabel";
            this.wolAuthorLabel.Size = new System.Drawing.Size(29, 15);
            this.wolAuthorLabel.TabIndex = 1;
            this.wolAuthorLabel.Text = "N/A";
            // 
            // wolDescDescriptionLabel
            // 
            this.wolDescDescriptionLabel.AutoSize = true;
            this.wolDescDescriptionLabel.Location = new System.Drawing.Point(3, 175);
            this.wolDescDescriptionLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.wolDescDescriptionLabel.Name = "wolDescDescriptionLabel";
            this.wolDescDescriptionLabel.Size = new System.Drawing.Size(70, 15);
            this.wolDescDescriptionLabel.TabIndex = 5;
            this.wolDescDescriptionLabel.Text = "Description:";
            // 
            // wolDescriptionLayout
            // 
            this.wolDescriptionLayout.AutoScroll = true;
            this.wolDescriptionLayout.Controls.Add(this.wolDescription);
            this.wolDescriptionLayout.Location = new System.Drawing.Point(3, 193);
            this.wolDescriptionLayout.Name = "wolDescriptionLayout";
            this.wolDescriptionLayout.Size = new System.Drawing.Size(188, 178);
            this.wolDescriptionLayout.TabIndex = 6;
            // 
            // wolDescription
            // 
            this.wolDescription.AutoSize = true;
            this.wolDescription.Location = new System.Drawing.Point(3, 3);
            this.wolDescription.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.wolDescription.Name = "wolDescription";
            this.wolDescription.Size = new System.Drawing.Size(29, 15);
            this.wolDescription.TabIndex = 0;
            this.wolDescription.Text = "N/A";
            // 
            // campaignLayoutPanel
            // 
            this.campaignLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.campaignLayoutPanel.Controls.Add(this.wolLayout);
            this.campaignLayoutPanel.Controls.Add(this.hosLayout);
            this.campaignLayoutPanel.Controls.Add(this.lotvLayout);
            this.campaignLayoutPanel.Controls.Add(this.ncoLayout);
            this.campaignLayoutPanel.Location = new System.Drawing.Point(12, 55);
            this.campaignLayoutPanel.Name = "campaignLayoutPanel";
            this.campaignLayoutPanel.Size = new System.Drawing.Size(921, 407);
            this.campaignLayoutPanel.TabIndex = 2;
            // 
            // hosLayout
            // 
            this.hosLayout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hosLayout.Controls.Add(this.hosTitle);
            this.hosLayout.Controls.Add(this.hosEnabledCheckbox);
            this.hosLayout.Controls.Add(this.hosCampaignLabel);
            this.hosLayout.Controls.Add(this.hosCampaignSelect);
            this.hosLayout.Controls.Add(this.tableLayoutPanel1);
            this.hosLayout.Controls.Add(this.label19);
            this.hosLayout.Controls.Add(this.flowLayoutPanel1);
            this.hosLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.hosLayout.Location = new System.Drawing.Point(209, 3);
            this.hosLayout.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
            this.hosLayout.Name = "hosLayout";
            this.hosLayout.Size = new System.Drawing.Size(234, 392);
            this.hosLayout.TabIndex = 1;
            // 
            // hosTitle
            // 
            this.hosTitle.AutoSize = true;
            this.hosTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hosTitle.Location = new System.Drawing.Point(3, 0);
            this.hosTitle.Name = "hosTitle";
            this.hosTitle.Size = new System.Drawing.Size(221, 32);
            this.hosTitle.TabIndex = 0;
            this.hosTitle.Text = "Heart of the Swarm";
            // 
            // hosEnabledCheckbox
            // 
            this.hosEnabledCheckbox.AutoSize = true;
            this.hosEnabledCheckbox.Location = new System.Drawing.Point(3, 42);
            this.hosEnabledCheckbox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.hosEnabledCheckbox.Name = "hosEnabledCheckbox";
            this.hosEnabledCheckbox.Size = new System.Drawing.Size(164, 19);
            this.hosEnabledCheckbox.TabIndex = 1;
            this.hosEnabledCheckbox.Text = "Enable Custom Campaign";
            this.hosEnabledCheckbox.UseVisualStyleBackColor = true;
            // 
            // hosCampaignLabel
            // 
            this.hosCampaignLabel.AutoSize = true;
            this.hosCampaignLabel.Location = new System.Drawing.Point(3, 71);
            this.hosCampaignLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.hosCampaignLabel.Name = "hosCampaignLabel";
            this.hosCampaignLabel.Size = new System.Drawing.Size(98, 15);
            this.hosCampaignLabel.TabIndex = 2;
            this.hosCampaignLabel.Text = "Active Campaign";
            // 
            // hosCampaignSelect
            // 
            this.hosCampaignSelect.Enabled = false;
            this.hosCampaignSelect.FormattingEnabled = true;
            this.hosCampaignSelect.Location = new System.Drawing.Point(3, 89);
            this.hosCampaignSelect.Name = "hosCampaignSelect";
            this.hosCampaignSelect.Size = new System.Drawing.Size(121, 23);
            this.hosCampaignSelect.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73F));
            this.tableLayoutPanel1.Controls.Add(this.hosAuthorDescLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.hosAuthorLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.hosVersionLabel, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 122);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(221, 43);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // hosAuthorDescLabel
            // 
            this.hosAuthorDescLabel.AutoSize = true;
            this.hosAuthorDescLabel.Location = new System.Drawing.Point(3, 0);
            this.hosAuthorDescLabel.Name = "hosAuthorDescLabel";
            this.hosAuthorDescLabel.Size = new System.Drawing.Size(47, 15);
            this.hosAuthorDescLabel.TabIndex = 0;
            this.hosAuthorDescLabel.Text = "Author:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 15);
            this.label8.TabIndex = 1;
            this.label8.Text = "Version:";
            // 
            // hosAuthorLabel
            // 
            this.hosAuthorLabel.AutoSize = true;
            this.hosAuthorLabel.Location = new System.Drawing.Point(62, 0);
            this.hosAuthorLabel.Name = "hosAuthorLabel";
            this.hosAuthorLabel.Size = new System.Drawing.Size(29, 15);
            this.hosAuthorLabel.TabIndex = 2;
            this.hosAuthorLabel.Text = "N/A";
            // 
            // hosVersionLabel
            // 
            this.hosVersionLabel.AutoSize = true;
            this.hosVersionLabel.Location = new System.Drawing.Point(62, 21);
            this.hosVersionLabel.Name = "hosVersionLabel";
            this.hosVersionLabel.Size = new System.Drawing.Size(29, 15);
            this.hosVersionLabel.TabIndex = 3;
            this.hosVersionLabel.Text = "N/A";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 175);
            this.label19.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(67, 15);
            this.label19.TabIndex = 5;
            this.label19.Text = "Description";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.hosDescriptionLabel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 193);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(221, 178);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // hosDescriptionLabel
            // 
            this.hosDescriptionLabel.AutoSize = true;
            this.hosDescriptionLabel.Location = new System.Drawing.Point(3, 0);
            this.hosDescriptionLabel.Name = "hosDescriptionLabel";
            this.hosDescriptionLabel.Size = new System.Drawing.Size(29, 15);
            this.hosDescriptionLabel.TabIndex = 0;
            this.hosDescriptionLabel.Text = "N/A";
            // 
            // lotvLayout
            // 
            this.lotvLayout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lotvLayout.Controls.Add(this.lotvTitle);
            this.lotvLayout.Controls.Add(this.lotvEnabledCheckbox);
            this.lotvLayout.Controls.Add(this.label5);
            this.lotvLayout.Controls.Add(this.lotvCampaignSelect);
            this.lotvLayout.Controls.Add(this.tableLayoutPanel2);
            this.lotvLayout.Controls.Add(this.label20);
            this.lotvLayout.Controls.Add(this.flowLayoutPanel2);
            this.lotvLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.lotvLayout.Location = new System.Drawing.Point(454, 3);
            this.lotvLayout.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
            this.lotvLayout.Name = "lotvLayout";
            this.lotvLayout.Size = new System.Drawing.Size(226, 392);
            this.lotvLayout.TabIndex = 2;
            // 
            // lotvTitle
            // 
            this.lotvTitle.AutoSize = true;
            this.lotvTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lotvTitle.Location = new System.Drawing.Point(3, 0);
            this.lotvTitle.Name = "lotvTitle";
            this.lotvTitle.Size = new System.Drawing.Size(212, 32);
            this.lotvTitle.TabIndex = 0;
            this.lotvTitle.Text = "Legacy of the Void";
            // 
            // lotvEnabledCheckbox
            // 
            this.lotvEnabledCheckbox.AutoSize = true;
            this.lotvEnabledCheckbox.Location = new System.Drawing.Point(3, 42);
            this.lotvEnabledCheckbox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.lotvEnabledCheckbox.Name = "lotvEnabledCheckbox";
            this.lotvEnabledCheckbox.Size = new System.Drawing.Size(164, 19);
            this.lotvEnabledCheckbox.TabIndex = 1;
            this.lotvEnabledCheckbox.Text = "Enable Custom Campaign";
            this.lotvEnabledCheckbox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 71);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Active Campaign";
            // 
            // lotvCampaignSelect
            // 
            this.lotvCampaignSelect.Enabled = false;
            this.lotvCampaignSelect.FormattingEnabled = true;
            this.lotvCampaignSelect.Location = new System.Drawing.Point(3, 89);
            this.lotvCampaignSelect.Name = "lotvCampaignSelect";
            this.lotvCampaignSelect.Size = new System.Drawing.Size(121, 23);
            this.lotvCampaignSelect.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73F));
            this.tableLayoutPanel2.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label12, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lotvAuthorLabel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lotvVersionLabel, 1, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 122);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(212, 43);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 15);
            this.label11.TabIndex = 0;
            this.label11.Text = "Author:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 15);
            this.label12.TabIndex = 1;
            this.label12.Text = "Version:";
            // 
            // lotvAuthorLabel
            // 
            this.lotvAuthorLabel.AutoSize = true;
            this.lotvAuthorLabel.Location = new System.Drawing.Point(60, 0);
            this.lotvAuthorLabel.Name = "lotvAuthorLabel";
            this.lotvAuthorLabel.Size = new System.Drawing.Size(29, 15);
            this.lotvAuthorLabel.TabIndex = 2;
            this.lotvAuthorLabel.Text = "N/A";
            // 
            // lotvVersionLabel
            // 
            this.lotvVersionLabel.AutoSize = true;
            this.lotvVersionLabel.Location = new System.Drawing.Point(60, 21);
            this.lotvVersionLabel.Name = "lotvVersionLabel";
            this.lotvVersionLabel.Size = new System.Drawing.Size(29, 15);
            this.lotvVersionLabel.TabIndex = 3;
            this.lotvVersionLabel.Text = "N/A";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 175);
            this.label20.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(67, 15);
            this.label20.TabIndex = 5;
            this.label20.Text = "Description";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoScroll = true;
            this.flowLayoutPanel2.Controls.Add(this.lotvDescriptionLabel);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 193);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(212, 178);
            this.flowLayoutPanel2.TabIndex = 6;
            // 
            // lotvDescriptionLabel
            // 
            this.lotvDescriptionLabel.AutoSize = true;
            this.lotvDescriptionLabel.Location = new System.Drawing.Point(3, 0);
            this.lotvDescriptionLabel.Name = "lotvDescriptionLabel";
            this.lotvDescriptionLabel.Size = new System.Drawing.Size(29, 15);
            this.lotvDescriptionLabel.TabIndex = 0;
            this.lotvDescriptionLabel.Text = "N/A";
            // 
            // ncoLayout
            // 
            this.ncoLayout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ncoLayout.Controls.Add(this.ncoTitle);
            this.ncoLayout.Controls.Add(this.ncoEnabledCheckbox);
            this.ncoLayout.Controls.Add(this.label6);
            this.ncoLayout.Controls.Add(this.ncoCampaignSelect);
            this.ncoLayout.Controls.Add(this.tableLayoutPanel3);
            this.ncoLayout.Controls.Add(this.label21);
            this.ncoLayout.Controls.Add(this.flowLayoutPanel3);
            this.ncoLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ncoLayout.Location = new System.Drawing.Point(691, 3);
            this.ncoLayout.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
            this.ncoLayout.Name = "ncoLayout";
            this.ncoLayout.Size = new System.Drawing.Size(209, 392);
            this.ncoLayout.TabIndex = 3;
            // 
            // ncoTitle
            // 
            this.ncoTitle.AutoSize = true;
            this.ncoTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ncoTitle.Location = new System.Drawing.Point(3, 0);
            this.ncoTitle.Name = "ncoTitle";
            this.ncoTitle.Size = new System.Drawing.Size(196, 32);
            this.ncoTitle.TabIndex = 0;
            this.ncoTitle.Text = "Nova Covert Ops";
            // 
            // ncoEnabledCheckbox
            // 
            this.ncoEnabledCheckbox.AutoSize = true;
            this.ncoEnabledCheckbox.Location = new System.Drawing.Point(3, 42);
            this.ncoEnabledCheckbox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.ncoEnabledCheckbox.Name = "ncoEnabledCheckbox";
            this.ncoEnabledCheckbox.Size = new System.Drawing.Size(164, 19);
            this.ncoEnabledCheckbox.TabIndex = 1;
            this.ncoEnabledCheckbox.Text = "Enable Custom Campaign";
            this.ncoEnabledCheckbox.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 71);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Active Campaign";
            // 
            // ncoCampaignSelect
            // 
            this.ncoCampaignSelect.Enabled = false;
            this.ncoCampaignSelect.FormattingEnabled = true;
            this.ncoCampaignSelect.Location = new System.Drawing.Point(3, 89);
            this.ncoCampaignSelect.Name = "ncoCampaignSelect";
            this.ncoCampaignSelect.Size = new System.Drawing.Size(121, 23);
            this.ncoCampaignSelect.TabIndex = 3;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72F));
            this.tableLayoutPanel3.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label16, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.ncoAuthorLabel, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.ncoVersionLabel, 1, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 122);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(196, 43);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(47, 15);
            this.label15.TabIndex = 0;
            this.label15.Text = "Author:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 21);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(48, 15);
            this.label16.TabIndex = 1;
            this.label16.Text = "Version:";
            // 
            // ncoAuthorLabel
            // 
            this.ncoAuthorLabel.AutoSize = true;
            this.ncoAuthorLabel.Location = new System.Drawing.Point(57, 0);
            this.ncoAuthorLabel.Name = "ncoAuthorLabel";
            this.ncoAuthorLabel.Size = new System.Drawing.Size(29, 15);
            this.ncoAuthorLabel.TabIndex = 2;
            this.ncoAuthorLabel.Text = "N/A";
            // 
            // ncoVersionLabel
            // 
            this.ncoVersionLabel.AutoSize = true;
            this.ncoVersionLabel.Location = new System.Drawing.Point(57, 21);
            this.ncoVersionLabel.Name = "ncoVersionLabel";
            this.ncoVersionLabel.Size = new System.Drawing.Size(29, 15);
            this.ncoVersionLabel.TabIndex = 3;
            this.ncoVersionLabel.Text = "N/A";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 175);
            this.label21.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(67, 15);
            this.label21.TabIndex = 5;
            this.label21.Text = "Description";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoScroll = true;
            this.flowLayoutPanel3.Controls.Add(this.ncoDescriptionLabel);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 193);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(196, 178);
            this.flowLayoutPanel3.TabIndex = 6;
            // 
            // ncoDescriptionLabel
            // 
            this.ncoDescriptionLabel.AutoSize = true;
            this.ncoDescriptionLabel.Location = new System.Drawing.Point(3, 0);
            this.ncoDescriptionLabel.Name = "ncoDescriptionLabel";
            this.ncoDescriptionLabel.Size = new System.Drawing.Size(29, 15);
            this.ncoDescriptionLabel.TabIndex = 0;
            this.ncoDescriptionLabel.Text = "N/A";
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 655);
            this.Controls.Add(this.installLocationButton);
            this.Controls.Add(this.messagesLabel);
            this.Controls.Add(this.messagesPanel);
            this.Controls.Add(this.campaignLayoutPanel);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.importButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainPage";
            this.Text = "SC2 Custom Campaign Manager: XP Edition";
            this.messagesPanel.ResumeLayout(false);
            this.messagesPanel.PerformLayout();
            this.wolLayout.ResumeLayout(false);
            this.wolLayout.PerformLayout();
            this.wolDetailsLayout.ResumeLayout(false);
            this.wolDetailsLayout.PerformLayout();
            this.wolDescriptionLayout.ResumeLayout(false);
            this.wolDescriptionLayout.PerformLayout();
            this.campaignLayoutPanel.ResumeLayout(false);
            this.hosLayout.ResumeLayout(false);
            this.hosLayout.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.lotvLayout.ResumeLayout(false);
            this.lotvLayout.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ncoLayout.ResumeLayout(false);
            this.ncoLayout.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button importButton;
        private Button aboutButton;
        private FlowLayoutPanel messagesPanel;
        private Label messagesLabel;
        private Button installLocationButton;
        private FlowLayoutPanel wolLayout;
        private Label wolTitle;
        private CheckBox wolEnabledCheckbox;
        private Label wolCampaignLabel;
        private ComboBox wolCampaignSelect;
        private TableLayoutPanel wolDetailsLayout;
        private Label wolAuthorDescLabel;
        private Label wolAuthorLabel;
        private Label wolModVersionLabel;
        private Label wolVersion;
        private Label wolDescDescriptionLabel;
        private FlowLayoutPanel wolDescriptionLayout;
        private Label wolDescription;
        private FlowLayoutPanel campaignLayoutPanel;
        private FlowLayoutPanel hosLayout;
        private Label hosTitle;
        private FlowLayoutPanel lotvLayout;
        private Label lotvTitle;
        private FlowLayoutPanel ncoLayout;
        private Label ncoTitle;
        private CheckBox hosEnabledCheckbox;
        private Label hosCampaignLabel;
        private ComboBox hosCampaignSelect;
        private TableLayoutPanel tableLayoutPanel1;
        private Label hosAuthorDescLabel;
        private Label label8;
        private Label hosAuthorLabel;
        private Label hosVersionLabel;
        private Label label19;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label hosDescriptionLabel;
        private CheckBox lotvEnabledCheckbox;
        private Label label5;
        private ComboBox lotvCampaignSelect;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label11;
        private Label label12;
        private Label lotvAuthorLabel;
        private Label lotvVersionLabel;
        private Label label20;
        private FlowLayoutPanel flowLayoutPanel2;
        private Label lotvDescriptionLabel;
        private CheckBox ncoEnabledCheckbox;
        private Label label6;
        private ComboBox ncoCampaignSelect;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label15;
        private Label label16;
        private Label ncoAuthorLabel;
        private Label ncoVersionLabel;
        private Label label21;
        private FlowLayoutPanel flowLayoutPanel3;
        private Label ncoDescriptionLabel;
        private Label messageLabel;
    }
}