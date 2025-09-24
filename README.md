# inner-circle-documents-api

## Run in Visual Studio

First run this script to run a db and mocked external deps:

```bash
docker compose --profile MockForDevelopment up --build
```

## Karate Tests

### Run Karate against Api, Db, and MockServer in Docker Compose

Run Api, Db, and MockServer executing the following command (don't close the terminal unless you want to stop the containers)

```bash
docker compose --profile MockForTests up --build
```

Then execute following command inside of the dev-container

```bash
java -jar /karate.jar .
```

### Running Karate Tests, Api, Db, and MockServer in Docker Compose

Run the docker compose with MockForPullRequest profile executing the following command (don't close the terminal unless you want to stop the containers)

```bash
docker compose --profile MockForPullRequest up --build
```
