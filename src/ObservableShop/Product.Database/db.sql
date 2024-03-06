CREATE TABLE IF NOT EXISTS products (
    id serial PRIMARY KEY,
    product_id varchar(50) unique not null,
    name varchar(256) not null,
    description varchar(256) not null,
    price money not null,
    stock NUMERIC not null
);

INSERT INTO products (product_id, name, description, price, stock) VALUES
('P001', 'Laptop', 'High-performance laptop with SSD', 1200.00, 0),
('P002', 'Smartphone', 'Latest model with advanced features', 800.00, 100),
('P003', 'Smart TV', '4K Ultra HD with Smart Hub', 1500.00, 30),
('P004', 'Digital Camera', 'Professional DSLR camera', 900.00, 20),
('P005', 'Wireless Headphones', 'Noise-canceling technology', 150.00, 80),
('P006', 'Fitness Tracker', 'Track your health and activities', 80.00, 120),
('P007', 'Gaming Console', 'Next-gen gaming experience', 400.00, 0),
('P008', 'Coffee Maker', 'Espresso and cappuccino machine', 200.00, 60),
('P009', 'External Hard Drive', '2TB storage capacity', 100.00, 25),
('P010', 'Wireless Router', 'High-speed internet connectivity', 70.00, 0),
('P011', 'Desk Chair', 'Ergonomic design for comfort', 120.00, 35),
('P012', 'Portable Speaker', 'Bluetooth-enabled for on-the-go music', 50.00, 70),
('P013', 'Cookware Set', 'Non-stick pots and pans', 150.00, 15),
('P014', 'Backpack', 'Water-resistant and spacious', 40.00, 90),
('P015', 'Printer', 'Color inkjet printer with wireless printing', 80.00, 30),
('P016', 'Smart Thermostat', 'Energy-efficient temperature control', 100.00, 40),
('P017', 'Yoga Mat', 'High-density foam for comfortable workouts', 20.00, 65),
('P018', 'Blender', 'Powerful blender for smoothies and shakes', 60.00, 25),
('P019', 'Security Camera', 'HD camera with motion detection', 120.00, 0),
('P020', 'LED Desk Lamp', 'Adjustable brightness for study or work', 30.00, 0);
