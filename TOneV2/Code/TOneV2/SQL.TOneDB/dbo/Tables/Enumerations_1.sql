CREATE TABLE [dbo].[Enumerations] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Enumeration] NVARCHAR (120) NOT NULL,
    [Name]        NVARCHAR (120) NOT NULL,
    [Value]       INT            NOT NULL,
    [timestamp]   ROWVERSION     NOT NULL
);

