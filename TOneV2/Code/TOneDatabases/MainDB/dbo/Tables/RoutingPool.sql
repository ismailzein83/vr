CREATE TABLE [dbo].[RoutingPool] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [IsEnabled] CHAR (1)       CONSTRAINT [DF_RoutingPool_IsEnable] DEFAULT ('N') NOT NULL,
    CONSTRAINT [PK_RoutingPool] PRIMARY KEY CLUSTERED ([ID] ASC)
);

