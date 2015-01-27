CREATE TABLE [dbo].[CustomerTimeinfo] (
    [ID]            SMALLINT      IDENTITY (1, 1) NOT NULL,
    [BaseUtcOffset] INT           NOT NULL,
    [DisplayName]   VARCHAR (250) NOT NULL,
    [UserID]        INT           NULL,
    [Timestamp]     ROWVERSION    NOT NULL
);

