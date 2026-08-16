-- Seed data for Categories, Products, Warehouses, Inventory, Customers, Invoices and Payments.
-- Run after database.sql and after the app has bootstrapped the default Admin user (Data/DatabaseSeeder.cs).
USE StockitDB;
GO

-- ====================== Warehouses ======================
INSERT INTO Warehouse (Name, Location, IsActive) VALUES
('Main Warehouse', '123 Main St, Managua, Nicaragua', 1),
('North Branch', '45 Industrial Ave, Leon, Nicaragua', 1),
('South Branch', '78 Commerce Rd, Rivas, Nicaragua', 1);
GO

-- ====================== Categories ======================
INSERT INTO Category (Name, Description, ParentCategoryId) VALUES
('Electronics', 'Electronic devices and gadgets', NULL),
('Groceries', 'Food and household consumables', NULL),
('Furniture', 'Home and office furniture', NULL),
('Office Supplies', 'Stationery and office essentials', NULL);
GO

INSERT INTO Category (Name, Description, ParentCategoryId)
SELECT 'Computers & Accessories', 'Computer peripherals and accessories', Id FROM Category WHERE Name = 'Electronics';
GO

-- ====================== Products ======================
INSERT INTO Product (Name, Description, Barcode, QrCode, Price, CategoryId, IsActive) VALUES
('Wireless Mouse', 'Ergonomic 2.4GHz wireless mouse', '8000000000012', 'QR-WIRELESS-MOUSE', 15.99,
    (SELECT Id FROM Category WHERE Name = 'Computers & Accessories'), 1),
('Mechanical Keyboard', 'RGB backlit mechanical keyboard', '8000000000029', 'QR-MECH-KEYBOARD', 45.50,
    (SELECT Id FROM Category WHERE Name = 'Computers & Accessories'), 1),
('27" Monitor', '27-inch 1440p IPS monitor', '8000000000036', 'QR-MONITOR-27', 189.99,
    (SELECT Id FROM Category WHERE Name = 'Computers & Accessories'), 1),
('Smartphone X12', '6.5-inch 128GB smartphone', '8000000000043', 'QR-SMARTPHONE-X12', 699.00,
    (SELECT Id FROM Category WHERE Name = 'Electronics'), 1),
('Bluetooth Speaker', 'Portable waterproof Bluetooth speaker', '8000000000050', 'QR-BT-SPEAKER', 39.99,
    (SELECT Id FROM Category WHERE Name = 'Electronics'), 1),
('Organic Coffee 1kg', 'Single-origin organic ground coffee', '8000000000067', 'QR-COFFEE-1KG', 12.50,
    (SELECT Id FROM Category WHERE Name = 'Groceries'), 1),
('Basmati Rice 5kg', 'Premium long-grain basmati rice', '8000000000074', 'QR-RICE-5KG', 18.75,
    (SELECT Id FROM Category WHERE Name = 'Groceries'), 1),
('Office Desk', 'Adjustable-height office desk', '8000000000081', 'QR-OFFICE-DESK', 220.00,
    (SELECT Id FROM Category WHERE Name = 'Furniture'), 1),
('Ergonomic Chair', 'Mesh-back ergonomic office chair', '8000000000098', 'QR-ERGO-CHAIR', 175.00,
    (SELECT Id FROM Category WHERE Name = 'Furniture'), 1),
('Notebook Pack (5-pack)', '5-pack of 100-page ruled notebooks', '8000000000104', 'QR-NOTEBOOK-PACK', 9.99,
    (SELECT Id FROM Category WHERE Name = 'Office Supplies'), 1),
('Ballpoint Pens Box (50-count)', 'Box of 50 black ballpoint pens', '8000000000111', 'QR-PENS-BOX', 7.25,
    (SELECT Id FROM Category WHERE Name = 'Office Supplies'), 1);
GO

-- ====================== Inventory ======================
INSERT INTO Inventory (ProductId, WarehouseId, Quantity)
SELECT p.Id, w.Id, 50
FROM Product p CROSS JOIN Warehouse w
WHERE w.Name = 'Main Warehouse';
GO

INSERT INTO Inventory (ProductId, WarehouseId, Quantity)
SELECT p.Id, w.Id, 25
FROM Product p CROSS JOIN Warehouse w
WHERE w.Name = 'North Branch';
GO

INSERT INTO Inventory (ProductId, WarehouseId, Quantity)
SELECT p.Id, w.Id, 10
FROM Product p CROSS JOIN Warehouse w
WHERE w.Name = 'South Branch'
AND p.Barcode IN ('8000000000081', '8000000000098', '8000000000067', '8000000000074');
GO

-- ====================== Customers ======================
INSERT INTO Customer (Name, Email, Phone, Address, IsActive) VALUES
('Maria Gonzalez', 'maria.gonzalez@example.com', '+505-8888-1001', 'Barrio Monsenor Lezcano, Managua', 1),
('Carlos Mendoza', 'carlos.mendoza@example.com', '+505-8888-1002', 'Reparto San Juan, Managua', 1),
('Ana Rodriguez', 'ana.rodriguez@example.com', '+505-8888-1003', 'Colonia Centroamerica, Managua', 1),
('Luis Herrera', 'luis.herrera@example.com', '+505-8888-1004', 'Barrio Sutiaba, Leon', 1),
('Sofia Castillo', 'sofia.castillo@example.com', '+505-8888-1005', 'Centro, Rivas', 1);
GO

-- ====================== Invoices & Invoice Items ======================
DECLARE @AdminId INT = (SELECT Id FROM [User] WHERE Email = N'danny.lopez@stockit.com');
DECLARE @MainWarehouseId INT = (SELECT Id FROM Warehouse WHERE Name = 'Main Warehouse');
DECLARE @NorthBranchId INT = (SELECT Id FROM Warehouse WHERE Name = 'North Branch');
DECLARE @SouthBranchId INT = (SELECT Id FROM Warehouse WHERE Name = 'South Branch');

DECLARE @MariaId INT = (SELECT Id FROM Customer WHERE Email = 'maria.gonzalez@example.com');
DECLARE @CarlosId INT = (SELECT Id FROM Customer WHERE Email = 'carlos.mendoza@example.com');
DECLARE @AnaId INT = (SELECT Id FROM Customer WHERE Email = 'ana.rodriguez@example.com');
DECLARE @LuisId INT = (SELECT Id FROM Customer WHERE Email = 'luis.herrera@example.com');
DECLARE @SofiaId INT = (SELECT Id FROM Customer WHERE Email = 'sofia.castillo@example.com');

DECLARE @MouseId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000012');
DECLARE @KeyboardId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000029');
DECLARE @MonitorId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000036');
DECLARE @SmartphoneId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000043');
DECLARE @CoffeeId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000067');
DECLARE @RiceId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000074');
DECLARE @DeskId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000081');
DECLARE @ChairId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000098');
DECLARE @NotebookId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000104');
DECLARE @PensId INT = (SELECT Id FROM Product WHERE Barcode = '8000000000111');

-- Invoice 1: Paid in full
INSERT INTO Invoice (InvoiceNumber, CustomerId, WarehouseId, SellerId, Subtotal, DiscountAmount, TotalAmount, Status, Currency, IssuedAt)
VALUES ('INV-2026-000001', @MariaId, @MainWarehouseId, @AdminId, 77.48, 0, 77.48, 'Paid', 'USD', GETUTCDATE());

DECLARE @Invoice1Id INT = SCOPE_IDENTITY();
INSERT INTO InvoiceItem (InvoiceId, ProductId, Quantity, UnitPrice, DiscountAmount, TotalAmount) VALUES
(@Invoice1Id, @MouseId, 2, 15.99, 0, 31.98),
(@Invoice1Id, @KeyboardId, 1, 45.50, 0, 45.50);

-- Invoice 2: Partially paid
INSERT INTO Invoice (InvoiceNumber, CustomerId, WarehouseId, SellerId, Subtotal, DiscountAmount, TotalAmount, Status, Currency, IssuedAt)
VALUES ('INV-2026-000002', @CarlosId, @MainWarehouseId, @AdminId, 888.99, 20.00, 868.99, 'Partially Paid', 'USD', GETUTCDATE());

DECLARE @Invoice2Id INT = SCOPE_IDENTITY();
INSERT INTO InvoiceItem (InvoiceId, ProductId, Quantity, UnitPrice, DiscountAmount, TotalAmount) VALUES
(@Invoice2Id, @MonitorId, 1, 189.99, 0, 189.99),
(@Invoice2Id, @SmartphoneId, 1, 699.00, 20.00, 679.00);

-- Invoice 3: Pending, no payment yet
INSERT INTO Invoice (InvoiceNumber, CustomerId, WarehouseId, SellerId, Subtotal, DiscountAmount, TotalAmount, Status, Currency, IssuedAt)
VALUES ('INV-2026-000003', @AnaId, @NorthBranchId, @AdminId, 75.00, 0, 75.00, 'Pending', 'USD', GETUTCDATE());

DECLARE @Invoice3Id INT = SCOPE_IDENTITY();
INSERT INTO InvoiceItem (InvoiceId, ProductId, Quantity, UnitPrice, DiscountAmount, TotalAmount) VALUES
(@Invoice3Id, @CoffeeId, 3, 12.50, 0, 37.50),
(@Invoice3Id, @RiceId, 2, 18.75, 0, 37.50);

-- Invoice 4: Paid in full
INSERT INTO Invoice (InvoiceNumber, CustomerId, WarehouseId, SellerId, Subtotal, DiscountAmount, TotalAmount, Status, Currency, IssuedAt)
VALUES ('INV-2026-000004', @LuisId, @SouthBranchId, @AdminId, 395.00, 15.00, 380.00, 'Paid', 'USD', GETUTCDATE());

DECLARE @Invoice4Id INT = SCOPE_IDENTITY();
INSERT INTO InvoiceItem (InvoiceId, ProductId, Quantity, UnitPrice, DiscountAmount, TotalAmount) VALUES
(@Invoice4Id, @DeskId, 1, 220.00, 0, 220.00),
(@Invoice4Id, @ChairId, 1, 175.00, 15.00, 160.00);

-- Invoice 5: Void, no payment
INSERT INTO Invoice (InvoiceNumber, CustomerId, WarehouseId, SellerId, Subtotal, DiscountAmount, TotalAmount, Status, Currency, IssuedAt)
VALUES ('INV-2026-000005', @SofiaId, @MainWarehouseId, @AdminId, 122.45, 0, 122.45, 'Void', 'USD', GETUTCDATE());

DECLARE @Invoice5Id INT = SCOPE_IDENTITY();
INSERT INTO InvoiceItem (InvoiceId, ProductId, Quantity, UnitPrice, DiscountAmount, TotalAmount) VALUES
(@Invoice5Id, @NotebookId, 5, 9.99, 0, 49.95),
(@Invoice5Id, @PensId, 10, 7.25, 0, 72.50);

-- ====================== Payments ======================
INSERT INTO Payment (InvoiceId, Amount, PaymentMethod, ReferenceNumber, PaymentDate, CreatedBy, PaidAt) VALUES
(@Invoice1Id, 77.48, 'Cash', NULL, GETUTCDATE(), @AdminId, GETUTCDATE()),
(@Invoice2Id, 400.00, 'Credit Card', 'TXN-2026-1002', GETUTCDATE(), @AdminId, GETUTCDATE()),
(@Invoice4Id, 380.00, 'Bank Transfer', 'TXN-2026-1004', GETUTCDATE(), @AdminId, GETUTCDATE());
GO
