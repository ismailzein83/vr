CREATE TABLE [dbo].[Dim_SubscriberType] (
    [Pk_SubscriberTypeId] INT          IDENTITY (1, 1) NOT NULL,
    [Name]                VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_Dim_SubscriberType] PRIMARY KEY CLUSTERED ([Name] ASC)
);



