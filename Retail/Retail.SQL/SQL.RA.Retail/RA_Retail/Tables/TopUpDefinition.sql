CREATE TABLE [RA_Retail].[TopUpDefinition] (
    [ID]               INT             IDENTITY (1, 1) NOT NULL,
    [SourceID]         NVARCHAR (255)  NULL,
    [Name]             NVARCHAR (255)  NULL,
    [OperatorID]       BIGINT          NULL,
    [DoesCreditExpire] BIT             NULL,
    [Amount]           DECIMAL (22, 8) NULL,
    [DuePeriod]        INT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK__TopUpDef__3214EC2736B12243] PRIMARY KEY CLUSTERED ([ID] ASC)
);

