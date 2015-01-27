CREATE TABLE [dbo].[AutoInvoiceSetting] (
    [AutomaticInvoiceSettingID] INT           IDENTITY (1, 1) NOT NULL,
    [Description]               VARCHAR (255) NULL,
    [Type]                      TINYINT       NULL,
    [StartDate]                 DATETIME      NULL,
    [DayNumber]                 TINYINT       NULL,
    [CheckUnpricedCDRs]         CHAR (1)      NULL,
    [SendEmail]                 CHAR (1)      NULL,
    [IssueDate]                 DATETIME      NULL,
    CONSTRAINT [PK_AutoInvoiceSetting_1] PRIMARY KEY CLUSTERED ([AutomaticInvoiceSettingID] ASC)
);

