CREATE TABLE [dbo].[RelatedNumbers] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [ReportID]      INT           NOT NULL,
    [RelatedNumber] VARCHAR (50)  NOT NULL,
    [RegisteredOn]  DATETIME2 (0) CONSTRAINT [DF_RelatedNumbers_RegisteredOn] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_RelatedNumbers] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_RelatedNumbers_Reports] FOREIGN KEY ([ReportID]) REFERENCES [dbo].[Reports] ([ID])
);

