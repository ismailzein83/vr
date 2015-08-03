CREATE TABLE [dbo].[CarrierDocument] (
    [DocumentID]  SMALLINT       IDENTITY (1, 1) NOT NULL,
    [ProfileID]   SMALLINT       NOT NULL,
    [Name]        NVARCHAR (450) NOT NULL,
    [Description] TEXT           NULL,
    [Category]    TEXT           NULL,
    [Document]    IMAGE          NULL,
    [Created]     SMALLDATETIME  CONSTRAINT [DF_CarrierDocument_Created] DEFAULT (getdate()) NOT NULL,
    [UserID]      INT            NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_CarrierDocument] PRIMARY KEY CLUSTERED ([DocumentID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CarrierDocument]
    ON [dbo].[CarrierDocument]([ProfileID] ASC);

