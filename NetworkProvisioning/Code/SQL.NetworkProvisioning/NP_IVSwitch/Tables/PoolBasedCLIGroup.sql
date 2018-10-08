CREATE TABLE [NP_IVSwitch].[PoolBasedCLIGroup] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (MAX) NULL,
    [CLIPatterns]      NVARCHAR (MAX) NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_PoolBasedCLIGroup_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_PoolBasedCLIGroup] PRIMARY KEY CLUSTERED ([ID] ASC)
);

