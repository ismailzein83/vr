CREATE TABLE [common].[File] (
    [ID]          BIGINT          IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255)  NOT NULL,
    [Extension]   VARCHAR (50)    NULL,
    [Content]     VARBINARY (MAX) NOT NULL,
    [IsUsed]      BIT             CONSTRAINT [DF_File_IsUsed] DEFAULT ((0)) NULL,
    [CreatedTime] DATETIME        CONSTRAINT [DF_File_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([ID] ASC)
);

