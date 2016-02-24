CREATE TABLE [dbo].[OperatorAccount] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [NameSuffix]              NVARCHAR (255) NOT NULL,
    [OperatorProfileID]       INT            NOT NULL,
    [SupplierSettings]        NVARCHAR (MAX) NULL,
    [CustomerSettings]        NVARCHAR (MAX) NULL,
    [OperatorAccountSettings] NVARCHAR (MAX) NULL,
    [timestamp]               ROWVERSION     NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_OperatorAccount_OperatorProfile] FOREIGN KEY ([OperatorProfileID]) REFERENCES [dbo].[OperatorProfile] ([ID])
);



