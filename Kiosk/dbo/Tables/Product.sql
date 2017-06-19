CREATE TABLE [dbo].[Product] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [UPC]              INT            NULL,
    [Name]             NVARCHAR (100) NULL,
    [Price]            MONEY          NULL,
    [ProductTypeName]  NVARCHAR (100) NULL,
    [ManufacturerName] NVARCHAR (100) NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Product_Manufacturer] FOREIGN KEY ([ManufacturerName]) REFERENCES [dbo].[Manufacturer] ([Name]),
    CONSTRAINT [FK_Product_ProductType] FOREIGN KEY ([ProductTypeName]) REFERENCES [dbo].[ProductType] ([Name])
);

