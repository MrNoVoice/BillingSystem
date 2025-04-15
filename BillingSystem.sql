CREATE DATABASE billingsystem;

USE billingsystem;

CREATE TABLE Customers (
    Name VARCHAR(255),
    Contact INT 
);

CREATE TABLE MenuItems (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL
);

INSERT INTO MenuItems (Name, Price) VALUES 
('Momo', 120),
('Chowmein', 100),
('Pasta', 150),
('Burger', 180),
('Pizza', 250),
('Noodles', 110),
('Americano Latte', 160),
('Coffee', 80);


CREATE TABLE Orders (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    CustomerID INT,
    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CustomerID) REFERENCES Customers(ID) ON DELETE CASCADE
);

CREATE TABLE OrderItems (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    OrderID INT NOT NULL,
    MenuItemID INT NOT NULL,
    Quantity INT NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(ID) ON DELETE CASCADE,
    FOREIGN KEY (MenuItemID) REFERENCES MenuItems(ID) ON DELETE CASCADE
);


