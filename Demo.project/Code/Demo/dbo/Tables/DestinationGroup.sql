CREATE TABLE [dbo].[DestinationGroup] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (50)   NOT NULL,
    [DestinationType] INT            NOT NULL,
    [GroupSettings]   NVARCHAR (MAX) NULL,
    [timestamp]       ROWVERSION     NULL,
    CONSTRAINT [PK_DestinationGroup] PRIMARY KEY CLUSTERED ([ID] ASC)
);



