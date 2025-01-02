# To run with docker compose

Build image in sensor-notifications project main directory.
```
docker build -t sensor-notifications-dev .
```
Modify the `compose.yaml` file to set used database.

Run the docker compose file in the main directory.
```
docker compose up -d
```
To see logs use docker desktop or run the following command.
```
docker logs sensor-notifications-dev
```

Database compose see in sensor-consumer repo.

## If you want an image file 
Save image in tar file
```
docker save sensor-notifications-dev > sensor-notifications-dev.tar
```