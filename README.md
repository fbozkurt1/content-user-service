# Content & User Service

This projects are RESTful APIs developed using .NET 8 which is fully dockerized.
It has swagger to test easily.

* To test APIs with swagger: http://localhost:8081/swagger/index.html AND http://localhost:8082/swagger/index.html


### Technologies
* .NET 8 WebApi
* PostgreSQL (Dapper ORM)
* Swagger (Swashbuckle)
* RefitClient for HttpClient management
* xUnit (for unit tests)


## Setup

1.  Clone the repository:

    ```bash
    git clone 
	https://github.com/fbozkurt1/merzigo-content-user-service.git
    cd merzigo-content-user-service
    ```

2.  Need the Docker to run:

    ```bash
    docker-compose up -d
    ```
3. Open links to test: 
	http://localhost:8081/swagger/index.html for ContentService 
	http://localhost:8082/swagger/index.html for User Service