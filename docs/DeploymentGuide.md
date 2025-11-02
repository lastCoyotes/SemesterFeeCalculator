# Deployment Guide
This guide is to aid system administrators in setting deploying the project.

## Requirements

- docker
- docker compose
- an ssl certificate(optional)

The deployment is broken into two parts: an API and an nginx web server.
Docker creates a neat wrapper around these two apps to make them appear as one.

## Quick Start

- Go to https://gitlab.com/CS-499/uah-fee-calc/-/releases and pick the latest version. (For this guide, 1.0.0 is used). 

- Download the Source Code `tar.gz` version.
Below is a sample command that downloads the project from the copied source code link on the releases page.
    ```sh
    curl -LO https://gitlab.com/CS-499/uah-fee-calc/-/archive/1.0.0/uah-fee-calc-1.0.0.tar.gz
    ```
- Extract the folder
    ```sh
    tar -xzvf uah-fee-calc-1.0.0.tar.gz
    cd uah-fee-calc-1.0.0
    ```
- Build the project
    ```sh
        cd ./deploy
        docker-compose build
    ```
- Deploy the project
    ```sh
        docker-compose up -d
    ```
The project will now be running an HTTP server on port 80.
See configuration notes for SSL.

## Configuration

There are two main sets of configuration files:
Application Configuration and Web Server Configuration.

- The Web Server Configuration is a one-time setup that may be ignored if the server admin wishes to treat the app as a black-box.
    - Simply edit the `nginx.conf` to configure the web server
    - SSL Configuration
        - Make a directory called `ssl` in the deploy directory
            - add the certificate and private key files to the directory
        - Edit `docker-compose.yml`:
        ```yml
        # Docker project spec for deployment
        # Hosts an api and a web-server
        version: "3.0"
        services:
          # API Server, built locally before deployment
          api:
            build: ..
            deploy:
              replicas: 1
            volumes:
              - ../api/UAHFeeSchedule.xlsx:/srv/uah-fee-calc/UAHFeeSchedule.xlsx
          nginx:
            image: nginx:latest
            ports:
              - 80:80
              - 443:443
            volumes:
              - ./nginx.conf:/etc/nginx/nginx.conf
              - ../www/:/srv/www
              - ./ssl:/srv/ssl
        ```
        - Add the following underneath the line `listen:80` in `nginx.conf`:
            ```
            listen 443 ssl;
            ssl_certificate /ssl/cert.pem;
            ssl_certificate_key /ssl/privkey.pem;
            ```
            - cert.pem is assumed to be the certificate file
            - privkey.pem is assumed to be the private key file.

- The Application Configuration is held in an Excel File and is detailed in the User Manual.
    - By default, the excel file used is in the `api` folder.
    - To modify the Application Configuration, edit the excel file and
    run the following command in the `./deploy` directory:
    ```
    docker-compose restart
    ```
    This will restart the server


## API
The docker image for the UAH-Fee-Calculator's API can be found at
`https://gitlab.com/CS-499/uah-fee-calc/container_registry/`

The image can be dowloaded with the following command:

```
docker-compose pull registry.gitlab.com/cs-499/uah-fee-calc/api:1.0.0
```
The `1.0.0` is the version of the API being pulled.

Replace `build: ..` with `image: registry.gitlab.com/cs-499/uah-fee-calc/api:1.0.0` in the `deploy/docker-compose.yml` to use the prebuilt image
instead of building the API yourself.

## Client
The client consists of all files in the `www` folder and the `nginx.conf` directory in the `deploy` folder. It is a mostly static website that expects the API to be served on `/api/` . Download the client from the Release page along with the source code.

