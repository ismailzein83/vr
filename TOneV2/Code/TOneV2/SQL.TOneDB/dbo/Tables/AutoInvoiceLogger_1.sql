CREATE TABLE [dbo].[AutoInvoiceLogger] (
    [LogID]         INT            IDENTITY (1, 1) NOT NULL,
    [BatchID]       INT            NULL,
    [Type]          TINYINT        NULL,
    [Note]          NVARCHAR (255) NULL,
    [IsSent]        CHAR (1)       NULL,
    [IsGenerated]   CHAR (1)       NULL,
    [IsCDRConflict] CHAR (1)       NULL,
    [GeneratedDate] DATETIME       NULL,
    [CustomerID]    VARCHAR (5)    NULL,
    [SettingID]     INT            NULL,
    [InvoiceID]     INT            NULL,
    CONSTRAINT [PK_AutoInvoiceLogger] PRIMARY KEY CLUSTERED ([LogID] ASC)
);

