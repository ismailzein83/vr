CREATE TABLE [RetailBilling].[ContractResource] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [ContractID]       BIGINT           NULL,
    [Name]             NVARCHAR (255)   NULL,
    [TypeID]           UNIQUEIDENTIFIER NULL,
    [BET]              DATETIME         NULL,
    [EET]              DATETIME         NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    CONSTRAINT [PK__Contract__3214EC2712FDD1B2] PRIMARY KEY CLUSTERED ([ID] ASC)
);

