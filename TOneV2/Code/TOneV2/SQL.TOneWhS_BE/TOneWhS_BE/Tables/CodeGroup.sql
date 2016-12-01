CREATE TABLE [TOneWhS_BE].[CodeGroup] (
    [ID]          INT          IDENTITY (1, 1) NOT NULL,
    [CountryID]   INT          NOT NULL,
    [Code]        VARCHAR (20) NOT NULL,
    [timestamp]   ROWVERSION   NULL,
    [SourceID]    VARCHAR (50) NULL,
    [CreatedTime] DATETIME     CONSTRAINT [DF_CodeGroup_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_CodeGroup_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_CodeGroup_Code] UNIQUE NONCLUSTERED ([Code] ASC)
);









