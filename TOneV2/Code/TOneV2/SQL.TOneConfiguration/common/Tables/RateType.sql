CREATE TABLE [common].[RateType] (
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (50) NOT NULL,
    [timestamp]        ROWVERSION    NULL,
    [LastModifiedTime] DATETIME      CONSTRAINT [DF_RateType_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_common.RateType] PRIMARY KEY CLUSTERED ([ID] ASC)
);





