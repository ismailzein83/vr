CREATE TABLE [PSTN_BE].[NormalizationRule] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Criteria]    NVARCHAR (MAX) NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [BED]         DATETIME       NOT NULL,
    [EED]         DATETIME       NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_NormalizationRule] PRIMARY KEY CLUSTERED ([ID] ASC)
);



