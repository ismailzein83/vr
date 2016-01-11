CREATE TABLE [dbo].[Dim_SubscriberType] (
    [Pk_SubscriberTypeId] INT          NOT NULL,
    [Name]                VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_Dim_SubscriberType] PRIMARY KEY CLUSTERED ([Pk_SubscriberTypeId] ASC)
);

