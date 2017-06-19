CREATE PROCEDURE sp_AddStoreWithAddress
@street1 NVARCHAR(100),
@street2 NVARCHAR(100),
@city NVARCHAR(100),
@state NVARCHAR(50),
@zip NVARCHAR(12),
@storename NVARCHAR(100)
AS

INSERT INTO [Address](Line1, Line2, State, Zip, City) VALUES
(@street1, @street2, @state, @zip, @city)

DECLARE @addressID INT

SET @addressID = (SELECT MAX(ID) FROM [Address])
INSERT INTO Store(Name, AddressID) VALUES
(@storename, @addressID)