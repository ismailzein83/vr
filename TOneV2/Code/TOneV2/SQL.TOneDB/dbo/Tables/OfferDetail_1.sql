CREATE TABLE [dbo].[OfferDetail] (
    [OfferDetailID] INT            IDENTITY (1, 1) NOT NULL,
    [OfferID]       INT            NOT NULL,
    [FromVolume]    INT            NULL,
    [ToVolume]      INT            NULL,
    [Value]         DECIMAL (9, 5) NOT NULL,
    [IsRetroActive] CHAR (1)       CONSTRAINT [DF_OfferDetails_IsRetroActive] DEFAULT ((0)) NULL,
    [Discount]      INT            CONSTRAINT [DF_OfferDetail_Discount] DEFAULT ((0)) NULL,
    [timestamp]     ROWVERSION     NOT NULL,
    CONSTRAINT [PK_OfferDetails] PRIMARY KEY CLUSTERED ([OfferDetailID] ASC)
);

