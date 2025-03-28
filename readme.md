# Real Estate management

## Objective

It's a project to manage real estates, with features to bulk add from a csv file and to query then with some filters available.

## Architecture
The projects architecture follows the represented in the image bellow:

![image](docs/architecture.png)

### Backend

#### WebApi

It's a .NET 9 WebApi, that uses [Kafka](#kafka) as a messaging service, and MongoDB as a non-sequel database to store the data.
With four endpoints, to authenticate, store and request data:

- **/login ->**

    It`s used to authenticate in the application, receiving an email and password in a json format, at the body request. This request returns a JWT token, to be used in the other request of the api.

    The following email and password will authenticate successfully:

    ```json
    {
        "email": "test@test.com",
        "password": "E34dfDF*dfe-MO21"
    }
    ```

    Example of request/response:
    ![image](docs/login-request-response.png)

- **/add-from-csvfile ->**
  
    Receives a csv file, that is batch (batchs of 1000) proccessed and messages with the content are sent to a [Kafka](#kafka) topic, to be post consumed by the [Background Service](#background-service).
    
    This request requires the JWT token generated at the login request.
    
    
    The csv file ([click here to get a sample](docs/csv-test-file.csv)) has to be separeted by semicolons (;), and must have a header in this sctructure:


    **PropertyNumber;State;City;Neighborhood;Address;Price;AppraisalValue;Discount;Description;SaleMode;AccessLink**

    Example of request/response:
    ![image](docs/csv-request-response.png)
    

- **/getById ->**

    Finds a real estate by the Id and returns its content, this request requires the JWT token generated at the login request.

    Example of request/response:
    ![image](docs/getbyid-request-response.png)

- **/get ->**

    Querie real estates, with the following filters (page, pageSize, state, city and saleMode) and returns its content, this request requires the JWT token generated at the login request.

    Example of request/response:
    ![image](docs/get-request-response.png)

#### Background Service
It's a .NET 9 Background Service, that consumes messages from a [Kafka](#kafka) topic, and stores the real estates received at MongoDB.

### Frontend
The frontend it's build with Angular, and it has only 2 pages:
- Login page (You can login with the credentials provided at [WebApi](#webapi))
  ![image](docs/login-page.png)
- Dashboard page, where you can upload the csv file, and also query the content
  ![image](docs/dashboard-page.png)

## Resources utilized by the project

### Kafka

### MongoDb

### JWT Token


## How to run?
