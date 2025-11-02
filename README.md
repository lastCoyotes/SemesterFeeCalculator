# Semester Fee Calculator

This project is meant for calculating fees for UAH courses.

[User Manual](./docs/UAH_Fee_Calc_User_Manual.md)

[Deployment Guide](./docs/DeploymentGuide.md)

# Structure

- api
  - backend for the webpage
- docs
  - technical documentation
- www
  - static webpage (front-end)
- deploy
  - deployment files
- tests
  - Unit tests and client use cases

# API

From the [./api](./api) directory, run the following command:

```
dotnet run
```

On success, the output should look as follows:

```txt
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Full\Path\To\uah-fee-calc\api
```

Use either the https or http endpoint to access the API.

Currently, the document `UAHFeeSchedule.xlsx` needs to be in the working directory of the running api. The current location of `./api/UAHFeeSchedule.xlsx` is due to the working directory when using the api with dotnet run.

Run the API stand-alone:

```
dotnet run --project api
```

# www

The client is a static website that uses custom HTML elements.
The basic structure is as follows:

- lib

  - external libraries
- services

  - scripts that directly interact with the api
- ui

  - GUI elements, grouped in folders of the following format:
    - component.js
      - the main logic for the component, including how it is imported.
    - component.htm
      - html template
    - component.css
      - styles for the component
  - logic for generating existing elements
    - e.g. table.js makes HTML tables
