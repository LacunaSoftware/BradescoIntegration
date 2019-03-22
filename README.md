# Bradesco .Net Integration
A .Net library created to create and retrieve bank billets for Bradesco Bank Billet API. 

The front-end of this project is the same of the standard Angular Asp.Net Core template.

## How to run the Test Project

1. Clone the project
2. Open the "Lacuna.BradescoIntegration.sln" file using Visual Studio
3. Build the project and Run it using IIS Express
4. The server will be running and requests can be made to test the Bradesco Integration

## How to test the API's (suggestion)

The integration can be tested using HTTP calls. A suggested method to test the API, is use [Postman](https://www.getpostman.com/) and the built-in Postman collection bellow. It has the 3 requests available, so the user must only fill the web server address and the order number when needed.

1. Download [Postman](https://www.getpostman.com/)
2. Run Postman
3. Import the collection below

```
https://www.getpostman.com/collections/aca34c8d02c150c91fa1
```

4. Right-click in the collection, select "Edit"
5. Look for the tab "Variables"
6. Set the variables "baseUrl" and "bankBilletId"
7. Run the requests
