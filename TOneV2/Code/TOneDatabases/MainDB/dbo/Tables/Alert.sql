CREATE TABLE [dbo].[Alert] (
    [ID]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [Created]     DATETIME      CONSTRAINT [DF_Alert_Created] DEFAULT (getdate()) NULL,
    [Source]      VARCHAR (255) NULL,
    [Level]       SMALLINT      NULL,
    [Progress]    SMALLINT      NULL,
    [Tag]         VARCHAR (255) NULL,
    [Description] VARCHAR (MAX) NULL,
    [IsVisible]   CHAR (1)      CONSTRAINT [DF_Alert_IsVisible] DEFAULT ('Y') NULL,
    CONSTRAINT [PK_Alert] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Alert_Source]
    ON [dbo].[Alert]([Source] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Alert_Date]
    ON [dbo].[Alert]([Created] DESC);

