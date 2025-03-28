db = db.getSiblingDB('real-estate');

db.createCollection('RealEstate');
db.createCollection('User');

db.User.insertOne({
    "Email": "test@test.com",
    "PasswordHash": "E34dfDF*dfe-MO21"
});

db.RealEstate.createIndex({ "State": 1 });
db.RealEstate.createIndex({ "City": 1 });
db.RealEstate.createIndex({ "SaleMode": 1 });
db.RealEstate.createIndex({ "PropertyNumber": 1 }, { unique: true });

print("Database and indexes initialized successfully.");