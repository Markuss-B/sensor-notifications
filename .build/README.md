# To run with docker compose

Build image in sensor-consumer project main directory.
```
docker build -t sensor-webapi-dev .
```
Modify the `compose.yaml` file to set used database.

Run the docker compose file in the main directory.
```
docker compose up -d
```
To see logs use docker desktop or run the following command.
```
docker logs sensor-webapi-dev
```
If everything is working correctly you should see new collections in the database with sensor data.

Database compose see in sensor-consumer repo.

## If you want an image file 
Save image in tar file
```
docker save sensor-webapi-dev > sensor-webapi-dev.tar
```